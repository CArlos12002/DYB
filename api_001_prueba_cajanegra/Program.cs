using api_001_prueba_cajanegra.Data;
using api_001_prueba_cajanegra.Model;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json.Serialization;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add configuration from appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Add services to the container.
        builder.Services.AddControllers()
            .AddXmlSerializerFormatters()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure Redis
        builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("RedisSettings"));
        builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<RedisConfiguration>>().Value;
            var options = new ConfigurationOptions
            {
                EndPoints = { $"{config.RedisHost}:{config.RedisPort}" },
                Password = config.RedisPassword,
                User = config.RedisUsername,
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(options);
        });

        var app = builder.Build();

        // Create a scope to resolve the IConnectionMultiplexer
        using (var serviceScope = app.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            try
            {
                var redis = services.GetRequiredService<IConnectionMultiplexer>();
                if (redis.IsConnected)
                {
                    Console.WriteLine("Successfully connected to Redis.");
                }
                else
                {
                    Console.WriteLine("Failed to connect to Redis.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to Redis: {ex.Message}");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
