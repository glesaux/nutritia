﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infralution.Localization.Wpf;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Logique d'interaction pour MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : UserControl
    {
        public MenuPrincipal()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;
            
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuPrincipal.Titre;
        }

        private void btnInscription_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new CreationProfil());
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreConnexion());
        }

        private void btnCalculatrice_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreCalculatriceNutritionelle(null));
        }

        private void btnMenuListe_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Instance.GetService<IApplicationService>().ChangerVue(new FenetreGenerateurMenus());
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = Nutritia.UI.Ressources.Localisation.FenetreMenuPrincipal.Titre;
        }
    }
}
