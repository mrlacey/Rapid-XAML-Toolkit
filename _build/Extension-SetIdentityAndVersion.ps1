# THIS SCRIPT IS USED TO SET IDENTITIES AND VERSIONS OF THE VSIX DURING BUILD. 
# IT ALSO CHANGES THE ENVIRONMENT IDENTIFIER ON VSIX AND COMMAND NAMES. 

# SAMPLE CALL:
# DEV:
# _build/Extension-SetIdentityAndVersion.ps1 -vsixManifestFile "RapidXAML.VSIX/RapidXamlToolkit/source.extension.vsixmanifest" -vsixIdentity "RapidXamlToolkit.DevNightly.25067190-b212-4873-aecf-84547ed9b962" -buildNumber "dev.version_1.2.3.4" -vsixEnvironment "(dev-nightly)" -packageGuid "bd81034c-4068-47ea-9983-95e652abaeb8" -cmdSetGuid "343F3EAC-C533-4120-8F96-032E50A8BA18" 
# PRO:
# _build/Extension-SetIdentityAndVersion.ps1 -vsixManifestFile "RapidXAML.VSIX/RapidXamlToolkit/source.extension.vsixmanifest" -vsixIdentity "RapidXamlToolkit.0C6D8454-3CA5-4BF8-8462-AD21A25EEEBD" -buildNumber "pro.version_1.2.3.4" -vsixEnvironment "" -packageGuid "0903C275-D535-49FC-B6AF-CBD5E41D429B" -cmdSetGuid "5464E1E8-F1A8-4400-B4A7-DED5388FAD4B" 

[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$vsixManifestFile,

  [Parameter(Mandatory=$True,Position=2)]
  [string]$vsixIdentity,

  [Parameter(Mandatory=$True,Position=3)]
  [string]$buildNumber,

  [Parameter(Mandatory=$False,Position=4)]
  [string]$vsixEnvironment,

  [Parameter(Mandatory=$True,Position=5)]
  [string]$packageGuid,

  [Parameter(Mandatory=$True,Position=6)]
  [string]$cmdSetGuid,

  [Parameter(Mandatory=$False,Position=7)]
  [string]$targetVsixEnvironment = "(local)",

  [Parameter(Mandatory=$False,Position=8)]
  [string]$targetPackageGuid = "c735dfc3-c416-4501-bc33-558e2aaad8c5",

  [Parameter(Mandatory=$False,Position=10)]
  [string]$targetCmdSetGuid = "8c20aab1-50b0-4523-8d9d-24d512fa8154"
)


$VersionRegex = "(\d+)\.(\d+)\.(\d+)\.(\d+)"

if($buildNumber -match $VersionRegEx){

  $revision =  [int]::Parse($matches[4]).ToString()
  
  $versionNumber = [int]::Parse($matches[1]).ToString() + "." + [int]::Parse($matches[2]).ToString() + "." + [int]::Parse($matches[3]).ToString() + "." + $revision
  Write-Host "Version Number" $versionNumber
  
}
else{
	throw "Build format does not match the expected pattern (buildName_w.x.y.z)"
}

## SET IDENTITY AND VERSION IN VSIX Manifest

Write-Host
Write-Host "Setting Identity in VSIX manifest."
if(Test-Path($vsixManifestFile)){
    [xml]$manifestContent = Get-Content $vsixManifestFile
    $manifestContent.PackageManifest.Metadata.Identity.Id = $vsixIdentity
    $manifestContent.PackageManifest.Metadata.Identity.Version = $versionNumber
    $manifestContent.PackageManifest.Metadata.DisplayName = $manifestContent.PackageManifest.Metadata.DisplayName.Replace($targetVsixEnvironment, $vsixEnvironment)
    $resolvedPath = Resolve-Path($vsixManifestFile)
    $manifestContent.Save($resolvedPath) 
    Write-Host "$resolvedPath - Version, Identity & Environment applied ($versionNumber, $vsixIdentity, $vsixEnvironment)"

    $vsixLangPacks = Get-ChildItem -include "Extension.vsixlangpack" -recurse |  Where-Object{ 
        $_.FullName -notmatch "\\Templates\\" -and 
        $_.FullName -notmatch "\\debug\\" -and
        $_.FullName -notmatch "\\obj\\" -and
        $_.FullName -match "\\RapidXamlToolkit\\"
    }
    if($vsixLangPacks){ 
      Write-Host "Applying Display Name to vsixlangpack files."
      foreach ($langPack in $vsixLangPacks) {
        [xml]$langContent = Get-Content $langPack
        $langContent.VsixLanguagePack.LocalizedName = $langContent.VsixLanguagePack.LocalizedName.Replace($targetVsixEnvironment, $vsixEnvironment)
        $langContent.Save($langPack) 
        Write-Host "$langPack - VsixEnvironment applied ($vsixEnvironment)"        
        }
    }
}
else{
    throw "No VSIX manifest file found."
}


