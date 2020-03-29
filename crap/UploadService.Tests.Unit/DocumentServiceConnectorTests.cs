using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using Moq;
using UploadService.Configurations;
using UploadService.Connectors;
using UploadService.FeatureToggle;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class DocumentServiceConnectorTests
    {
        private readonly IOptions<CloudStorageRouterConfiguration> _cloudStorageRouterConfig;
        private readonly IOptions<DocumentServiceConfiguration> _documentServiceConfig;
        private readonly Mock<IFeatureClient> _featureClient;
        private readonly Mock<ILogger<DocumentServiceConnector>> _logger;

        public DocumentServiceConnectorTests()
        {
            _featureClient = new Mock<IFeatureClient>();
            _cloudStorageRouterConfig = Options.Create(new CloudStorageRouterConfiguration
            {
                Uri = "http://csr.test.com"
            });
            _documentServiceConfig = Options.Create(new DocumentServiceConfiguration
            {
                Uri = "http://documentservice.test.com"
            });
            _logger = new Mock<ILogger<DocumentServiceConnector>>();

        }

        [Fact]
        public void Given_OrgId_Exists_In_LD_Expect_CSRUri()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _featureClient.Setup(
                client => client.BoolFeature(
                    FeatureFlags.RedirectUploadServiceToCSR,
                    It.Is<FeatureModel>(model => model.OrgId == orgId.ToString()),
                    false)).Returns(true);

            var documentServiceConnector = new DocumentServiceConnector(
                _documentServiceConfig,
                _cloudStorageRouterConfig,
                _featureClient.Object,
                _logger.Object);

            var uploadServiceUri = documentServiceConnector.GetUploadServiceUri(orgId, userId);

            Assert.Equal(_cloudStorageRouterConfig.Value.Uri, uploadServiceUri);
        }

        [Fact]
        public void Given_OrgId_Does_Not_Exist_In_LD_Expect_DocumentServiceUri()
        {
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _featureClient.Setup(
                client => client.BoolFeature(
                    FeatureFlags.RedirectUploadServiceToCSR,
                    It.Is<FeatureModel>(model => model.OrgId == orgId.ToString()),
                    false)).Returns(false);

            var documentServiceConnector = new DocumentServiceConnector(
                _documentServiceConfig,
                _cloudStorageRouterConfig,
                _featureClient.Object,
                _logger.Object);

            var uploadServiceUri = documentServiceConnector.GetUploadServiceUri(orgId, userId);

            Assert.Equal(_documentServiceConfig.Value.Uri, uploadServiceUri);
        }
    }
}
