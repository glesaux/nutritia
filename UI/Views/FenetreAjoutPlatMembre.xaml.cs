﻿using Infralution.Localization.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Nutritia.UI.Ressources.Localisation;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for AjoutPlat.xaml
    /// </summary>
    public partial class AjoutPlat : UserControl
    {

        private const string cheminSauvegarde = "\\Images\\";
        public bool Plat { get; set; }

        public bool Erreur { get; set; }

        IList<Aliment> listeAliments = new List<Aliment>();
        IList<Plat> listePlats = new List<Plat>();
        IList<Aliment> listeAlimentsPlateau = new List<Aliment>();

        public AjoutPlat()
        {
            CultureManager.UICultureChanged += CultureManager_UICultureChanged;

            InitializeComponent();
            // Header de la fenetre
            App.Current.MainWindow.Title = FenetreAjoutPlatMembre.Titre;

            Erreur = false;


            listeAliments = ServiceFactory.Instance.GetService<IAlimentService>().RetrieveAll();
            listePlats = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll();

            listeAliments = listeAliments.OrderBy(aliment => aliment.Nom).ToList();
            listePlats = listePlats.OrderBy(plat => plat.Nom).ToList();

            Construire_Accordeon();
        }

        /// <summary>
        /// Méthode affichant un message d'erreur en cas d'un ou plusieurs champs invalides.
        /// </summary>
        private void Erreur_Champ()
        {
            MessageBoxResult resultat;
            resultat = MessageBox.Show(FenetreAjoutPlatMembre.ErreurMessage
                                        , FenetreAjoutPlatMembre.ErreurTitre
                                        , MessageBoxButton.OK
                                        , MessageBoxImage.Warning
                                        , MessageBoxResult.OK);
        }

        /// <summary>
        /// Méthode qui permet de charger à l'écran une image pour un aliment ou un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ajouter_Image(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string chemin = dlg.FileName;

                if (sender == btn_ajout_img_plat)
                {
                    img_plat.Source = new BitmapImage(new Uri(chemin));
                }
            }
        }

        /// <summary>
        /// Méthode de construction automatisé de l'accordéon des aliments pour la création et modification de plats.
        /// </summary>
        private void Construire_Accordeon()
        {
            Construire_Categorie_Accordeon("Arachides et noix");
            Construire_Categorie_Accordeon("Autres");
            Construire_Categorie_Accordeon("Boissons");
            Construire_Categorie_Accordeon("Céréales");
            Construire_Categorie_Accordeon("Épices");
            Construire_Categorie_Accordeon("Fruits et légumes");
            Construire_Categorie_Accordeon("Matières grasses");
            Construire_Categorie_Accordeon("Pâtes");
            Construire_Categorie_Accordeon("Poissons et fruits de mers");
            Construire_Categorie_Accordeon("Produits laitiers");
            Construire_Categorie_Accordeon("Produits sucrés");
            Construire_Categorie_Accordeon("Viandes et substituts");
        }

        /// <summary>
        /// Méthode d'ajout d'une catégorie d'aliments à l'accordéon et qui peuple cell-ci avec les bons aliments.
        /// </summary>
        /// <param name="categorie"></param>
        private void Construire_Categorie_Accordeon(string categorie)
        {
            AccordionItem categorieAccordeon = new AccordionItem();
            categorieAccordeon.Header = categorie;
            StackPanel stackCategorie = new StackPanel();
            stackCategorie.Background = Brushes.White;
            stackCategorie.Width = 284;

            foreach (Aliment unAliment in listeAliments)
            {
                if (unAliment.Categorie == categorie)
                {
                    Button btnAliment = Creer_Bouton_Aliment(true, unAliment);
                    btnAliment.ToolTip = Produire_Aide_Contextuelle(unAliment);
                    stackCategorie.Children.Add(btnAliment);
                }
            }

            categorieAccordeon.Content = stackCategorie;
            accordeon_aliments.Items.Add(categorieAccordeon);
        }

        /// <summary>
        /// Méthode qui permet de créer le bouton d'ajout d'un aliment au plateau d'un plat.
        /// </summary>
        /// <param name="plus"></param>
        /// <param name="unAliment"></param>
        /// <returns></returns>
        Button Creer_Bouton_Aliment(bool plus, Aliment unAliment)
        {
            Button btnControl = new Button();

            // Création du bouton pour supprimer ou ajouter un Plat/Aliment
            btnControl.HorizontalContentAlignment = HorizontalAlignment.Left;
            Thickness margin = btnControl.Margin;
            margin.Left = 0;
            btnControl.Margin = margin;
            btnControl.Height = 32;
            btnControl.Name = "btn" + ((int)unAliment.IdAliment).ToString();
            btnControl.ToolTip = Produire_Aide_Contextuelle(unAliment);
            btnControl.Click += Ajout_Aliment_Plateau;

            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/plusIcon.png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            btnControl.Content = stackLigne;

            return btnControl;
        }

        /// <summary>
        /// Méthode permettant de créer le bouton de retrait d'un aliment du plateau d'un plat.
        /// </summary>
        /// <param name="unBouton"></param>
        /// <param name="unAliment"></param>
        void Creer_Bouton_Retrait(Button unBouton, Aliment unAliment)
        {
            Button btnControl = new Button();

            // Création du bouton pour supprimer ou ajouter un Plat/Aliment
            btnControl.HorizontalContentAlignment = HorizontalAlignment.Left;
            Thickness margin = btnControl.Margin;
            margin.Left = 0;
            btnControl.Margin = margin;
            btnControl.Height = 32;
            btnControl.Name = unBouton.Name;
            btnControl.Click += Retirer_Aliment_Plateau;
            btnControl.ToolTip = Produire_Aide_Contextuelle(unAliment);

            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            btnControl.Content = stackLigne;

            composition_plat.Children.Add(btnControl);
        }

        /// <summary>
        /// Méthode permettant de modifier le multiplicateur d'un aliment dans le plateau d'un plat.
        /// </summary>
        /// <param name="unAliment"></param>
        /// <returns></returns>
        private StackPanel Modifier_Quantite_Bouton_Plateau(Aliment unAliment)
        {
            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblQuantite = new Label();
            lblQuantite.Content = unAliment.Quantite.ToString() + "x";
            lblQuantite.Style = (Style)(this.Resources["fontNutitia"]);
            lblQuantite.FontSize = 12;
            lblQuantite.Width = 40;
            stackLigne.Children.Add(lblQuantite);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            return stackLigne;
        }

        /// <summary>
        /// Méthode permettant de retirer le multiplicateur d'un aliment dans le plateau d'un plat.
        /// </summary>
        /// <param name="unAliment"></param>
        /// <returns></returns>
        private StackPanel Modifier_Bouton_Retrait_Plateau(Aliment unAliment)
        {
            StackPanel stackLigne = new StackPanel();
            stackLigne.Orientation = Orientation.Horizontal;
            stackLigne.HorizontalAlignment = HorizontalAlignment.Left;
            stackLigne.Width = 275;

            Image imgBouton = new Image();
            imgBouton.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/minusIcon.png"));
            imgBouton.Width = 15;
            imgBouton.Height = 15;
            stackLigne.Children.Add(imgBouton);

            Label lblNom = new Label();
            lblNom.Content = unAliment.Nom;
            lblNom.Style = (Style)(this.Resources["fontNutitia"]);
            lblNom.FontSize = 12;
            lblNom.Width = 230;
            stackLigne.Children.Add(lblNom);

            return stackLigne;
        }

        /// <summary>
        /// Méthode d'ajout d'un aliment au plateau d'un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ajout_Aliment_Plateau(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            int idButton = int.Parse(btnSender.Name.Substring(3));

            bool test = false;

            foreach (Aliment alimentVerif in listeAliments)
            {
                // Il faut trouver l'aliment dont l'id correspond à celle du bouton.
                if (idButton == alimentVerif.IdAliment)
                {
                    // Il faut ensuite vérifier si cet aliment existe déjà dans la liste d'aliments du plat.
                    foreach (Aliment alimVerif in listeAlimentsPlateau)
                    {
                        if (alimentVerif.IdAliment == alimVerif.IdAliment)
                        {
                            test = true;
                        }
                    }

                    // S'il n'est pas dans la composition de l'aliment, alors on l'ajoute.
                    if (!test)
                    {
                        listeAlimentsPlateau.Add(alimentVerif);
                        Creer_Bouton_Retrait(btnSender, alimentVerif);
                        alimentVerif.Quantite += 1;
                    }
                    // Sinon, il existe auquel cas il faut changer la quantité et modifier l'affichage du bouton.
                    else
                    {
                        alimentVerif.Quantite += 1;

                        foreach (Button btnPlateau in composition_plat.Children)
                        {
                            if (btnPlateau.Name == btnSender.Name)
                            {
                                btnPlateau.Content = Modifier_Quantite_Bouton_Plateau(alimentVerif);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode de retrait d'un aliment du plateau d'un plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Retirer_Aliment_Plateau(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            int idButton = int.Parse(btnSender.Name.Substring(3));

            for (int i = 0; i < listeAlimentsPlateau.Count; i++)
            {
                if (listeAlimentsPlateau[i].IdAliment == idButton)
                {
                    listeAlimentsPlateau[i].Quantite -= 1;

                    if (listeAlimentsPlateau[i].Quantite == 0)
                    {
                        composition_plat.Children.Remove(btnSender);
                        listeAlimentsPlateau.Remove(listeAlimentsPlateau[i]);
                    }
                    else
                    {
                        if (listeAlimentsPlateau[i].Quantite == 1)
                        {
                            btnSender.Content = Modifier_Bouton_Retrait_Plateau(listeAlimentsPlateau[i]);
                        }
                        else
                        {
                            btnSender.Content = Modifier_Quantite_Bouton_Plateau(listeAlimentsPlateau[i]); ;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Méthode permettant de valider le contenu des différents champs et de créer un nouveau plat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Valider_Inserer_Plat(object sender, RoutedEventArgs e)
        {
            /*-----------------------------------Validation des différents champs de saisie-----------------------------------*/

            // Initialement, il n'y a aucune erreur, alors la valeur est à false.
            Erreur = false;

            Valider_Nom_plat(lbl_nom_plat, Nom_plat);
            Valider_Si_Type(lbl_type, cbo_type);
            Valider_Composition_Plat(compo_plat, listeAlimentsPlateau);

            /* S'il y a une seule erreur suite au validations, alors la variable booléenne sera à true et conséquement,
               il faut signaler à l'utilisateur qu'il y a un problème. */
            if (Erreur)
            {
                Erreur_Champ();
            }
            // Si tout c'est bien passé, alors on insère .
            else
            {
                Inserer_Plat();

                MessageBoxResult resultat;
                resultat = MessageBox.Show(FenetreAjoutPlatMembre.SuccessMessage
                                            , FenetreAjoutPlatMembre.SuccessTitre
                                            , MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Méthode de validation du champ "Nom du plat".
        /// De plus, la longueur du nom doit être comprise entre 2 et 50 caractères.
        /// </summary>
        private void Valider_Nom_plat(Label unLabel, TextBox unTextBox)
        {
            /*-----------------------------------Vérifier que le nom n'est pas vide-----------------------------------*/
            if (string.IsNullOrEmpty(unTextBox.Text))
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = FenetreAjoutPlatMembre.NomVide;
                Erreur = true;
            }
            else
            {
                /*-----------------------------------Vérifier que le nom n'est pas trop grand-----------------------------------*/
                if (unTextBox.Text.Length > 50 || unTextBox.Text.Length < 2)
                {
                    unLabel.Foreground = Brushes.Red;
                    unLabel.Content = FenetreAjoutPlatMembre.NomTaille;
                    Erreur = true;
                }
                else
                {
                    /*-----------------------------------Vérifier que le nom n'inclus que des caractères valides (A-z, 0-9, ', %, espaces et caractères accentués)-----------------------------------*/
                    Regex RgxNom = new Regex("[^A-z0-9ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ|'| |%]");
                    bool ContientSeulement = RgxNom.IsMatch(unTextBox.Text);

                    if (ContientSeulement)
                    {
                        unLabel.Foreground = Brushes.Red;
                        unLabel.Content = FenetreAjoutPlatMembre.NomInvalide;
                        Erreur = true;
                    }
                    else
                    {
                        Regex RgxApostropheDebut = new Regex("^['| ]");
                        bool ContientApostropheDebut = RgxApostropheDebut.IsMatch(unTextBox.Text);

                        if (ContientApostropheDebut)
                        {
                            unLabel.Foreground = Brushes.Red;
                            unLabel.Content = FenetreAjoutPlatMembre.NomDebut;
                            Erreur = true;
                        }
                        else
                        {
                            /*-----------------------------------Vérifier que le nom du plat est unique par rapport à la BD-----------------------------------*/
                            /* Afin de simplifier et tenir compte d'une multitudes de possibilités, les boucles suivantes convertissent les noms de plats en lettre majuscules. */
                            bool unique = true;

                            for (int i = 0; i < listePlats.Count; i++)
                            {
                                string platBD = "";
                                string platText = "";

                                for (int j = 0; j < listePlats[i].Nom.Length; j++)
                                {
                                    if (char.IsLower(listePlats[i].Nom[j]))
                                    {
                                        platBD += char.ToUpper(listePlats[i].Nom[j]);
                                    }
                                    else
                                    {
                                        platBD += listePlats[i].Nom[j];
                                    }
                                }

                                for (int k = 0; k < unTextBox.Text.Length; k++)
                                {
                                    if (char.IsLower(unTextBox.Text[k]))
                                    {
                                        platText += char.ToUpper(unTextBox.Text[k]);
                                    }
                                    else
                                    {
                                        platText += unTextBox.Text[k];
                                    }
                                }

                                if (platBD == platText)
                                {
                                    unLabel.Foreground = Brushes.Red;
                                    unLabel.Content = FenetreAjoutPlatMembre.NomUnique;
                                    Erreur = true;
                                    unique = false;
                                }

                            }

                            if (unique)
                            {
                                unLabel.Foreground = Brushes.DarkGreen;
                                unLabel.Content = FenetreAjoutPlatMembre.NomPlat;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Méthode qui permet de valider si un groupe alimentaire est sélectionné ou non.
        /// </summary>
        private void Valider_Si_Type(Label unLabel, ComboBox unComboBox)
        {
            if (unComboBox.SelectedIndex == -1)
            {
                unLabel.Foreground = Brushes.Red;
                unLabel.Content = FenetreAjoutPlatMembre.TypeVide;
                Erreur = true;
            }
            else
            {
                unLabel.Foreground = Brushes.DarkGreen;
                unLabel.Content = FenetreAjoutPlatMembre.TypePlat;
            }
        }

        /// <summary>
        /// Méthode permettant de valider si un plat est composé d'au moins un aliment.
        /// </summary>
        /// <param name="unGroupBox"></param>
        /// <param name="uneListeAliments"></param>
        private void Valider_Composition_Plat(GroupBox unGroupBox, IList<Aliment> uneListeAliments)
        {
            string text = unGroupBox.Header.ToString();

            if (uneListeAliments.Count == 0)
            {
                unGroupBox.Foreground = Brushes.Red;
                unGroupBox.Header = FenetreAjoutPlatMembre.CompositionVide;
                Erreur = true;
            }
            else
            {
                unGroupBox.Foreground = Brushes.DarkGreen;
                unGroupBox.Header = FenetreAjoutPlatMembre.CompositionPlat;
            }
        }

        /// <summary>
        /// Méthode appelant la validation du nom à chaque changement du champ de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nom_plat_TextChanged(object sender, TextChangedEventArgs e)
        {
            Valider_Nom_plat(lbl_nom_plat, Nom_plat);
        }

        /// <summary>
        /// Méthode d'insertion dans la base de données d'un nouvel aliment.
        /// </summary>
        private void Inserer_Plat()
        {
            Plat nouveauPlat = new Plat();

            //----------------------------------------Enregistrement des valeurs nutritionnelles----------------------------------------

            // Conversion automatisée de la première lettre du nom de l'aliment.
            if (char.IsLower(Nom_plat.Text[0]))
            {
                Nom_plat.Text = char.ToUpper(Nom_plat.Text[0]) + Nom_plat.Text.Substring(1);
            }

            nouveauPlat.Nom = Nom_plat.Text;
            nouveauPlat.Createur = App.MembreCourant.NomUtilisateur;
            nouveauPlat.TypePlat = cbo_type.SelectionBoxItem.ToString();

            if (Description.Text.Length == 0 || Description.Text == null)
            {
                nouveauPlat.Description = "Aucune description.";
            }
            else
            {
                nouveauPlat.Description = Description.Text;
            }

            nouveauPlat.ListeIngredients = listeAlimentsPlateau;

            //----------------------------------------Enregistrement du chemin de l'image----------------------------------------
            string chemin = img_plat.Source.ToString();
            int position = chemin.LastIndexOf('/');
            string image = chemin.Substring(position + 1);
            string actuel;

            int positionPack = chemin.IndexOf("pack");

            if (positionPack == -1)
            {
                chemin = chemin.Substring(8);
                actuel = Directory.GetCurrentDirectory();

                actuel += cheminSauvegarde;

                bool dossierExiste = System.IO.Directory.Exists(actuel);
                if (!dossierExiste)
                    System.IO.Directory.CreateDirectory(actuel);

                actuel += image;

                if (!File.Exists(actuel))
                {
                    System.IO.File.Copy(chemin, actuel);
                }

                nouveauPlat.ImageUrl = "pack://application:,,,/UI/Images/" + image;
            }
            else
            {
                nouveauPlat.ImageUrl = chemin;
            }

            //----------------------------------------Insertion en BD----------------------------------------
            ServiceFactory.Instance.GetService<IPlatService>().Insert(nouveauPlat);
            listePlats = ServiceFactory.Instance.GetService<IPlatService>().RetrieveAll();

            //----------------------------------------Remise à neuf des champs de saisie----------------------------------------
            Nom_plat.Text = "";
            cbo_type.SelectedIndex = -1;
            Description.Text = "";
            img_plat.Source = new BitmapImage(new Uri("pack://application:,,,/UI/Images/nonDisponible.png"));
            listeAlimentsPlateau = new List<Aliment>();
            composition_plat.Children.Clear();
        }

        /// <summary>
        /// Méthode servant à générer l'aide contextuelle pour chaque aliment.
        /// </summary>
        /// <param name="unAliment"></param>
        /// <returns></returns>
        private ToolTip Produire_Aide_Contextuelle(Aliment unAliment)
        {
            ToolTip aideContextuelle = new ToolTip();
            StackPanel unStackPanel = new StackPanel();

            Label lblEntete = new Label();
            lblEntete.Content = unAliment.Nom;
            unStackPanel.Children.Add(lblEntete);

            Label lblMesure = new Label();
            lblMesure.Content = unAliment.Mesure + " " + unAliment.UniteMesure + " pour 1";
            unStackPanel.Children.Add(lblMesure);

            aideContextuelle.Content = unStackPanel;

            return aideContextuelle;
        }

        private void CultureManager_UICultureChanged(object sender, EventArgs e)
        {
            App.Current.MainWindow.Title = FenetreAjoutPlatMembre.Titre;
        }
    }
}
