﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour FenetreOuvrirMenu.xaml
    /// </summary>
    public partial class FenetreOuvrirMenu : Window
    {
        private IMenuService MenuService { get; set; }
        private ObservableCollection<Menu> ListeMenus { get; set; }
        public Menu MenuSelectionne { get; set; }
        
        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreOuvrirMenu()
        {
            InitializeComponent();

            MenuService = ServiceFactory.Instance.GetService<IMenuService>();
            
            ListeMenus = new ObservableCollection<Menu>(MenuService.RetrieveSome(new RetrieveMenuArgs { IdMembre = (int)App.MembreCourant.IdMembre }));

            if(ListeMenus.Count == 0)
            {
                lblAucunMenu.Visibility = Visibility.Visible;
            }

            dgMenus.ItemsSource = ListeMenus;
        }

        /// <summary>
        /// Événement lancé lors de la sélection d'un menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSelection_Checked(object sender, RoutedEventArgs e)
        {
            btnOuvrir.IsEnabled = true;
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Ouvrir.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOuvrir_Click(object sender, RoutedEventArgs e)
        {
            MenuSelectionne = (Menu)dgMenus.SelectedItem;
            DialogResult = true;
            Close();
        }
    }
}
