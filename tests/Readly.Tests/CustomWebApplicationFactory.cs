using CsvHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Readly.Entities;
using Readly.Persistence;
using System.Globalization;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private IServiceScope scope => Services.GetService<IServiceScopeFactory>().CreateScope();
    public ReadlyContext DatabaseContext => scope.ServiceProvider.GetService<ReadlyContext>()!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Keep a connection open to the database so that it remains available for the integration tests
            //services.AddSingleton<DbConnection>(container =>
            //{
            //    var connection = new SqliteConnection("DataSource=file::memory:?cache=shared");
            //    connection.Open();

            //    return connection;
            //});

            // Seed database with customers, also ensuring that the tables have been reset to seed data defaults
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            using (var reader = new StreamReader(@"TestFiles\Test_Accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<ReadlyContext>();

                // context.Database.EnsureCreated();

                context.MeterReadings.RemoveRange(context.MeterReadings);
                context.Customers.RemoveRange(context.Customers);
                context.SaveChanges();

                var seedCustomers = csv.GetRecords<Account>();
                context.Customers.AddRange(seedCustomers);
                context.SaveChanges();
            }
        });
    }
}