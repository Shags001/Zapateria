using System.IO;
using System.Text.Json;

namespace ZapateriaWinForms.Utilities
{
    public static class ConfigHelper
    {
        public static string GetConnectionString()
        {
            var json = File.ReadAllText("appsettings.json");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return root.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString();
        }
    }
}
