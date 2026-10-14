#requires -Version 7.6
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $ModuleRoot,

    [string] $OutputPath = (Join-Path (Join-Path (Split-Path $PSScriptRoot -Parent) 'content') 'cmdlets')
)

$ErrorActionPreference = 'Stop'
$moduleRoot = [IO.Path]::GetFullPath($ModuleRoot)
$outputPath = [IO.Path]::GetFullPath($OutputPath)
$helpDir = Join-Path (Join-Path $moduleRoot 'docs') 'PSSlideShow'
$manifestPath = Join-Path (Join-Path $moduleRoot 'module') 'PSSlideShow.psd1'

if (-not (Test-Path -LiteralPath (Join-Path $outputPath '_index.md'))) {
    throw "Expected the Hugo cmdlets section at '$outputPath'."
}
if (-not (Test-Path -LiteralPath $manifestPath)) {
    throw "Module manifest not found at '$manifestPath'."
}

foreach ($framework in @('net10.0', 'net472')) {
    $binDir = Join-Path (Join-Path (Join-Path $moduleRoot 'module') 'bin') $framework
    if (-not (Test-Path -LiteralPath (Join-Path $binDir 'PSSlideShow.dll')) -or
        -not (Get-ChildItem -LiteralPath $binDir -Filter 'PSSlideShow.dll-Help.xml' -Recurse -File |
            Select-Object -First 1)) {
        throw "Build the module and its command help for $framework before generating site pages."
    }
}

$exports = @((Import-PowerShellDataFile -Path $manifestPath).CmdletsToExport)
$pages = @(Get-ChildItem -LiteralPath $helpDir -Filter '*.md' -File |
    Where-Object BaseName -NE 'PSSlideShow' | Sort-Object Name)
$knownPages = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($page in $pages) {
    $null = $knownPages.Add($page.Name)
    if ($page.BaseName -notin $exports) {
        throw "Help page '$($page.Name)' does not match an exported cmdlet."
    }
}
foreach ($command in $exports) {
    if (-not $knownPages.Contains("$command.md")) {
        throw "Missing Markdown help for exported cmdlet '$command'."
    }
}

$existing = @(Get-ChildItem -LiteralPath $outputPath -Filter '*.md' -File |
    Where-Object Name -NE '_index.md')
foreach ($page in $existing) {
    if (@(Get-Content -LiteralPath $page.FullName -TotalCount 5) -notcontains 'generated: true') {
        throw "Refusing to replace the non-generated cmdlet page '$($page.FullName)'."
    }
}
foreach ($page in $existing) {
    Remove-Item -LiteralPath $page.FullName
}

foreach ($page in $pages) {
    $source = Get-Content -LiteralPath $page.FullName -Raw
    $sourceFrontMatter = [regex]::Match($source, '\A---\r?\n[\s\S]*?\r?\n---\r?\n')
    if (-not $sourceFrontMatter.Success) {
        throw "Expected PlatyPS front matter in '$($page.FullName)'."
    }

    $body = $source.Substring($sourceFrontMatter.Length)
    $synopsis = [regex]::Match(
        $body,
        '(?m)^## SYNOPSIS[ \t]*\r?\n\r?\n(?<text>[\s\S]*?)(?=\r?\n\r?\n## )')
    if (-not $synopsis.Success) {
        throw "Missing synopsis in '$($page.FullName)'."
    }
    $description = ($synopsis.Groups['text'].Value -replace '\s+', ' ').Trim()
    $heading = [regex]::new('(?m)^# ' + [regex]::Escape($page.BaseName) + '[ \t]*\r?\n')
    if (-not $heading.IsMatch($body)) {
        throw "Expected the cmdlet heading in '$($page.FullName)'."
    }
    $body = $heading.Replace($body, '', 1)

    $body = [regex]::Replace(
        $body,
        '\]\((?<file>[^)#]+\.md)(?<fragment>#[^)]*)?\)',
        [Text.RegularExpressions.MatchEvaluator] {
            param([Text.RegularExpressions.Match] $link)
            $target = $link.Groups['file'].Value
            if (-not $knownPages.Contains($target)) {
                throw "Unknown cmdlet help link '$target' in '$($page.Name)'."
            }
            $slug = [IO.Path]::GetFileNameWithoutExtension($target).ToLowerInvariant()
            return "](../$slug/$($link.Groups['fragment'].Value))"
        })

    $frontMatter = @(
        '---'
        "title: $(ConvertTo-Json -InputObject $page.BaseName -Compress)"
        "description: $(ConvertTo-Json -InputObject $description -Compress)"
        'generated: true'
        '---'
        ''
    ) -join "`n"
    $destination = Join-Path $outputPath ($page.BaseName.ToLowerInvariant() + '.md')
    [IO.File]::WriteAllText(
        $destination,
        $frontMatter + $body.TrimStart() + "`n",
        [Text.UTF8Encoding]::new($false))
}

Write-Host "Generated $($pages.Count) cmdlet pages from validated module help."
