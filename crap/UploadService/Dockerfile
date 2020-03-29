FROM microsoft/dotnet:2.1-sdk
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "UploadService.dll"]
