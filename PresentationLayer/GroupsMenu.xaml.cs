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

namespace PresentationLayer
{
    /// <summary>
    /// Menu Partii.
    /// Umożliwia podgląd, dodawanie i przenoszenie partii.
    /// </summary>
    public partial class GroupsMenu : UserControl   // 10
    {
        private MainWindow mainWindow;
        private CancellationTokenSource tokenSource;
        private List<DatabaseAccess.Group> groups;
        private bool isLoaded;
        private bool showInternal = false;
        private bool showExternal = false;

        /// <summary>
        /// Inicjalizacja menu
        /// </summary>
        /// <param name="mainWindow">Referencja do okna głównego</param>
        public GroupsMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Title = "Przetwarzane Partie";
            tokenSource = new CancellationTokenSource();

            mainWindow.ReloadWindow = LoadData;

            isLoaded = false;
            InitializeComponent();

            showInternal = (bool)Internal.IsChecked;
            showExternal = (bool)External.IsChecked;

            LoadData();
        }

        /// <summary>
        /// Ładowanie danych
        /// </summary>
        private void LoadData()
        {
            DatabaseAccess.SystemContext.Transaction(context =>
                {
                    groups = (from g in context.Groups.Include("Sector.Warehouse")
                              where true
                              select g).ToList();
                    return true;
                }, t => Dispatcher.BeginInvoke(new Action(() =>
                {
                    isLoaded = true;
                    InitializeData();
                }
                )), tokenSource);
        }

        /// <summary>
        /// Wyświetlanie danych
        /// </summary>
        private void InitializeData()
        {
            if (!isLoaded)
                return;

            GroupsGrid.Items.Clear();

            foreach (DatabaseAccess.Group g in groups)
            {
                if ((showInternal && g.InInternal()) || (showExternal && !g.InInternal()))
                    GroupsGrid.Items.Add(g);
            }

            LoadingLabel.Visibility = System.Windows.Visibility.Hidden;
            GroupsGrid.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Nowa partia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            GroupDialog dlg = new GroupDialog(mainWindow, 0);
            dlg.Show();
        }

        /// <summary>
        /// Manu główne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new MainMenu(mainWindow));
        }

        /// <summary>
        /// Ładowanie menu
        /// </summary>
        /// <param name="menu"></param>
        private void LoadNewMenu(UserControl menu)
        {
            Grid content = Parent as Grid;

            tokenSource.Cancel();

            content.Children.Remove(this);
            content.Children.Add(menu);
        }

        /// <summary>
        /// Zmiana zaznaczenia magazynów wewnętrznych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Internal_Checked(object sender, RoutedEventArgs e)
        {
            if (Internal != null)
            {
                showInternal = (bool)Internal.IsChecked;
                LoadData();
            }
        }

        /// <summary>
        /// Zmiana zaznaczenia magazynów wewnętrznych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Internal_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Internal != null)
            {
                showInternal = (bool)Internal.IsChecked;
                LoadData();
            }
        }

        /// <summary>
        /// Zmiana zaznaczenia magazynow partnerów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void External_Unchecked(object sender, RoutedEventArgs e)
        {
            if (External != null)
            {
                showExternal = (bool)External.IsChecked;
                LoadData();
            }
        }

        /// <summary>
        /// Zmiana zaznaczenia magazynów partnerów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void External_Click(object sender, RoutedEventArgs e)
        {
            if (External != null)
            {
                showExternal = (bool)External.IsChecked;
                LoadData();
            }
        }

        /// <summary>
        /// Partia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupIdClick(object sender, RoutedEventArgs e)
        {
            LoadNewMenu(new GroupMenu(mainWindow, (int)(sender as Button).Tag));
        }

        /// <summary>
        /// Przesuń
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShiftClick(object sender, RoutedEventArgs e)
        {
            DatabaseAccess.Group toMove = groups.Find(delegate(DatabaseAccess.Group gr)
            {
                return gr.Id == (int)((sender as Button).Tag);
            });

            if (toMove.InInternal())
            {
                ShiftDialog dlg = new ShiftDialog(mainWindow, (int)((sender as Button).Tag));
                dlg.Show();
            }
            else
                MessageBox.Show("Partia znajduje się u partnera. Nie mozna przesunąć", "Uwaga");
        }
    }
}
