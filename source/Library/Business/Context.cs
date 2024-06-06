namespace Library.Business
{
    public class Context
    {
        private const int total = 10;

        private static List<Sensor>? _sensors;
        public static List<Sensor> Sensors
        {
            get
            {
                _sensors ??= GenerateSensors();

                return _sensors;
            }
        }
        private static List<Sensor> GenerateSensors()
        {
            var initialize = new List<Sensor>(total);

            for (var i = 0; i < total; i++)
            {
                string name = Guid.NewGuid().ToString().ToUpper()[..6];
                int max = Random.Shared.Next(60, 80);
                int min = Random.Shared.Next(20, 40);
                var calibration = new Calibration(min, max);

                initialize.Add(new Sensor
                {
                    Name = name,
                    Calibration = calibration,
                    Pressure = Random.Shared.Next(calibration.Min, calibration.Max)
                });
            }

            return initialize;
        }
    }
}
