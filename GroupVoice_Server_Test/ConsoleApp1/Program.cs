using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GroupVoice_Client_Test
{
    class Program
    {
        static void Main(string[] args)
        {

            String message = "";
            UdpClient udpClient = new UdpClient(6001);


            for (int i = 0; i < 5; i++)
            {
                message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, "192.168.0.4", 50001);
                Console.WriteLine("[Send] " + message);
                IPEndPoint epRemote = new IPEndPoint(IPAddress.Any, 0);
                byte[] datat = udpClient.Receive(ref epRemote);
                Console.WriteLine("[Receive] " + Encoding.UTF8.GetString(datat));
            }

            udpClient.Close();
        }
    }
}
