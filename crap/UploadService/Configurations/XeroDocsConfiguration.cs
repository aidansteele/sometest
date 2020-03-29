using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadService.Configurations
{
    public class XeroDocsConfiguration
    {
        public static string Key => "XeroDocs";

        public string ConnectionString { get; set; }
    }
}
