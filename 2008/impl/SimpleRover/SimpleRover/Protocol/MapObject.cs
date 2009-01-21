using System;
using System.Diagnostics;

namespace SimpleRover.Protocol
{
    public class MapObject
    {
        private readonly MapObjectKind kind;
        private readonly Double x;
        private readonly Double y;
        private readonly Double r;

        public MapObject(MapObjectKind kind, Double x, Double y, Double r)
        {
            this.kind = kind;
            this.x = x;
            this.y = y;
            this.r = r;
        }

        public MapObjectKind Kind
        {
            [DebuggerStepThrough]
            get { return kind; }
        }

        public Double X
        {
            [DebuggerStepThrough]
            get { return x; }
        }

        public Double Y
        {
            [DebuggerStepThrough]
            get { return y; }
        }

        public Double R
        {
            [DebuggerStepThrough]
            get { return r; }
        }
    }

    public enum MapObjectKind
    {
        Boulder,
        Crater,
        Home,
        Martian
    }

    public class Martian: MapObject
    {
        private readonly Double dir;
        private readonly Double speed;

        public Martian(Double x, Double y, Double dir, Double speed)
            : base(MapObjectKind.Martian, x, y, 0.4)
        {
            this.dir = dir;
            this.speed = speed;
        }

        public double Dir
        {
            [DebuggerStepThrough]
            get { return dir; }
        }

        public double Speed
        {
            [DebuggerStepThrough]
            get { return speed; }
        }
    }
}