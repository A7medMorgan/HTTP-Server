using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
namespace HeadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000);
            Socket newClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Waiting ...... ");
            Console.WriteLine("Start conn (Enter)");
            Console.ReadLine();

            newClient.Connect(iep);
            string del = "\r\n";
            string request = "HEAD /main HTTP/1.1" + del +
                            "Host: "+ newClient.RemoteEndPoint + del +
                            "Connection: One-Time" + del +
                            "DNT: 1" + del +
                            "Upgrade - Insecure - Requests: 1" + del +
                            "User - Agent: Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 91.0.4472.77 Safari / 537.36 Edg / 91.0.864.41" + del +
                            "Accept: text / html" + del +
                            "Accept-Encoding: gzip, deflate" + del +
                            "Accept-Language: en-US,en;q=0.9,ar;q=0.8" + del +
                            del; //Blank line

            Console.WriteLine("\tSent \n{0}", request); // to trace traffic

            byte[] messageByteArray = Encoding.ASCII.GetBytes(request);
            newClient.Send(messageByteArray);

            byte[] recieved = new byte[1024];
            int recieved_length = newClient.Receive(recieved);

            Console.WriteLine("Recived \t{0} Byte", recieved_length);
            Console.WriteLine("\tRecived \n{0} ", Encoding.ASCII.GetString(recieved , 0 , recieved_length)); // to trace traffic

            newClient.Shutdown(SocketShutdown.Both);
        }
    }
}

