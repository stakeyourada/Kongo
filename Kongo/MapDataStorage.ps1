#Requires -Version 3.0
# ------------------------------------------------------------
<#
 .SYNOPSIS
    Maps file shares for data storage

 .DESCRIPTION
    Creates full set of release assets for Kongo Stakepool Monitoring services
#>
Param(
    [Parameter(Mandatory=$true)] 
    [ValidateNotNullOrEmpty()]
    [string]$DriveLetter = "K",
	
    [Parameter(Mandatory=$true)] 
    [ValidateNotNullOrEmpty()]
    [string]$StorageFqdn = "testnetstorage.file.core.windows.net",
	
	[Parameter(Mandatory=$true)] 
    [ValidateNotNullOrEmpty()]
    [string]$StoragePath = "\kongostorage",
	
    [Parameter(Mandatory=$true)] 
    [ValidateNotNullOrEmpty()]
    [string]$User = "Azure\testnetstorage",
	
    [Parameter(Mandatory=$true)] 
    [ValidateNotNullOrEmpty()]
    [string]$Saskey = "abcdefghijklmnopqrstuvwxyzBoNIt3xN2aXOtvKoeEzkE//heQ=="
)

$Networkpath = $DriveLetter + ":\"
$LetterToMap = $DriveLetter + ":"
$FullNetworkStoragePath = "\\" + $StorageFqdn + $StoragePath
$pathExists = Test-Path -Path $Networkpath
If (-not ($pathExists)) {
	Test-NetConnection -ComputerName $StorageFqdn -Port 445 -InformationLevel "Detailed"
	
	cmdkey /add:$StorageFqdn /user:$User /pass:$SasKey
	net use $LetterToMap $FullNetworkStoragePath /persistent:Yes
}
