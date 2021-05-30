using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString = string.Empty;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            headerLines.Add("Content-Type" + Configuration.Header_delimter + "text/html");
            headerLines.Add("Date" + Configuration.Header_delimter + Logger.Get_Date());
            headerLines.Add("Content-Length" + Configuration.Header_delimter + content.Length.ToString());

            if (code == StatusCode.Redirect) // add redirected path
            {
                headerLines.Add("Location" + Configuration.Header_delimter + redirectoinPath);
            }

            // TODO: Create the request string
            responseString += Configuration.ServerHTTPVersion + " "; // Add ver
            responseString += GetStatusLine(code) + Configuration.Delimter; // Add Status Respond

            foreach (string line in headerLines)
            {
                responseString += line + Configuration.Delimter;
            }
            responseString += Configuration.Delimter; // Add Blank Line

            responseString += content; // Add content
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            switch (code)
            {
                case StatusCode.OK:
                    statusLine = StatusCode.OK.ToString() + " OK";
                    break;
                case StatusCode.InternalServerError:
                    statusLine = StatusCode.InternalServerError.ToString() + " InternalServerError";
                    break;
                case StatusCode.NotFound:
                    statusLine = StatusCode.NotFound.ToString() + " NotFound";
                    break;
                case StatusCode.BadRequest:
                    statusLine = StatusCode.BadRequest.ToString() + " BadRequest";
                    break;
                case StatusCode.Redirect:
                    statusLine = StatusCode.Redirect.ToString() + " Redirect";
                    break;
                default:
                    statusLine = StatusCode.BadRequest.ToString() + " BadRequest";
                    break;
            }

            return statusLine;
        }
    }
}
