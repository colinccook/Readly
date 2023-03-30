namespace Readly.Entities
{
    public class MeterReading
    {
        public int MeterReadingId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }

        public static bool IsValidMeterReadValue(string? meterReadValue)
        {
            return meterReadValue != null && meterReadValue.Any() && meterReadValue.Length == 5 && meterReadValue.All(c => Char.IsDigit(c));
        }
    }
}
