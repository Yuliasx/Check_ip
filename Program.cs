using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;

namespace IpChecker
{
    class IpData
{
    public string ip { get; set; } = "";
    public string city { get; set; } = "";
    public string region { get; set; } = "";
    public string country { get; set; } = "";
}

    class Program
    {
        static HttpClient client = new HttpClient();

        static IpData GetIpInfo(string ip)
        {
            try
            {
                string url = "https://ipinfo.io/" + ip + "/json";
                var response = client.GetStringAsync(url).Result;
                var data = JsonConvert.DeserializeObject<IpData>(response);
                return data;
            }
            catch
            {
                return null;
            }
        }

        static void Main(string[] args)
        {
            string file = "IPs.txt";
            
            if (!File.Exists(file))
            {
                Console.WriteLine("file not found");
                return;
            }

            string[] lines = File.ReadAllLines(file);
            List<string> ips = new List<string>();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0)
                {
                    ips.Add(trimmed);
                }
            }

            Console.WriteLine("loaded " + ips.Count + " ips");
            Console.WriteLine("");

            List<IpData> results = new List<IpData>();

            foreach (string ip in ips)
            {
                Console.WriteLine("checking " + ip);
                var data = GetIpInfo(ip);
                
                if (data != null)
                {
                    results.Add(data);
                    System.Threading.Thread.Sleep(500);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("results:");
            Console.WriteLine("");

            Dictionary<string, int> countries = new Dictionary<string, int>();

            foreach (var item in results)
            {
                if (countries.ContainsKey(item.country))
                {
                    countries[item.country] = countries[item.country] + 1;
                }
                else
                {
                    countries[item.country] = 1;
                }
            }

            Console.WriteLine("countries:");
            foreach (var pair in countries)
            {
                Console.WriteLine(pair.Key + " - " + pair.Value);
            }

            Console.WriteLine("");

            string max_country = "";
            int max_count = 0;

            foreach (var pair in countries)
            {
                if (pair.Value > max_count)
                {
                    max_count = pair.Value;
                    max_country = pair.Key;
                }
            }

            Console.WriteLine("most ips from: " + max_country + " (" + max_count + ")");
            Console.WriteLine("");

            List<string> cities = new List<string>();

            foreach (var item in results)
            {
                if (item.country == max_country)
                {
                    cities.Add(item.city);
                }
            }

            Console.WriteLine("cities in " + max_country + ":");
            foreach (string city in cities)
            {
                Console.WriteLine(city);
            }

            Console.WriteLine("");
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}