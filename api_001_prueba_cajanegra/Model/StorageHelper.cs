using api_001_prueba_cajanegra.Data;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace api_001_prueba_cajanegra.Model
{
    public class StorageHelper
    {
        private static readonly string StoragePathTemplate = "QA_XML_FE/facturas/Recepcion{business_model}/{client_id}/{year}/{month}/{day_of_month}/{hour}/{guid}/{client_id}{document_name}.xml";
        private static readonly ILogger logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger("StorageHelper");

        public static string GetStoragePath(RedisConfiguration config, DocumentPropertiesPUT properties)
        {
            return StoragePathTemplate
                .Replace("{business_model}", properties.BusinessModelType.ToString())
                .Replace("{client_id}", properties.ClientId)
                .Replace("{year}", properties.Year)
                .Replace("{month}", properties.Month)
                .Replace("{day_of_month}", properties.DayOfMonth)
                .Replace("{hour}", properties.Hour)
                .Replace("{guid}", properties.Guid)
                .Replace("{document_name}", properties.DocumentName);
        }

        public static async Task WriteFileAsync(string filePath, string content)
        {
            if (File.Exists(filePath))
            {
                throw new InvalidOperationException($"File already exists at path: {filePath}");
            }

            var directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllTextAsync(filePath, content);
        }

        public static async Task SaveDocumentToStorageAsync(RedisConfiguration config, DocumentPropertiesPUT properties, string documentXml)
        {
            var storagePath = GetStoragePath(config, properties);
            try
            {
                await WriteFileAsync(storagePath, documentXml);
                properties.StoragePath = storagePath;
                await properties.SaveToRedis();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to save document to storage: {ex.Message}");
                throw;
            }
        }
    }
}
