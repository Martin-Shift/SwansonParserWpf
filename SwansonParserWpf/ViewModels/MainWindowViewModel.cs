using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using My.BaseViewModels;
using SwansonParserWpf.Models;
using SwansonParserWpf.Windows;

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
            SelectedProducts = new();
        }
        public List<Product> SearchProducts { get; set; } = new();
        private List<Product> AllProducts { get; set; }
        private string _searchstr { get; set; } = "";
        public string Searchstr
        {
            get => _searchstr;
            set { _searchstr = value; OnPropertyChanged(nameof(Searchstr)); Sort(); }
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
        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                IsSelected = Visibility.Visible;
                Task.Run(async () =>
                {
                    try
                    {
                        if (_selectedProduct != null)
                        {
                            var product = new ProductViewModel(value.Product);
                            var parser = new SiteParser();
                            await parser.ParseSelectedPageAsync(product.URL, images =>
                            {
                                product.Images = new ObservableCollection<string>(images);
                                IsSelected = Visibility.Visible;
                                OnPropertyChanged(nameof(IsSelected));
                                _selectedProduct = product;
                                _selectedProduct.Content = parser.Content;
                                _selectedProduct.SelectedImage = _selectedProduct.Images.FirstOrDefault();
                                OnPropertyChanged(nameof(SelectedProduct));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        IsSelected = Visibility.Hidden;
                    }
                });
            }
        }
        private bool _parserWork = false;

        public ObservableCollection<ProductViewModel> SelectedProducts { get; set; }
        public string Total
        {
            get => Math.Round(SelectedProducts.Sum(x => double.Parse(x.Price, CultureInfo.InvariantCulture)), 2).ToString();
        }
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
                        IsSelected = Visibility.Hidden;
                        OnPropertyChanged(nameof(IsSelected));
                        AllProducts.AddRange(list);
                        SearchProducts.AddRange(list);
                        OnPropertyChanged(nameof(Products));
                        OnPropertyChanged(nameof(Labels));
                        OnPropertyChanged(nameof(ByVendorChart));
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
                    SearchProducts = AllProducts.OrderBy(p => double.Parse(p.Price, CultureInfo.InvariantCulture)).ToList();
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
            SearchProducts = SearchProducts.Where(x => x.Name.Contains(Searchstr) || x.Vendor.Contains(Searchstr) || x.Price.Contains(Searchstr) || x.ID.Contains(Searchstr)).ToList();
            OnPropertyChanged(nameof(Products));
        }
        private Visibility _isSelected = Visibility.Hidden;
        public Visibility IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
        public ICommand Delete => new RelayCommand(x =>
        {
            AllProducts = AllProducts.Where(x => x.ID != SelectedProduct.ID).ToList();
            SearchProducts.Clear();
            SearchProducts.AddRange(AllProducts);
            OnPropertyChanged(nameof(Products));
            IsSelected = Visibility.Hidden;
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(ByVendorChart));
        });
        public ICommand FullView => new RelayCommand(x =>
        {
            var window = new ProductWindow(SelectedProduct);
            window.ShowDialog();
        });
        public ICommand CopyURL => new RelayCommand(x =>
        {
            Clipboard.SetText(SelectedProduct.URL);
        });
        public ICommand OpenInBrowser => new RelayCommand(x =>
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = SelectedProduct.URL;
                process.Start();
            }
            catch (Exception ex) { }
        });
        public ICommand AddToCart => new RelayCommand(x =>
        {
            SelectedProducts.Add(SelectedProduct);
            OnPropertyChanged(nameof(Total));
        });
        public ICommand ClearCart => new RelayCommand(x =>
        {
            SelectedProducts.Clear();
            IsSelected = Visibility.Hidden;
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(Total));
        });
        public ICommand RemoveFromCart => new RelayCommand(x =>
        {
            SelectedProducts.RemoveAt(SelectedProducts.ToList().FindIndex(x => x.ID == SelectedProduct.ID));
            IsSelected = Visibility.Hidden;
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(Total));

        });
        public ObservableCollection<string> Labels
        {
            get => new ObservableCollection<string>(Products.GroupBy(x => x.Vendor).OrderByDescending(x => x.Count()).Select(g => g.Key));
        }
        public Func<int, string> Formatter { get; set; } = value => value.ToString("N");
        public SeriesCollection ByVendorChart
        {
            get
            {
                var result = new SeriesCollection();

                var chart = new ColumnSeries()
                {
                    Values = new ChartValues<int>(Products.GroupBy(x => x.Vendor).Select(x => x.Count()).OrderByDescending(x => x)),

                    Title = "Number of Products by vendor"
                };
                result.Add(chart);
                return result;
            }
        }
        public ICommand SaveToFile => new RelayCommand(x =>
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Json file (*.json)|*.json",
            };
            if (saveFileDialog.ShowDialog().Value == true)
                File.WriteAllText(saveFileDialog.FileName, JsonSerializer.Serialize(AllProducts));
        });
        public ICommand OpenFromFile => new RelayCommand(x =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Json file (*.json)|*.json",
            };
            if (openFileDialog.ShowDialog().Value == true)
            AllProducts = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(openFileDialog.FileName));
            SearchProducts.AddRange(AllProducts);
            OnPropertyChanged(nameof(Products));
        });
    }
}