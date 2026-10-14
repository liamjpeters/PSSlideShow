@{
    RootModule        = 'PSSlideShow.psm1'
    ModuleVersion     = '0.1.0'
    GUID              = 'b3f2c1a7-6d4e-4a2b-9c8f-1e0a5d7b3c21'
    Author            = 'PSSlideShow contributors'
    CompanyName       = 'PSSlideShow'
    Copyright         = 'Copyright (c) PSSlideShow contributors'
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
        'New-SlideAlert'
        'New-SlideDynamic'
    )
    FunctionsToExport = @()
    VariablesToExport = @()
    AliasesToExport   = @()

    PrivateData = @{
        PSData = @{
            Tags       = @('Console', 'Presentation', 'SlideShow', 'Spectre', 'Terminal')
            ProjectUri = 'https://github.com/PSSlideShow/PSSlideShow'
            LicenseUri = 'https://github.com/PSSlideShow/PSSlideShow/blob/master/LICENSE'
        }
    }
}
