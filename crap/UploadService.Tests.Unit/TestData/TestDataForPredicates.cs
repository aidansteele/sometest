using System;
using System.Collections.Generic;

namespace UploadService.Tests.Unit.TestData
{
    public class TestDataForPredicates
    {
        private static readonly List<object[]> MaxBoundaryData = new List<object[]>
        {
            new object[] { 26214500, true },
            new object[] { 26214499, false }
        };

        private static readonly List<object[]> EmptyBoundaryData = new List<object[]>
        {
            new object[] { 0, true },
            new object[] { -1, true },
            new object[] { 1, false },
            new object[] { null, true }
        };

        private static readonly List<object[]> GuidData = new List<object[]>
        {
            new object[] { Guid.Empty, false },
            new object[] { Guid.NewGuid(), true },
            new object[] { null, false }
        };

        private static readonly List<object[]> FileNameExtensionData = new List<object[]>
        {
            new object[] { "name.png", true },
            new object[] { "name.JPG", true },
            new object[] { "name.abc", false },
            new object[] { "name.pngabd", false },
            new object[] { string.Empty, false },
            new object[] { "FilenameWithoutAnExtension", false },
            new object[] { null, false }
        };

        private static readonly List<object[]> FileNameData = new List<object[]>
        {
            new object[] { "name.png", true },
            new object[] { ".JPG", false },
            new object[] { "na<me.png", false },
            new object[] { "na>me.png", false },
            new object[] { "na:me.png", false },
            new object[] { "na\"me.png", false },
            new object[] { "na/me.png", false },
            new object[] { "na\\me.png", false },
            new object[] { "na|me.png", false },
            new object[] { "na?me.png", false },
            new object[] { "na*me.png", false },
        };

        private static readonly List<object[]> ObjectTypeData = new List<object[]>
        {
            new object[] { "INVOICETYPE/ACCPAY", true },
            new object[] { "invoiceType/accpay", true },
            new object[] { "invoiceType", true },
            new object[] { "$@%$^&&*", false },
            new object[] { string.Empty, false },
            new object[] { "INVOICETYPE%ACCPAY", false },
            new object[] { "<>?%ACCPAY", false },
            new object[] { null, false }
        };

        public static IEnumerable<object[]> ExceedsMaximumFileSizeData => MaxBoundaryData;

        public static IEnumerable<object[]> IsEmptyFileData => EmptyBoundaryData;

        public static IEnumerable<object[]> IsValidOrganisationData => GuidData;

        public static IEnumerable<object[]> IsValidExtension => FileNameExtensionData;

        public static IEnumerable<object[]> IsValidFilename => FileNameData;

        public static IEnumerable<object[]> IsValidObjectType => ObjectTypeData;
    }
}
