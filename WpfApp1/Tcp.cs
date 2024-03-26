using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;

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

            try
            {
                socket.Connect(IPAddress.Parse(ipAddress), port);
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
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
            try
            {
                data = new byte[socket.SendBufferSize];
                int j = socket.Receive(data);

                byte[] adata = new byte[j];
                for (int i = 0; i < j; i++)
                    adata[i] = data[i];
                //accepteddata.Close();
                return adata;
            }
            catch
            {
                return null;
            }


        }

        public void Close()
        {
            socket.Close();
        }

        public string sendmessages(List<MessageOut> messages)
        {
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(MessageOut));

            foreach(MessageOut m in messages)
            {
                ser.WriteObject(stream1, m);
            }
            stream1.Position = 0;
            var sr = new StreamReader(stream1);
            string mes = sr.ReadToEnd();
            Send(mes);
            sr.Close();
            return mes;
        }

        public string sendmessages(MessageOut messages)
        {
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(MessageOut));

            ser.WriteObject(stream1, messages);

            stream1.Position = 0;
            var sr = new StreamReader(stream1);
            string mes = sr.ReadToEnd();
            Send(mes);
            sr.Close();
            return mes;
        }

        public List<MessageIn> receivemessages()
        {
            
            List<MessageIn> m2 = new List<MessageIn>();

            Byte[] buffer = Receive();

            if (buffer == null)
            {
                return m2;
            }

            var stream1 = new MemoryStream(buffer);
            var ser = new DataContractJsonSerializer(typeof(MessageIn));

            stream1.Position = 0;
            MessageIn a = ser.ReadObject(stream1) as MessageIn;

            m2.Add(a);

            return m2;
        }
    }

}

