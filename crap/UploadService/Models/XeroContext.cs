using System;

namespace UploadService.Models
{
    public class XeroContext
    {
        public Guid CorrelationId { get; set; }
        public string RequestId { get; set; }
    }
}
