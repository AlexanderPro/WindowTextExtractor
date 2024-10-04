$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v2.2.0/WindowTextExtractor_v2.2.0.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = 'bba6722c48b8d1ec1d209f809cd09f356ef37f69773ae94d2c1b62facd21a7a2'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
