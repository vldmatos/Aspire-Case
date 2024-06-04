namespace Library.Business
{
    public class Sensor
    {
        public string Name { get; set; } = null!;

        public Calibration Calibration { get; set; } = null!;

        public int Pressure { get; set; }

        public bool IsCalibrate =>
            Pressure >= Calibration.Min &&
            Pressure <= Calibration.Max;

        public bool Maintenance { get; set; } = false;

        public Sensor Calibrate()
        {
            Pressure += CalculeBalance();
            return this;
        }

        public int CalculeBalance()
        {
            if (Pressure < Calibration.Min)
            {
                return (Calibration.Min - Pressure) + (Calibration.Max - Calibration.Min) + 1;
            }

            if (Pressure > Calibration.Max)
            {
                return ((Pressure - Calibration.Max) + (Calibration.Max - Calibration.Min) + 1) * -1;
            }

            return 0;
        }
    }

    public record Calibration(int Min, int Max);
}
