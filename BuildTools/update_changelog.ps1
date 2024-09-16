param([string]$RELEASE_VERSION=$null, [switch]$Force)

function Update-MarkdownContent ([string] $targetFile, [string] $contentFile, [string] $injectionMarker = ""){
  $changelogAdded = $false
  if(!$(Test-Path $targetFile)){
    Write-Host "Markdown file not updated because it does not exist" -ForegroundColor DarkYellow
    Write-Host "File: '${targetFile}'" -ForegroundColor DarkYellow
    return
  }

  (Get-Content $targetFile) |
      Foreach-Object {
          if($injectionMarker -eq ""){
              if(-not $changelogAdded){
                  $(Get-Content $contentFile)
                  $changelogAdded = $true
              }
              $_ # Send the current line to output
          } else {
              $_ # Send the current line to output
              if($_ -match $injectionMarker){
                if ($targetFile.Contains("changelog.md")) {
                    $newHeader = "##"
                } else {
                    $newHeader = "#"
                }
                "`r`n"
                $(Get-Content $contentFile).Replace("##", $newHeader)
              }
          }
      } |
  Set-Content $targetFile
}

if(!$RELEASE_VERSION){
    $RELEASE_VERSION = Read-Host "Version number"
}

# Get release version and suffix from version string.
$VersionTag = $RELEASE_VERSION
$VersionTag -match '((\d+)\.\d+\.\d+)(-(RC|DEV))?' > $null
$VersionNumber = $Matches[1]
$MajorVersionNumber = $Matches[2]
$VersionSuffix = $Matches[4]
$IsReleaseCandidate = $VersionSuffix -eq "RC"
$IsReleaseDeveloper = $VersionSuffix -eq "DEV"

# Get version string to include in changelog
if($IsReleaseCandidate -or $IsReleaseDeveloper) {
    $changelogVersion = "VECTO v$VersionNumber-$VersionSuffix"
} else {
    $changelogVersion = "VECTO v$VersionNumber Official Release"
}

# PREVIOUS_RELEASE_SHA is the commit the previous release tag points to.
# CURRENT_RELEASE_SHA  is the commit the current release tag points to.
$tags = @($(git tag -l --sort=-v:refname) | Where-Object { $_.Contains("Release/v$MajorVersionNumber") })
$CI_COMMIT_SHA = $(git rev-parse --verify HEAD)
$CURRENT_RELEASE_SHA = $CI_COMMIT_SHA
$PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[0])")

if($CI_COMMIT_TAG){
  $CURRENT_RELEASE_SHA = $(git rev-list -1 "tags/$($tags[0])")
  $PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[1])")
}

Write-Host "Current version points to $($CURRENT_RELEASE_SHA)"
Write-Host "Previous version points to $($PREVIOUS_RELEASE_SHA)"

$ReleaseNotesUpdateMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/release_notes.md";
if(-not $Force){
    # Get the latest changes for the release changelog.
    git cliff "$CURRENT_RELEASE_SHA..$PREVIOUS_RELEASE_SHA" --unreleased --tag "$changelogVersion" -o cliff_changelog.md --config ./BuildTools/cliff.toml

    Move-Item cliff_changelog.md ./$ReleaseNotesUpdateMarkdown -Force
}

# Update Release Notes and changelog markdowns.
# Based on the major, determine the ReleaseNotes for the given version.
if ($MajorVersionNumber -ne 3 -and $MajorVersionNumber -ne 4){
    throw "Release Notes version ${MajorVersionNumber} not supported."
} else {
    $ReleaseNotesMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/ReleaseNotesVecto${MajorVersionNumber}x.md"
    $ReleaseNotesPdf = "Documentation/User Manual Source/Release Notes Vecto${MajorVersionNumber}.x.pdf"
}

# Insert new changelog features into Release Notes.
$InjectNewFeaturesMark = "<!-- Cover Slide -->"
Update-MarkdownContent $ReleaseNotesMarkdown $ReleaseNotesUpdateMarkdown $InjectNewFeaturesMark

# Insert new changelog features into VECTO changelog.
$ChangelogInjectMark = "# Changelog"
$ChangelogFilePath = "Documentation/User Manual/6-changelog/changelog.md"
Update-MarkdownContent $ChangelogFilePath $ReleaseNotesUpdateMarkdown $ChangelogInjectMark

$ChangesMarkdown = "CHANGES.md"
Copy-Item $ChangelogFilePath $ChangesMarkdown -Force

# Convert md to pdf
Push-Location "Documentation/User Manual Source/ReleaseNotesMDs"
pandoc "..\..\..\$ReleaseNotesMarkdown" -o "..\..\..\$ReleaseNotesPdf" --css "..\..\..\BuildTools\templates\md-style.css" --pdf-engine=$Env:weasyprint  --title="Changelog"
Pop-Location

Push-Location "Documentation/User Manual/"
& './convert.bat'
Pop-Location

$UserManualHtml = "Documentation/User Manual/help.html"

# Stage the modified files by the script in git.
git add $ReleaseNotesUpdateMarkdown
git add $ReleaseNotesMarkdown
git add $ChangelogFilePath
git add $ChangesMarkdown
git add $ReleaseNotesPdf
git add $UserManualHtml