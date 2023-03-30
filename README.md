## Introduction

Readly - an API to import a CSV of meter readings with basic validation

## Running on your machine

Requires .NET 6

- Clone the repository
- Open ```Readly.sln``` in the root with Visual Studio 2022, or:
- Call ```dotnet test tests/Readly.Tests``` to ensure all tests are passing
- Call ```dotnet run --project src/Readly``` and open the provided URL

## Implementation

Minimal API has been used and serves two purposes:
 - An importing endpoint, that should be POSTed to to import meter reading records
 - A static file host, to host a rudamentory page for hitting the import endpoint from the browser

Due to not having a machine with SQL Server installed, EF Core with SQLLite (in memory) is used for persistence. 

## Testing

I have used a mixture of:
 - Unit tests, to validate business logic
 - In-memory Integration tests, to validate orchestration logic in the API. The API and database are completely hosted in memory and seeded before tests.

 ## Requirements

 Listed below are the requirements along with their supporting tests:

| Requirement  | Notes |
| ----------- | ----------- |
| Please seed the Test_Accounts.csv data into your chosen data storage technology and validate the Meter Read data against the accounts. | ⚠️ Test Accounts are seeded in the integration test code, but due to a lack of SQL Server I haven't written a means of seeding data outside of the integration tests |
| Create the following endpoint: POST => /meter-reading-uploads | ✅ Endpoint is defined as per the requirement |
| The endpoint should be able to process a CSV of meter readings. An example CSV file has been provided (Meter_reading.csv) | ✅ Example integration test featuring the same Meter_reading.csv file has been created | 
| Each entry in the CSV should be validated and if valid, stored in a DB. |✅ Multiple unit tests cover the meter read format and whether an account already has a read in the future. An integration test confirms the write into the database |
| After processing, the number of successful/failed readings should be returned |✅ A JSON response is given with the number of successful and unsuccessful reads, the UI shows this in an alert box  |
| You should not be able to load the same entry twice |✅ An integration test confirms that the first entry is imported successfully, and the second entry is rejected |
| A meter reading must be associated with an Account ID to be deemed valid |✅ An integration test confirms that AccountIDs that do not match are rejected |
| Reading values should be in the format NNNNN |⚠️ The API checks for exactly 5 digits. Many reads in the import file have four digits, and *it has been assumed these are to be ignored*. This is supported by unit tests. |
| Create a client in the technology of your choosing to consume the API. You can use angular/react/whatever you like |✅ A rudimentary static HTML page has been written that hits the import endpoint. |
| When an account has an existing read, ensure the new read isn’t older than the existing read | ✅ If the DateTime of an accounts latest read is newer than the read being imported, it is rejected. A unit test supports this business rule.





