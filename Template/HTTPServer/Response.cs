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

            headerLines.Add("Content-Type" + Configuration.Header_delimter + contentType);
            headerLines.Add("Date" + Configuration.Header_delimter + Logger.Get_Date());
            headerLines.Add("Content-Length" + Configuration.Header_delimter + content.Length.ToString());

            if (code == StatusCode.Redirect) // add redirected path
            {
                headerLines.Add("Location" + Configuration.Header_delimter + redirectoinPath );
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
        public Response() { }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            statusLine = (int)code + " " + code.ToString();

            return statusLine;
        }
        public Response CreateRespond(bool Request_ParseRequest,RequestMethod requestMethod ,string ReletiveURI, int server_id)
        {
            string Physical_path = string.Empty;
            string content;
            StatusCode statusCode;
            try
            {
                //TODO: check for bad request 
                if (!Request_ParseRequest) // parsing not successed
                {
                    statusCode = StatusCode.BadRequest;
                    Physical_path = Configuration.ReletivePath + Configuration.BadRequestDefaultPageName;

                    //TODO: read the physical file
                    content = this.LoadDefaultPage(Physical_path);
                    // Create OK response
                    return new Response(statusCode, Configuration.WebPagesTextType, content, string.Empty);
                }

                switch (requestMethod)
                {
                    case RequestMethod.GET:
                        return Create_Get_Respond(ReletiveURI);
                    case RequestMethod.POST:
                        break;
                    case RequestMethod.HEAD:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex, this.GetType().ToString());

                // TODO: in case of exception, return Internal Server Error. 
                statusCode = StatusCode.InternalServerError;
                Physical_path = Configuration.ReletivePath + Configuration.InternalErrorDefaultPageName;
                //TODO: read the physical file
                content = this.LoadDefaultPage(Physical_path);
                // Create OK response
                return new Response(statusCode, Configuration.WebPagesTextType, content, string.Empty);
            }
            return null;
        }

        private Response Create_Get_Respond(string relativeURI)
        {
            string content = string.Empty;
            string Redirection_path = string.Empty;
            string Physical_path = null;
            StatusCode statusCode;
                 // request has all the info need by server
                
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    Physical_path = this.MAP_URI_ToPage_PhysicalPath(relativeURI);

            //TODO: check file exists

            if (Physical_path != null)
            {
                statusCode = StatusCode.OK;
            }
            else
            {
                //TODO: check for redirect
                Redirection_path = GetRedirectionPagePathIFExist(relativeURI);

                if (Redirection_path == null) // no redirection enteries
                {
                    Physical_path = Configuration.ReletivePath + Configuration.NotFoundDefaultPageName;
                    statusCode = StatusCode.NotFound;
                }
                else
                {
                    Physical_path = Configuration.ReletivePath + Configuration.RedirectionDefaultPageName;
                    statusCode = StatusCode.Redirect;
                }
            }
            //TODO: read the physical file
            content = this.LoadDefaultPage(Physical_path);
            // Create OK response
            return new Response(statusCode, Configuration.WebPagesTextType, content, Redirection_path);
        }


        private string MAP_URI_ToPage_PhysicalPath(string ReletiveURI)
        {
            string filepath = null;

            if (ReletiveURI == null) return filepath;

            string[] splited = ReletiveURI.Split(Configuration.URI_delemiter, StringSplitOptions.RemoveEmptyEntries);

            string page_name = splited[splited.Length - 1]; // get the last name in URI which corespond to page name

            if (Configuration.Pages_path != null)
                Configuration.Pages_path.TryGetValue(page_name, out filepath);

            return filepath;
        }

        public string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirectied_path = null;
            string[] splited = relativePath.Split(Configuration.URI_delemiter, StringSplitOptions.RemoveEmptyEntries);

            string old_page_name = splited[splited.Length - 1]; // get the last name in URI which corespond to page name

            if (Configuration.RedirectionRules != null)
                Configuration.RedirectionRules.TryGetValue(old_page_name, out redirectied_path);

            return redirectied_path;
        }


        public string LoadDefaultPage(string defaultPageName)
        {
            //string filePath = Path.Combine(Configuration.RootPath, defaultPageName); // From any Physical File at the Drive
            //string filePath = Path.Combine(Configuration.ReletivePath, defaultPageName); // Debug Mode

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (File.Exists(defaultPageName))
                return File.ReadAllText(defaultPageName);

            // else read file and return its content
            else
            {
                Logger.LogException(new FileNotFoundException(), this.GetType().ToString());
                return string.Empty;
            }
        }
    }
}
