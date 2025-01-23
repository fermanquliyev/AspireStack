param(
    [string]$NewProjectName
)

$RootPath = (Get-Location).Path
$ExcludedFolders = @("node_modules", "bin", "obj", ".git", ".vs")
$ScriptFileName = "RenameAspireProject.ps1"

function Rename-Folders {
    Get-ChildItem -Path $RootPath -Recurse -Directory | Where-Object {
        $_.FullName -notmatch "\\($($ExcludedFolders -join "|"))\\" -and
        $_.Name -like "*GeoPlanner*"
    } | Sort-Object -Property FullName -Descending | ForEach-Object {
        $ParentPath = Split-Path -Parent $_.FullName
        $NewFolderName = Join-Path -Path $ParentPath -ChildPath ($_.Name -replace "GeoPlanner", $NewProjectName)

        if (!(Test-Path $NewFolderName)) {
            Write-Host "Renaming folder: $_.FullName to $NewFolderName" -ForegroundColor Green
            Rename-Item -Path $_.FullName -NewName $NewFolderName -Force
        } else {
            Write-Host "Skipping folder rename for $_.FullName as $NewFolderName already exists" -ForegroundColor Yellow
        }
    }
}

function Rename-Files {
    Get-ChildItem -Path $RootPath -Recurse -File | Where-Object {
        $_.FullName -notmatch "\\($($ExcludedFolders -join "|"))\\" -and
        $_.Name -like "*GeoPlanner*" -and
        $_.Name -ne $ScriptFileName
    } | ForEach-Object {
        $ParentPath = Split-Path -Parent $_.FullName
        $NewFileName = Join-Path -Path $ParentPath -ChildPath ($_.Name -replace "GeoPlanner", $NewProjectName)

        if (!(Test-Path $NewFileName)) {
            Write-Host "Renaming file: $_.FullName to $NewFileName" -ForegroundColor Green
            Rename-Item -Path $_.FullName -NewName $NewFileName -Force
        } else {
            Write-Host "Skipping file rename for $_.FullName as $NewFileName already exists" -ForegroundColor Yellow
        }
    }
}

function Update-FileContents {
    Get-ChildItem -Path $RootPath -Recurse -File | Where-Object {
        $_.FullName -notmatch "\\($($ExcludedFolders -join "|"))\\" -and
        $_.Name -ne $ScriptFileName
    } | ForEach-Object {
        (Get-Content -Path $_.FullName) -replace "GeoPlanner", $NewProjectName | Set-Content -Path $_.FullName
        Write-Host "Updated contents in file: $_.FullName" -ForegroundColor Cyan
    }
}

# Execution
Write-Host "Starting rename operations..." -ForegroundColor Green
Rename-Folders
Rename-Files
Update-FileContents
Write-Host "Rename operations completed." -ForegroundColor Green
