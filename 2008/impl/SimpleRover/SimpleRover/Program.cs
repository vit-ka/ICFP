// Main.cs created with MonoDevelop
// User: lattyf at 11:45 16.07.2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Net;
using System.Net.Sockets;
using SimpleRover.Protocol;

namespace SimpleRover
{
    internal class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(typeof (Program));


        public static void Main(String[] args)
        {
            if (args.Length != 2)
            {
                OutputUsage();
                return;
            }

            String hostname = args[0];
            String portString = args[1];
            Int32 port = 0;

            try
            {
                port = Int32.Parse(portString);
            }
            catch (FormatException)
            {
                log.FatalFormat("Can't parse port number: \"{0}\".", portString);
                return;
            }
            catch (OverflowException)
            {
                log.FatalFormat("Overflow while parsing port number: \"{0}\"", portString);
                return;
            }

            log.InfoFormat("Client started");
            log.InfoFormat("Hostname: {0}. Port: {1}.", hostname, port);

            IPAddress ipAddress;
            try
            {
                log.InfoFormat("Resolving hostname...");
                ipAddress = Dns.GetHostEntry(hostname).AddressList[0];
                log.InfoFormat("\"{0}\" resolved as {1}", hostname, ipAddress);
            }
            catch (SocketException ex)
            {
                log.FatalFormat("Can't resolve ip address of hostname: {0}", ex);
                return;
            }

            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);


            Socket socket;
            try
            {
                log.Info("Connecting...");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipEndpoint);
                log.Info("Connected.");
            }
            catch (SocketException ex)
            {
                log.FatalFormat("Can't connect to server: {0}", ex);
                return;
            }

            ProtocolManager protocolManager = new ProtocolManager(socket);
            protocolManager.ReceiveAll();

            System.Threading.Thread.Sleep(3000);
            protocolManager.Send(new RoverState(AccelerationState.Accelerating, RotationState.Left));
            protocolManager.Send(new RoverState(AccelerationState.Accelerating, RotationState.Left));
        }

        private static void OutputUsage()
        {
            Console.WriteLine("Usage: SimpleRover.exe hostname port");
        }
    }
}