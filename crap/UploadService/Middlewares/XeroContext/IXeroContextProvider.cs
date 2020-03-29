namespace UploadService.Middlewares.XeroContext
{
    public interface IXeroContextProvider
    {
        void Save(Models.XeroContext xeroContext);
        Models.XeroContext Get();
    }
}
