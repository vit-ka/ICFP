using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;

namespace RnaRunner
{
    /// <summary>
    /// Class to produce picture from RNA.
    /// </summary>
    public class RnaRunner
    {
        private readonly object _stateMutex = new object();
        private int _currentIndexOfRna;
        private readonly string _sourceRna;

        private IList<Color> _bucket;

        private RnaRunnerState _state;

        private IList<Bitmap> _bitmaps;

        private static readonly Color _black = Color.FromArgb(0, 0, 0);
        private static readonly Color _red = Color.FromArgb(255, 0, 0);
        private static readonly Color _green = Color.FromArgb(0, 255, 0);
        private static readonly Color _yellow = Color.FromArgb(255, 255, 0);
        private static readonly Color _blue = Color.FromArgb(0, 0, 255);
        private static readonly Color _magenta = Color.FromArgb(255, 0, 255);
        private static readonly Color _cyan = Color.FromArgb(0, 255, 255);
        private static readonly Color _white = Color.FromArgb(255, 255, 255);

        private static readonly Color _transparent = Color.FromArgb(0, _white);
        private static readonly Color _opaque = Color.FromArgb(255, _white);

        private Point _position;
        private Point _mark;

        private Size _defaultSize = new Size(600, 600);

        private enum Direction
        {
            N, E, S, W
        }

        private Direction _direction;
        private Color _currentColor;

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
        public Bitmap Bitmap
        {
            get
            {
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
            thread.Start();

            _currentIndexOfRna = 0;
            _bitmaps = new List<Bitmap>();
            _position = new Point(0, 0);
            _mark = new Point(0, 0);
            _direction = Direction.E;
            _bucket = new List<Color>();
            _bitmaps.Add(CreateTransperentBitmap());
        }

        private Bitmap CreateTransperentBitmap()
        {
            return new Bitmap(_defaultSize.Width, _defaultSize.Height, PixelFormat.Format32bppArgb);
        }

        private void ProcessRna()
        {
            while (_currentIndexOfRna + 7 < _sourceRna.Length)
            {
                var command = _sourceRna.Substring(_currentIndexOfRna, 7);

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
                        _bucket = new List<Color>();
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

        private void Clip()
        {
            if (_bitmaps.Count >= 2)
            {
                for (int x = 0; x < 600; ++x)
                    for (int y = 0; y < 600; ++y)
                    {
                        var color0 = _bitmaps[0].GetPixel(x, y);
                        var color1 = _bitmaps[1].GetPixel(x, y);

                        _bitmaps[1].SetPixel(
                            x,
                            y,
                            Color.FromArgb(
                                (color1.A * color0.A) / 255,
                                (color1.R * color0.A) / 255,
                                (color1.G * color0.A) / 255,
                                (color1.B * color0.A) / 255));
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
                        var color0 = _bitmaps[0].GetPixel(x, y);
                        var color1 = _bitmaps[1].GetPixel(x, y);

                        _bitmaps[1].SetPixel(
                            x,
                            y,
                            Color.FromArgb(
                                color0.A + (color1.A * (255 - color0.A)) / 255,
                                color0.R + (color1.R * (255 - color0.A)) / 255,
                                color0.G + (color1.G * (255 - color0.A)) / 255,
                                color0.B + (color1.B * (255 - color0.A)) / 255));
                    }

                _bitmaps.RemoveAt(0);
            }
        }

        private void AddBitmap(Bitmap bitmap)
        {
            if (_bitmaps.Count < 10)
            {
                _bitmaps.Insert(0, bitmap);
            }
        }

        private void TryFill()
        {
            var newColor = _currentColor;
            var oldColor = GetPixel(_position);

            if (newColor != oldColor)
            {
                Fill(_position, oldColor);
            }
        }

        private void Fill(Point position, Color oldColor)
        {
            var front = new Queue<Point>();
            front.Enqueue(position);

            while (front.Count > 0)
            {
                var point = front.Dequeue();

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

        private Color GetPixel(Point position)
        {
            return _bitmaps[0].GetPixel(position.X, position.Y);
        }

        private void Line(Point position, Point mark)
        {
            var deltaX = mark.X - position.X;
            var deltaY = mark.Y - position.Y;

            var d = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

            int c = deltaX * deltaY <= 0 ? 1 : 0;

            var x = position.X * d + (d - c) / 2;
            var y = position.Y * d + (d - c) / 2;

            for (int index = 0; index < d; ++index)
            {
                SetPixel(x / d, y / d);

                x += deltaX;
                y += deltaY;
            }

            SetPixel(mark.X, mark.Y);
        }

        private void SetPixel(int x, int y)
        {
            _bitmaps[0].SetPixel(x, y, _currentColor);
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
                    return new Point((position.X + 1) % 600, position.Y);
                case Direction.S:
                    return new Point(position.X, (position.Y + 1) % 600);
                case Direction.W:
                    return new Point(position.X != 0 ? position.X - 1 : 599, position.Y);
            }

            return new Point();
        }

        private void AddColorToBucket(Color color)
        {
            _bucket.Add(color);
            EvaluateCurrentColor();
        }

        private void EvaluateCurrentColor()
        {
            var rC = _bucket.Sum(color => color.R) / _bucket.Count;
            var gC = _bucket.Sum(color => color.G) / _bucket.Count;
            var bC = _bucket.Sum(color => color.B) / _bucket.Count;
            var aC = _bucket.Sum(color => color.A) / _bucket.Count;

            _currentColor = Color.FromArgb(rC * aC / 255, gC * aC / 255, bC * aC / 255, aC);
        }

        #region Nested type: RnaRunnerState

        private enum RnaRunnerState
        {
            Running,
            Stoped
        }

        #endregion
    }
}