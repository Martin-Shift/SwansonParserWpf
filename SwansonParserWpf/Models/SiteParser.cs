using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwansonParserWpf.Models
{
    public class SiteParser
    {
        public string Content = "";
        public async Task ParseCatalogAsync(string url, Action<List<Product>?> update)
        { 
            var parser = new CatalogPageParser();
            var contentProvider = new ContentProvider();
            int page = 1;
            bool isDone = false;
            do
            {
                var pageUrl = $"{url}/q?page={page}";
                var text = await contentProvider.GetContentAsync(pageUrl);
                var products = parser.GetProducts(text);
                update(products);
                page++;
                //if (page > parser.GetPageCount(text))
                if (page > 3)
                {
                    break;
                }
            } while (!isDone);
        }
        public async Task ParseSelectedPageAsync(string url,Action<List<string>> update)
        {
            var contentProvider = new ContentProvider();
            Content = await contentProvider.GetContentAsync(url);
            var matches = Regex.Matches(Content, @"img src=""(\S+)""").Select(x => x.Groups[1].Value).ToList();
            var images = matches.Select(x => Regex.Replace(x, @"/(\d+)/", "/master/"));
            update(images.ToList());
        }
    }
}
