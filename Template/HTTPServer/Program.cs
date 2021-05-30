using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection
                CreateRedirectionRulesFile();
           
            //Start server
            // 1) Make server object on port 1000
            Console.WriteLine("Init Server ...");

            Server server = new Server(1000, Configuration.Redirection_file_path);
            // 2) Start Server
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // Do it manully

            // TODO: Create file named redirectionRules.txt 
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2

            string Rules = string.Empty;
            while (true)
            {
                Console.WriteLine("Add Rule to Redirection.txt File press (Y) to Cansel Press (N)");
                char c = Console.ReadLine()[0];
                if (c != 'y') break;

                Console.WriteLine("Enter Rule as example1.html,example2.html");

                Rules += Console.ReadLine() + "\n";
                
            }
            if (Rules != string.Empty)
            {
                File.AppendAllText(Configuration.Redirection_file_path, Rules);

                Console.WriteLine("Rules for redirection changed Successfly");
            }
        }
         
    }
}
