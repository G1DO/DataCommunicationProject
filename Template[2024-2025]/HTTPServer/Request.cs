using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod //must be one of these
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion //must be one of these
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {

        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            requestLines = requestString.Split(new[] { "\r\n" }, StringSplitOptions.None);

            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;
            // Parse Request line
            if (!ParseRequestLine())
                return false;
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {

            //throw new NotImplementedException();
            string[] parts = requestLines[0].Split(' ');
            if (parts.Length != 3)
                return false;

            // Method
            if (!Enum.TryParse(parts[0], out method))
                return false;

            // URI
            string uri = parts[1];
            if (!ValidateIsURI(uri))
                return false;
            relativeURI = uri.StartsWith("/") ? uri : "/" + uri;

            // Version
            switch (parts[2])
            {
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                case "HTTP/0.9":
                    httpVersion = HTTPVersion.HTTP09;
                    break;
                default:
                    return false;
            }
            return true;
            // responed not implmented yet

        }



        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            return requestLines.Contains(string.Empty);

            // throw new NotImplementedException();
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            headerLines = new Dictionary<string, string>();
            int i = 1;
            while (i < requestLines.Length && requestLines[i] != string.Empty)
            {
                string line = requestLines[i];
                int idx = line.IndexOf(':');
                if (idx <= 0)
                    return false;

                string name = line.Substring(0, idx).Trim();
                string value = line.Substring(idx + 1).Trim();
                headerLines[name] = value;
                i++;
            }

            // Load body/content if any
            contentLines = requestLines.Skip(i + 1).ToArray();
            return true;
        }

    }
}
