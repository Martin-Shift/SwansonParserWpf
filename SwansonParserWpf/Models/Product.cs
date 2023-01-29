using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SwansonParserWpf.Models
{
    public class Product
    {
        [JsonPropertyName("productName")]
        public string Name { get; set; }
        [JsonPropertyName("productVendor")]
        public string Vendor { get; set; }
        [JsonPropertyName("productPartNumber")]
        public string ID { get; set; }
        [JsonPropertyName("productDiscountPrice")]
        public string Price { get; set; }
        [JsonPropertyName("statusMessage")]
        public string Message { get; set; }
        [JsonPropertyName("longURL")]
        public string URL { get; set; }
        [JsonPropertyName("productDetails")]
        public string Details { get; set; }
        [JsonPropertyName("rating")]
        public double Rating { get; set; }
    }
}