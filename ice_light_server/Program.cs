namespace ice_light_server
{
    internal class Program
    {
        public static string Version = "0.1.0";
        public static server_system.server_system ApiServer;
        public static server_system.server_system CdnServer;
        static void Main(string[] args)
        {
            ApiServer = new server_system.server_system("Api.ice_light", 2020);
            CdnServer = new server_system.server_system("Cdn.ice_light", 2021);
            new Thread(new ThreadStart(ApiServer.Start)).Start();
            new Thread(new ThreadStart(CdnServer.Start)).Start();
            Console.WriteLine($"Ice Light Server started. Version: {Version}");
            Console.WriteLine("Press any key to stop the server...");
            while (!Console.KeyAvailable)
            {
                
            }
        }
    }
}
