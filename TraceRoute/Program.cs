using System;

namespace TraceRouteApp
{
    class Program
    {
        static void Main(string[] args)
        {   // by default
            string ip   = "8.8.8.8";
            int hops    = 30;
            int latency = 1000;
            bool dns    = true;

            StringParser commandLine = new StringParser(args);
            // don't check ip again because we have a function in the TraceRoute class
            if (commandLine["ip"] != null)
                ip = commandLine["ip"];
            // check hops value - only digits
            if (commandLine["hops"] != null)
                if (Program.isDigitsOnly(commandLine["hops"]))
                    hops = Int32.Parse(commandLine["hops"]);
            // check latency value - only digits
            if (commandLine["latency"] != null)
                if (Program.isDigitsOnly(commandLine["latency"]))
                    latency = Int32.Parse(commandLine["latency"]);
            // dns feature
            if (commandLine["dns"] != null)
                dns = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" Another TraceRoute app with a configurable latency and DNS output\n");
            Console.Write(" Developped for fun by Geckoo1337 - for educational purposes only \n");
            Console.Write(" -----------------------------------------------------------------\n");
            Console.Write(" Usage : -ip [target] -hops [int] -latency [int] -dns\n");
            Console.Write(" Example : -ip 8.8.8.8 -hops 30 -latency 1000 -dns\n\n");
            Console.ResetColor();

            TraceRoute t = new TraceRoute();
            t.TraceRouteX(ip, hops, latency, dns); 
            // end of file
            return;
        }
        // check if this argument has only digits
        static bool isDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}