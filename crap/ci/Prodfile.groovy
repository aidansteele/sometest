import groovy.json.JsonSlurper

appName = "uploadservice"
imageRegistry = "files-docker-common.artifactory.xero-support.com"
imageRepository = "${imageRegistry}/${appName}"
imageRepositoryCredentialsId = "4e835dbc-d622-4abd-84bf-12c14d4354d1"
slackChannel="files-alerts"
kubeNamespace = "files"
deploymentRole = "XRO-Deployment"

timestamps {
    color {
        try {

            node('master') {

                stage('Check out code from git') { checkout scm }

                dockerci('ci/Jenkinsenv') {

                    if(env.BRANCH_NAME == 'master') {

                            def selectedBuild
                            stage('Choose build/release to promote'){
                                selectedPromotion = askForPromotionDetails()
                                println selectedPromotion
                            }

                            awsacct.paasuat {
                                    deployToEnvironment("UAT",
                                        "us-west-2-k8s-paas-uat-rua",
                                        "uat.yaml",
                                        "arn:aws:kms:us-west-2:333186395126:key/f6ab82ad-a883-4587-96ac-98c602640f16", selectedPromotion.commitId)
                            }

                            def release
                            stage('Confirm PROD release') {
                                if(selectedPromotion.isRollback){
                                    release = askRollbackApproval(selectedPromotion.number)
                                }
                                else{
                                    release = askForReleaseVersion()
                                }

                                println "${release.approver} approved ${release.number} deployment to prod"
                            }

                            awsacct.paasprod {
                                    deployToEnvironment("PROD",
                                            "us-east-1-k8s-paas-prod-rua",
                                            "prod.yaml",
                                            "arn:aws:kms:us-east-1:333186395126:key/9b69faf4-d4eb-442c-902d-71b7d3c5ff4c", selectedPromotion.commitId)
                            }

                            setGitReleaseVersion(release.number, selectedPromotion.commitId)
                    }
                }
            }
        } catch (e) {

            if (env.BRANCH_NAME == 'master') {
                slackSend(
                    color: 'danger',
                    message: "*${env.JOB_NAME}* build ${env.BUILD_NUMBER} failed\n\nMore details: ${env.BUILD_URL}",
                    channel: slackChannel
                )
            }

            error('Build failed: ' + e)
        }
    }
}

def deployToEnvironment(environmentName, context, varsFileName, pstoreKmsKeyArn, commitId) {
  def serviceAccount = "${env.XERO_AWS_ACCOUNT_ID}"
  def serviceRegion = "${env.AWS_REGION}"

  def helmHome = "/tmp/.helm"

    stage("Deploy to ${environmentName}") {
        stackit("uploadservice-iam",
                'infrastructure/k8s/iam-role.yml',
                [
                    Name: "files-k8s-uploadservice-role",
                    PStoreKmsKeyArn: pstoreKmsKeyArn
                ]
        )

        def imageTag = commitId

        docker.image('luigi.artifactory.xero-support.com/paas-kubernetes/deploy2pk8s:04aaad1badb8e0546ff41a2faa9d8d48ee085754').inside {
                withAWS(role: deploymentRole, roleAccount: serviceAccount, region: serviceRegion) {
                sh "helm --home ${helmHome} init --wait --upgrade --kube-context ${context} --tiller-namespace ${kubeNamespace} --service-account tiller"
                sh "helm --home ${helmHome} upgrade --install --wait ${appName} 'generic-service' --repo https://artifactory.xero-support.com/helm-incubator -f infrastructure/k8s/values.yaml -f infrastructure/k8s/environments/${varsFileName} --set image.repository=${imageRepository} --set image.tag=${imageTag} --kube-context ${context} --namespace ${kubeNamespace} --tiller-namespace ${kubeNamespace}"
                }
        }

        addDeploymentMarker("${appName}-${environmentName.toLowerCase()}", commitId, generateChangelog())

        slackSend(color: 'GREEN',
                message: "Completed release for ${environmentName}:\r\nJob '${env.JOB_NAME} [${env.BUILD_NUMBER}]' (${env.BUILD_URL})",
                channel: slackChannel)
    }
}

def generateChangelog() {
  def changeLog = ""

  currentBuild.changeSets.each { changeSet ->
    changeSet.items.each { entry ->
      changeLog += "* ${entry.commitId} by ${entry.author} on ${new Date(entry.timestamp)}: ${entry.msg}\n\n"
    }
  }

  return changeLog
}

def askForPromotionDetails(){
    def builds = getBuildsList()
    def buildNumberDescriptionDelimiter='-'
    def buildNewlineDelimitedList = selectProperties(builds, ['number','description'],buildNumberDescriptionDelimiter).join("\n")

    def releases =getReleasesList()
    def releaseNewlineDelimitedList = selectProperties(releases,['number']).join("\n")
    chosenOption = input (
        message: 'select either a build to promote or a release to rollback',
        submitterParameter: 'PROMOTED_BY',
        parameters: [
            choice(choices: buildNewlineDelimitedList,  name: 'SELECTED_BUILD'),
            choice(choices: releaseNewlineDelimitedList, name: 'SELECTED_RELEASE_NUMBER')]
    )
    def buildNumber=chosenOption['SELECTED_BUILD'].split(buildNumberDescriptionDelimiter)[0]
    def selectedBuild = builds.find{build -> build.number == buildNumber}
    def selectedRelease = releases.find{release -> release.number == chosenOption['SELECTED_RELEASE_NUMBER']}
    if(selectedBuild.revision==null && selectedRelease.revision==null ){
        error "BuildOrReleaseNotSelected: Please select at least a build or release"
    }
    if(selectedBuild.revision!=null && selectedRelease.revision!=null ){
        error "BothBuildAndReleaseSelected: Please select only build or release, not both"
    }
    def isRollback=(selectedBuild.revision==null)
    def commitId=""
    def number=""
    if(isRollback){
        commitId=selectedRelease.revision
        number=selectedRelease.number
    }
    else{
        commitId=selectedBuild.revision
        number=selectedBuild.number
    }
    return [isRollback:isRollback, commitId:commitId, number:number];
}

