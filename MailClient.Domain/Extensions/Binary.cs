using System.IO.Compression;
using System.Text;

namespace MailClient.Domain.Extensions
{
    public static class Binary
    {
        public static byte[] CompressString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            byte[] textAsByte = Encoding.UTF8.GetBytes(text);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gzipStream.Write(textAsByte, 0, textAsByte.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static string DecompressString(byte[] content)
        {
            if (content == null || content.Length == 0) return null;

            try
            {
                using (var memoryStream = new MemoryStream(content))
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (InvalidDataException)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao descomprimir dados.", ex);
            }
        }
    }
}
