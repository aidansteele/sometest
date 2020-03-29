using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using UploadService.Models;
using UploadService.Constants;
using UploadService.Configurations;
using Microsoft.Extensions.Options;
using Xero.Diagnostics;
using Newtonsoft.Json;
using UploadService.Models.Exceptions;
using UploadService.FeatureToggle;

namespace UploadService.Connectors
{
    public class DocumentServiceConnector : IDocumentServiceConnector
    {
        private readonly DocumentServiceConfiguration _documentServiceConfiguration;
        private readonly CloudStorageRouterConfiguration _cloudStorageRouterConfiguration;
        private readonly IFeatureClient _featureClient;
        private readonly ILogger<IDocumentServiceConnector> _logger;

        public DocumentServiceConnector(
            IOptions<DocumentServiceConfiguration> documentServiceConfiguration,
            IOptions<CloudStorageRouterConfiguration> cloudStorageRouterConfiguration,
            IFeatureClient featureClient,
            ILogger<IDocumentServiceConnector> logger)
        {
            _logger = logger;
            _documentServiceConfiguration = documentServiceConfiguration.Value;
            _cloudStorageRouterConfiguration = cloudStorageRouterConfiguration.Value;
            _featureClient = featureClient;
        }

        public async Task<List<HealthStatus>> Ping()
        {
            _logger.LogInformation($"ping document service url: {_documentServiceConfiguration.Uri}");

            if (!_documentServiceConfiguration.Uri.StartsWith("http"))
            {
                return new List<HealthStatus> { new HealthStatus { Status = "Invalid Uri", Name = _documentServiceConfiguration.Uri } };
            }

            using (var httpClient = new HttpClient())
            {
                var status = await httpClient.GetAsync($"{_documentServiceConfiguration.Uri}/ping");

                return new List<HealthStatus> { new HealthStatus { Status = status.ReasonPhrase, Name = _documentServiceConfiguration.Uri } };
            }
        }

        public async Task<DocumentServicePostResponse> PostDocumentAsync(DocumentStream documentStream)
        {
            if (documentStream == null)
            {
                throw new UploadException(HttpStatusCode.InternalServerError, "document stream is null. Cannot send null value to document service");
            }

            var requestUri = GetUploadServiceUri(documentStream.OrganisationId, documentStream.UserId);

            using (var httpClient = new HttpClient())
            using (var requestContent = CreateRequestContent(documentStream))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(30);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(new Uri(requestUri), "Documents"),
                    Method = HttpMethod.Post,
                    Content = requestContent
                };
                request.Headers.Add(XeroHeaderKeys.UserId, documentStream.UserId.ToString());
                request.Headers.Add(XeroHeaderKeys.TenantId, documentStream.OrganisationId.ToString());
                request.Headers.Add(XeroHeaderKeys.ClientName, "XeroFiles");
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DocumentServicePostResponse>(json);
            }
        }

        private MultipartFormDataContent CreateRequestContent(DocumentStream documentStream)
        {
            var requestContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(documentStream.Content);

            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(documentStream.MimeType);
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = documentStream.FileName };

            requestContent.Add(streamContent);

            return requestContent;
        }

        public string GetUploadServiceUri(Guid organisationId, Guid userId)
        {
            var featureModel = new FeatureModel()
            {
                OrgId = organisationId.ToString(),
                UserId = userId.ToString()
            };

            var shouldRedirectToCSR = _featureClient.BoolFeature(
                FeatureFlags.RedirectUploadServiceToCSR,
                featureModel,
                false);

            var requestUri = _documentServiceConfiguration.Uri;

            if (shouldRedirectToCSR)
            {
                requestUri = _cloudStorageRouterConfiguration.Uri;
            }

            _logger.LogInformation($"UploadSevice Uri for OrganisationId {organisationId} and UserId {userId}: {requestUri}");

            return requestUri;
        }
    }
}
