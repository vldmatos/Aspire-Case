
namespace Library.Business
{
    public class Mapping
    {
        private static List<Sensor>? _sensors;
        public static List<Sensor> Sensors 
        { 
            get 
            { 
                _sensors ??= MapAllSensors();

                return _sensors;
            } 
        }
        private static List<Sensor> MapAllSensors()
        {
            List<Sensor> initialize = [];

            for (var i = 0; i < 10; i++)
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
