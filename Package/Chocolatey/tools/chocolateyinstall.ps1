$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v1.15.0/WindowTextExtractor_v1.15.0.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = '454f477dd62a2092a2aa09e751bc6a70cdb847a25d060b7e57bf7b8a426ba3c6'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
