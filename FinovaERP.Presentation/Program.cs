using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using FinovaERP.Presentation.Forms;

namespace FinovaERP.Presentation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms. Application.EnableVisualStyles();
            System.Windows.Forms. Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                var services = ConfigureServices();
                var serviceProvider = services.BuildServiceProvider();
                
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                System.Windows.Forms. Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            try
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
                
                // Register forms
                services.AddTransient<LoginForm>();
                services.AddTransient<DashboardForm>();
                services.AddTransient<MainForm>();
                
                return services;
            }
            catch
            {
                // Fallback configuration
                var fallbackConfig = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:FinovaDbConnection"] = "Server=localhost\\SQLEXPRESS;Database=FinovaERP;Trusted_Connection=true;TrustServerCertificate=true;"
                    })
                    .Build();

                services.AddSingleton<IConfiguration>(fallbackConfig);
                services.AddTransient<LoginForm>();
                services.AddTransient<DashboardForm>();
                services.AddTransient<MainForm>();
                
                return services;
            }
        }
    }
}
