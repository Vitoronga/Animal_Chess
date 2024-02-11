using System.Net.NetworkInformation;

namespace NetworkTester
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ips = await SocketHelper.GetAllNetworkIPs(NetworkInterface.GetAllNetworkInterfaces());

            ips.ForEach(ip => Console.WriteLine(ip));
        }
    }
}