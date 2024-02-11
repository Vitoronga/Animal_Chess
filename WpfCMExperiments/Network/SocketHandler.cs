using SocketWrapperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace WpfCMExperiments.Network
{
    public enum ConnectionOwnerships
    {
        Host = 0,
        Client = 1
    }

    public enum ConnectionTypes
    {
        Gameplay = 0,
        Chat = 1
    }

    public enum ConnectionPorts
    {
        Gameplay = 28080,
        Chat = 28081
    }

    public class SocketHandler
    {
        public ConnectionOwnerships ConnectionOwnership { get; private init; }
        private SocketServer? socketServer;
        private SocketClient socketClient;
        private ConnectionTypes ConnectionType { get; init; }
        public int Port { get; init; }
        public string IP { get; private set; }

        private SocketHandler(ConnectionOwnerships conOwnership, ConnectionTypes conType)
        {
            ConnectionOwnership = conOwnership;
            ConnectionType = conType;

            if (conOwnership == ConnectionOwnerships.Host)
            {
                // Server configuration
                socketServer = new SocketServer();
                
            } else
            {
                // Client configuration

            }

            Port = (int)Enum.Parse<ConnectionPorts>(ConnectionType.ToString()); // Gets the port from the same connectionType tag
            socketClient = new SocketClient();
        }

        // Connection starters
        public static SocketHandler CreateHost(ConnectionTypes conType)
        {
            SocketHandler sh = new SocketHandler(ConnectionOwnerships.Host, conType);

            // Configure

            return sh;
        }
        public bool StartHost()
        {
            socketServer?.Start(Port);

            if (ConnectClient(System.Net.IPAddress.Loopback.ToString()))
            {
                return true;
            } else
            {
                socketServer?.StopServer();
                return false;
            }
        }
        public static SocketHandler CreateClient(ConnectionTypes conType)
        {
            SocketHandler sh = new SocketHandler(ConnectionOwnerships.Client, conType);

            // Configure

            return sh;
        }
        public bool ConnectClient(string ip)
        {
            if (socketClient.Connect(ip, Port))
            {
                IP = ip;
                return true;
            }
            else return false;
        }

        

        // Send Data
        public bool SendData(ISocketMessage message) => socketClient.SendData(message);
        // Receive Data
        public bool ReceiveData(out ISocketMessage? message)
        {
            message = null;

            //byte[] formattedBytes;
            if (!socketClient.ReceiveData(out byte[] byteArray))
            {
                return false;
            }

            message = NetworkDataTypeHelper.InstantiateClassFromByteArray(byteArray);

            //message = new SocketMessage(formattedBytes, (byte)ConnectionType); // Check this (what is formattedBytes? xd)
            return true;
        }
    }
}