def getBuildsList()
{
    sh "chmod +x ci/GetBuilds.ps1"
    def commaDelimitedBuildList=""

    withCredentials([string(credentialsId: 'JenkinsBasicAuthToken', variable: 'basicAuthToken')])
    {
        commaDelimitedBuildList = batsh(script: "pwsh ci/GetBuilds.ps1 -jenkinsBasicAuthToken ${basicAuthToken}", returnStdout: true).trim()
    }
    return [[number:"select a build to promote",description:'',revision:null]]+parseText(commaDelimitedBuildList)
}

def getReleasesList()
{
    sh "chmod +x ci/GetGitReleases.ps1"
    def commaDelimitedReleaseList=""

    withCredentials([string(credentialsId: 'InboxGitApiToken', variable: 'gitApiAccessToken')])
    {
        commaDelimitedReleaseList = batsh(script: "pwsh ci/GetGitReleases.ps1 -gitApiAccessToken ${gitApiAccessToken}", returnStdout: true).trim()
    }
    return [[number:"select a release to rollback", revision:null]]+parseText(commaDelimitedReleaseList)
}

def askForReleaseVersion(){
    def now = new Date()
    def currentTimeStamp = now.format("yyyyMMdd-HH-mm-ss", TimeZone.getTimeZone('UTC'))

    def release = input(
        id: 'release',
        message: 'Please provide a version for prod release',
        submitterParameter: 'APPROVED_BY',
        parameters: [
            [$class: 'TextParameterDefinition', defaultValue: "", description: 'Version', name: 'RELEASE_VERSION']
        ]
    )
    return [approver:release['APPROVED_BY'], number:release['RELEASE_VERSION']]
}

def askRollbackApproval(releaseNumber){
    def now = new Date()
    def currentTimeStamp = now.format("yyyyMMdd-HH-mm-ss", TimeZone.getTimeZone('UTC'))

    def approver = input(
        id: 'release',
        message: "Are you sure you want to re-deploy ${releaseNumber}",
        submitterParameter: 'APPROVED_BY'
    )
    return [approver:approver, number:releaseNumber]
}

def setGitReleaseVersion(releaseVersion, commitId)
{
    sh 'chmod +x ci/CreateGitRelease.ps1'
    withCredentials([string(credentialsId: 'svc-jenkins-token', variable: 'gitApiAccessToken')])
    {
        batsh(script: "pwsh ci/CreateGitRelease.ps1 -gitApiAccessToken '${gitApiAccessToken}' -version '${releaseVersion}' -commitId '${commitId}'", returnStdout: true)
    }
}

def selectProperties(items, propertyNames, delimiter=''){
  def properties = []
  for (item in items) {
      def propertyNameIndex=0
      def property=""
      for(propertyName in propertyNames){
        property=property + item[propertyName]
        if(propertyNames.size()>1 && (propertyNameIndex<propertyNames.size()-1)){
            property=property+delimiter
        }
        propertyNameIndex=propertyNameIndex+1

      }
        properties = properties + [property]
  }
  return properties
}

def parseText(text){
  def itemsText = text.split(/\[comma\]/)
  def items = []
  for (itemText in itemsText) {
      def itemInfo = itemText.split(/\[colon\]/)
      def description=''
      if(itemInfo.size()>2){
        description=itemInfo[2]
      }

      items.add([
        number: itemInfo[0],
        revision: itemInfo[1],
        description: description
      ]);
  }
  return items
}

def addDeploymentMarker(newRelicAppName, deploymentTag, changeLog) {
  withCredentials([string(credentialsId: 'NewRelicDeploymentMarkerKey', variable: 'NR_APIKEY')]) {
    def applicationsResponse = sh(script: "curl --silent --fail --show-error -X GET 'https://api.newrelic.com/v2/applications.json' -H 'X-Api-Key: ${NR_APIKEY}' -d filter[name]='${newRelicAppName}'", returnStdout: true)
    echo "Response was: ${applicationsResponse}"

    def newRelicAppId = new JsonSlurper().parseText(applicationsResponse).applications[0].id
    echo "Application ID is: ${newRelicAppId}"

    def newRelicUrl = "https://api.newrelic.com/v2/applications/${newRelicAppId}/deployments.json"
    def cleanedChangeLog = changeLog.replace("\n", "\\n")
    def deploymentInfo = """{
                              "deployment": {
                                "revision": "${deploymentTag}",
                                "changelog": "${cleanedChangeLog}"
                                }
                            }"""

    def deploymentInfoPath = pwd(tmp: true) + "/deploymentInfo.json"
    writeFile(file: deploymentInfoPath, text: deploymentInfo)

    def creationResponse = sh(script: "cat '${deploymentInfoPath}' | curl --silent --fail --show-error -X POST -H 'X-Api-Key: ${NR_APIKEY}' -i -H 'Content-Type: application/json' -d @- ${newRelicUrl}", returnStdout: true)
    echo "Response was: ${creationResponse}"
  }
}
