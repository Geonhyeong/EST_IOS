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

namespace GroupVoice_Client_Test
{
    class Program
    {
        public const int SEND_REGISTER = 0;
        public const int SEND_CANCEL = 1;
        public static UdpClient udpClient = new UdpClient(6000);

        static void Main(string[] args)
        {
            //ThreadStart th_send = new ThreadStart(work_send);
            //Thread t_send = new Thread(th_send);
            //t_send.Start();

            ThreadStart th_receive = new ThreadStart(work_receive);
            Thread t_receive = new Thread(th_receive);
            t_receive.Start();

        }

        public static void work_send()
        {

            Protocol protocol;

            string cmd = "VOI:";
            string id;
            string data;
            int id_length;
            int data_length;
            Console.WriteLine("ID : ");
            id = Console.ReadLine();
            id_length = id.Length;

            while (true)
            {
                //data = "hihihihihihihihihi";
                data = Console.ReadLine();
                data_length = data.Length;

                protocol = new Protocol(cmd, id_length, id, data_length, data);

                byte[] dgram = new byte[4 + 4 + id_length + 4 + data_length];
                Array.Copy(protocol.get_cmd(), 0, dgram, 0, 4);
                Array.Copy(protocol.get_id_length(), 0, dgram, 4, 4);
                Array.Copy(protocol.get_phoneID(), 0, dgram, 8, id_length);
                Array.Copy(protocol.get_data_length(), 0, dgram, 8 + id_length, 4);
                Array.Copy(protocol.get_data(), 0, dgram, 8 + id_length + 4, data_length);


                //string _cmd = Encoding.UTF8.GetString(dgram, 0, 4);
                //int _id_length = bytesToint(dgram, 4);
                //string _phoneID = Encoding.UTF8.GetString(dgram, 8, id_length);
                //int _data_length = bytesToint(dgram, 8 + id_length);
                //string _data = Encoding.UTF8.GetString(dgram, 8 + id_length + 4, data_length);

                //Console.WriteLine(_cmd + _id_length + _phoneID + _data_length + _data);

                udpClient.Send(dgram, dgram.Length, "192.168.0.3", 50001);
            }

            udpClient.Close();
        }

        public static void work_receive()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                    byte[] receive = udpClient.Receive(ref remoteEP);
                    Console.WriteLine("Receive complete");

                    MemoryStream ms2 = new MemoryStream(receive);
                    Console.WriteLine(ms2.Capacity);


                    SoundPlayer player = new SoundPlayer(ms2);
                    player.Load();

                    player.Play();
                }
                catch
                {
                    Console.WriteLine("Error");
                }
            }
            udpClient.Close();
        }

        public static int bytesToint(byte[] dgram, int index)
        {
            byte[] len = new byte[4];
            Array.Copy(dgram, index, len, 0, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(len);

            return BitConverter.ToInt32(len, 0);
        }
    }

    class Protocol
    {
        private byte[] cmd;
        private byte[] id_length;
        private byte[] id;
        private byte[] data_length;
        private byte[] data;

        public Protocol(string cmd, int id_length, string id, int data_length, string data)
        {
            this.cmd = Encoding.UTF8.GetBytes(cmd);
            this.id_length = intTobyte(id_length);
            this.id = Encoding.UTF8.GetBytes(id);
            this.data_length = intTobyte(data_length);
            this.data = Encoding.UTF8.GetBytes(data);
        }

        public byte[] intTobyte(int length)
        {
            byte[] bytes = { 0, 0, 0, 0 };
            bytes[0] = (Byte)(length >> 24);
            bytes[1] = (Byte)(length >> 16);
            bytes[2] = (Byte)(length >> 8);
            bytes[3] = (Byte)(length);

            return bytes;
        }

        public byte[] get_cmd()
        {
            return cmd;
        }

        public byte[] get_phoneID()
        {
            return id;
        }

        public byte[] get_id_length()
        {
            return id_length;
        }

        public byte[] get_data_length()
        {
            return data_length;
        }

        public byte[] get_data()
        {
            return data;
        }
    }
}
