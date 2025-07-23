using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using static ice_light_server.server_system.route_system;

namespace ice_light_server.server_system
{
    class server_system
    {
        public int serverport = 8080; // Default port for the server
        public string servername = "ice_light_server";
        public server_system(string Server_Name = "ice_light_server", int Server_port = 8000)
        {
            serverport = Server_port;
            servername = Server_Name;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:" + serverport + "/");

            RegisterRoutes();
            
            Running = true;
        }
        public bool Running = false;
        private readonly HttpListener _listener;
        public void RefreshApplicationState(object sender, FileSystemEventArgs e)
        {
            RegisterRoutes(true);
        }

        #region server_handling

        private readonly List<(Regex RoutePattern, MethodInfo Handler, ParameterInfo[] Params, url_level Priority, short port)> _routeHandlers = new();
        
        private void RegisterRoutes(bool reload = false)
        {
            if (reload)
            {
                Console.WriteLine($"{servername} [DEBUG]: Reregistering all of the Route");
                _routeHandlers.Clear();
            }
            else
                Console.WriteLine($"{servername}: Registering Route");

            foreach (var method in Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
                if (routeAttribute != null)
                {
                    var pattern = "";
                    if (routeAttribute.IsRawRegex)
                    {
                        // Use raw regex pattern directly
                        pattern = routeAttribute.Path;
                    }
                    else
                    {
                        pattern = "^" + Regex.Escape(routeAttribute.Path)
                            .Replace("\\*", ".*")      // Match any string for wildcards
                            .Replace("\\{", "(?<")     // Start named group
                            .Replace("}", ">[^/]+)")   // End named group
                            + "$";
                    }
                    var regex = new Regex(pattern, RegexOptions.Compiled);
                    var parameters = method.GetParameters();

                    // Store the route including ContentType metadata
                    _routeHandlers.Add((regex, method, parameters, routeAttribute.Priority, routeAttribute.Port));
                }
            }
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"{servername}: Listening...");
            while (true)
            {
                var context = _listener.GetContext();
                try
                {
                    ProcessRequest(context);
                }
                catch
                {
                }
            }
        }
        private void ProcessRequest(HttpListenerContext context)
        {
            var path = context.Request.Url.AbsolutePath;
            var query = context.Request.QueryString; // Retrieve query parameters
            string contentType = context.Request.ContentType;
            if (string.IsNullOrEmpty(contentType))
                contentType = "none";
            Console.WriteLine($"{servername}: Start of request.");

            Console.WriteLine($"{servername}: Requested: {path}");
            Console.WriteLine($"{servername}: Content-Type: {contentType}");
            Console.WriteLine($"{servername}: Request-Method-Type: {context.Request.HttpMethod}");

            var response = context.Response;
            object response_data = null;

            var matchedRoutes = _routeHandlers.Where(r => r.RoutePattern.IsMatch(path))
                           .OrderByDescending(r => r.Priority) // Process highest priority first
                           .ToList();

            foreach (var (regex, method, parameters, routePriority, port) in _routeHandlers)
            {
                if (port != 0 && port != context.Request.Url.Port)
                {
                    continue; // Skip routes that don't match the requested port
                }
                var match = regex.Match(path);
                if (match.Success)
                {
                    // Prepare arguments for handler
                    var args = new object[parameters.Length];
                    string body = "";
                    if (context.Request.ContentType == "application/json" || context.Request.ContentType == "application/x-www-form-urlencoded")
                    {
                        body = ParseRequestBody(context.Request);
                        Console.WriteLine($"{servername}: API Data: {body}");
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var paramType = parameters[i].ParameterType;

                        if (paramType == typeof(HttpListenerContext))
                        {
                            args[i] = context; // Pass the context directly
                        }
                        else if (paramType == typeof(Dictionary<string, string>) && context.Request.ContentType == "application/x-www-form-urlencoded")
                        {
                            // Parse form data
                            body = ParseRequestBody(context.Request);
                            args[i] = ParseFormData(body);
                        }
                        else if (context.Request.ContentType == "application/json")
                        {
                            // Parse JSON body and deserialize to expected type
                            args[i] = ParseJsonBody(body, paramType);
                        }
                        else if (paramType == typeof(string) && query != null && query[parameters[i].Name] != null)
                        {
                            // Retrieve query parameter value
                            args[i] = query[parameters[i].Name];
                        }
                        else if ((
                                paramType == typeof(int) ||
                                paramType == typeof(uint) ||
                                paramType == typeof(long) ||
                                paramType == typeof(ulong) ||
                                paramType == typeof(string)) && context.Request.ContentType == "application/x-www-form-urlencoded")
                        {
                            // Extract parameter value from form data
                            var formData = ParseFormData(body);
                            var paramName = parameters[i].Name;
                            args[i] = Convert.ChangeType(formData[paramName], paramType);
                        }
                        else if (
                                paramType == typeof(int) ||
                                paramType == typeof(uint) ||
                                paramType == typeof(long) ||
                                paramType == typeof(ulong) ||
                                paramType == typeof(string))
                        {
                            // Extract URL parameters
                            var paramName = parameters[i].Name;
                            if (match.Groups[paramName]?.Success == true)
                            {
                                args[i] = Convert.ChangeType(match.Groups[paramName].Value, paramType);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported parameter type: {paramType}");
                        }
                    }
                    try
                    {
                        // Invoke the handler
                        var result = method.Invoke(null, args);

                        // Handle void return types
                        if (method.ReturnType == typeof(void))
                        {

                            response.StatusCode = 200; // OK
                            response_data = "{ \"success\": \"true\" }";

                        }
                        else if (method.ReturnType == typeof(bool))
                        {
                            if ((bool)result == true)
                            {
                                response.StatusCode = 200; // OK
                                response_data = "{ \"success\": \"true\" }";
                            }
                            else
                                response_data = null;

                        }
                        else if (method.ReturnType == typeof(string) || method.ReturnType == typeof(byte) || method.ReturnType == typeof(byte[]))
                        {
                            response_data = result;
                        }
                        else
                        {
                            response_data = JsonConvert.SerializeObject(result);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{servername}: {e}");
                        response_data = null;
                    }
                    break;
                }
            }

            if (response_data == null)
            {
                response_data = HandleNotFound(context);
                response.StatusCode = 404;
            }

            WriteResponse(response, response_data);
        }

        private void WriteResponse(HttpListenerResponse response, object response_data)
        {

            if (response_data is string responseString)
            {
                if (responseString.Length <= 0x1ff)
                {
                    Console.WriteLine($"{servername} json Response: " + responseString);
                }
                else
                {
                    Console.WriteLine($"{servername}: json Response Length: " + responseString.Length);
                }
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (response_data is byte[] responseBytes)
            {
                Console.WriteLine($"{servername}: byte[] Response Length: " + responseBytes.Length);

                response.ContentType = "application/octet-stream";
                response.ContentLength64 = responseBytes.Length;
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                throw new InvalidOperationException("Unsupported response data type.");
            }
            Console.WriteLine($"{servername}: End of request.\n");

            response.OutputStream.Close();
        }

        private string HandleNotFound(HttpListenerContext context)
        {
            return "{\"Success\": false, \"Error\": \"404 URL Not Found: " + context.Request.Url + "\"}";
        }
        #endregion

        public static string BlankResponse = "";
        public static string BracketResponse = "[]";
    }
}
