using api_001_prueba_cajanegra.Data;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Threading.Tasks;

namespace api_001_prueba_cajanegra.Model
{
    public class Document
    {
        private readonly XElement _documentXmlTree;
        private readonly string _documentXml;
        private readonly string _documentType;

        public string ClientId { get; private set; }
        public string DocumentName { get; private set; }
        public DocumentPropertiesGET Properties { get; set; }
        public DocumentPropertiesPUT PropertiesData { get; set; }
        public DocumentPropertiesPUT.BusinessModel BusinessModelType { get; private set; }

        private readonly IDatabase _db;
        private readonly RedisConfiguration _redisConfiguration;

        public Document(XElement documentXml, RedisConfiguration redisConfiguration, IDatabase db)
        {
            _documentXml = documentXml.ToString();
            _documentXmlTree = documentXml;
            _documentType = _documentXmlTree.Name.LocalName;
            _redisConfiguration = redisConfiguration;
            _db = db;
            ParseXml();
        }

        private void ParseXml()
        {
            if (!DocumentXpaths.Paths.ContainsKey(_documentType))
            {
                throw new ArgumentException($"Invalid document type: {_documentType}");
            }
            var paths = DocumentXpaths.Paths[_documentType];

            ClientId = _documentXmlTree.XPathSelectElement(paths.ClientIdXpath)?.Value;
            if (paths.DocumentNameXpath != null)
            {
                DocumentName = _documentXmlTree.XPathSelectElement(paths.DocumentNameXpath)?.Value;
            }
        }

        public static async Task<string> GetByGuid(RedisConfiguration config, string guid)
        {
            var configuration = new ConfigurationOptions
            {
                EndPoints = { $"{config.RedisHost}:{config.RedisPort}" },
                Password = config.RedisPassword,
                User = config.RedisUsername
            };
            try
            {
                var redis = ConnectionMultiplexer.Connect(configuration);
                var db = redis.GetDatabase();

                return await db.StringGetAsync($"{config.KeyPrefix}:raw:{guid}");
            }
            catch (Exception ex)
            {
                var storagePath = $"{config.StorageBasePath}\\{guid}.xml";
                if (File.Exists(storagePath))
                {
                    return await File.ReadAllTextAsync(storagePath);
                }
                return null;
            }
        }

        public async Task SaveToRedis()
        {
            var propertiesPUT = PropertiesData;
            var hashEntries = propertiesPUT.ToHashEntries();

            await _db.HashSetAsync($"{_redisConfiguration.KeyPrefix}:properties:{PropertiesData.Guid}", hashEntries);
            await _db.StringSetAsync($"{_redisConfiguration.KeyPrefix}:raw:{PropertiesData.Guid}", _documentXml);
        }
    }
}
