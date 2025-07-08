using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        private ProductRepository productRepo = new ProductRepository();

        [ObservableProperty]
        private ObservableCollection<Product> products = new ObservableCollection<Product>();

        [ObservableProperty]
        private int productInfo;

        public ProductViewModel()
        {
            AllProductInfo();
        }

        private async Task AllProductInfo()
        {
            var productList = await productRepo.GetproductAsync();

            Products.Clear();
            foreach (var product in productList)
            {
                Products.Add(product);
            }
        }
    }
}
