using System;
using System.IO;


namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            File.Delete(Configuration.Log_file_path); // clear the log file
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

            Console.WriteLine("Modify Rule in Redirection.txt File press (Y) to Cansel Press (Any)");
            char c = Console.ReadLine()[0];
            if (c != 'y') return;

            Console.WriteLine("Enter Rule as \"example1,example2\" just THE NAME and press Enter \n" +
                              "To End Enter \"Null\"");
            int i = 0;
            while (true)
            {
                Console.Write("Rule Number {0} :->  ", ++i);
                string temp = Console.ReadLine();
                if (temp.Equals("null")) break;

                Rules += temp + "\n";
            }
            if (Rules != string.Empty)
            {
                File.AppendAllText(Configuration.Redirection_file_path, Rules);

                Console.WriteLine("Rules for redirection changed Successfly");
            }
        }
         
    }
}
