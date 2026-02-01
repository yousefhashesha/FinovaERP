using System;
using System.IO;
using System.Text.Json;
using Finova.Presentation.WinForms.Config;
using Microsoft.Extensions.Configuration;

namespace Finova.Presentation.WinForms.Bootstrap
{
    public static class ConfigLoader
    {
        public static IConfiguration BuildConfiguration()
        {
            // Base directory = output folder (bin/Debug/...)
            var basePath = AppContext.BaseDirectory;

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static ModulesConfig LoadModulesConfig()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Config", "modules.json");
            return LoadJsonFile<ModulesConfig>(path) ?? new ModulesConfig();
        }

        public static PermissionsConfig LoadPermissionsConfig()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Config", "permissions.json");
            return LoadJsonFile<PermissionsConfig>(path) ?? new PermissionsConfig();
        }

        private static T? LoadJsonFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
