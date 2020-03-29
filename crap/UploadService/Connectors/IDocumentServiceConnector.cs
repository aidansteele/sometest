using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UploadService.Models;
using Xero.Diagnostics;

namespace UploadService.Connectors
{
    public interface IDocumentServiceConnector : IPingable
    {
        Task<DocumentServicePostResponse> PostDocumentAsync(DocumentStream documentStream);
    }
}
