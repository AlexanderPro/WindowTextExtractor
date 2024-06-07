$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v2.1.3/WindowTextExtractor_v2.1.3.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = '6734466b7de74eecc4b5ca6e6e53e1b81632d7d5073a8eb00dec4ebf1a49d68d'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
