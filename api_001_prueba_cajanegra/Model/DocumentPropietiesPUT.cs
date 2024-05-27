using api_001_prueba_cajanegra.Data;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace api_001_prueba_cajanegra.Model
{
    public class DocumentPropertiesPUT
    {
        public enum BusinessModel
        {
            B2C,
            B2B
        }


        public string DocumentName { get; set; }
        [XmlElement("BusinessModel")]
        public BusinessModel BusinessModelType { get; set; }
        [XmlElement("Year")]
        public string Year { get; set; }
        [XmlElement("Month")]
        public string Month { get; set; }
        [XmlAttribute("DayOfMonth")]
        public string DayOfMonth { get; set; }
        [XmlElement("Hour")]
        public string Hour { get; set; }
        [XmlElement("Guid")]

        [XmlElement("RNCEmisor")]
        public string ClientId { get; set; }
        [XmlElement("DocumentName")]
        public string Guid { get; set; }
        [XmlElement("StoragePath")]
        public string? StoragePath { get; set; }

        public readonly IDatabase _db;
        public readonly RedisConfiguration _redisConfigurationData;

        public DocumentPropertiesPUT(RedisConfiguration config)
        {
            _redisConfigurationData = config;
            var configuration = new ConfigurationOptions
            {
                EndPoints = { $"{_redisConfigurationData.RedisHost}:{_redisConfigurationData.RedisPort}" },
                Password = _redisConfigurationData.RedisPassword,
                User = _redisConfigurationData.RedisUsername
            };
            var redis = ConnectionMultiplexer.Connect(configuration);
            _db = redis.GetDatabase();

            StoragePath = StorageHelper.GetStoragePath(config, this);
        }

        public HashEntry[] ToHashEntries()
        {
            return new HashEntry[]
            {
                new HashEntry("BusinessModel", BusinessModelType.ToString()),
                new HashEntry("Year", Year ?? ""),
                new HashEntry("Month", Month ?? ""),
                new HashEntry("DayOfMonth", DayOfMonth ?? ""),
                new HashEntry("Hour", Hour ?? ""),
                new HashEntry("ClientId", ClientId ?? ""),
                new HashEntry("DocumentName", DocumentName ?? ""),
                new HashEntry("Guid", Guid ?? ""),
                new HashEntry("StoragePath", StoragePath ?? "")
            };
        }

        public async Task SaveToRedis()
        {
            var hashEntries = ToHashEntries();
            await _db.HashSetAsync($"{_redisConfigurationData.KeyPrefix}:properties:{Guid}", hashEntries);
        }
    }
}
