using System;
using System.Collections.Generic;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        public RequestMethod method { get; private set; }
        public string relativeURI;
        public Dictionary<string, string> headerLines {  get ;private set; }



        public HTTPVersion httpVersion { get; private set; }
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest(int server_ID)
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   

            requestLines = requestString.Split(new string[] { Configuration.Delimter }, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            if (!LoadHeaderLines(3)) return false;
            
            // check Host is valide
            if (!Validate_Host(server_ID)) return false;

            // Parse Request line
            ParseRequestLine();

            // Validate blank line exists
            if (!ValidateBlankLine()) return false;

            // Load header lines into HeaderLines dictionary
            return true;
        }
        private bool Validate_Host(int server_ID)
        {
            string Host = "";
            bool Host_found = headerLines.TryGetValue("Host", out Host); // host line
            if (Host_found)
            {
                //if (Host.Equals(Server.List_Servers[server_ID].LocalEndPoint.ToString())) return true;
                if (Server.ServerSocket.LocalEndPoint.ToString().Equals(Host)) return true;
            }
            
            return false;
        }

        public bool Multiple_Connection_Over_time() // presestance Connection
        {
            if (headerLines.Count != 0)
            {
                string conn = "";
                headerLines.TryGetValue("Connection", out conn);
                if (conn.Equals("keep-alive"))
                    return true;
            }
            return false;
        }

        private bool ParseRequestLine()
        {
            string[] RequestLine = requestLines[0].Split(new string[] { " " }, StringSplitOptions.None);
            if (RequestLine.Length != 3)
                return false;
            switch (RequestLine[0])
            {
                case "GET":
                    method = RequestMethod.GET;
                    break;
                case "HEAD":
                    method = RequestMethod.HEAD;
                    break;
                case "POST":
                    method = RequestMethod.POST;
                    break;
                default:
                    return false;

            }
            relativeURI = RequestLine[1];
            if (!ValidateIsURI(relativeURI))
                return false;
            switch (RequestLine[2])
            {
                case "":
                    httpVersion = HTTPVersion.HTTP09;
                    break;
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(int min_lines)
        {
            // skip the request line
            for (int line = 1; line < requestLines.Length; line++)
            {
                string[] header = requestLines[line].Split(new string[] { Configuration.Header_delimter }, StringSplitOptions.None);

                if (header.Length == 2)
                {
                    headerLines.Add(header[0], header[1]); // key is attribute name  :: Value is attribute value 
                    //content_type: text/html
                }
            }
            if (headerLines.Count < min_lines) return false; 
            return true;
        }

        private bool ValidateBlankLine()
        {
            bool blank_line = false;
            int start_of_content = -1 , i = 0;

            foreach (string item in requestLines)
            {
                
                if (blank_line)
                {
                    start_of_content = i; // get the start index of content
                    break;
                }
                if (item.Equals(""))
                {
                    blank_line = true;
                }
                i++;
            }
            contentLines = new string[requestLines.Length - start_of_content];
            for (int line = start_of_content; line < requestLines.Length; line++) // load the content into cotentline array
            {
                contentLines[line - start_of_content] = requestLines[line];
            }

            return blank_line;
        }

    }
}
