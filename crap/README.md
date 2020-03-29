# UploadService

The UploadService is a dotnet core web api responsible for uploading files to Xero Files, and associating files to transactions.

## Developer Installation Guide

There are two configurations for the docker image:

## Local Debug

In visual studio, change from IIS Express to UploadService to run as a console app.

### DEV docker image

This is built on the dotnet core SDK in order to allow debugging within docker.

### LATEST docker image

If you run `Build-DockerImage.ps1`, a new docker image will be created built on the dot net core runtime. It will be tagged as `uploadservice:latest` so that the Inbox BFF can link to it. For more deets on running these locally for development, check out https://github.dev.xero.com/Inbox/Inbox.BFF.

## Deployment
* Test deployment: https://jenkins.biz.xero-support.com/job/Applications/job/Inbox/job/X-Files/job/UploadService/job/master/

* UAT and Production deployment: https://jenkins.biz.xero-support.com/job/Applications/job/Inbox/job/UAT-Prod/job/UploadService/job/master/

## Logging

* Test environment
    * Logs are available in Sumo Logic with the query `namespace=files AND pod=uploadservice* AND _sourceCategory="test/pk8s/ap-southeast-2_k8s-paas-test-rua"`
* UAT environment
    * Logs are available in Sumo Logic with the query `namespace=files AND pod=uploadservice* AND _sourceCategory="uat/pk8s/us-west-2_k8s-paas-uat-rua"`
* Prod environment
    * Logs are available in Sumo Logic with the query `namespace=files AND pod=uploadservice* AND _sourceCategory="prod/pk8s/us-east-1_k8s-paas-prod-rua"`

## References

Test(sb):
https://uploadservice-k8s.sb.xero-test.com/swagger

Uat:
https://uploadservice-k8s.global.livestage6.test.xero.com/swagger

Prod:
https://uploadservice-k8s.global.xero.com/swagger


