using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace WpfApp1
{
    public class Tcp
    {
        static byte[] data;  // 1
        static Socket socket; // 1
        static bool connected;
        static Socket accepteddata;

        public Tcp()
        {

        }

        public bool getconnected()
        {
            return connected;
        }

        
        public void Connect(string ipAddress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 1;
            try
            {
                socket.Connect(IPAddress.Parse(ipAddress), port);
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
                Trace.WriteLine("failed to connect");
            }

        }

        public void Send(string q)
        {
            try { 
                byte[] data = Encoding.UTF8.GetBytes(q);
                socket.Send(data);
            }
            catch { }
        }

        public byte[] Receive()
        {
            
            data = new byte[socket.SendBufferSize];
            if (socket.Available > 0)
            {
                int j = socket.Receive(data);

                byte[] adata = new byte[j];
                for (int i = 0; i < j; i++)
                    adata[i] = data[i];
                //accepteddata.Close();
               
                return adata;
            }
            return null;

        }

        public void Close()
        {
            socket.Close();
        }

        public string sendmessages(Mainmessage messages)
        {
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(Mainmessage));

            ser.WriteObject(stream1, messages);

            stream1.Position = 0;
            var sr = new StreamReader(stream1);
            string mes = sr.ReadToEnd();
            Send(mes);
            sr.Close();
            return mes;
        }

        public MainMessageIn receivemessages()
        {
            Byte[] buffer = Receive();

            if (buffer == null)
            {
                return new MainMessageIn();
            }

            var stream1 = new MemoryStream(buffer);
            var ser = new DataContractJsonSerializer(typeof(MainMessageIn));

            stream1.Position = 0;
            //Trace.WriteLine(System.Text.Encoding.Default.GetString(buffer));

            MainMessageIn m2 = ser.ReadObject(stream1) as MainMessageIn; //todo add list

            return m2;

        }
    }

}

