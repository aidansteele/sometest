namespace UploadService.FeatureToggle
{
    public interface IFeatureClient
    {
        bool BoolFeature(string featureName, FeatureModel model, bool defaultValue);

    }
}
