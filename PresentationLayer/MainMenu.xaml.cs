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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
//using System.Data.Entity;
using DatabaseAccess;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl // 1
    {
        private MainWindow mainWindow;

        private void ShowStats()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
            {
                return new
                {
                    WarehousesCount = context.GetWarehousesCount(),
                    ProductsCount = context.Products.Count(),
                    PartnersCount = context.Partners.Count(),
                    GroupsCount = context.GetInternalGroupsCount(),
                    ShiftsCount = context.Shifts.Count(),
                    Fill = context.GetFillRate()
                };
            },
                t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    WarehousesCountInfo.Text = t.WarehousesCount.ToString();
                    ProductsCountInfo.Text = t.ProductsCount.ToString();
                    PartnersCountInfo.Text = t.PartnersCount.ToString();
                    GroupsCountInfo.Text = t.GroupsCount.ToString();
                    ShiftsCountInfo.Text = t.ShiftsCount.ToString();
                    WarehousesInfo.Text = String.Format("{0}%", t.Fill);
                }
                )), tokenSource);
        }

        private CancellationTokenSource tokenSource;

        public MainMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Menu Główne";
            mainWindow.ReloadWindow = ShowStats;

            InitializeComponent();

            tokenSource = new CancellationTokenSource();

            ShowStats();
        }

        private void ChangeMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        private void ButtonWarehouses_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new WarehousesMenu(mainWindow));
        }

        private void ButtonPartners_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new PartnersMenu(mainWindow));
        }

        private void ButtonGroups_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new GroupsMenu(mainWindow));
        }

        private void ButtonProducts_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(new ProductsMenu(mainWindow));
        }
    }
}
