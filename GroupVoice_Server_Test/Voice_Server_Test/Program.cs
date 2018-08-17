using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Voice_Server_Test
{
    class Program
    {
        public static UdpClient srv_data;
        public static Dictionary<int, IPEndPoint> endpoint = new Dictionary<int, IPEndPoint>();
        public static int endpoint_cnt = 0;
        static void Main(string[] args)
        {
            ThreadStart th_data = new ThreadStart(work_data);
            Thread t_data = new Thread(th_data);
            t_data.Start();
        }

        public static void work_server()
        {
            int recv = 0;
            byte[] data = new byte[1024];

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 9050);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(ep);

            Console.WriteLine("Waiting for a client");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remoteEP = (EndPoint)sender;

            recv = server.ReceiveFrom(data, ref remoteEP);

            Console.WriteLine("[first] Message received from {0}", remoteEP.ToString());
            Console.WriteLine("[first] received data : {0}", Encoding.UTF8.GetString(data, 0, recv));

            
        }
        

        public static void work_data()
        {
            srv_data = new UdpClient(60000);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            bool add = false;
            bool reg = true;

            while(true)
            {
                try
                {
                    byte[] dgram = srv_data.Receive(ref remoteEP);
                    Console.WriteLine(remoteEP + "Sever : [Received] {0} bytes", dgram.Length);

                    string cmd = Encoding.UTF8.GetString(dgram, 0, 4);

                    if (cmd != "REG:")
                    {
                        MemoryStream ms = new MemoryStream(dgram);
                        SoundPlayer myPlayer = new SoundPlayer(ms);
                        //myPlayer.PlaySync();
                    }
                    
                    
                    //if (reg == true)
                    //{
                    //    Console.WriteLine("Respond");
                    //    srv_data.Send(dgram, dgram.Length, remoteEP);
                    //    reg = false;
                    //}


                    if (endpoint.Count == 0 && dgram.Length != 0)
                    {
                        endpoint.Add(endpoint_cnt++, remoteEP);
                        Console.WriteLine("Register");
                    }
                    else if (endpoint.Count > 0 && dgram.Length != 0)
                    {
                        foreach (var ep in endpoint.Values)
                        {
                            if (ep.Equals(remoteEP))
                            {
                                add = false;
                                break;
                            }
                            add = true;
                        }
                    }

                    if (add)
                    {
                        endpoint.Add(endpoint_cnt++, remoteEP);
                        Console.WriteLine("Register");
                    }
                    //srv_data.Send(dgram, dgram.Length, "192.168.0.4", 6000);

                    foreach (var ep in endpoint.Values)
                    {
                        if (!ep.Equals(remoteEP))
                        {
                            Console.WriteLine("Send " + remoteEP + " " + dgram.Length + " to " + ep);
                            srv_data.Send(dgram, dgram.Length, ep);
                        }
                    }


                }
                catch (SocketException s)
                {
                    Console.WriteLine("No client");
                }

                


            }
            srv_data.Close();
        }

    }
}
