using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CatjiApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public class Tools
    {
        public static List<int> RandomList(int num, int max)
        {
            var rnd = new Random();
            var result = new List<int>();
            int i;
            while (result.Count < (num < max ? num : max))
            {
                i = rnd.Next(max);
                if (!result.Contains(i))
                    result.Add(i);
            }
            return result;
        }
    }
}
