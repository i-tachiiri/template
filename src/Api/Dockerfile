FROM mcr.microsoft.com/azure-functions/dotnet-isolated:8 AS build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:8
WORKDIR /home/site/wwwroot
COPY --from=build /home/site/wwwroot ./

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
CMD ["/azure-functions-host/Microsoft.Azure.Functions.JobHost"]
