using CsvHelper;
using Readly.DTOs;
using System.Globalization;
using Readly.Repositories;
using Readly.Entities;
using Microsoft.EntityFrameworkCore;
using Readly.Persistence;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment()) {
    builder.Services.AddDbContext<ReadlyContext>(opt => opt.UseSqlite(@"DataSource=file::memory:?cache=shared"));
}
else {
    // add configurations for real database connections here
}

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/meter-reading-uploads", async (HttpRequest request, IAccountRepository accountRepository) =>
{
    if (request.Form.Files.Count == 0 || request.Form.Files[0].Length == 0)
        return Results.BadRequest("Invalid CSV file selected");

    var result = new MeterReadingsUploadResult();

    // iterate through it
    using (var reader = new StreamReader(request.Form.Files[0].OpenReadStream()))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        await csv.ReadAsync();
        csv.ReadHeader();
        while (await csv.ReadAsync())
        {
            var accountId = csv.GetField<int>("AccountId");
            var meterReadingDateTime = csv.GetField("MeterReadingDateTime");
            var meterReadingValue = csv.GetField("MeterReadValue");

            var customer = await accountRepository.GetAccount(accountId);

            if (customer == null)
            {
                result.FailedReads++;
                continue;
            }

            DateTime meterReadingDateTimeParsed;
            if (!DateTime.TryParseExact(meterReadingDateTime, "dd/MM/yyyy HH:mm",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.AssumeLocal,
                           out meterReadingDateTimeParsed))
            {
                result.FailedReads++;
                continue;
            }

            if (!customer.CanAcceptReadingFrom(meterReadingDateTimeParsed))
            {
                result.FailedReads++;
                continue;
            }

            if (!MeterReading.IsValidMeterReadValue(meterReadingValue))
            {
                result.FailedReads++;
                continue;
            }

            var meterReading = new MeterReading { MeterReadingDateTime = meterReadingDateTimeParsed, MeterReadValue = meterReadingValue };
            customer.AddMeterReading(meterReading);

            result.SuccessfulReads++;
        }
    }

    await accountRepository.Save();

    return Results.Ok(result);
});

app.Run();