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

        private int port;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.137.1"), portNumber);
            port = portNumber;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            List_Servers.Add(ServerSocket);
            server_ID = List_Servers.Count - 1;
            ServerSocket.Bind(endpoint);
            Console.WriteLine("Server ID : {0}  -> Binded successfly to Local EndPoint {1}" , server_ID , ServerSocket.LocalEndPoint);
        }

        public void StartServer()
        {

            // TODO: Listen to connections, with large backlog.
            ServerSocket.Listen(100);
            Console.WriteLine("Server ID: {0} -> Start Listening on Port : {1}", server_ID ,port);
            Console.WriteLine("Listenning .....");
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

            bool Multiple_Connection_Over_time = false;
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
                    else
                        Console.WriteLine("Recieved request from :{0}", newConnection.RemoteEndPoint);

                    Console.WriteLine("Recived \n{0}", msg);
                    // TODO: Create a Request object using received request string                    
                    Request request = new Request(msg);

                    HandleRequest(request);

                    if (request.Multiple_Connection_Over_time()) Multiple_Connection_Over_time = true;
                    else Multiple_Connection_Over_time = false;

                    // TODO: Call HandleRequest Method that returns the response

                    // TODO: Send Response back to client
                    //newConnection.Send(data, 0, receivedLen, SocketFlags.None);

                    msg = string.Empty; // clear the msg
                    if (!Multiple_Connection_Over_time) break; // end the connection
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
            string content;
            try
            {
                //TODO: check for bad request 
                

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
            return null;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            string redirectied_path = string.Empty;
            Configuration.RedirectionRules.TryGetValue(relativePath, out redirectied_path);

            return redirectied_path;
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
                string[] lines = File.ReadAllLines(filePath);
                Console.WriteLine("Load Redirection File .... " + "\n" + "With {0} Enteries", lines.Length);

                Configuration.RedirectionRules = new Dictionary<string, string>();

                bool all_lines_in_format = true;

                if (lines.Length > 0)
                {
                    foreach (string line in lines)
                    {
                        string[] rule = line.Split(Configuration.Redirection_File_delimter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (rule.Length == 2)
                            Configuration.RedirectionRules.Add(rule[0], rule[1]); // key is the old site name :: Value is new site name
                        else
                            all_lines_in_format = false;
                    }
                }
                if (!all_lines_in_format)
                    throw new FormatException();
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (FormatException ef)
            {
                Console.WriteLine("Redirection file is in bad fromat Need Fix");
                Logger.LogException(ef);
            }
            catch (Exception e)
            {
                Console.WriteLine("Redirection File is Missing OR Corupted");
                Logger.LogException(e);
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}