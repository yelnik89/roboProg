using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    class UDPReciever
    {
        private static UDPReciever udpClient;
        private UdpClient reciever;

        public static UDPReciever getInstans()
        {
            if (udpClient == null)
                udpClient = new UDPReciever();
            return udpClient;
        }

        private UDPReciever()
        {
            this.reciever = new UdpClient(8888);

        }

        public string[] getMessage()
        {
            IPEndPoint remoteIpEndPoint = null;
            try
            {
                while (true)
                {
                    byte[] recieveBytes = this.reciever.Receive(ref remoteIpEndPoint);
                    string recieveData = Encoding.UTF8.GetString(recieveBytes);
                    string[] result =  { recieveData, remoteIpEndPoint.Address.ToString(), remoteIpEndPoint.Port.ToString() };
                    return result;
                }
            }catch(Exception e)
            {
                Loger.getInstance().writeLog(e.Message);
                string[] result = { e.Message, remoteIpEndPoint.ToString() };
                return result;
            }

        }
    }
}
