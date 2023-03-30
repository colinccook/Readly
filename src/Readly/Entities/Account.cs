namespace Readly.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<MeterReading> MeterReadings { get; set; }

        public void AddMeterReading(MeterReading meterReading)
        {
            if (!CanAcceptReadingFrom(meterReading.MeterReadingDateTime))
                throw new InvalidOperationException($"Customer {AccountId} has an existing reading that is greater than {meterReading.MeterReadingDateTime}");

            MeterReadings.Add(meterReading);
        }

        public bool CanAcceptReadingFrom(DateTime readingDateTime)
        {
            if (MeterReadings == null || !MeterReadings.Any())
                return true;

            return readingDateTime > MeterReadings.Max(x => x.MeterReadingDateTime);
        }
    }
}
