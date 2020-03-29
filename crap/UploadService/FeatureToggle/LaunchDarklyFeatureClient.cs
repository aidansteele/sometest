using LaunchDarkly.Client;
using System;
using Microsoft.Extensions.Options;
using UploadService.Configurations;

namespace UploadService.FeatureToggle
{
    public class LaunchDarklyFeatureClient : IFeatureClient, IDisposable
    {
        private readonly LdClient _ldClient;
        private bool _disposedValue = false;

        public LaunchDarklyFeatureClient(IOptions<LaunchDarklyConfiguration> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _ldClient = new LdClient(config.Value.SdkKey);
        }

        public bool BoolFeature(string featureName, FeatureModel model, bool defaultValue)
        {
            return _ldClient.BoolVariation(featureName, LaunchDarklyUserMapper.GetLaunchDarklyUser(model), defaultValue);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _ldClient.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
