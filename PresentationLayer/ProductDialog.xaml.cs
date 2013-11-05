﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Dodawanie i edycja produktów.
    /// </summary>
    public partial class ProductDialog : Window     // 19
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private int productID = -1;
        private DatabaseAccess.Product product;

        public ProductDialog(MainWindow mainWindow, int id)
        {
            this.mainWindow = mainWindow;
            productID = id;
            tokenSource = new CancellationTokenSource();

            InitializeComponent();
            this.DataContext = new ProductValidationRule();

            if (productID == -1)
            {
                Header.Content = "Wprowadź dane:";
                Title = "Tworzenie nowego produktu";
            }
            else
            {
                Header.Content = "Edytuj dane:";
                Title = "Edycja produktu";

                LoadData();
            }
        }

        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    product = (from w in context.Products
                               where w.Id == productID
                               select w).FirstOrDefault();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() => InitializeData())), tokenSource);
        }

        private void InitializeData()
        {
            ProductValidationRule rule = DataContext as ProductValidationRule;

            rule.Name = NameTB.Text = product.Name;
            rule.PatternPrice = PriceTB.Text = product.Price.ToString();
            DateTB.Text = product.Date.ToString();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var data = new
                {
                    Name = NameTB.Text,
                    Price = PriceTB.Text,
                    Date = DateTB.Text
                };

            if (string.IsNullOrEmpty(data.Date))
            {
                MessageBox.Show("Wprowadź poprawną datę.", "Uwaga");
                (sender as Button).IsEnabled = true;
                return;
            }

            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    DatabaseAccess.Product p;

                    if (productID == -1)
                    {
                        p = new DatabaseAccess.Product();
                        p.Name = data.Name;
                        p.Price = decimal.Parse(data.Price);
                        p.Date = DateTime.Parse(data.Date);

                        context.Products.Add(p);
                    }
                    else
                    {
                        p = (from warehouse in context.Products where warehouse.Id == productID select warehouse).FirstOrDefault();
                        p.Name = data.Name;
                        p.Price = decimal.Parse(data.Price);
                        p.Date = DateTime.Parse(data.Date);
                    }

                    context.SaveChanges();

                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mainWindow.ReloadWindow();
                        this.Close();
                    })), tokenSource);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            this.Close();
        }
    }
}
