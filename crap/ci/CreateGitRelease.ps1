param(
  $version,
  $commitId,
  $gitApiAccessToken,
  $gitServer='github.dev.xero.com',
  $owner='Inbox',
  $repository='UploadService'
)

$release =@{"tag_name"= $version
  "target_commitish"= $commitId
  "name"= $version
  "body"= $version
  "draft"= $false
  "prerelease"= $false
}|ConvertTo-Json -Depth 100

Invoke-WebRequest "https://github.dev.xero.com/api/v3/repos/Inbox/UploadService/releases?access_token=$($gitApiAccessToken)" -Method POST -Body $release
