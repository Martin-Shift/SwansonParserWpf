using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwansonParserWpf.Models
{
    public class SiteParser
    {
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
                if (page > parser.GetPageCount(text))
                {
                    break;
                }
            } while (!isDone);
        }
    }
}
