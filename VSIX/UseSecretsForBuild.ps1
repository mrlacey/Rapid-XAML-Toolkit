Param(
	[Parameter(Mandatory=$true)]
	[String]$mode,

	[Parameter(Mandatory=$true)]
	[String]$rootDir
)

$PREbuild = 1
$POSTbuild = 2

$specifiedMode = 0

try
{
	Write-Host " "
	Write-Host "UseSecretsForBuild.ps1 is executing."

	if ($mode -eq "PREBUILD")
	{
		Write-Host "UseSecretsForBuild mode = Pre-Build"
		$specifiedMode = $PREbuild
	}

	if ($mode -eq "POSTBUILD")
	{
		Write-Host "UseSecretsForBuild mode = Post-Build"
		$specifiedMode = $POSTbuild
	}

	if ($specifiedMode -eq 0)
	{
		Write-Host "UseSecretsForBuild: no (or unknown) mode specified. Should be 'PREBUILD' or 'POSTBUILD'."
	}
	else
	{
		# Make the changes in the PRE-build and then swap them back in the POST-build
		if ($specifiedMode -eq $PREbuild)
		{
			Write-Host "Searching for secret files in $rootDir"

			$secretFiles = Get-Childitem –Path "$rootDir" -Include *.secret -File -Recurse -ErrorAction SilentlyContinue

			Write-Host "Found "$secretFiles.Count" secret file(s)"

			Foreach($secretFile in $secretFiles)
			{
				Write-Host "Processing: '$secretFile'"
				$nonSecretFile = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($secretFile), [System.IO.Path]::GetFileNameWithoutExtension($secretFile))

				Write-Host "- Checking existence of: '$nonSecretFile'"

				if ([System.IO.File]::Exists($nonSecretFile))
				{
					Write-Host "- Replacing: '$secretFile'"
					Rename-Item -Path $nonSecretFile -NewName "$nonSecretFile.TemporarilyRenamedDuringBuild"
					Rename-Item -Path $secretFile -NewName $nonSecretFile
				}
				else
				{
					Write-Host "- No non-secret version found for: $secretFile"
				}
			}
		}
		else
		{
			Write-Host "Searching for temporarily renamed files in $rootDir"

			$tempFiles = Get-Childitem –Path "$rootDir" -Include *.TemporarilyRenamedDuringBuild -File -Recurse -ErrorAction SilentlyContinue

			Write-Host "Found "$tempFiles.Count" temporary file(s)"

			Foreach($tempFile in $tempFiles)
			{
				Write-Host "Processing: '$tempFile'"
				$nonTempFile = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($tempFile), [System.IO.Path]::GetFileNameWithoutExtension($tempFile))

				Write-Host "- Checking existence of: '$nonTempFile'"

				if ([System.IO.File]::Exists($nonTempFile))
				{
					Write-Host "- Replacing: '$tempFile'"
					Rename-Item -Path $nonTempFile -NewName "$nonTempFile.secret"
					Rename-Item -Path $tempFile -NewName $nonTempFile
				}
				else
				{
					Write-Host "- No non-temp version found for: $tempFile"
				}
			}
		}
	}
}
catch
{
	Write-Host "UseSecretsForBuild error:" $_.Exception.Message
	Write-Host "UseSecretsForBuild error:" $_.InvocationInfo.PositionMessage
	Write-Host "UseSecretsForBuild error:" $_.Exception.StackTrace
	# Suppress errors as don't want to break the build
}
finally
{
	exit 0
}