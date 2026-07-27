param([Parameter(Mandatory=$true)][string]$IsoPath)
$ErrorActionPreference = 'Stop'
Mount-DiskImage -ImagePath $IsoPath -StorageType ISO -PassThru | Get-Volume | ForEach-Object {
    Start-Process (Join-Path ($_.DriveLetter + ':') 'setup.exe') -Verb RunAs
}
