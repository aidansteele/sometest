namespace UploadService.Constants
{
    public class MimeType
    {
        public const string OCTETSTREAM = "application/octet-stream";
        public static string[] ValidMimeTypes => @"jpg,jpeg,jpe,gif,png,xpng,bmp,x-bmp,tiff,tif,doc,docx,xls,xlsx,ppt,pptx,odf,ods,odt,pdf,x-pdf,t-pdf,csv,txt,rtf,rtf2,rtx,keynote,key,pages-tef,pages,numbers-tef,numbers,zip,xzip,zipx,rar,eml,msg,7z".Split(',');
    }
}
