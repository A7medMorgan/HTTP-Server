using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    static class Configuration
    {
        public static string ServerHTTPVersion = "HTTP/1.1";
        public static string WebPagesTextType = "text/html";
        public static string S_WebPagesExtention = ".html";

        // Delemiter
        public static string Delimter = "\r\n";
        public static string Header_delimter = ": ";
        public static string Redirection_File_delimter = ",";

        // Server Typies
        public static string ServerType = "FCISServer";

        // Redirection Holder
        public static Dictionary<string, string> RedirectionRules;

        // Physical Path of Pages Holder
        public static Dictionary<string, string> Pages_path;

        // Absolute & Relative Pathies for Server File
        private static string _RELATIVE_PATH = "../../../File\\";
        public static string Redirection_file_path = _RELATIVE_PATH + "redirectionRules.txt";
        public static string Log_file_path = _RELATIVE_PATH + "log.txt";

        // Absolute & Relative Pathies for WEB PAGES
        public static string RootPath = "C:\\inetpub\\wwwroot\\fcis1"; // of the OS
        public static string ReletivePath = "../../../../inetpub\\wwwroot\\fcis1\\"; // from the Debug File of the VS proj

        // Exceptional Pages
        public static string RedirectionDefaultPageName = "Redirect.html";
        public static string BadRequestDefaultPageName = "BadRequest.html";
        public static string NotFoundDefaultPageName = "NotFound.html";
        public static string InternalErrorDefaultPageName = "InternalError.html";

/*        public static string P_Main = "main.html";

        public static string P_aboutus = "aboutus.html";
        public static string P_aboutus2 = "aboutus2.html";*/

    }
}
