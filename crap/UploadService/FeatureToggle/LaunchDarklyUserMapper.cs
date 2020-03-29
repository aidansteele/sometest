using LaunchDarkly.Client;

namespace UploadService.FeatureToggle
{
    public static class LaunchDarklyUserMapper
    {
        public static User GetLaunchDarklyUser(FeatureModel model)
        {
            var user = User.Builder(model?.UserId)
                .Custom("orgId", model?.OrgId).AsPrivateAttribute()
                .Custom("region", model?.Region).AsPrivateAttribute()
                .Email(model?.Email).AsPrivateAttribute()
                .Build();
            return user;
        }
    }
}
