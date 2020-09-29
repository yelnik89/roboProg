using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace roboProg
{
    class UDPReciver
    {
        private static UDPReciver instans;
        private Socket listenSocket;

        public static UDPReciver getInstans()
        {
            if (instans == null)
                instans = new UDPReciver();
            return instans;
        }

        private UDPReciver()
        {
            ListenSocket();
        }


        private void ListenSocket()
        {
            try
            {
                CreateListenSocket();
            }
            catch (Exception)
            {
                Close(listenSocket);
                throw;
            }
        }

        private void CreateListenSocket()
        {
            IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

            if (listenSocket == null)
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            listenSocket.Bind(localIP);
        }

        public string[] getMessage()
        {
            try
            {
                return message();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string[] message()
        {
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

            string message = compilMessage(remoteIp);
            IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

            string[] result = { message, remoteFullIp.Address.ToString(), remoteFullIp.Port.ToString() };
            return result;
        }

        private string compilMessage(EndPoint remoteIp)
        {
            StringBuilder builder = new StringBuilder();
            cachData(builder, remoteIp);

            return builder.ToString();
        }

        private void cachData(StringBuilder builder, EndPoint remoteIp)
        {
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных

            do
            {
                bytes = listenSocket.ReceiveFrom(data, ref remoteIp);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (listenSocket.Available > 0);
        }

        public void Close(Socket socket)
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }
    }
}
