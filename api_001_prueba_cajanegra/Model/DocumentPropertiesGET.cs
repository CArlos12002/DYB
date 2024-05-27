using api_001_prueba_cajanegra.Data;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace api_001_prueba_cajanegra.Model
{
    public class DocumentPropertiesGET
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

        [XmlElement("RNCEmisor")]
        public string ClientId { get; set; }
        [XmlElement("DocumentName")]
      
        public string DayOfMonth { get; set; }
        [XmlElement("Hour")]
        public string Hour { get; set; }
        [XmlElement("Guid")]
        public string Guid { get; set; }
        public string StoragePath { get; set; }

        private readonly IDatabase _db;
        private readonly RedisConfiguration _redisConfiguration;

        public DocumentPropertiesGET() { }

        public DocumentPropertiesGET(RedisConfiguration config)
        {
            _redisConfiguration = config;
            var configuration = new ConfigurationOptions
            {
                EndPoints = { $"{_redisConfiguration.RedisHost}:{_redisConfiguration.RedisPort}" },
                Password = _redisConfiguration.RedisPassword,
                User = _redisConfiguration.RedisUsername
            };
            var redis = ConnectionMultiplexer.Connect(configuration);
            _db = redis.GetDatabase();
        }

        public async Task<DocumentPropertiesGET> GetByGuid(string guid)
        {
            var properties = await _db.HashGetAllAsync($"{_redisConfiguration.KeyPrefix}:properties:{guid}");
            if (properties.Length == 0)
            {
                return null;
            }

            var dict = new Dictionary<string, string>();
            foreach (var entry in properties)
            {
                dict[entry.Name] = entry.Value;
            }

            return JsonConvert.DeserializeObject<DocumentPropertiesGET>(JsonConvert.SerializeObject(dict));
        }
    }
}
