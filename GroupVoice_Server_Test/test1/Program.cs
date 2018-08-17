using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Media;


// 찐 서버
namespace test1
{
    class Program
    {
        public static UdpClient srv_data;
        public static UdpClient srv_voice;
        public static bool quit = false;
        public static Dictionary<String, IPEndPoint> contacts = new Dictionary<string, IPEndPoint>();

        static void Main(string[] args)
        {
            ThreadStart th_data = new ThreadStart(work_data);
            Thread t_data = new Thread(th_data);
            t_data.Start();

            ThreadStart th_voice = new ThreadStart(work_voice);
            Thread t_voice = new Thread(th_voice);
            t_voice.Start();

            ConsoleKeyInfo cki;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(handler);
            while (true)
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Q)
                {
                    quit = true;
                    srv_data.Close();
                }
            }
        }

        public static void work_data()
        {
            srv_data = new UdpClient(50000);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            Protocol protocol;

            while(!quit)
            {
                Console.WriteLine("Server Open");
                byte[] dgram = srv_data.Receive(ref remoteEP);

                protocol = new Protocol(dgram);

                Console.WriteLine("cmd : " + protocol.get_cmd());
                Console.WriteLine("ID_length : " + protocol.get_id_length());
                Console.WriteLine("ID : " + protocol.get_phoneID());
                Console.WriteLine("Data_length : " + protocol.get_data_length());

                if (protocol.get_cmd().Equals("REG:") == true)
                {
                    srv_data.Send(dgram, dgram.Length, remoteEP);
                    regContacts(protocol.get_phoneID(), remoteEP);
                }
                else if (protocol.get_cmd().Equals("CAN:") == true)
                {
                    srv_data.Send(dgram, dgram.Length, remoteEP);                   
                    delContacts(protocol.get_phoneID(), remoteEP);
                }
                else
                {
                    srv_data.Send(dgram, dgram.Length, remoteEP);
                    Console.WriteLine("Wrong data");
                }

                foreach (var id in contacts)
                {
                    Console.WriteLine(id.Key + ", " + id.Value);
                }
            }
            srv_data.Close();
        }

        public static void work_voice()
        {
            srv_voice = new UdpClient(50001);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            Protocol protocol;

            while(!quit)
            {
                try
                {
                    byte[] dgram = srv_voice.Receive(ref remoteEP);


                    protocol = new Protocol(dgram);

                    if (protocol.get_cmd().Equals("VOI:") == false)
                    {
                        Console.WriteLine("Wrong data");
                        continue;
                    }

                    byte[] data = new byte[protocol.get_data_length()];
                    Array.Copy(dgram, 8 + protocol.get_id_length() + 4, data, 0, protocol.get_data_length());

                    //MemoryStream ms = new MemoryStream(data);
                    //SoundPlayer myPlayer = new SoundPlayer(ms);
                    //myPlayer.PlaySync();

                    string phoneID = protocol.get_phoneID();
                    // 주소록 갱신
                    if (contacts.ContainsKey(phoneID) == true)
                    {
                        if (contacts[phoneID].Address.ToString() != remoteEP.Address.ToString() || contacts[phoneID].Port != remoteEP.Port)
                        {
                            Console.WriteLine(phoneID + " : " + contacts[phoneID]);
                            contacts[phoneID] = remoteEP;
                            Console.WriteLine("갱신");
                            Console.WriteLine(phoneID + " : " + contacts[phoneID]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("This user doesn't register on Server");
                        //regContacts("123", remoteEP);
                    }
                        

                    //test
                    Console.WriteLine(protocol.get_cmd() + ", " + protocol.get_id_length() + ", " + protocol.get_phoneID() + ", " + protocol.get_data_length());
                    //srv_voice.Send(data, protocol.get_data_length(), "192.168.0.4", 5000);

                    foreach (var id in contacts.Keys)
                    {
                        if (id.Equals(protocol.get_phoneID()))
                        {
                            //Console.WriteLine(protocol.get_phoneID() + ", " + protocol.get_id_length() + ", " + protocol.get_phoneID() + ", " + protocol.get_data_length());
                            srv_voice.Send(data, protocol.get_data_length(), contacts[id].Address.ToString(), contacts[id].Port);
                        }
                    }
                }
                catch(SocketException s)
                {
                    Console.WriteLine("받을 클라이언트가 없습니다.");
                }
            }
        }

        public static void regContacts(string phoneID, IPEndPoint remoteEP)
        {
            if (contacts.ContainsKey(phoneID) == true)
            {
                Console.WriteLine("already exist");
                if (contacts[phoneID] != remoteEP)
                    contacts[phoneID] = remoteEP;
            }
                
            else
            {
                contacts.Add(phoneID, remoteEP);
                Console.WriteLine("Register Complete");
            }            
        }

        public static void delContacts(string phoneID, IPEndPoint remoteEP)
        {
            if (contacts.ContainsKey(phoneID) == true)
            {
                contacts.Remove(phoneID);
                Console.WriteLine("Cancle Complete");
            }
            else
                Console.WriteLine("Doesn't exist");
        }

        protected static void handler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Quit");
        }
    }

    class Protocol
    {
        private string cmd;
        private int id_length;
        private string phoneID;
        private int data_length;
        

        public Protocol(byte[] dgram)
        {
            cmd = Encoding.UTF8.GetString(dgram, 0, 4);
            id_length = bytesToint(dgram, 4);
            phoneID = Encoding.UTF8.GetString(dgram, 8, id_length);
            data_length = bytesToint(dgram, 8 + id_length);
        }

        public int bytesToint(byte[] dgram, int index)
        {
            byte[] len = new byte[4];
            Array.Copy(dgram, index, len, 0, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(len);

            return BitConverter.ToInt32(len, 0);
        }

        public string get_cmd()
        {
            return cmd;
        }

        public string get_phoneID()
        {
            return phoneID;
        }

        public int get_id_length()
        {
            return id_length;
        }

        public int get_data_length()
        {
            return data_length;
        }

    }
}
