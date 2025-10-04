$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v2.2.1/WindowTextExtractor_v2.2.1.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = '63d509516e6c256040b532d301d993113a8f04edf92fbdbcc44f34810b375e33'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
