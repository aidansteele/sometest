namespace UploadService.Constants
{
    public enum EncryptionMethod : byte
    {
        None = 0,
        Aes256 = 1,
        Rsa2048 = 2
    }

    public enum CompressionMethod : byte
    {
        None = 0,
        GZip = 1
    }
}
