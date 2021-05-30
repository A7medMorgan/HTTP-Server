using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        private Socket ServerSocket { get; set; }
        public static List<Socket> List_Servers = new List<Socket>();
        public int server_ID { get; private set; }

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.137.1"), portNumber);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            List_Servers.Add(ServerSocket);
            server_ID = List_Servers.Count - 1;
            ServerSocket.Bind(endpoint);
            Console.WriteLine("Server : {0}  -> Binded successfly" , server_ID);
        }

        public void StartServer()
        {

            // TODO: Listen to connections, with large backlog.
            ServerSocket.Listen(100);
            Console.WriteLine("Server : {0} -> Start Listening on ip address {1}", server_ID,ServerSocket.LocalEndPoint);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = ServerSocket.Accept();
                Console.WriteLine("New Client accepted:{0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket newConnection = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            newConnection.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.

            byte[] data;
            int receivedLen;
            string msg = "";
            while (true)
            {
                try
                {
                    data = new byte[1024 * 1024];
                    // TODO: Receive request
                    receivedLen = newConnection.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    msg = Encoding.ASCII.GetString(data, 0, receivedLen);
                    if (receivedLen == 0)
                    {
                        Console.WriteLine("Client: {0}ended the connection", newConnection.RemoteEndPoint);
                        break;
                    }
                    Console.WriteLine("Recieved:{0}", msg);
                    Request request = new Request(msg);

                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = msg.Split(stringSeparators, StringSplitOptions.None);

                    Console.WriteLine(lines.Length);

                    request.ParseRequest(server_ID);



                    // TODO: Create a Request object using received request string

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client
                    //newConnection.Send(data, 0, receivedLen, SocketFlags.None);

                    msg = ""; // clear the msg
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class

                }
            }

            // TODO: close client socket
            newConnection.Shutdown(SocketShutdown.Both);
            newConnection.Close();
        }

        Response HandleRequest(Request request)
        {
            throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 
                if (request == null)
                {
                    Console.WriteLine("400 Bad Request Error");
                    content = "BadRequest.html";

                }
                //TODO: map the relativeURI in request to get the physical path of the resource.

                //TODO: check for redirect

                //TODO: check file exists

                //TODO: read the physical file

                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}