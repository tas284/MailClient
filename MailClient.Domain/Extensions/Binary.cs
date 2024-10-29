using System.IO.Compression;
using System.Text;

namespace MailClient.Domain.Extensions
{
    public static class Binary
    {
        public static byte[] CompressString(string text)
        {
            if (string.IsNullOrEmpty(text)) { return null; }

            byte[] textAsByte = Encoding.UTF8.GetBytes(text);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(textAsByte, 0, textAsByte.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static string DecompressString(byte[] compressData)
        {
            if (compressData == null) { return null; }

            using (var memoryStream = new MemoryStream())
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
