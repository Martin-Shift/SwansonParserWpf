using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using My.BaseViewModels;
using SwansonParserWpf.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace SwansonParserWpf.ViewModels
{
    public class ProductViewModel : NotifyPropertyChangedBase
    {    
        public string Content;
        public ProductViewModel(Product product) { Product = product; }
        public Product Product { get; set; }
        public string Name
        {
            get => Product.Name;
            set { Product.Name = value; OnPropertyChanged(nameof(Name)); }
        }
        public string Vendor
        { get => Product.Vendor; set { Product.Vendor = value; OnPropertyChanged(nameof(Vendor)); } }
        public string Price
        { get => Product.Price; set { Product.Price = value; OnPropertyChanged(nameof(Price)); } }
        public string ID
        { get => Product.ID; set { Product.ID = value; OnPropertyChanged(nameof(ID)); } }
        public string Message
        {
            get => Product.Message; set
            {
                Product.Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public double Rating
        {
            get => Product.Rating; set { Product.Rating = value; OnPropertyChanged(nameof(Rating)); }
        }
        public Brush RatingColor
        {
            get
            {
                if (Rating > 4) return Brushes.Green;
                if (Rating > 3) return Brushes.Yellow;
                return Brushes.Red;
            }
        }
        public string Details
        {
            get => Product.Details; set { Product.Details = value; OnPropertyChanged(nameof(Details)); }
        }
        public string URL { get => $"https://www.swansonvitamins.com/{Product.URL}"; }
        private string _selectedImage { get; set; }
        public string SelectedImage
        {
            get => _selectedImage;
            set { _selectedImage = value; OnPropertyChanged(nameof(SelectedImage)); }
        }
        private ObservableCollection<string> _images;
        public ObservableCollection<string> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
            }
        }
        public string Description
        {
            get
            {
                var list = new List<string>();
                var doc = new HtmlDocument();
                doc.LoadHtml(Content);
                var node = doc.DocumentNode;
                var desc = node.QuerySelector("div[itemprop=description]");
                return desc.InnerText.Replace("Product Description","");
            }
        }
        public string ImageSource
        {
            get => $"https://media.swansonvitamins.com/images/items/master/{ID}.jpg";
        }
    }
}
