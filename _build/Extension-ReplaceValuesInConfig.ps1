# THIS SCRIPT IS USED TO SET CONFIG VALUES IN VSIX DURING BUILD. 

# SAMPLE CALL:
# _build/Extension-ReplaceValuesInConfig.ps1 -configFilePath "RapidXAML.VSIX\RapidXamlToolkit\appsettings.json" -telemetryKey "MyKey"

[CmdletBinding()]
Param(
	[Parameter(Mandatory=$true)]
	[string]$configFilePath,

	[Parameter(Mandatory=$true)]
	[string]$telemetryKey
)



$command = "Command Excuted: " + $MyInvocation.Line
Write-Output $command

# Formats JSON in a nicer format than the built-in ConvertTo-Json does.
# This is based on code from https://github.com/PowerShell/PowerShell/issues/2736 and will be built into PS6.0

function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
  $indent = 0;
  ($json -Split '\n' |
    % {
      if ($_ -match '[\}\]]') {
        # This line contains  ] or }, decrement the indentation level
        $indent--
      }

      $line = (' ' * $indent * 4) + $_.TrimStart().Replace(':  ', ': ')

      if ($_ -match '[\{\[]') {
        # This line contains [ or {, increment the indentation level
        $indent++
      }

      # Regex ensures that output isn't encoded in the powershell unicode style. (We want '&', not '\u0026')
      [regex]::Unescape($line)
  }) -Join "`n"

}


if(Test-Path $configFilePath){
	$configFile = Get-Content $configFilePath | Out-String | ConvertFrom-Json  
		
    $configFile.TelemetryKey = $TelemetryKey

	$configFile | ConvertTo-Json | Format-Json | Out-File $configFilePath -Encoding utf8
	Write-Output "Done."
		
}
else{
	throw "File not found '$configFilePath'"
}

