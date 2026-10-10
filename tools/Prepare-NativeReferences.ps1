param(
    [Parameter(Mandatory)][string]$GameManagedDir,
    [Parameter(Mandatory)][string]$BepInExCoreDir,
    [Parameter(Mandatory)][string]$JotunnDir,
    [Parameter(Mandatory)][string]$PublicizerPath,
    [Parameter(Mandatory)][string]$OutputRoot
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot)).TrimEnd('\') + '\'
$output = [IO.Path]::GetFullPath($OutputRoot)
$allowed = $output.StartsWith($repoRoot,[StringComparison]::OrdinalIgnoreCase)
if ($env:RUNNER_TEMP) {
    $runnerRoot = [IO.Path]::GetFullPath($env:RUNNER_TEMP).TrimEnd('\') + '\'
    $allowed = $allowed -or $output.StartsWith($runnerRoot,[StringComparison]::OrdinalIgnoreCase)
}
if (-not $allowed -or (Test-Path -LiteralPath $output)) { throw 'Use a new reference directory inside the repository or runner temporary directory.' }
foreach ($file in @((Join-Path $BepInExCoreDir 'BepInEx.dll'),(Join-Path $BepInExCoreDir '0Harmony.dll'),(Join-Path $JotunnDir 'Jotunn.dll'),$PublicizerPath)) {
    if (-not (Test-Path -LiteralPath $file -PathType Leaf)) { throw "Required native input missing: $file" }
}
$assemblies = @('assembly_valheim.dll','assembly_guiutils.dll','assembly_utils.dll','SoftReferenceableAssets.dll')
$before = foreach ($name in $assemblies) {
    $path = Join-Path $GameManagedDir $name
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Required game assembly missing: $name" }
    [pscustomobject]@{ Name=$name; Source=$path; Sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash }
}
$null = New-Item -ItemType Directory -Path $output
$oldRollForward = $env:DOTNET_ROLL_FORWARD
try {
    $env:DOTNET_ROLL_FORWARD = 'Major'
    foreach ($item in $before) {
        if ([IO.Path]::GetExtension($PublicizerPath) -eq '.dll') {
            & dotnet $PublicizerPath $item.Source --output $output --target All --strip --overwrite | Out-Host
        } else {
            & $PublicizerPath $item.Source --output $output --target All --strip --overwrite | Out-Host
        }
        if ($LASTEXITCODE -ne 0) { throw "Native publicizing failed: $($item.Name)" }
        if (-not (Test-Path -LiteralPath (Join-Path $output $item.Name))) { throw 'Publicized assembly missing' }
        if ((Get-FileHash -LiteralPath $item.Source -Algorithm SHA256).Hash -ne $item.Sha256) { throw 'Game assembly changed during reference preparation' }
    }
} finally { $env:DOTNET_ROLL_FORWARD = $oldRollForward }
$manifest = [pscustomobject]@{
    PreparedAtUtc=[DateTime]::UtcNow.ToString('o'); PublicizedRoot=$output; GameInputs=$before
    BepInExSha256=(Get-FileHash -LiteralPath (Join-Path $BepInExCoreDir 'BepInEx.dll')).Hash
    HarmonySha256=(Get-FileHash -LiteralPath (Join-Path $BepInExCoreDir '0Harmony.dll')).Hash
    JotunnSha256=(Get-FileHash -LiteralPath (Join-Path $JotunnDir 'Jotunn.dll')).Hash
    PublicizerSha256=(Get-FileHash -LiteralPath $PublicizerPath).Hash
    GameDirectoryWrites=0
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $output 'reference-manifest.json') -Encoding UTF8
$manifest
