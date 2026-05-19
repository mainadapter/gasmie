using Microsoft.Extensions.Configuration;

namespace gasmie.settings
{
    public static class Settings
    {
        private static IConfigurationRoot Setup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine("settings", "appsettings.json"), optional: false, reloadOnChange: true);
            
            return builder.Build();
        }

        public static (string, string, string) GetNotionConnectionStrings()
        {
            var settings = Setup();
            var url = settings.GetConnectionString("NotionUrl") ?? throw new InvalidOperationException("NotionUrl not configured");
            var id = settings.GetConnectionString("NotionDatabaseId") ?? throw new InvalidOperationException("NotionDatabaseId not configured");
            var key = settings.GetConnectionString("NotionKey") ?? throw new InvalidOperationException("NotionKey not configured");
            
            return (url, id, key);
        }
    }
}