## REPLACE Command Guids
if($cmdSetGuid -and $packageGuid){
  Write-Host
  Write-Host "Setting PackageGuid and CmdSetGuid in VSCT Files."
  $vsctFiles = Get-ChildItem -include "*.vsct" -recurse |  Where-Object{ 
      $_.FullName -notmatch "\\Templates\\" -and 
      $_.FullName -notmatch "\\debug\\" -and
      $_.FullName -notmatch "\\obj\\" -and
      $_.FullName -match "\\RapidXamlToolkit\\"
  }
  if($vsctFiles){ 
    Write-Host
    Write-Host "Applying guids $cmdSetGuid, $packageGuid and command name enviornment $vsixEnvironment to VSCT Files."
    foreach ($vsctFile in $vsctFiles) {
      $vsctFileContent = Get-Content $vsctFile
      attrib $vsctFile -r
      $replacedVsctContent = $vsctFileContent.Replace($targetPackageGuid, $packageGuid)
      $replacedVsctContent = $replacedVsctContent.Replace($targetCmdSetGuid, $cmdSetGuid)
      $replacedVsctContent = $replacedVsctContent.Replace($targetVsixEnvironment, $vsixEnvironment)
      $replacedVsctContent | Out-File $vsctFile -Encoding utf8 -Force
      Write-Host "$vsctFile - Guids applied (PackageGuid:$packageGuid, CmdGuid:$cmdSetGuid)"        
    }
  }
  else{
    throw "No VSCT files found."
  }

  Write-Host
  Write-Host "Applying guids $packageGuid to RapidXamlPackage.cs"
  $path = Resolve-Path $vsixManifestFile
  $installerPath = Split-Path $path
  $constFile = Join-Path  $installerPath "RapidXamlPackage.cs"
  
  if($constFile){
    $constFileContent = Get-Content $constFile
    attrib $constFile -r
    $replacedConstContent = $constFileContent.Replace($targetPackageGuid, $packageGuid)
    $replacedConstContent | Out-File $constFile -Encoding utf8 -Force
    Write-Host "$constFile - Guid applied (PackageGuid:$packageGuid)"
  }
  else{
     throw "RapidXamlPackage.cs constants file not found."
  }

  Write-Host
  Write-Host "Applying guids $cmdSetGuid to BaseCommand.cs"
  $path = Resolve-Path $vsixManifestFile
  $installerPath = Split-Path $path
  $constFile = Join-Path  $installerPath "Commands\BaseCommand.cs"
  
  if($constFile){
    $constFileContent = Get-Content $constFile
    attrib $constFile -r
    $replacedConstContent = $constFileContent.Replace($targetCmdSetGuid, $cmdSetGuid)
    $replacedConstContent | Out-File $constFile -Encoding utf8 -Force
    Write-Host "$constFile - Guid applied (CmdGuid:$cmdSetGuid)"
  }
  else{
     throw "BaseCommand.cs constants file not found."
  }
}
else{
  throw "PackageGuid and CmdSetGuid are mandatory."
}


## APPLY VERSION TO ASSEMBLY FILES (AssemblyVersion and AssemblyFileVersion)
Write-Host
Write-Host "Applying version to AssemblyInfo Files in matching the path pattern '$codePathPattern'" 
$files = Get-ChildItem -include "*AssemblyInfo.cs" -Recurse #|  Where-Object{ $_.FullName -notmatch "\\Templates\\" }
if($files)
{
    Write-Host "Will apply $versionNumber to $($files.count) files."

    $assemblyVersionRegEx = "\(""$VersionRegex""\)" 
    $assemblyVersionReplacement = "(""$versionNumber"")"

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $assemblyVersionRegEx, $assemblyVersionReplacement | Out-File $file utf8
        Write-Host "$file - version applied"
    }
}
else
{
    Write-Warning "No files found to apply version."
}


## APPLY VERSION RapidXamlPackage
Write-Host
Write-Host "Applying version to RapidXamlPackage.cs"
$files = Get-ChildItem -include "RapidXamlPackage.cs" -Recurse #|  Where-Object{ $_.FullName -notmatch "\\Templates\\" }
if($files)
{
    Write-Host "Will apply $versionNumber to $($files.count) files."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $VersionRegex, $versionNumber | Out-File $file utf8
        Write-Host "$file - version applied"
    }
}
else
{
    Write-Warning "No RapidXamlPackage found to apply version."
}
