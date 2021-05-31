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

            // Get Web Pages name of the specified Physical Path
            Configuration.Pages_path = new Dictionary<string, string>();
            this.GetPages_path(Configuration.ReletivePath ,Configuration.S_WebPagesExtention,Configuration.Pages_path);

            //TODO: initialize this.serverSocket
            EndPoint endpoint = null;
            try
            {
                endpoint = new IPEndPoint(IPAddress.Parse("192.168.137.1"), portNumber);
            
            port = portNumber;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(endpoint);
            }
            catch (Exception e)
            {
                Logger.LogException(e , this.GetType().ToString());
                Console.WriteLine("Server Cannot be Init at the specified ip | port");
                Environment.Exit(1);
            }
            List_Servers.Add(ServerSocket);
            server_ID = List_Servers.Count - 1;
            Console.WriteLine("Server ID : {0}  -> Binded successfly to Local EndPoint {1}" , server_ID , ServerSocket.LocalEndPoint);
        }

        public void StartServer()
        {

            // TODO: Listen to connections, with large backlog.
            ServerSocket.Listen(100);
            Console.WriteLine("Server ID: {0} -> Start Listening on Port : {1}", server_ID ,port);
            Console.WriteLine("Listenning .....");
            // Mark the start of the server on the log file
            File.WriteAllText(Configuration.Log_file_path,string.Format("" +
                "Server Start With ID {0} binded on EndPoint {1} \n" +
                "Date : {2}", server_ID, ServerSocket.LocalEndPoint , Logger.Get_Date()));
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

                    Console.WriteLine("\tRecived \n{0} Byte", msg.Length);
                    // TODO: Create a Request object using received request string                    
                    Request request = new Request(msg);

                    // Check to see if the client want the connection to stay open
                    if (request.Multiple_Connection_Over_time()) Multiple_Connection_Over_time = true;
                    else Multiple_Connection_Over_time = false;

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client

                    data = Encoding.ASCII.GetBytes(response.ResponseString);
                    newConnection.Send(data,SocketFlags.None);
                    Console.WriteLine("Sent \n{0} Byte", data.Length);

                    msg = string.Empty; // clear the msg
                    if (!Multiple_Connection_Over_time) break; // end the connection
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex , this.GetType().ToString());
                    break;
                }
            }

            // TODO: close client socket
            Console.WriteLine("Connection: {0} Ended by Server", newConnection.RemoteEndPoint);
            newConnection.Shutdown(SocketShutdown.Both);
            newConnection.Close();
        }

        Response HandleRequest(Request request)
        {
            string content = string.Empty;
            string Redirection_path = string.Empty;
            string RelativePath = string.Empty;
            string Physical_path = Configuration.InternalErrorDefaultPageName;
            StatusCode statusCode = StatusCode.OK;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest(server_ID))
                {
                    statusCode = StatusCode.BadRequest;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                //GetFiles();
                //TODO: check for redirect
                Redirection_path = GetRedirectionPagePathIFExist(RelativePath);
                //TODO: check file exists

                //TODO: read the physical file
                content = File.ReadAllText(Physical_path);
                // Create OK response

                Response re = new Response(statusCode, Configuration.WebPagesTextType, content, Redirection_path);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex, this.GetType().ToString());
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
        private void GetPages_path(string path , string Type , Dictionary<string, string> Holder)
        {
            DirectoryInfo dir = new DirectoryInfo(@path);
            FileInfo[] files = dir.GetFiles('*'+Type, SearchOption.AllDirectories);

            foreach (FileInfo name in files)
            {
                // add only the name without the Extention and (in lower case) just for now
                Holder.Add(name.Name.Substring(0,name.Name.Length - Type.Length).ToLower() , name.FullName); // key Name : value Full path
            }
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            //string filePath = Path.Combine(Configuration.RootPath, defaultPageName); // From any Physical File at the Drive
            string filePath = Path.Combine(Configuration.ReletivePath, defaultPageName); // Debug Mode
            
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
                Logger.LogException(ef, this.GetType().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Redirection File is Missing OR Corupted");
                Logger.LogException(e, this.GetType().ToString());
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}