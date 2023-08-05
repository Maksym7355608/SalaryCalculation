namespace Identity.Api.CollectionExtensions;

public static class WebHostBuilderExtensions
{

    public static IHostBuilder ConfigureSettings(this ConfigureHostBuilder builder, string fileName)
    {
        return builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;
            if (env.IsDevelopment())
            {
                var folder = AppDomain.CurrentDomain.BaseDirectory;
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.json"), true);
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.{env.EnvironmentName}.json"), true);
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.secrets.json"), true);
                return;
            }

            config.AddJsonFile($"{fileName}.json", true);
            config.AddJsonFile($"{fileName}.{env.EnvironmentName}.json", true);
            config.AddJsonFile(Path.Combine("secrets", $"{fileName}.secrets.json"), true);
        });
    }
}