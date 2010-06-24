using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace RnaRunner
{
    /// <summary>
    /// Class to produce picture from RNA.
    /// </summary>
    public class RnaRunner
    {
        private const byte _transparent = 0;
        private const byte _opaque = 255;
        private static readonly Color _black = Color.FromArgb(0, 0, 0);
        private static readonly Color _red = Color.FromArgb(255, 0, 0);
        private static readonly Color _green = Color.FromArgb(0, 255, 0);
        private static readonly Color _yellow = Color.FromArgb(255, 255, 0);
        private static readonly Color _blue = Color.FromArgb(0, 0, 255);
        private static readonly Color _magenta = Color.FromArgb(255, 0, 255);
        private static readonly Color _cyan = Color.FromArgb(0, 255, 255);
        private static readonly Color _white = Color.FromArgb(255, 255, 255);
        private readonly string _sourceRna;
        private readonly object _stateMutex = new object();
        private IList<PixelMap> _bitmaps;
        private IList<byte> _bucketAlpha;
        private IList<Color> _bucketColor;
        private Pixel _currentColor;
        private int _currentIndexOfRna;

        private Size _defaultSize = new Size(600, 600);

        private Direction _direction;
        private Point _mark;
        private Point _position;
        private RnaRunnerState _state;

        /// <summary>
        /// Constructor of RNA processor.
        /// </summary>
        /// <param name="rnaStream">Stream with RNA.</param>
        public RnaRunner(Stream rnaStream)
        {
            if (rnaStream.CanSeek)
                rnaStream.Seek(0, SeekOrigin.Begin);

            _sourceRna = new StreamReader(rnaStream).ReadToEnd();

            _state = RnaRunnerState.Stoped;
        }

        /// <summary>
        /// Image produced from RNA.
        /// </summary>
        public PixelMap PixelMap
        {
            get
            {
//                var result = new Bitmap(_defaultSize.Width, _defaultSize.Height, PixelFormat.Format32bppArgb);
//                for (int x = 0; x < 600; ++x)
//                    for (int y = 0; y < 600; ++y )
//                        result.SetPixel(x, y, _bitmaps[0][x, y].Color);

                return _bitmaps[0];
            }
        }

        ///<summary>
        /// Raises if some draw commands will be processed.
        ///</summary>
        public event EventHandler<EventArgs> SomeDrawCommandsExecuted;

        private void InvokeSomeDrawCommandsExecuted()
        {
            EventHandler<EventArgs> handler = SomeDrawCommandsExecuted;
            if (handler != null)
                handler(this, new EventArgs());
        }

        /// <summary>
        /// Raises if execution is finished.
        /// </summary>
        public event EventHandler<EventArgs> ExecutionFinished;

        private void InvokeExecutionFinished()
        {
            EventHandler<EventArgs> handler = ExecutionFinished;
            if (handler != null)
                handler(this, new EventArgs());
        }

        /// <summary>
        /// Start processing.
        /// </summary>
        public void Start()
        {
            lock (_stateMutex)
            {
                if (_state == RnaRunnerState.Running)
                    return;

                _state = RnaRunnerState.Running;
            }

            var thread = new Thread(ProcessRna)
                             {
                                 IsBackground = true
                             };
          
            _currentIndexOfRna = 0;
            _bitmaps = new List<PixelMap>();
            _position = new Point(0, 0);
            _mark = new Point(0, 0);
            _direction = Direction.E;
            _bucketColor = new List<Color>();
            _bucketAlpha = new List<byte>();
            _bitmaps.Add(CreateTransperentBitmap());

            EvaluateCurrentColor();

            thread.Start();
        }

        private static PixelMap CreateTransperentBitmap()
        {
            var result = new PixelMap();

            for (int x = 0; x < 600; ++x)
                for (int y = 0; y < 600; ++y)
                {
                    result[x,y] = new Pixel(_black, _transparent);
                }

            return result;
        }

        private void ProcessRna()
        {
            while (_currentIndexOfRna + 7 < _sourceRna.Length)
            {
                string command = _sourceRna.Substring(_currentIndexOfRna, 7);

                switch (command)
                {
                    case "PIPIIIC":
                        AddColorToBucket(_black);
                        break;
                    case "PIPIIIP":
                        AddColorToBucket(_red);
                        break;
                    case "PIPIICC":
                        AddColorToBucket(_green);
                        break;
                    case "PIPIICF":
                        AddColorToBucket(_yellow);
                        break;
                    case "PIPIICP":
                        AddColorToBucket(_blue);
                        break;
                    case "PIPIIFC":
                        AddColorToBucket(_magenta);
                        break;
                    case "PIPIIFF":
                        AddColorToBucket(_cyan);
                        break;
                    case "PIPIIPC":
                        AddColorToBucket(_white);
                        break;
                    case "PIPIIPF":
                        AddColorToBucket(_transparent);
                        break;
                    case "PIPIIPP":
                        AddColorToBucket(_opaque);
                        break;
                    case "PIIPICP":
                        _bucketColor = new List<Color>();
                        _bucketAlpha = new List<byte>();
                        EvaluateCurrentColor();
                        break;
                    case "PIIIIIP":
                        _position = Move(_position, _direction);
                        break;
                    case "PCCCCCP":
                        _direction = TurnCounterClockwise(_direction);
                        break;
                    case "PFFFFFP":
                        _direction = TurnClockwise(_direction);
                        break;
                    case "PCCIFFP":
                        _mark = _position;
                        break;
                    case "PFFICCP":
                        Line(_position, _mark);
                        InvokeSomeDrawCommandsExecuted();
                        break;
                    case "PIIPIIP":
                        TryFill();
                        InvokeSomeDrawCommandsExecuted();
                        break;
                    case "PCCPFFP":
                        AddBitmap(CreateTransperentBitmap());
                        InvokeSomeDrawCommandsExecuted();
                        break;
                    case "PFFPCCP":
                        Compose();
                        InvokeSomeDrawCommandsExecuted();
                        break;
                    case "PFFICCF":
                        Clip();
                        InvokeSomeDrawCommandsExecuted();
                        break;
                }

                _currentIndexOfRna += 7;
            }

            InvokeExecutionFinished();
        }

        private void AddColorToBucket(byte color)
        {
            _bucketAlpha.Insert(0, color);
            EvaluateCurrentColor();
        }

        private void Clip()
        {
            if (_bitmaps.Count >= 2)
            {
                for (int x = 0; x < 600; ++x)
                    for (int y = 0; y < 600; ++y)
                    {
                        Pixel p0 = _bitmaps[0][x, y];
                        Pixel p1 = _bitmaps[1][x, y];

                        _bitmaps[1][x, y].Color = Color.FromArgb(p1.Color.R*p0.Transparency/255,
                                                                 p1.Color.G*p0.Transparency/255,
                                                                 p1.Color.B*p0.Transparency/255);
                        _bitmaps[1][x, y].Transparency =
                            (byte) (p1.Transparency*p0.Transparency/255);
                    }

                _bitmaps.RemoveAt(0);
            }
        }

        private void Compose()
        {
            if (_bitmaps.Count >= 2)
            {
                for (int x = 0; x < 600; ++x)
                    for (int y = 0; y < 600; ++y)
                    {
                        Pixel p0 = _bitmaps[0][x, y];
                        Pixel p1 = _bitmaps[1][x, y];

                        _bitmaps[1][x, y].Color = Color.FromArgb(p0.Color.R + p1.Color.R*(255 - p0.Transparency)/255,
                                                                 p0.Color.G + p1.Color.G*(255 - p0.Transparency)/255,
                                                                 p0.Color.B + p1.Color.B*(255 - p0.Transparency)/255);
                        _bitmaps[1][x, y].Transparency = 
                                                     (byte) (p0.Transparency + p1.Transparency*(255 - p0.Transparency)/255);
                    }

                _bitmaps.RemoveAt(0);
            }
        }

        private void AddBitmap(PixelMap bitmap)
        {
            if (_bitmaps.Count < 10)
            {
                _bitmaps.Insert(0, bitmap);
            }
        }

        private void TryFill()
        {
            Pixel newColor = _currentColor;
            Pixel oldColor = GetPixel(_position);

            if (newColor != oldColor)
            {
                Fill(_position, oldColor);
            }
        }

        private void Fill(Point position, Pixel oldColor)
        {
            var front = new Queue<Point>();
            front.Enqueue(position);

            while (front.Count > 0)
            {
                Point point = front.Dequeue();

                if (GetPixel(point) == oldColor)
                {
                    SetPixel(point.X, point.Y);
                    if (point.X > 0)
                        front.Enqueue(new Point(point.X - 1, point.Y));
                    if (point.X < 599)
                        front.Enqueue(new Point(point.X + 1, point.Y));
                    if (point.Y > 0)
                        front.Enqueue(new Point(point.X, point.Y - 1));
                    if (point.Y < 599)
                        front.Enqueue(new Point(point.X, point.Y + 1));
                }
            }
        }

        private Pixel GetPixel(Point position)
        {
            return _bitmaps[0][position.X, position.Y];
        }

        private void Line(Point position, Point mark)
        {
            int deltaX = mark.X - position.X;
            int deltaY = mark.Y - position.Y;

            int d = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

            int c = deltaX*deltaY <= 0 ? 1 : 0;

            int x = position.X*d + (d - c)/2;
            int y = position.Y*d + (d - c)/2;

            for (int index = 0; index < d; ++index)
            {
                SetPixel(x/d, y/d);

                x += deltaX;
                y += deltaY;
            }

            SetPixel(mark.X, mark.Y);
        }

        private void SetPixel(int x, int y)
        {
            _bitmaps[0][x, y] = _currentColor.Clone();
        }

        private Direction TurnClockwise(Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return Direction.E;
                case Direction.E:
                    return Direction.S;
                case Direction.S:
                    return Direction.W;
                case Direction.W:
                    return Direction.N;
            }

            return Direction.W;
        }

        private Direction TurnCounterClockwise(Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return Direction.W;
                case Direction.E:
                    return Direction.N;
                case Direction.S:
                    return Direction.E;
                case Direction.W:
                    return Direction.S;
            }

            return Direction.W;
        }

        private Point Move(Point position, Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return new Point(position.X, position.Y != 0 ? position.Y - 1 : 599);
                case Direction.E:
                    return new Point((position.X + 1)%600, position.Y);
                case Direction.S:
                    return new Point(position.X, (position.Y + 1)%600);
                case Direction.W:
                    return new Point(position.X != 0 ? position.X - 1 : 599, position.Y);
            }

            return new Point();
        }

        private void AddColorToBucket(Color color)
        {
            _bucketColor.Insert(0, color);
            EvaluateCurrentColor();
        }

        private void EvaluateCurrentColor()
        {
            double rC = Math.Floor(_bucketColor.Sum(color => color.R)/(double) _bucketColor.Count);
            double gC = Math.Floor(_bucketColor.Sum(color => color.G)/(double) _bucketColor.Count);
            double bC = Math.Floor(_bucketColor.Sum(color => color.B)/(double) _bucketColor.Count);
            double aC = Math.Floor(_bucketAlpha.Sum(color => color)/(double) _bucketAlpha.Count);

            if (_bucketAlpha.Count == 0)
                aC = 255;

            if (_bucketColor.Count == 0)
            {
                rC = gC = bC = 0;
            }

            _currentColor = new Pixel(Color.FromArgb((int) Math.Floor(rC*aC/255.0),
                                                     (int) Math.Floor(gC*aC/255.0),
                                                     (int) Math.Floor(bC*aC/255.0)), (byte) Math.Floor(aC));
        }

        #region Nested type: Direction

        private enum Direction
        {
            N,
            E,
            S,
            W
        }

        #endregion

        #region Nested type: RnaRunnerState

        private enum RnaRunnerState
        {
            Running,
            Stoped
        }

        #endregion
    }
}