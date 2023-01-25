using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using My.BaseViewModels;
using SwansonParserWpf.Models;

namespace SwansonParserWpf.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<string> SortVariants { get; }
        = new ObservableCollection<string>(new List<string> { "Relevance", "Price Low to high", "Price High to low", "By name A-Z", "By name Z-A", "By vendor A-Z", "By vendor Z-A" });
        private int _sortNum { get; set; } = 0;
        public int SortNum { get => _sortNum; set { _sortNum = value; OnPropertyChanged(nameof(SortNum)); Sort(); } }
        public MainWindowViewModel()
        {
            AllProducts = new();
        }
        public List<Product> SearchProducts { get; set; } = new();
        private List<Product> AllProducts { get; set; }
        private string _searchstr { get; set; } = "";
        public string Searchstr
        {
            get => _searchstr;
            set { _searchstr = value; OnPropertyChanged(nameof(Searchstr)); }
        }
        public ObservableCollection<ProductViewModel> Products
        {
            get
            {
                var collection = new ObservableCollection<ProductViewModel>();
                SearchProducts.ForEach(p => collection.Add(new ProductViewModel(p)));
                return collection;
            }
        }
        private ProductViewModel _selectedProduct;
        public ProductViewModel SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(nameof(ImageSource)); } }
        private bool _parserWork = false;
        public string ImageSource { get => _selectedProduct == null ? "" : _selectedProduct.ImageSource; }
        public ICommand Parse => new RelayCommand(x =>
        {
            var url = "https://www.swansonvitamins.com/ncat1/Vitamins+and+Supplements/ncat2/Letter+Vitamins/ncat3/Vitamin+C";
            _parserWork = true;
            Task.Run(async () =>
            {
                var parser = new SiteParser();
                await parser.ParseCatalogAsync(url, list =>
                {
                    if (list != null)
                    {
                        AllProducts.AddRange(list);
                        SearchProducts.AddRange(list);
                        OnPropertyChanged(nameof(Products));
                    }
                });

                _parserWork = false;
            });

        }, x => !_parserWork);
        private void Sort()
        {
            switch (SortNum)
            {
                case 0:
                    SearchProducts.Clear();
                    SearchProducts.AddRange(AllProducts);
                    break;
                case 1:
                    SearchProducts = AllProducts.OrderBy(p => double.Parse(p.Price,CultureInfo.InvariantCulture)).ToList();
                    break;
                case 2:
                    SearchProducts = AllProducts.OrderByDescending(p => double.Parse(p.Price, CultureInfo.InvariantCulture)).ToList();
                    break;
                case 3:
                    SearchProducts = AllProducts.OrderBy(p => p.Name).ToList();
                    break;
                case 4:
                    SearchProducts = AllProducts.OrderByDescending(p => p.Name).ToList();
                    break;
                case 5:
                    SearchProducts = AllProducts.OrderBy(p => p.Vendor).ToList();
                    break;
                case 6:
                    SearchProducts = AllProducts.OrderByDescending(p => p.Vendor).ToList();
                    break;
            }
            OnPropertyChanged(nameof(Products));
        }
    }
}
