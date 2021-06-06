using System;
using System.IO;
using System.Globalization;
namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sr = new StreamWriter(Configuration.Log_file_path);
        public static void LogException(Exception ex , string source_class)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            
            string msg = Get_Date()+"\nSource :" + ex.Source + "\n" + ex.Message + "\n" +
                "Fun Caused EX :\t"+ex.TargetSite +"\nSourceClass :-> " + source_class;
            File.WriteAllText(Configuration.Log_file_path, msg + Configuration.Delimter);
        }

        public static string Get_Date()
        {
            return DateTime.Now.ToString(new CultureInfo("en-US"));
        }
    }
}
