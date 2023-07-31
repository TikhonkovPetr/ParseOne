using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, List<List<string>>> result = Parsing(url: "https://liquipedia.net/dota2/Riyadh_Masters/2023");
            if(result!=null)
            {
                foreach(var item in result)
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine(item.Key);
                    Console.WriteLine("-----------------------------------");
                    item.Value.ForEach(r => Console.WriteLine(string.Join("\t",r)));
                    Console.WriteLine("-----------------------------------");
                }
                Console.ReadKey();
            }
        }

        private static Dictionary<int, List<List<string>>> Parsing(string url)
        {
            try
            {
                Dictionary<int ,List<List<string>>> result = new Dictionary<int ,List<List<string>>>();
                using (HttpClientHandler hdl = new HttpClientHandler {AllowAutoRedirect=false,AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.None | System.Net.DecompressionMethods.GZip })
                {
                    using (var client= new HttpClient(hdl))
                    {
                        using (HttpResponseMessage respons = client.GetAsync(url).Result)
                        {
                            if(respons.IsSuccessStatusCode)
                            {
                                var html = respons.Content.ReadAsStringAsync().Result;
                                if(!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                    var tables = doc.DocumentNode.SelectNodes(".//div[@class='template-box']//div//table[@class='wikitable wikitable-bordered grouptable']");
                                    if(tables != null && tables.Count()>0)
                                    {
                                        int counter = 1;
                                        foreach (var tab in tables)
                                        {
                                            var table = tab.SelectSingleNode(".//tbody");
                                            if(table != null)
                                            {
                                                var rows = table.SelectNodes(".//tr");
                                                if(rows != null && rows.Count>0)
                                                {
                                                    var res = new List<List<string>>();
                                                    foreach(var row in rows)
                                                    {
                                                        var cells = row.SelectNodes(".//td");
                                                        if(cells!= null && cells.Count>0)
                                                        {
                                                            res.Add(new List<string>(cells.Select(c => c.InnerText)));
                                                        }
                                                    }
                                                    result.Add(counter,res);
                                                    counter++;
                                                }
                                            }
                                        }
                                        return result;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Table is null");
                                        Console.ReadKey();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            return null;
        }
    }
}
