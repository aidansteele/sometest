using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Validators;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class StorageServiceInfoValidatorTests
    {
        [Fact]
        public void GivenNull_ExpectException()
        {
            var exception = Assert.Throws<UploadException>(() => StorageServiceInfoValidator.EnsureOrgIsProvisioned(null));
            Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
        }

        [Fact]
        public void GivenNotNull_ExpectNoException()
        {
            var ssi = new StorageServiceInfo();
            ssi.EnsureOrgIsProvisioned();
        }
    }
}
