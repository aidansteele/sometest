param(
  $version,
  $commitId,
  $gitApiAccessToken,
  $gitServer='github.dev.xero.com',
  $owner='Inbox',
  $repository='UploadService'
)

$result=Invoke-WebRequest "https://$($gitServer)/api/v3/repos/$($owner)/$($repository)/releases?access_token=$($gitApiAccessToken)"
$releases = $result.Content|ConvertFrom-Json
[string]::Join('[comma]',@($releases|ForEach-Object{ "$($_.tag_name)[colon]$($_.target_commitish)"}))
