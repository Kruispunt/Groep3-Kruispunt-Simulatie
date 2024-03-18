using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApp1
{
    public class Tcp
    {
        public TcpClient tcpClient;

        public string connect()
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
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Ontvangen: " + message);
                    return message;
                }
            }
        }

    }

}

