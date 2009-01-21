// InitData.cs created with MonoDevelop
// User: lattyf at 13:14 16.07.2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Text;
using System.Globalization;

namespace SimpleRover.Protocol
{
    public class InitData
    {
        private readonly Double dx;
        private readonly Double dy;
        private readonly Int32 timeLimit;
        private readonly Double minSensor;
        private readonly Double maxSensor;
        private readonly Double maxSpeed;
        private readonly Double maxTurn;
        private readonly Double maxHardTurn;

        public InitData(String initString)
        {
            String[] splittedString = initString.Split(new Char[] {' '});

            //TODO: Оптимизация.
            CultureInfo culture = new CultureInfo("en-US");

            dx = Double.Parse(splittedString[0], culture);
            dy = Double.Parse(splittedString[1], culture);
            timeLimit = Int32.Parse(splittedString[2], culture);
            minSensor = Double.Parse(splittedString[3], culture);
            maxSensor = Double.Parse(splittedString[4], culture);
            maxSpeed = Double.Parse(splittedString[5], culture);
            maxTurn = Double.Parse(splittedString[6], culture);
            maxHardTurn = Double.Parse(splittedString[7], culture);
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder(GetType().Name);

            result.AppendFormat("[dx={0} m, dy={1} m, timeLimit={2} msec, minSensor={3} m, " +
                                "maxSensor={4} m, maxSpeed={5} m/sec, " +
                                "maxTurn={6} deg/sec, maxHardTurn={7} deg/sec]",
                                dx, dy, timeLimit, minSensor, maxSensor, maxSpeed, maxTurn, maxHardTurn);

            return result.ToString();
        }
    }
}