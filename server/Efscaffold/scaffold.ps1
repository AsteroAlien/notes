[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$EnvFile = ".env"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Import-DotEnv {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Env file not found: $Path"
    }

    Get-Content -LiteralPath $Path | ForEach-Object {
        $line = $_

        if ($null -eq $line) { return }
        $line = $line.Trim()
        if ($line.Length -eq 0) { return }
        if ($line.StartsWith('#')) { return }

        # Supports: KEY=VALUE, KEY="VALUE", KEY='VALUE'
        $match = [regex]::Match($line, '^(?<key>[^=\s#]+)\s*=\s*(?<value>.*)$')
        if (-not $match.Success) {
            return
        }

        $key = $match.Groups['key'].Value
        $value = $match.Groups['value'].Value.Trim()

        # Strip surrounding quotes if present
        if (($value.StartsWith('"') -and $value.EndsWith('"')) -or ($value.StartsWith("'") -and $value.EndsWith("'"))) {
            $value = $value.Substring(1, $value.Length - 2)
        }

        # Assign as process environment variable
        Set-Item -Path "Env:$key" -Value $value
    }
}

Import-DotEnv -Path $EnvFile

if (-not $env:CONN_STR) {
    throw "CONN_STR is not set. Ensure it exists in $EnvFile"
}

try {
    dotnet tool install -g dotnet-ef | Out-Host
} catch {
    dotnet tool update -g dotnet-ef | Out-Host
}

$arguments = @(
    'ef', 'dbcontext', 'scaffold',
    $env:CONN_STR,
    'Npgsql.EntityFrameworkCore.PostgreSQL',
    '--output-dir', './Entities',
    '--context-dir', '.',
    '--context', 'MyDbContext',
    '--no-onconfiguring',
    '--namespace', 'Efscaffold.Entities',
    '--context-namespace', 'Infrastructure.Postgres.Scaffolding',
    '--schema', 'notes_system',
    '--force'
)

dotnet @arguments
