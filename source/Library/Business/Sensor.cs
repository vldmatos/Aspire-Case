namespace Library.Business
{
    public class Sensor
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Max { get; set; }

        public int Min { get; set; }

        public int Pressure { get; set; }

        public bool IsCalibrate => 
            Pressure >= Min && Pressure <= Max;

        public bool Maintenance { get; set; } = false;

        public Sensor Calibrate()
        {
            Pressure += CalculeBalance();
            return this;
        }

        public int CalculeBalance()
        {
            if (Pressure < Min)
            {
                return ((Min - Pressure) + (Max - Min)) + 1;
            }

            if (Pressure > Max)
            {
                return ((Pressure - Max) + (Max - Min) + 1) * -1;
            }

            return 0;
        }
    }
}
