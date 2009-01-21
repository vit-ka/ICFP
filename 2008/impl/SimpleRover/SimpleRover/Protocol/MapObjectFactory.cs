using System;
using System.Globalization;

namespace SimpleRover.Protocol
{
    public class MapObjectFactory
    {
        public static MapObject Parse(String initString)
        {
            initString = initString.Trim();
            Char objectType = initString[0];
            initString = initString.Substring(2);

            String[] splittedString = initString.Split(new[] {' '});

            //TODO: Оптимизация.
            CultureInfo culture = new CultureInfo("en-US");

            Double x = Double.Parse(splittedString[0], culture);
            Double y = Double.Parse(splittedString[1], culture);
            Double r = Double.Parse(splittedString[2], culture);

            MapObject result;

            switch (objectType)
            {
                case 'b':
                    result = new MapObject(MapObjectKind.Boulder, x, y, r);
                    break;
                case 'c':
                    result = new MapObject(MapObjectKind.Crater, x, y, r);
                    break;
                case 'h':
                    result = new MapObject(MapObjectKind.Home, x, y, r);
                    break;
                case 'm':
                    Double speed = Double.Parse(splittedString[3], culture);
                    result = new Martian(x, y, r /*Из-за совпадения типа данных передаем направление как радиус*/
                                         , speed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Illegel type of object: " + objectType);
            }

            return result;
        }
    }
}