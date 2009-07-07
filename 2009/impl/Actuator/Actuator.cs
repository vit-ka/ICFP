using System;
using System.Collections.Generic;
using ICFP2009.Common;
using ICFP2009.VirtualMachineLib;

namespace ICFP2009.Actuator
{
    public class Actuator
    {
        private static readonly object _instanceMutex = new object();
        private static Actuator _instance;
        private static double G = 6.67e-11;
        private static double M = 6.00e24;

        private bool _firstImpulseDone;
        private bool _secondImpulseDone;
        private double _hoffmanTime;
        private double _originalOrbitRadius;

        private Actuator()
        {
            Init();

            VirtualMachine.Instance.StateReseted += VMStateReseted;
            VirtualMachine.Instance.StepCompleted += VMStepCompleted;
        }

        private void Init()
        {
            Speed = new Vector(0, 0);
            Position = new Point(0, 0);
            DeltaSpeed = new Vector(0, 0);
            Track = new List<Point>();
        }

        public static Actuator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceMutex)
                        if (_instance == null)
                            _instance = new Actuator();
                }

                return _instance;
            }
        }

        public Vector Speed { get; private set; }

        public Vector DeltaSpeed { get; private set; }

        public Point Position { get; private set; }

        public IList<Point> Track { get; private set; }

        public int Time { get; private set; }

        public double DistanceToTargetOrbit { get; private set; }

        private void VMStepCompleted(object sender, StepCompletedEventArgs e)
        {
            DeltaSpeed.X -= VirtualMachine.Instance.Ports.Input[0x0002];
            DeltaSpeed.Y -= VirtualMachine.Instance.Ports.Input[0x0003];

            Position.X = -VirtualMachine.Instance.Ports.Output[0x0002];
            Position.Y = -VirtualMachine.Instance.Ports.Output[0x0003];

            if (Time == 0)
                _originalOrbitRadius = Math.Sqrt(Position.X * Position.X + Position.Y * Position.Y);

            Time += 1;

            DistanceToTargetOrbit = (Math.Sqrt(Position.X * Position.X + Position.Y * Position.Y) -
                                    VirtualMachine.Instance.Ports.Output[0x0004]) / 1000;

            DetermineSpeed();

            if (!_firstImpulseDone)
            {
                Vector transferImpulse = DetermineHoffmannTransferSpeed();

                VirtualMachine.Instance.Ports.Input[0x0002] = transferImpulse.X;
                VirtualMachine.Instance.Ports.Input[0x0003] = transferImpulse.Y;

                _firstImpulseDone = true;
                _hoffmanTime = DetermineHoffmannTransferTime();
            }
            else
            {
                VirtualMachine.Instance.Ports.Input[0x0002] = 0;
                VirtualMachine.Instance.Ports.Input[0x0003] = 0;
            }

            if (Time >= _hoffmanTime)
            {
                if (!_secondImpulseDone)
                {
                    Vector transferImpulse = DetermineHoffmannTransferSpeed2();

                    VirtualMachine.Instance.Ports.Input[0x0002] = transferImpulse.X;
                    VirtualMachine.Instance.Ports.Input[0x0003] = transferImpulse.Y;

                    _secondImpulseDone = true;
                }
                else
                {
                    VirtualMachine.Instance.Ports.Input[0x0002] = 0;
                    VirtualMachine.Instance.Ports.Input[0x0003] = 0;
                }
            }

            if (Time % 100 == 0)
                Track.Add(Position.Clone());
        }

        private Vector DetermineHoffmannTransferSpeed2()
        {
            double mu = G * M;

            double targetRadius = VirtualMachine.Instance.Ports.Output[0x0004];

            double v1 = Math.Sqrt(mu / targetRadius) *
                        (1 - Math.Sqrt(2 * _originalOrbitRadius / (targetRadius + _originalOrbitRadius)));


            return new Vector(0, v1);
        }

        private double DetermineHoffmannTransferTime()
        {
            double targetRadius = VirtualMachine.Instance.Ports.Output[0x0004];
            double mu = G * M;

            double result = Math.PI * Math.Sqrt(Math.Pow(_originalOrbitRadius + targetRadius, 3) / 8.0 / mu);

            return result;
        }

        private Vector DetermineHoffmannTransferSpeed()
        {
            double mu = G * M;

            double currentRadius = Math.Sqrt(Position.X * Position.X + Position.Y * Position.Y);
            double targetRadius = VirtualMachine.Instance.Ports.Output[0x0004];

            double v1 = Math.Sqrt(mu / currentRadius) *
                        (Math.Sqrt(2 * targetRadius / (targetRadius + currentRadius)) - 1);


            return new Vector(0, -v1);
        }

        private void DetermineSpeed()
        {
            if (Track.Count > 0)
            {
                Point lastPoint = Track[Track.Count - 1];
                Speed.X = Position.X - lastPoint.X;
                Speed.Y = Position.Y - lastPoint.Y;
            }
        }

        private void VMStateReseted(object sender, StateResetedEventArgs e)
        {
            Init();
        }
    }
}