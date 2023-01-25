using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;

namespace SwansonParserWpf.Models
{
    public class CatalogPageParser
    {
        public List<Product>? GetProducts(string content)
        {
            var pattern = @"""adobeRecords"":(\[.+\]),""topProduct""";
            var match = Regex.Match(content, pattern);
            var jsonStr = match.Groups[1].Value;
            return JsonSerializer.Deserialize<List<Product>>(jsonStr);
        }
        public int GetProductCount(string content) { return (int)Math.Round(decimal.Parse(Regex.Match(content, "of <!-- -->(\\d+)").Groups[1].Value)); }
        public int GetPageCount(string content) { return (int)Math.Round((double)GetProductCount(content) / 24); }
        public bool MorePages(string content)
        {
            return false; //TODO
        }
    }
}
/*

public class XmlHelper<T>
{
    public static void Serialize(T obj, string file)
    {
        var taskSerializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(file, FileMode.Create))
        {
            taskSerializer.Serialize(fs, obj);
        }
    }
    public static T Deserialize(string file)
    {
        using (var sr = new FileStream(file, FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(sr);
        }
    }
}
public void Example()
{
    List<Product> Product = new List<Product>();
    for (int i = 1; i <= 14; i++)
    {
        var provider = new ContentProvider();
        var url = $"https://www.swansonvitamins.com/ncat1/Vitamins+and+Supplements/ncat2/Supplements/ncat3/Probiotics/q?page={i}";
        var pattern = @"""adobeRecords"":(\[.+\]),""topProduct""";
        var match = Regex.Match(provider.GetContent(url), pattern);
        var jsonStr = match.Groups[1].Value;
        var products = JsonSerializer.Deserialize<List<Product>>(jsonStr);
        Console.WriteLine("Page:" + i);
        products.ForEach(product => { Console.WriteLine($"{product.Sku} {product.Title} {product.Vendor} {product.Price}"); });
        Product.AddRange(products);
    }
    XmlHelper<List<Product>>.Serialize(Product, "result2.xml");
}
*/
