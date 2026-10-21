@{
    RootModule        = 'PSSlideShow.psm1'
    ModuleVersion     = '0.1.0'
    GUID              = '4e68bda7-b85d-4cbd-a8a3-a0fcc1fbb949'
    Author            = 'Liam Peters'
    CompanyName       = ''
    Copyright         = 'Copyright (c) Liam Peters (@liamjpeters).'
    Description       = 'Author and present interactive terminal slide shows using a PowerShell DSL.'

    # net10.0 build targets .NET 10, so the PowerShell 7 floor is 7.6+. Windows PowerShell 5.1 uses
    # the net472 build.
    PowerShellVersion = '5.1'

    CmdletsToExport   = @(
        'Start-SlideShow'
        'New-Slide'
        'New-SlideText'
        'New-SlideFiglet'
        'New-SlideList'
        'New-SlideTable'
        'New-SlideCode'
        'New-SlideColumns'
        'New-SlideColumn'
        'New-SlideRows'
        'New-SlideRow'
        'New-SlideTree'
        'New-SlideTreeNode'
        'New-SlideBarChart'
        'New-SlideBreakdownChart'
        'New-SlideCalendar'
        'New-SlideCanvas'
        'New-SlideQrCode'
        'New-SlideAlert'
        'New-SlideDynamic'
    )
    FunctionsToExport = @()
    VariablesToExport = @()
    AliasesToExport   = @()

    PrivateData = @{
        PSData = @{
            Tags       = @('Console', 'Presentation', 'SlideShow', 'Spectre', 'Terminal')
            ProjectUri = 'https://github.com/liamjpeters/PSSlideShow'
            LicenseUri = 'https://github.com/liamjpeters/PSSlideShow/blob/main/LICENSE'
        }
    }
}
