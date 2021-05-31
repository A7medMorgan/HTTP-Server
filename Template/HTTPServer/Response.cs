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
        public Response()
        {
            
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            switch (code)
            {
                case StatusCode.OK:
                    statusLine = StatusCode.OK.ToString() + " " + StatusCode.OK.ToString();
                    break;
                case StatusCode.InternalServerError:
                    statusLine = StatusCode.InternalServerError + " " + StatusCode.InternalServerError.ToString();
                    break;
                case StatusCode.NotFound:
                    statusLine = StatusCode.NotFound + " " + StatusCode.NotFound.ToString();
                    break;
                case StatusCode.BadRequest:
                    statusLine = StatusCode.BadRequest + " " + StatusCode.BadRequest.ToString();
                    break;
                case StatusCode.Redirect:
                    statusLine = StatusCode.Redirect + " " + StatusCode.Redirect.ToString();
                    break;
                default:
                    statusLine = StatusCode.BadRequest + " " + StatusCode.BadRequest.ToString();
                    break;
            }

            return statusLine;
        }
        public Response CreateRespond(Request request , int server_id)
        {
            string Physical_path = string.Empty;
            string content;
            StatusCode statusCode;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest(server_id)) // parsing not successed
                {
                    statusCode = StatusCode.BadRequest;
                    Physical_path = Configuration.ReletivePath + Configuration.BadRequestDefaultPageName;

                    //TODO: read the physical file
                    content = this.LoadDefaultPage(Physical_path);
                    // Create OK response
                    return new Response(statusCode, Configuration.WebPagesTextType, content, string.Empty);
                }

                switch (request.method)
                {
                    case RequestMethod.GET:
                        return Create_Get_Request(request, server_id);
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

        private Response Create_Get_Request(Request request, int server_id)
        {
            string content = string.Empty;
            string Redirection_path = string.Empty;
            string Physical_path = string.Empty;
            StatusCode statusCode;
                 // request has all the info need by server
                
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    Physical_path = this.MAP_URI_ToPage_PhysicalPath(request.relativeURI);

            //TODO: check file exists

            if (!Physical_path.Equals(string.Empty))
            {
                statusCode = StatusCode.OK;
            }
            else
            {
                //TODO: check for redirect
                Redirection_path = GetRedirectionPagePathIFExist(request.relativeURI);

                if (Redirection_path.Equals(string.Empty)) // no redirection enteries
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
            string filepath = string.Empty;

            if (ReletiveURI.Equals(string.Empty)) return filepath;

            string[] splited = ReletiveURI.Split(Configuration.URI_delemiter, StringSplitOptions.RemoveEmptyEntries);

            string page_name = splited[splited.Length - 1]; // get the last name in URI which corespond to page name

            if (Configuration.Pages_path != null)
                Configuration.Pages_path.TryGetValue(page_name, out filepath);

            return filepath;
        }

        public string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirectied_path = string.Empty;
            if (Configuration.RedirectionRules != null)
                Configuration.RedirectionRules.TryGetValue(relativePath, out redirectied_path);

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
