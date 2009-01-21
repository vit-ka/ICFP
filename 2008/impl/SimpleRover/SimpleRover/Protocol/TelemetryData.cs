// TelemetryData.cs created with MonoDevelop
// User: lattyf at 13:44 16.07.2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleRover.Protocol
{
    public class TelemetryData
    {
        private readonly Int32 timeStamp;
        private readonly RoverState vehicleCtl;
        private readonly Double vehicleX;
        private readonly Double vehicleY;
        private readonly Double vehicleDir;
        private readonly Double vehicleSpeed;
        private readonly IList<MapObject> objects;

        public TelemetryData(String initString)
        {
            String[] splittedString = initString.Split(new Char[] {' '}, 7);

            //TODO: Оптимизация.
            CultureInfo culture = new CultureInfo("en-US");

            timeStamp = Int32.Parse(splittedString[0], culture);
            vehicleCtl = RoverState.Parse(splittedString[1]);
            vehicleX = Double.Parse(splittedString[2], culture);
            vehicleY = Double.Parse(splittedString[3], culture);
            vehicleDir = Double.Parse(splittedString[4], culture);
            vehicleSpeed = Double.Parse(splittedString[5], culture);

            objects = new List<MapObject>();

            foreach (String str in LazyParseObjectStrings(splittedString[6]))
            {
                MapObject newObject = MapObjectFactory.Parse(str);
                objects.Add(newObject);
            }
        }

        private static IEnumerable<String> LazyParseObjectStrings(String initString)
        {
            String[] splittedString = initString.Split(' ');

            StringBuilder partialResult = new StringBuilder();
            Int32 index = 0;

            while (index < splittedString.Length)
            {
                if (Char.IsLetter(splittedString[index][0]) && index != 0)
                {
                    yield return partialResult.ToString();
                    partialResult = new StringBuilder();
                }
                partialResult.Append(splittedString[index]).Append(' ');

                ++index;
            }
        }

        public override String ToString()
        {
            return String.Format("TimeStamp: {0}, VehicleCtl: {1}, VehicleX: {2}, VehicleY: {3}, VehicleDir: {4}, VehicleSpeed: {5}, Objects: {6}",
                timeStamp, vehicleCtl, vehicleX, vehicleY, vehicleDir, vehicleSpeed, objects);
        }
    }
}