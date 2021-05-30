using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    static class Configuration
    {
        public static string ServerHTTPVersion = "HTTP/1.1";
        public static string Delimter = "\r\n";
        public static string Header_delimter = ": ";
        public static string ServerType = "FCISServer";
        public static Dictionary<string, string> RedirectionRules;
        public static string Redirection_file_path = "../../File/redirectionRules.txt";
        public static string Redirection_File_delimter = ",";
        public static string Log_file_path = "../../File/log.txt";
        public static string RootPath = "C:\\inetpub\\wwwroot\\fcis1";
        public static string RedirectionDefaultPageName = "Redirect.html";
        public static string BadRequestDefaultPageName = "BadRequest.html";
        public static string NotFoundDefaultPageName = "NotFound.html";
        public static string InternalErrorDefaultPageName = "InternalError.html";

    }
}
