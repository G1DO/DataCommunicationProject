using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.LoadRedirectionRules(redirectionMatrixPath);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1000));
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000); // large backlog

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Thread thread = new Thread(HandleConnection);
                thread.Start(clientSocket);
                

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = obj as Socket;
           
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    // TODO: Receive request
                    int receivedLen = clientSocket.Receive(buffer);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0) break;
                    // TODO: Create a Request object using received request string
                    string requestString = Encoding.UTF8.GetString(buffer, 0, receivedLen);

                    // TODO: Call HandleRequest Method that returns the response
                    Request request = new Request(requestString);
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response.ResponseString);
                    clientSocket.Send(responseBytes);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    break;
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
           // throw new NotImplementedException();
            //string content;
            try
            {

                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    string bad = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", bad, string.Empty);
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string relativePath = request.relativeURI;
                if (relativePath.EndsWith("/"))
                    relativePath += "index.html";

                //TODO: check for redirect
                string redirectTarget = GetRedirectionPagePathIFExist(relativePath);
                if (!string.IsNullOrEmpty(redirectTarget))
                {
                    string redirectPage = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    return new Response(StatusCode.Redirect, "text/html", redirectPage, redirectTarget);
                }
                //TODO: check file exists
                string physicalPath = Path.Combine(Configuration.RootPath, relativePath.TrimStart('/'));
                if (!File.Exists(physicalPath))
                {
                    string notFound = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", notFound, string.Empty);
                }


                //TODO: read the physical file
                string content = File.ReadAllText(physicalPath);
               

                // Create OK response and return it to handleConnection
                return new Response(StatusCode.OK, "text/html", content, string.Empty);

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                string error = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", error, string.Empty);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules != null &&
                Configuration.RedirectionRules.TryGetValue(relativePath, out string target))
                return target;
            return string.Empty;
           
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new FileNotFoundException($"Default page not found: {filePath}"));
                return string.Empty;
            }
            // else read file and return its content
            return File.ReadAllText(filePath);
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                Configuration.RedirectionRules = new Dictionary<string, string>();

                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { ',' });
                    // ensure both sides include a leading slash
                    string from = "/" + parts[0].Trim().TrimStart('/');
                    string to = "/" + parts[1].Trim().TrimStart('/');
                    Configuration.RedirectionRules[from] = to;
                }
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                
                Environment.Exit(1);
            }
        }
    }
}
