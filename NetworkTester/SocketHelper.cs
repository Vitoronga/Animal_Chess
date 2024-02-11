using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace NetworkTester
{
    public static class SocketHelper
    {
        public const int PING_MAX_CONCURRENT_AMOUNT = 20000;
        public const int NETWORK_MINIMUM_MASK_SIZE_BIT_COUNT = 16;

        public static List<UnicastIPAddressInformation> GetAllHostIPv4UnicastAddresses()
        {
            Stopwatch s = Stopwatch.StartNew();

            List<UnicastIPAddressInformation> unicastIps = new List<UnicastIPAddressInformation>();

            var netInterfaces = NetworkInterface.GetAllNetworkInterfaces(); // Get all network interface/connections to the machine

            // Subnet mask -> defines with parts of the IP address belong to the network itself or to the specified host
            // default gateway -> generally is the router's ip

            foreach (NetworkInterface netInterface in netInterfaces)
            {
                if (!netInterface.Supports(NetworkInterfaceComponent.IPv4) ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

                IPInterfaceProperties ipProperties = netInterface.GetIPProperties();

                foreach (UnicastIPAddressInformation unicastAddress in ipProperties.UnicastAddresses)
                {
                    IPAddress unicastIP = unicastAddress.Address;
                    if (unicastIP.AddressFamily != AddressFamily.InterNetwork) continue;

                    unicastIps.Add(unicastAddress);

                    break; // It will already have scanned the whole subnet, no need to see other IPs (could even extract the necessary base unicast IP to do it outside the loop)
                }
            }

            //MessageBox.Show($"Time taken to find all IPs: {s.ElapsedMilliseconds}ms");
            //await Console.Out.WriteLineAsync($"Time taken to find all IPs: {s.ElapsedMilliseconds}ms");
            return unicastIps;
        }

        public static async Task<List<IPAddress>> GetAllNetworkIPs(IEnumerable<NetworkInterface> netInterfaces, bool scanBigNetworks = false)
        {
            List<IPAddress> ips = new List<IPAddress>();
            //List<byte[]> recognizedBaseIPs = new List<byte[]>();

            List<Task<PingReply>> pingTasks = new List<Task<PingReply>>();
            List<PingReply> pingReplies = new List<PingReply>();

            long counter = 0;
            foreach(NetworkInterface netInterface in netInterfaces)
            {
                if (!netInterface.Supports(NetworkInterfaceComponent.IPv4) || 
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback || 
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

                UnicastIPAddressInformation? unicastAddress = null;
                IPAddress? unicastIP = null;

                foreach (UnicastIPAddressInformation uniAddress in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (uniAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        unicastAddress = uniAddress;
                        unicastIP = unicastAddress.Address;
                        break;
                    }
                }

                // Check if the IPv4 has been set in the wrappers
                if (unicastAddress == null || unicastIP == null) continue;

                byte[] unicastIPBytes = unicastIP.GetAddressBytes();

                // Interpret the mask used for the ip
                IPAddress ipMask = unicastAddress.IPv4Mask;
                byte[] ipMaskBytes = ipMask.GetAddressBytes();
                byte networkMaskBits = GetCountOfSequenceBitsOn(ipMaskBytes);

                // Check if it should scan big networks
                if (!scanBigNetworks && networkMaskBits < NETWORK_MINIMUM_MASK_SIZE_BIT_COUNT) continue;

                // Discover the base IP (practically the network IP)
                byte[] baseIPBytes = DoANDOperationOnByteArrays(ipMaskBytes, unicastIP.GetAddressBytes());

                // Avoid repetition of IPs (UNNECESSARY -> there won't be 2 networks with same ip structure (I'm pretty sure))
                //if (recognizedBaseIPs.Contains(baseIPBytes)) continue;
                //recognizedBaseIPs.Add(baseIPBytes);

                // Ping all possible variations
                //Task<List<IPAddress>> responsiveIPs = PingWholeSubnet2(ipMaskBytes, baseIPBytes); // Returns a list of all valid IPs

                while (IncrementByteArray(ref baseIPBytes) && !IsByteArrayBitsFullySet(DoOROperationOnByteArrays(baseIPBytes, ipMaskBytes))) // Accidently avoids network and broadcast ips lol
                {
                    IPAddress ip = new IPAddress(baseIPBytes);
                    Console.WriteLine($"counter:{++counter}\tpingTasks:{pingTasks.Count}\tip:{ip}");
                    //ips.Add(new IPAddress(baseIPBytes));
                    Ping p = new Ping();

                    pingTasks.Add(p.SendPingAsync(ip));

                    // Handle pings when amount is too many (to avoid using too much memory too)
                    if (pingTasks.Count == PING_MAX_CONCURRENT_AMOUNT)
                    {
                        pingReplies.AddRange((await Task.WhenAll(pingTasks)));
                        pingTasks.Clear();
                    }
                }
            }

            pingReplies = (await Task.WhenAll(pingTasks)).ToList();

            foreach(PingReply pingReply in pingReplies)
            {
                if (pingReply.Status == IPStatus.Success) ips.Add(pingReply.Address);
            }

            return ips;
        }

        public static List<IPAddress> GetAllHostIPv4s()
        {
            //List<IPAddress> hostIPs = new List<IPAddress>();
            //var netInterfaces = NetworkInterface.GetAllNetworkInterfaces(); // Get all network interface/connections to the machine

            //foreach (NetworkInterface netInterface in netInterfaces)
            //{
            //    if (!netInterface.Supports(NetworkInterfaceComponent.IPv4)) continue;

            //    IPInterfaceProperties ipProperties = netInterface.GetIPProperties();

            //    foreach (UnicastIPAddressInformation unicastAddress in ipProperties.UnicastAddresses)
            //    {
            //        IPAddress unicastIP = unicastAddress.Address;
            //        if (unicastIP.AddressFamily != AddressFamily.InterNetwork) continue;

            //        hostIPs.Add(unicastIP);
            //        break; // I guess there is no reason to check for more IPv4s in this net interface
            //    }
            //}

            //return hostIPs;

            return Dns.GetHostAddresses(Dns.GetHostName())
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();
        }

        public static byte[] DoOROperationOnByteArrays(byte[] array1, byte[] array2)
        {
            int length = array1.Length < array2.Length ? array1.Length : array2.Length;
            byte[] newArray = new byte[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = (byte)(array1[i] | array2[i]);
            }

            return newArray;
        }

        public static byte[] DoANDOperationOnByteArrays(byte[] array1, byte[] array2)
        {
            int length = array1.Length < array2.Length ? array1.Length : array2.Length;
            byte[] newArray = new byte[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = (byte)(array1[i] & array2[i]);
            }

            return newArray;
        }

        public static bool IncrementByteArray(ref byte[] array, int limitTopIndex = -1)
        {
            //for (int i = array.Length - 1; i >= 0; i--)
            //{
            //    if (array[i] == 255)
            //    {
            //        if (i == 0) return false;

            //        array[i] = 0;
            //        array[i - 1]++;
            //    }

            //    array[i]++;
            //}

            int currentIndex = array.Length - 1;

            while (IncrementByte(ref array[currentIndex], true)) // Will keep being true while the byte keeps overflowing
            {
                // Check if it couldn't increment even at first index
                if (currentIndex-- == 0 || currentIndex == limitTopIndex) return false; // It will end with currentIndex at -1
            }

            return true;
        }

        public static bool IncrementByte(ref byte b, bool allowAndReturnIfOverflowed = true)
        {
            if (b == 255)
            {
                if (!allowAndReturnIfOverflowed) return false; // no success

                b = 0;
                return true; // overflowed
            }

            b++;
            return !allowAndReturnIfOverflowed; // Return false for no overflow, return true for success
        }

        public static bool IsByteArrayBitsFullySet(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 255) return false;
            }

            return true;
        }

        public static byte GetCountOfSequenceBitsOn(byte[] array)
        {
            byte bits = 0;

            // Check up to which bit is set in the mask value
            foreach (byte b in array)
            {
                byte i = 128;

                while (i != 0 && (b & i) != 0) // while i isn't 0 and certain bit is on
                {
                    i >>= 1;
                    bits++;
                }

                // Check for when the network mask bits stops in the middle of a byte
                //if (i != 0) 
            }

            return bits;
        }

        public static string? GetHostName(IPAddress ipAddress)
        {
            try
            {
                IPHostEntry ipHostEntry = Dns.GetHostEntry(ipAddress);
                return ipHostEntry?.HostName;
            }
            catch (SocketException)
            {
                // socket exception
            }

            return null;
        }
    }
}
