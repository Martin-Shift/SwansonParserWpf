using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwansonParserWpf.Models
{
    public class ContentProvider
    {
        public Task<string> GetContentAsync(string url)
        {
            var client = new HttpClient();
            return client.GetStringAsync(url);
        }
    }
}
