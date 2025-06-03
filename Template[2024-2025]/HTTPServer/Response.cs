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
        string responseString;
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
            this.code = code;
            headerLines.Add($"Content-Type: {contentType}");
            headerLines.Add($"Content-Length: {content.Length}");
            headerLines.Add($"Date: {DateTime.Now.ToString("r")}");
            if(code == StatusCode.Redirect && !string.IsNullOrEmpty(redirectoinPath))
            {
                headerLines.Add($"Location: {redirectoinPath}");
            }

            // TODO: Create the responseString
            string statusline = GetStatusLine(code);
            responseString = statusline + "\r\n" + string.Join("\r\n", headerLines) + "\r\n\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = code.ToString();
            if (code == StatusCode.OK)
            {
                statusLine = "OK";
            }
            else if (code == StatusCode.Redirect)
            {
                statusLine = "301 Redirection Error";
            }
            else if (code == StatusCode.NotFound)
            {
                statusLine = "404 Not Found error.";
            }
            else if (code == StatusCode.BadRequest)
            {
                statusLine = "400 Bad Request";
            }
            else if (code == StatusCode.InternalServerError)
            {
                statusLine = "500 Internal Server Error.";
            }
            else
            {
                statusLine = "UnKnown";
            }
            return $"HTTP/1.1 {(int)code} {statusLine}";
        }
    }
}
