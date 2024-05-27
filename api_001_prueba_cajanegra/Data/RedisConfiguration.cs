using api_001_prueba_cajanegra.Model;
using ServiceStack.Redis;

namespace api_001_prueba_cajanegra.Data
{
    public class RedisConfiguration
    {
        public string RedisHost { get; set; }
        public string RedisPort { get; set; }
        public string RedisUsername { get; set; }
        public string RedisPassword { get; set; }
        public string StorageBasePath { get; set; }
        public string KeyPrefix { get; set; } = "dgii:document";
    }
}

