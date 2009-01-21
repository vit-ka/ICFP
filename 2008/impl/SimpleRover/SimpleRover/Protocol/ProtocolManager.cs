// Parser.cs created with MonoDevelop
// User: lattyf at 12:21 16.07.2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleRover.Protocol
{
    public class ProtocolManager
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(typeof (ProtocolManager));

        private Socket socket;

        public ProtocolManager(Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");

            this.socket = socket;
        }

        public void ReceiveAll()
        {
            Thread thread = new Thread(ReceiveAllImpl);
            thread.Start();
        }

        private void ReceiveAllImpl()
        {
            log.Info("Input stream parsing has began.");
            Byte[] buffer = new Byte[2048];
            Int32 readed = socket.Receive(buffer);

            while (readed > 0)
            {
                String receivedString = Encoding.ASCII.GetString(buffer, 0, readed);

                // Ищем тип сообщения. Должен быть первой буквой.
                Char messageType = receivedString[0];
                receivedString = receivedString.Substring(2);

                switch (messageType)
                {
                    case 'I':
                        InitData initData = new InitData(receivedString);
                        log.InfoFormat("Init: {0}", initData);
                        break;
                    case 'T':
                        TelemetryData teleData = new TelemetryData(receivedString);
                        log.InfoFormat("Telemetry: {0}", teleData);
                        break;
                    case 'E':
                        break;
                    case 'B':
                        break;
                    case 'C':
                        break;
                    case 'K':
                        break;
                    case 'S':
                        break;
                    default:
                        throw new ArgumentException("Illegal message type " + messageType + ".");
                }

                readed = socket.Receive(buffer);
            }

            log.Info("Input stream parsing has finished.");
        }

        public void Send(RoverState message)
        {
            String stringMessage = "";

            switch (message.AccelerationState)
            {
                case AccelerationState.Accelerating:
                    stringMessage += "a";
                    break;
                case AccelerationState.Braking:
                    stringMessage += "b";
                    break;
            }

            switch (message.RotationState)
            {
                case RotationState.Left:
                    stringMessage += "l";
                    break;
                case RotationState.Right:
                    stringMessage += "r";
                    break;
            }

            stringMessage += ";";

            socket.Send(Encoding.ASCII.GetBytes(stringMessage));
        }
    }


    public enum AccelerationState
    {
        Braking,
        Rolling,
        Accelerating
    }

    public enum RotationState
    {
        Straight,
        Left,
        HardLeft,
        Right,
        HardRight
    }

    public class RoverState
    {
        private readonly AccelerationState accState;
        private readonly RotationState rotState;

        public RoverState(AccelerationState accState, RotationState rotState)
        {
            this.accState = accState;
            this.rotState = rotState;
        }

        public AccelerationState AccelerationState
        {
            get { return accState; }
        }

        public RotationState RotationState
        {
            get { return rotState; }
        }

        public static RoverState Parse(String initString)
        {
            Char accStateChar = initString[0];
            Char turnStateChar = initString[1];

            AccelerationState accState;
            RotationState rotState;

            switch(accStateChar)
            {
                case 'a':
                    accState = AccelerationState.Accelerating;
                    break;
                case 'b':
                    accState = AccelerationState.Braking;
                    break;
                case '-':
                    accState = AccelerationState.Rolling;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Can't parse acceleration state: " + accStateChar);
            }

            switch (turnStateChar)
            {
                case 'L':
                    rotState = RotationState.HardLeft;
                    break;
                case 'l':
                    rotState = RotationState.Left;
                    break;
                case '-':
                    rotState = RotationState.Straight;
                    break;
                case 'r':
                    rotState = RotationState.Right;
                    break;
                case 'R':
                    rotState = RotationState.HardRight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Can't parse rotation state: " + turnStateChar);
            }

            return new RoverState(accState, rotState);
        }
    }
}