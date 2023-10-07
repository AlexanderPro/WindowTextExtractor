$ErrorActionPreference = 'Stop';
$packageName= 'windowtextextractor'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/AlexanderPro/WindowTextExtractor/releases/download/v2.0.0/WindowTextExtractor_v2.0.0.zip'

$packageArgs = @{
  packageName   = $packageName
  destination   = $toolsDir
  fileType      = 'zip'
  url           = $url
  softwareName  = 'WindowTextExtractor*'
  checksum      = '7aaf143cd68c2b5ff16df9a12de4a092209d52373202111f3a226e8ed860e2e4'
  checksumType  = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs
