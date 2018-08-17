using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Media;
using System.Web;
using System.Windows.Input;


namespace VoiceSendTest
{
    class Program
    {
       
        public const int SEND_REGISTER = 0;
        public const int SEND_CANCEL = 1;
        public static UdpClient udpClient = new UdpClient(5000);

        static void Main(string[] args)
        {
            ThreadStart th_send = new ThreadStart(work_send);
            Thread t_send = new Thread(th_send);
            t_send.Start();

            //ThreadStart th_receive = new ThreadStart(work_receive);
            //Thread t_receive = new Thread(th_receive);
            //t_receive.Start();
        }

        public static void work_send()
        {

            Protocol protocol;

            string cmd = "VOI:";
            string id;
            string data;
            int id_length;
            int data_length;
            //Console.WriteLine("ID : ");
            id = "lgh";
            id_length = id.Length;


            while (true)
            {
                SoundPlayer soundPlayer = new SoundPlayer(@"C:\Users\PC\Desktop\한글2007\Install\Hnc\HncUtils\HncNote\WHIZ.wav");
                string path1 = @"C:\Users\PC\Desktop\한글2007\Install\Hnc\HncTT20\MsgBell.wav";
                string path2 = @"C:\Users\PC\Desktop\한글2007\Install\Hnc\HncUtils\HncNote\ZOOP.wav";
                string path3 = @"C:\Users\PC\Desktop\한글2007\Install\Hnc\HncUtils\HncNote\BIGRING.wav";
                


                //soundPlayer.PlaySync();
                byte[] audiobyte = System.IO.File.ReadAllBytes(path3);
                //byte[] audiobyte = System.IO.File.ReadAllBytes(path2);
                //byte[] audiobyte = System.IO.File.ReadAllBytes(path3);

                // Voice send test
                MemoryStream ms = new MemoryStream(audiobyte);
                SoundPlayer myPlayer = new SoundPlayer(ms);
                Console.WriteLine("Send : " + audiobyte.Length);
                //myPlayer.PlaySync();
                Console.ReadLine();
                udpClient.Send(audiobyte, audiobyte.Length, "192.168.0.4", 60000);
                                
                //MemoryStream ms2 = new MemoryStream(audiobyte2);
                //SoundPlayer soundPlayer2 = new SoundPlayer(ms2);
                //soundPlayer2.PlaySync();

                //MemoryStream ms3 = new MemoryStream(audiobyte3);
                //SoundPlayer soundplayer3 = new SoundPlayer(ms3);
                //soundplayer3.PlaySync();

                //data = Console.ReadLine();
                //data_length = data.Length;

                //protocol = new Protocol(cmd, id_length, id, audiobyte.Length, data);

                //byte[] dgram = new byte[4 + 4 + id_length + 4 + audiobyte.Length];
                //Array.Copy(protocol.get_cmd(), 0, dgram, 0, 4);
                //Array.Copy(protocol.get_id_length(), 0, dgram, 4, 4);
                //Array.Copy(protocol.get_phoneID(), 0, dgram, 8, id_length);
                //Array.Copy(protocol.get_data_length(), 0, dgram, 8 + id_length, 4);
                //Array.Copy(audiobyte, 0, dgram, 8 + id_length + 4, audiobyte.Length);

                //udpClient.Send(dgram, dgram.Length, "192.168.0.4", 50001);
                //IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                //byte[] receive = udpClient.Receive(ref remoteEP);
                //Console.WriteLine("Receive complete");
                //MemoryStream ms2 = new MemoryStream(receive);
                //SoundPlayer player = new SoundPlayer(ms2);
                //Console.WriteLine("Receive : " + receive);
                //player.PlaySync();
            }
            
            udpClient.Close();
        }

        public static void work_receive()
        {
            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receive_data = udpClient.Receive(ref remoteEP);
                Console.WriteLine("[Received]");
                MemoryStream ms = new MemoryStream(receive_data);
                SoundPlayer player = new SoundPlayer(ms);
                player.PlaySync();
                //Console.WriteLine("[Receive] : " + Encoding.UTF8.GetString(receive_data));
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

    class Client
    {
        private int m_ThreadNo = 0;
        public const string SERVERIP = "192.168.0.3";
        public const int SERVERPORT = 50000;
        public const int STATE_REGISTER = 1;
        public const int STATE_CANCEL = 2;
        private int currentState = STATE_REGISTER;
        private string currentStateVal = null;

        public Client(int threadNo)
        {
            m_ThreadNo = threadNo;
        }

        public void run()
        {
            UdpClient client = new UdpClient();
            switch (currentState)
            {
                case STATE_REGISTER:
                    byte[] dgram = new byte[4 + 4 + 3 + 4];
                    client.Send(dgram, dgram.Length, SERVERIP, SERVERPORT);

                    break;
                case STATE_CANCEL:

                    break;
                default:
                    break;
            }
        }

        public void setCurrentState(int state)
        {
            currentState = state;
            switch (currentState)
            {
                case STATE_REGISTER:
                    currentStateVal = "REG:";
                    break;

                case STATE_CANCEL:
                    currentStateVal = "CAN:";
                    break;

                default:
                    break;
            }
        }
    }

    class WaveReader
    {
        FileInfo m_fInfo;
        FileStream m_fStream;
        BinaryReader m_binReader;

        // RIFF chunk
        byte[] chunkID;
        UInt32 chunkSize;
        byte[] format;

        // fmt subchunk
        byte[] fmtChunkID;
        UInt32 fmtChunkSize;
        UInt16 audioFormat;
        UInt16 numChannels;
        UInt32 sampleRate;
        UInt32 byteRate;
        UInt16 blockAssign;
        UInt16 BitsPerSample;

        // data subchunk
        byte[] dataChunkID;
        UInt32 dataChunkSize;
        byte[] data8L;              // 8-bit left channel
        byte[] data8R;              // 8-bit right channel
        Int16[] data16L;           // 16-bit left channel
        Int16[] data16R;           // 16-bit right channel
        int numSamples;

        public WaveReader()
        {

        }

        public bool Open(String filename)
        {
            string str;
            m_fInfo = new FileInfo(filename);
            m_fStream = m_fInfo.OpenRead();
            m_binReader = new BinaryReader(m_fStream);

            chunkID = new byte[4];
            format = new byte[4];

            chunkID = m_binReader.ReadBytes(4);
            chunkSize = m_binReader.ReadUInt32();
            format = m_binReader.ReadBytes(4);

            str = System.Text.ASCIIEncoding.ASCII.GetString(chunkID, 0, 4);
            if (str != "RIFF")
                return false;

            str = System.Text.ASCIIEncoding.ASCII.GetString(format, 0, 4);
            if (str != "WAVE")
                return false;

            if (ReadFmt() == false)
                return false;
            if (ReadData() == false)
                return false;

            m_fStream.Close();

            return true;
        }

        private bool ReadFmt()
        {
            fmtChunkID = new byte[4];
            fmtChunkID = m_binReader.ReadBytes(4);

            string str = System.Text.ASCIIEncoding.ASCII.GetString(fmtChunkID, 0, 4);
            if (str != "fmt ")
                return false;

            fmtChunkSize = m_binReader.ReadUInt32();
            audioFormat = m_binReader.ReadUInt16();
            numChannels = m_binReader.ReadUInt16();
            sampleRate = m_binReader.ReadUInt32();
            byteRate = m_binReader.ReadUInt32();
            blockAssign = m_binReader.ReadUInt16();
            BitsPerSample = m_binReader.ReadUInt16();

            return true;
        }

        private bool ReadData()
        {
            dataChunkID = new byte[4];
            dataChunkID = m_binReader.ReadBytes(4);
            string str = System.Text.ASCIIEncoding.ASCII.GetString(dataChunkID, 0, 4);
            if (str != "data")
                return false;

            // Read the size of data chunk
            // chunkSize = numSamples * numChannels * BitsPerSample/8
            dataChunkSize = m_binReader.ReadUInt32();
            numSamples = (int)dataChunkSize / (int)(numChannels * BitsPerSample / 8);

            int i;
            // Read sound data
            if (BitsPerSample == 8)
            {
                if (numChannels == 1)
                {
                    data8L = new byte[numSamples];
                    data8L = m_binReader.ReadBytes(numSamples);
                }
                else if (numChannels == 2)
                {
                    data8L = new byte[numSamples];
                    data8R = new byte[numSamples];

                    for (i = 0; i < numSamples; i++)
                    {
                        data8L[i] = m_binReader.ReadByte();
                        data8R[i] = m_binReader.ReadByte();
                    }
                }
            }
            else if (BitsPerSample == 16)
            {
                if (numChannels == 1)
                {
                    data16L = new Int16[numSamples];
                    for (i = 0; i < numSamples; i++)
                        data16L[i] = m_binReader.ReadInt16();
                }
                else if (numChannels == 2)
                {
                    data16L = new Int16[numSamples];
                    data16R = new Int16[numSamples];

                    for (i = 0; i < numSamples; i++)
                    {
                        data16L[i] = m_binReader.ReadInt16();
                        data16R[i] = m_binReader.ReadInt16();
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Return 16-bit sound data
        /// </summary>
        ///
        /// 0 : return left channel
        /// 1 : return right channel
        ///
        /// <returns>
        /// return 16-bit sound data
        /// </returns>
        public Int16[] getData16(int channel)
        {
            if (channel == 0)
                return data16L;
            else
                return data16R;
        }

        /// <summary>
        /// Return 8-bit sound data
        /// </summary>
        ///
        /// 0 : return left channel
        /// 1 : return right channel
        ///
        /// <returns>
        /// return 8-bit sound data
        /// </returns>
        public byte[] getData8(int channel)
        {
            if (channel == 0)
                return data8L;
            else
                return data8R;
        }

        public UInt16 getAudioFormat()
        {
            return audioFormat;
        }

        public UInt16 getNumChannels()
        {
            return numChannels;
        }

        public UInt32 getSampleRate()
        {
            return sampleRate;
        }

        public UInt32 getByteRate()
        {
            return byteRate;
        }

        public UInt16 getBitsPerSample()
        {
            return BitsPerSample;
        }

        public int getNumSamples()
        {
            return numSamples;
        }
    }
}
 
