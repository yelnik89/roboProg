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
        private Socket UDPSocket;
        private EndPoint remotePoint;

        public UDPSendler(string ip, string port)
        {
            this.UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.remotePoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
        }

        public void sendTo(string data)
        {
            byte[] toSend = toByte(data);
            this.UDPSocket.SendTo(toSend, this.remotePoint);
            UDPSocket.Close();
        }

        private byte[] toByte(string data)
        {
            int LengthOfString = data.Length;
            byte[] sendToUDP = new byte[LengthOfString];
            for (int i = 0; i < LengthOfString; i++)
            {
                sendToUDP[i] = Convert.ToByte(data[i]);
            }
            return sendToUDP;
        }
    }
}
