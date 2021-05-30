using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter(Configuration.Log_file_path);
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
        }
        public static string Get_Date()
        {
            return DateTime.Now.ToString(new CultureInfo("en-US"));
        }
    }
}
