namespace UploadService.Middlewares.XeroContext
{
    public class XeroContextProvider : IXeroContextProvider
    {
        private Models.XeroContext _xeroContext;

        public void Save(Models.XeroContext xeroContext)
        {
            _xeroContext = xeroContext;
        }

        public Models.XeroContext Get()
        {
            return _xeroContext;
        }
    }
}
