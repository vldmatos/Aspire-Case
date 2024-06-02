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
    }

    public record Calibration(int Min, int Max);
}
