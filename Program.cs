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

    public static class Extensionmethods
    {
        public static int? ToTimestamp(this DateTime? d)
        {
            if (d==null)
                return null;
            DateTime D = (DateTime)d;
            return Convert.ToInt32((D.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }

        public static int ToTimestamp(this DateTime d)
        {
            return Convert.ToInt32((d.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }

        public static DateTime ToDateTime(this int i)
        {
            DateTime dt = new DateTime(i * 10000000L + 621355968000000000).ToLocalTime();
            return dt;
        }
    }
}