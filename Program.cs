using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvoiceApi.Data;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // SqlConnectionString is set by the Bicep template as an Azure app setting.
        // When running locally it is absent, so we fall back to SQLite.
        // This means 'func start' still works exactly as in Task 1.
        var sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

        if (!string.IsNullOrEmpty(sqlConnectionString))
        {
            // Running in Azure: use Azure SQL (SQL Server provider)
            services.AddDbContext<InvoiceDbContext>(options =>
                options.UseSqlServer(sqlConnectionString));
        }
        else
        {
            // Running locally: use SQLite (Task 1 behaviour unchanged)
            var sqliteConnStr = Environment.GetEnvironmentVariable("SqliteConnectionString")
                                ?? "Data Source=invoice.db";
            services.AddDbContext<InvoiceDbContext>(options =>
                options.UseSqlite(sqliteConnStr));
        }
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
    db.Database.EnsureCreated();
}

await host.RunAsync();
