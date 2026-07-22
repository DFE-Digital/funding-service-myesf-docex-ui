# Manage Your Education and Skills Funding Document Exchange

The Manage Your Education and Skills Funding (MYESF) Document Exchange web application to allow the following:

- Secure exchange of documents with supported file formats (CSV, DOC, DOCX, JPG, ODS, ODT, PDF, XLS, XLSX)
- For users in DfE-funded organisations to download received documents and upload new ones.
- External users having the ability to view, download and send documents back to DfE

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 web application utilising Azure App Service for deployment.

The web application runs on an Azure App service on Azure.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `appsettings.json` file will need to be created in the `Pds.DocumentExchange.Web` project. Below, and included in the repo, there is `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "PdsApplicationInsights": {
    "InstrumentationKey": "",
    "Environment": "local"
  },
  "Logging": {
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Error"
      }
    },
    "LogLevel": {
      "Default": "Information"
    }
  },
  "DfESignIn": {
    "DfELegacyCodeId": "",
    "OpenIDConnect": {
      "Authority": "",     
      "ClientId": "",
      "ClientSecret": ""
    },
    "PublicApi": {
      "Url": "",
      "ClientID": "",
      "ClientSecret": "",
      "TokenIssuer": ""
    },
    "Cookie": {
      "Name": ""
    }
  },
  "DocumentExchangeServices": {
    "AdminApiClient": {
      "ApiBaseAddress": "",
      "Authority": "https://login.microsoftonline.com/",
      "ClientId": "",
      "TenantId": "",
      "ClientSecret": "",
      "AppUri": ""
    },
    "DataApiClient": {
      "ApiBaseAddress": "",
      "AppUri": "",
      "Authority": "https://login.microsoftonline.com/",
      "ClientId": "",
      "ClientSecret": "",
      "TenantId": "",
      "Timeout": 180
    }
  },
  "DocumentExchangeWeb": {
    "ShowServiceStartPage": true,
    "MSClarityId": ""
  }
}
```

### Setting Details

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for Application Insights resource for logging purposes.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.

- **`Logging:ApplicationInsights:LogLevel:Default`**
  The default logging level for the service when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**
  The default logging level for Microsoft specific information when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:LogLevel:Default`**
  The default logging level for the service; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`DfESignIn:DfELegacyCodeId`**
  The comma separated list of DfE legacy code ids used to map or track legacy system records.

- **`DfESignIn:OpenIDConnect:Authority`**
  The Authority is the base URL of the OpenID Connect that issues authentication tokens and verifies user identities.

- **`DfESignIn:OpenIDConnect:ClientId`**
  The unique public identifier assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`DfESignIn:OpenIDConnect:ClientSecret`**
  The confidential credential assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`DfESignIn:PublicApi:Url`**
  The base endpoint URL used to communicate with the Department for Education (DfE) Sign-in Public API.

- **`DfESignIn:PublicApi:ClientID`**    
  The unique identifier assigned to the application to authenticate its identity when calling the Department for Education (DfE) Sign-in Public API.

- **`DfESignIn:PublicApi:ClientSecret`**
  The confidential credential used for the application to authenticate its identity when making secure backend requests to the Department for Education (DfE) Sign-in Public API.

- **`DfESignIn:PublicApi:TokenIssuer`**
  The unique identifier (typically a string or URL) that specifies the entity or authority that generates and signs the security tokens used for authentication.

- **`DfESignIn:Cookie:Name`**
  The unique text used as the name of the HTTP cookie that stores a user's session data after they log in via Department for Education (DfE) Sign-in.

- **`DocumentExchangeServices:AdminApiClient:ApiBaseAddress`**
  The base URL endpoint used by an internal client application to route network requests to the Admin API backend.

- **`DocumentExchangeServices:AdminApiClient:Authority`**
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the Admin API client.

- **`DocumentExchangeServices:AdminApiClient:ClientId`**
  The unique identifier assigned to the admin client application to authenticate its identity against the security provider when calling the Admin API.

- **`DocumentExchangeServices:AdminApiClient:TenantId`**
  The unique identifier that specifies the exact organization or cloud instance within the Identity Provider where the Admin API client is registered.

- **`DocumentExchangeServices:AdminApiClient:ClientSecret`**
  The confidential credential used by the admin client application to securely prove its identity to the Identity Provider.

- **`DocumentExchangeServices:AdminApiClient:AppUri`**
  The unique Application ID URI used as the identifier for the protected Admin API resource within the Identity Provider.

- **`DocumentExchangeServices:DataApiClient:ApiBaseAddress`**
  The base URL endpoint used by a client application to route network requests to the Docex Data API backend.

- **`DocumentExchangeServices:DataApiClient:AppUri`**
  The unique Application ID URI used as the identifier for the protected DocEx Data API resource within the Identity Provider.

- **`DocumentExchangeServices:DataApiClient:Authority`**
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the DocEx Data API client.

- **`DocumentExchangeServices:DataApiClient:ClientId`**
  The unique identifier assigned to the docex data client application to authenticate its identity against the security provider when calling the Docex Data API.

- **`DocumentExchangeServices:DataApiClient:ClientSecret`**
   The confidential credential used by the docex data client application to securely prove its identity to the Identity Provider.

- **`DocumentExchangeServices:DataApiClient:TenantId`**
  The unique identifier that specifies the exact organization or cloud instance within the Identity Provider where the Docex Data API client is registered.

- **`DocumentExchangeServices:DataApiClient:Timeout`**
  The maximum duration, in seconds or minutes, that the application will wait for a response from the Docex Data API before aborting the request.

- **`DocumentExchangeWeb:ShowServiceStartPage`**
  A configuration setting that determines whether an application displays the landing/start page to users before routing them to a login screen or a dashboard.

- **`DocumentExchangeWeb:MSClarityId`**
  The unique tracking identifier assigned by Microsoft Clarity to a specific website project. It tells the Microsoft Clarity tracking script where to send session recordings, heatmaps, and user behavior analytics for that particular website.

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
