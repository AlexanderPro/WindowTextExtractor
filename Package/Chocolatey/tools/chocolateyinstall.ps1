$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v1.16.0/WindowTextExtractor_v1.16.0.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = 'f19da68248f29bfc29a1664b47e4272a0725d308af8d325927134d93f88a3f98'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
