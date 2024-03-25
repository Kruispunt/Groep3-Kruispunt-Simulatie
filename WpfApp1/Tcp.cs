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

        public Tcp()
        {

        }

        public bool getconnected()
        {
            return connected;
        }
        public string connectTcpListener()
        {
            string ipAddress = "127.0.0.1";  // Alissa 80.61.129.65
            int port = 12345;                 // De poort om naar te luisteren

            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();
            Console.WriteLine("Wacht op verbindingen...");

            using (TcpClient client = listener.AcceptTcpClient())
            {
                Console.WriteLine("Verbinding geaccepteerd.");

                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.Default.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Ontvangen: " + message);
                    return message;
                }
            }
        }

        public string connectSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345)); 

            socket.Listen(1); 
            Socket accepteddata = socket.Accept(); 
            data = new byte[accepteddata.SendBufferSize]; 
            int j = accepteddata.Receive(data); 
            byte[] adata = new byte[j];         
            for (int i = 0; i < j; i++)         
                adata[i] = data[i];             
            return Encoding.Default.GetString(adata);
                                   

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
            try { 
            Socket accepteddata = socket.Accept(); 
            data = new byte[accepteddata.SendBufferSize]; 
            int j = accepteddata.Receive(data); 
            byte[] adata = new byte[j];         
            for (int i = 0; i < j; i++)         
                adata[i] = data[i];             
            accepteddata.Close();
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


        public List<MessageIn> receivemessages()
        {
            List<MessageIn> m2 = new List<MessageIn>();
            if (Receive() == null)
            {
                return m2;
            }
            var stream1 = new MemoryStream(Receive());
            var ser = new DataContractJsonSerializer(typeof(MessageIn));

            stream1.Position = 0;
            m2 = (List<MessageIn>)ser.ReadObject(stream1);

            return m2;
        }
    }

}

