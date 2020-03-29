param(
	$jenkinsUrl='https://jenkins.biz.xero-support.com',
	$topFolderName='Inbox',
	$repoName='UploadService',
	$jenkinsBasicAuthToken,
	$numberOfBuilds=10,
	$numberOfSucessfulBuilds=5
)


function getPastBuilds
{
    param(
        $jenkinsUrl,
        $topFolderName,
        $repoName,
        $jenkinsBasicAuthToken,
        $numberOfBuilds,
        $numberOfSucessfulBuilds,
        $branchName='master'
    )
    $jobPath="job/Applications/job/$($topFolderName)/job/X-Files/job/$($repoName)/job/$($branchName)"

    $basicAuthToken= "Basic $jenkinsBasicAuthToken"

    #may be we can limit it from the end point
    $jobApiUrl="$jenkinsUrl/$jobPath/api/json?pretty=true"
    $builds=((iwr $jobApiUrl -Headers @{Authorization=$basicAuthToken}).Content|ConvertFrom-Json).builds
    $selectedBuilds=$builds | select -first $numberOfBuilds

    $successfulbuilds=$selectedbuilds|%{
        (iwr "$($_.url)/api/json?pretty=true" -headers @{authorization=$basicauthtoken}).content|convertfrom-json
    }|where { $_.result -eq 'success' -and @($_.actions|where { $_._class -eq 'hudson.plugins.git.util.BuildData' -and $_.remoteUrls -like "*$($repoName).git" }).Count -ge 1} | select -first $numberofsucessfulbuilds

    #todo : it's really rubbish to loop through each build, the api must have capabilities to filter - check out depth parameter
    $successfulbuilds| %{
        $build=$_
        $repoBuildDataActionNode=$build.actions|where { $_._class -eq 'hudson.plugins.git.util.BuildData' -and $_.remoteUrls -like "*$($repoName).git" }
        if ( $repoBuildDataActionNode -eq $null )
        {
            throw "MissingRepositoryBuildDataActionNode: actions nodes is missing build data (_class: hudson.plugins.git.util.BuildData) for the repository ($($repoName))"
        }
        $revision=$repoBuildDataActionNode.buildsByBranchName."$($branchName)".revision.sha1
        $description=$build.changeSets|%{$_.items}|where{$_.id -eq $revision}|select -ExpandProperty msg
        @{number=$build.displayName;revision=$revision;description=$description}
    }

}

$builds=getPastBuilds -jenkinsUrl $jenkinsUrl `
	-topFolderName $topFolderName `
	-repoName $repoName `
	-jenkinsBasicAuthToken $jenkinsBasicAuthToken `
	-numberOfBuilds $numberOfBuilds `
	-numberOfSucessfulBuilds $numberOfSucessfulBuilds
[string]::Join("[comma]", @($builds|%{"$($_.number)[colon]$($_.revision)[colon]$($_.description)"}))
