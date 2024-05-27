using api_001_prueba_cajanegra.Data;
using api_001_prueba_cajanegra.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Xml.Linq;
using static api_001_prueba_cajanegra.Model.DocumentPropertiesPUT;

namespace api_001_prueba_cajanegra.Controllers
{
    [ApiController]
    [Route("api/xml-docs")]
    public class XmlDocsController : ControllerBase
    {
        private readonly RedisConfiguration _redisConfiguration;
        private readonly IConnectionMultiplexer _redis;

        public XmlDocsController(IOptions<RedisConfiguration> redisConfig, IConnectionMultiplexer redis)
        {
            _redisConfiguration = redisConfig.Value;
            _redis = redis;
        }

        [HttpGet("document/{documentId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(DocumentPropertiesGET), 410)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 422)]
        public async Task<IActionResult> GetDocument(string documentId)
        {
            var db = _redis.GetDatabase();
            var documentData = await db.StringGetAsync($"{_redisConfiguration.KeyPrefix}:raw:{documentId}");

            if (documentData.IsNullOrEmpty)
            {
                var storagePath = $"{_redisConfiguration.StorageBasePath}\\{documentId}.xml";
                bool fileExists = System.IO.File.Exists(storagePath);
                if (fileExists)
                {
                    try
                    {
                        var content = await System.IO.File.ReadAllTextAsync(storagePath);
                        return Ok(content);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error reading document from storage: {ex.Message}");
                    }
                }

                var properties = await new DocumentPropertiesGET(_redisConfiguration).GetByGuid(documentId);
                if (properties == null)
                {
                    return NotFound(new { message = "Document and its properties not found" });
                }

                return StatusCode(410, properties);
            }

            return Ok(documentData.ToString());
        }

        [HttpPut("document/{businessModel}/{year}/{month}/{dayOfMonth}/{hour}/{guid}")]
        [Consumes("application/xml")]
        [ProducesResponseType(typeof(DocumentPropertiesGET), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveDocument(
            string businessModel, string year, string month, string dayOfMonth, string hour, string guid,
            [FromBody] XElement documentXml)
        {
            Document document;
            try
            {
                var db = _redis.GetDatabase();
                document = new Document(documentXml, _redisConfiguration, db);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { message = $"Invalid XML: {e.Message}" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = $"Error parsing XML: {e.Message}" });
            }

            if (string.IsNullOrWhiteSpace(document.ClientId))
            {
                return BadRequest(new { message = "Could not extract client_id (RNCEmisor) from XML document" });
            }

            var businessModelType = businessModel.ToLower() switch
            {
                "b2c" => DocumentPropertiesPUT.BusinessModel.B2C,
                "b2b" => DocumentPropertiesPUT.BusinessModel.B2B,
                _ => throw new ArgumentException($"Invalid business model: {businessModel}")
            };

            var properties = new DocumentPropertiesPUT(_redisConfiguration)
            {

                BusinessModelType = businessModelType,
                Year = year,
                Month = month,
                DayOfMonth = dayOfMonth,
                Hour = hour,
                ClientId = document.ClientId,
                DocumentName = document.DocumentName,
                
                Guid = guid,
            };

            document.PropertiesData = properties;
            try
            {
                await document.SaveToRedis();
                await StorageHelper.SaveDocumentToStorageAsync(_redisConfiguration, properties, documentXml.ToString());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = $"Error saving document: {e.Message}" });
            }
            return Ok(new
            {
                businessModelType = properties.BusinessModelType.ToString(),

                year = properties.Year,
                month = properties.Month,
                dayOfMonth = properties.DayOfMonth,
                clientId = properties.ClientId,
                documentName = properties.DocumentName,
                hour = properties.Hour,
                guid = properties.Guid,
                storagePath = properties.StoragePath
            });
        }
    }
}
