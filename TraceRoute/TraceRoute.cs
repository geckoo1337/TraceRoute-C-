using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TraceRouteApp
{
    class TraceRoute
    {
        public void TraceRouteX(string ipAddressOrHostName, int hops = 30, int latency = 1000, bool dns = false)
        {   // this way we can check if IP or HostName are valid 
            try
            {   // a tip to get only IPv4
                IPAddress ipAddress = Dns.GetHostAddresses(ipAddressOrHostName).First(address => address.AddressFamily == AddressFamily.InterNetwork);

                using (Ping pingSender = new Ping())
                {
                    PingOptions pingOptions = new PingOptions();
                    Stopwatch stopWatch = new Stopwatch();
                    byte[] bytes = new byte[32]; // 32 bits as packet
                    string hostName; // for DNS
                    PingReply pingReply;

                    pingOptions.DontFragment = true;
                    pingOptions.Ttl = 1;
                    int maxHops = hops;
                    // just a first message on screen before TraceRoute
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(string.Format(" TraceRoute to {0} with a maximum of {1} hops, setting latency to {2} and DNS feature to {3}\n", ipAddress, maxHops, latency, dns));
                    Console.ResetColor();

                    for (int i = 1; i < maxHops + 1; i++)
                    {   // for latence
                        stopWatch.Reset();
                        stopWatch.Start(); // ping options allows us to set TTL                  
                        pingReply = pingSender.Send(ipAddress, latency, new byte[32], pingOptions);
                        stopWatch.Stop();

                        if (dns)
                        {   // DNS resolving using a timer
                            Task<string> task = Task<string>.Factory.StartNew(() =>
                            {
                                var t = Dns.GetHostEntry(pingReply.Address).HostName;
                                return t.ToString(); // well well ?
                            });
                            // waiting an answer
                            bool success = task.Wait(latency);

                            if (success)
                                hostName = task.Result;
                            else
                                hostName = "TimeOut (no DNS info)";                  
                        }
                        else
                            hostName = "";
                        // display string with all informations - easy reading
                        if (i % 2 == 0)
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        else
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        // display final output
                        Console.WriteLine(string.Format(" h{0}\t{1} ms\t{2}\t\t{3}", i, stopWatch.ElapsedMilliseconds, pingReply.Address, hostName));
                        Console.ResetColor();
                        // status
                        if (pingReply.Status == IPStatus.Success)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n The IP target " + pingReply.Address + " has been reached");
                            Console.ResetColor();
                            break;
                        }
                        // increment TTL to get the next node
                        pingOptions.Ttl++;
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ERROR: You Have Some TIMEOUT Issue Or An Invalid IP/Host");
                Console.ResetColor();
            }
        }
    }
}
