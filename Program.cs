using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            //make c:\temp if it doesnt exist
            Directory.CreateDirectory(@"c:\temp");

            //exit if the commain line is wrong
            if (args.Length !=1)
            {
                using (StreamWriter w = File.AppendText(@"C:\temp\logger.log"))
                {
                    Log("------------------------------------------------------------------------------", w);
                    Log("No comment was entered. quit now with error", w);
                    Log("------------------------------------------------------------------------------", w);
                }
                Environment.Exit(1);
            }

            //convert the cmd line arg to a usible string
            
            string note = args[0].ToString();
            using (StreamWriter w = File.AppendText(@"C:\temp\logger.log"))
            {
                Log("New run------------------------------------------------------------------------------", w);
                Log(note, w);
                
            }


            //get the FW config using netsh command
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "advfirewall show currentprofile";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            // parse output and look for results
            using (StreamWriter w = File.AppendText(@"C:\temp\logger.log"))
            {
                Log(output, w);
            }

            //get the IP of the local wired NIC
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //Console.WriteLine(ni.Name);

                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            //Console.WriteLine(ip.Address.ToString());
                            using (StreamWriter w = File.AppendText(@"C:\temp\logger.log"))
                            {
                                Log("The IP of " +ni.Name +" is " + ip.Address.ToString(), w);
                                
                            }
                        }
                    }
                }
            }

            //log the ip and datetime info along with the cmd line arg
            
            
        }

        //generic class I use for writing log files
        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0}: {1}: {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), logMessage);
        }

    }

}
