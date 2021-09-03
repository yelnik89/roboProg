using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    class UDPSendler
    {
        private IPEndPoint remotePoint;

        public UDPSendler(string ip, string port)
        {
            this.remotePoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
        }

        public void sendTo(string data)
        {
            UdpClient udpClient = new UdpClient();

            byte[] toSend = Encoding.ASCII.GetBytes(data);
            udpClient.Send(toSend, toSend.Length, this.remotePoint);
        }
    }
}
