using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace lgh
{
    class Program
    {
        public static RoomManager rooms = new RoomManager();
        public static UdpClient srv_cmd;
        public static UdpClient srv_create;
        public static UdpClient srv_search;
        public static UdpClient srv_data;
        public static bool quit = false;

        static void Main(string[] args)
        {
            ThreadStart th_create = new ThreadStart(work_create);
            Thread t_create = new Thread(th_create);
            t_create.Start();

            ThreadStart th_search = new ThreadStart(work_search);
            Thread t_search = new Thread(th_search);
            t_search.Start();

            ThreadStart th_cmd = new ThreadStart(work_command);
            Thread t_cmd = new Thread(th_cmd);
            t_cmd.Start();

            ConsoleKeyInfo cki;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(handler);
            while(true)
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Q)
                {
                    quit = true;
                    srv_create.Close();
                    srv_search.Close();
                    srv_cmd.Close();
                }
            }
        }

        public static void work_create()
        {
            srv_create = new UdpClient(50000);
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);

            while(!quit)
            {
                byte[] dgram_room = srv_create.Receive(ref remoteEp);
                string room_name = Encoding.UTF8.GetString(dgram_room);

                byte[] dgram_pw = srv_create.Receive(ref remoteEp);
                string pw = Encoding.UTF8.GetString(dgram_pw);

                Random rnd = new Random();
                string port = rnd.Next(1024, 49000).ToString();

                while(true)
                {
                    if (rooms.addRooms(port, room_name, pw) == 0) // 같은 방이 이미 존재
                    {
                        string reply = "Fail";
                        srv_create.Send(Encoding.UTF8.GetBytes(reply), Encoding.UTF8.GetBytes(reply).Length, remoteEp);
                        break;
                    }
                    else if (rooms.addRooms(port, room_name, pw) == 1) // 생성 가능
                    {
                        srv_create.Send(Encoding.UTF8.GetBytes(port), Encoding.UTF8.GetBytes(port).Length, remoteEp);
                        break;
                    }
                    else if (rooms.addRooms(port, room_name, pw) == 2) // port 번호 겹침
                    {
                        port = rnd.Next(1024, 49000).ToString();
                    }
                }
            }
            srv_create.Close();
        }

        public static void work_search()
        {
            srv_search = new UdpClient(50001);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while(!quit)
            {
                byte[] dgram_room = srv_search.Receive(ref remoteEP);
                string roomName = Encoding.UTF8.GetString(dgram_room);

                byte[] dgram_pw = srv_search.Receive(ref remoteEP);
                string pw = Encoding.UTF8.GetString(dgram_pw);

                if (rooms.searchRooms(roomName, pw).Equals("Fail") == true) // 방 이름이나 비번이 틀렷을 경우
                {
                    srv_search.Send(Encoding.UTF8.GetBytes(rooms.searchRooms(roomName, pw)), Encoding.UTF8.GetBytes(rooms.searchRooms(roomName, pw)).Length, remoteEP);
                }
                else // 방과 비번이 일치 -> port 번호 전송
                {
                    srv_search.Send(Encoding.UTF8.GetBytes(rooms.searchRooms(roomName, pw)), Encoding.UTF8.GetBytes(rooms.searchRooms(roomName, pw)).Length, remoteEP);
                }
            }
            srv_search.Close();
        }

        public static void work_command()
        {
            srv_cmd = new UdpClient(50002);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while(!quit)
            {
                byte[] dgram = srv_cmd.Receive(ref remoteEP);
                Console.WriteLine("Receive {0} bytes from {1}", dgram.Length, remoteEP.ToString());

                string data = Encoding.UTF8.GetString(dgram);
                string action = data.Substring(0, 4);
                if (action.Equals("Ent:") == true)
                {
                    srv_cmd.Send(Encoding.UTF8.GetBytes(action), Encoding.UTF8.GetBytes(action).Length, remoteEP);
                    byte[] dgram_port = srv_cmd.Receive(ref remoteEP);
                    string port = Encoding.UTF8.GetString(dgram_port);
                    byte[] dgram_name = srv_cmd.Receive(ref remoteEP);
                    string name = Encoding.UTF8.GetString(dgram_name);
                    byte[] dgram_number = srv_cmd.Receive(ref remoteEP);
                    string number = Encoding.UTF8.GetString(dgram_number);

                }
                else if (action.Equals("Out:") == true)
                {
                    srv_cmd.Send(Encoding.UTF8.GetBytes(action), Encoding.UTF8.GetBytes(action).Length, remoteEP);

                }
            }
            srv_cmd.Close();
        }

        public static void work_data(int port)
        {
            srv_data = new UdpClient(port);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            Dictionary<int, IPEndPoint> endpoint = new Dictionary<int, IPEndPoint>();
            int endpoint_cnt = 0;
            bool add = false;

            while (!quit)
            {
                byte[] dgram = srv_data.Receive(ref remoteEP);
                //Console.WriteLine("[Receive1] {0} bytes from {1}", dgram.Length, remoteEP.ToString());

                // Add contact
                if (endpoint.Count == 0 && dgram.Length != 0)
                {
                    endpoint.Add(endpoint_cnt++, remoteEP);
                    Console.WriteLine("[1.1]" + endpoint_cnt);
                }
                else if (endpoint.Count > 0 && dgram.Length != 0)
                {
                    // Search same remoteEP
                    foreach (var ep in endpoint.Values)
                    {
                        //Console.WriteLine("[{0},{1}]", ep, remoteEP);
                        if (ep.Equals(remoteEP))
                        {
                            //Console.WriteLine("false");
                            add = false;
                            break;
                        }
                        //Console.WriteLine("true");
                        add = true;
                    }
                }

                if (add)
                {
                    endpoint.Add(endpoint_cnt++, remoteEP);
                    Console.WriteLine("[1.2]" + endpoint_cnt);
                }

                // Transmit data
                foreach (var ep in endpoint.Values)
                {
                    //Console.WriteLine("1<-" + remoteEP);
                    if (!ep.Equals(remoteEP))
                    {
                        //Console.WriteLine("1->" + ep);
                        srv_data.Send(dgram, dgram.Length, ep);
                    }
                }
            }
            srv_data.Close();
        }

        protected static void handler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Quit");
        }
    }
}

class RoomManager
{
    private Dictionary<String, String> passwords = new Dictionary<string, string>();
    private Dictionary<String, String> ports = new Dictionary<string, string>();

    public Dictionary<String, String> getPort()
    {
        return ports;
    }

    public Dictionary<String, String> getPassword()
    {
        return passwords;
    }

    public int addRooms(String port, String roomName, String password)
    {
        if (ports.ContainsKey(roomName) == true)
        {
            Console.WriteLine(roomName + "is already exist.");
            return 0;
        }
        else
        {
            if (ports.ContainsValue(port) == true)
            {
                return 2;
            }
            ports.Add(roomName, port);
            passwords.Add(roomName, password);
            Console.WriteLine("[{0}] room : {1} is saved", port, roomName);
            return 1;
        }
    }

    public string searchRooms(String roomName, String pw)
    {
        if (ports.ContainsKey(roomName) == true)
        {
            if (passwords[roomName] == pw)
                return ports[roomName];
            else
                return "Fail";
        }
        else
        {
            return "Fail";
        }
    }
}