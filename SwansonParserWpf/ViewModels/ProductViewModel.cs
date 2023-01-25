using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using My.BaseViewModels;
using SwansonParserWpf.Models;

namespace SwansonParserWpf.ViewModels
{
    public class ProductViewModel : NotifyPropertyChangedBase
    {
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
        public string ImageSource
        {
            get => $"https://media.swansonvitamins.com/images/items/master/{ID}.jpg";
        }
    }
}
