using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace AppliQCM
{
    public partial class FrmQuestionnaire : Form
    {

        //**********
        // ATTRIBUTS
        //**********

        // Constantes
        private const int LARGEUR_CONTROLES = 300;
        private const int CARACTERES_PAR_LIGNE = 30;
        private const int HAUTEUR_PAR_LIGNE = 19;

        // Va permettre de définir l'emplacement :
        // 		a) Des contrôles créés dans la feuille
        // 		b) D'une nouvelle feuille en fonction du nombre et 
        // 		de la taille des contrôles qui seront créés dynamiquement
        //
        // Remarque : la structure "Point" représente une paire 
        // ordonnée de coordonnées x et y entières qui définit 
        // un point dans un plan à deux dimensions.
        private Point emplacement = new Point(10, 10);

        // Document XML associé
        private XmlDocument xr;

        // Titre de la feuille
        private string titre;


        //*************
        // CONSTRUCTEUR
        //*************
        public FrmQuestionnaire(string docXML, Form fenMere)
        {
            InitializeComponent();
            // Associer cette feuille fille à la fenêtre mère
            this.MdiParent = fenMere;

            this.ControlBox = false;

            // Remplir le questionnaire à partir du document XML
            CreerAPartirXML(docXML);
        }

        //***********
        // ACCESSEURS
        //***********

        // Retourne ou modifie la propriété "Height" de la feuille
        private int LaHauteur
        {
            get { return this.Height; }
            set { this.Height = value; }
        }

        // Retourne ou modifie la propriété "Width" de la feuille
        private int Largeur
        {
            get { return this.Width; }
            set { this.Width = value; }
        }

        // Retourne une COLLECTION des CONTROLES graphiques figurant sur la feuille
        private Control.ControlCollection TousLesControles
        {
            get { return this.Controls; }
        }

        // Retourne ou modifie la propriété privée "Titre", et dans ce dernier cas, 
        // la propriété "Text" de la feuille est renseignée.
        private string LeTitre
        {
            get { return titre; }

            set
            {
                titre = value;
                this.Text = titre;
            }
        }

        public string GetCleXML()
        {
            return this.xr.SelectSingleNode("questionnaire").Attributes["cle"].Value;
        }


        //**********
        //  METHODES
        //**********

        //---------------------------------------------------------
        // Création dynamique des contrôles sur la feuille à partir
        //  du contenu d'un document XML représentant un QCM
        //---------------------------------------------------------
        private void CreerAPartirXML(string doc)
        {
            // Accesseur
            this.LeTitre = "Questionnaire";

            // Appel de l'accesseur 'TousLesControles' pour récupérer la collection de  
            // contrôles sur la feuille
            Control.ControlCollection lesControles = this.TousLesControles;

            // Initialisation de l'emplacement
            emplacement = new Point(10, 10);

            // Creation d'un document XML qui servira à remplir la nouvelle feuille
            xr = new XmlDocument();
            xr.Load(doc);

            // Sélectionne le premier noeud (ici : <questionnaire>) et récupère la valeur
            // de son attribut "name" 
            string premierNoeud = xr.SelectSingleNode("questionnaire").Attributes["name"].Value;

            // Initialise la propriété "Titre" de la nouvelle feuille à partir de la valeur
            // de l'attribut "displayName" 
            this.LeTitre = xr.SelectSingleNode("questionnaire").Attributes["displayName"].Value;

            // Création d'une collection ordonnée de noeuds <question>
            XmlNodeList lesNoeuds;
            lesNoeuds = xr.GetElementsByTagName("question");

            // Parcours de l'ensemble des noeuds <question> présents dans la collection
            foreach (XmlNode unNoeud in lesNoeuds)
            {
                if (unNoeud.Attributes != null)
                {
                    // Détermine le type du contrôle à créer.
                    // Le type est spécifié dans l'attribut "type" : <question type= ... >
                    // Suivant le type de contrôle, une procédure "Add..." est
                    // appelée. Les paramètres sont les suivants :
                    // 		a) L'objet noeud <question> en cours
                    // 		b) La collection de contrôles de la feuille
                    // 		c) L'emplacement (coordonnées X et Y)
                    //		d) L'objet premier noeud du document XML (<questionnaire>)
                    switch (unNoeud.Attributes["type"].Value)
                    {
                        case "combo":
                            emplacement = AddComboBox(unNoeud, lesControles, emplacement, premierNoeud);
                            break;
                        case "text":
                            emplacement = AddTextBox(unNoeud, lesControles, emplacement, premierNoeud);
                            break;
                        case "liste":
                            emplacement = AddListBox(unNoeud, lesControles, emplacement, premierNoeud);
                            break;

                    }
                }
            }

            // On spécifie la largeur et la hauteur de la feuille créée dynamiquement.
            // En effet, sa dimension dépend du nombre de contrôles à placer, et par
            // conséquent du contenu du document XML.
            // Un ajustement (de 40) s'avère cependant nécessaire...
            this.Largeur = emplacement.X + LARGEUR_CONTROLES + 40;
            this.LaHauteur = emplacement.Y + 40;

            // Affichage du questionnaire
            this.Show();
        }

        //-----------------------------------------------------------------------------------------
        // Ensemble des méthodes qui, suivant le cas vont ajouter une ComboBox, une ListBox ou 
        // une TextBox à la collection passée en paramètre. 
        //
        // Retournent des coordonnées (X,Y) permettant de définir la dimension de la feuille 
        // qui va contenir ces contrôles...
        //
        // Ces méthodes sont appelées par la méthode "creerAPartirXML" qui crée d'abord
        // dynamiquement une feuille, puis l'ensemble de ses contrôles, et ceci à partir des 
        // données d'un document XML (un contrôle par noeud <question>)
        //
        // Les paramètres sont les suivants :
        // 	a) L'objet noeud <question> en cours
        // 	b) La collection de contrôles de la feuille
        //	c) L'emplacement (coordonnées X et Y) en cours (permet de placer les nouveaux contrôles)
        //	d) L'objet premier noeud du document XML (<questionnaire>)
        //------------------------------------------------------------------------------------------
        private Point AddComboBox(XmlNode unNoeud, Control.ControlCollection desControles, Point unEmplacement, string tag)
        {
            // Création d'un contrôle ComboBox.
            ComboBox maComboBox = new ComboBox();

            // Valeur de l'attribut "name" de la balise <question> en cours
            if (unNoeud.Attributes["name"] != null)
                maComboBox.Name = unNoeud.Attributes["name"].Value;

            maComboBox.Tag = tag;
            maComboBox.Width = LARGEUR_CONTROLES;

            // Il y a-t-il un nombre maximal de caractères ? 
            if (unNoeud.SelectSingleNode("maxCharacters") != null)
                maComboBox.MaxLength = int.Parse(unNoeud.SelectSingleNode("maxCharacters").InnerText);

            // Désactiver la possibilité d'écrire dans la ComboBox
            maComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Création d'une collection ordonnée de noeuds <reponse>
            XmlNodeList lesReponses;
            lesReponses = unNoeud.SelectNodes("reponses/reponse");
            foreach (XmlNode uneReponse in lesReponses)
            {
                if (uneReponse != null)
                {
                    // Ajouter chaque réponse à la ComboBox
                    maComboBox.Items.Add(uneReponse.InnerText);
                }
            }

            // Il y a-t-il une réponse par défaut ? 
            XmlNode defaultReponseNode = unNoeud.SelectSingleNode("reponses/reponse[@default='true']");
            if (defaultReponseNode != null)
            {
                // Trouver l'index de la réponse par défaut
                int defaultIndex = maComboBox.Items.IndexOf(defaultReponseNode.InnerText);
                if (defaultIndex != -1)
                {
                    // Sélectionner l'élément par défaut
                    maComboBox.SelectedIndex = defaultIndex;
                }
            }

            // Création d'un Label
            Label monLabel = new Label();
            monLabel.Name = maComboBox.Name + "Label";
            if (unNoeud.SelectSingleNode("text") != null)
                monLabel.Text = unNoeud.SelectSingleNode("text").InnerText;

            monLabel.Width = LARGEUR_CONTROLES;

            // Ajout à la collection
            monLabel.Location = unEmplacement;
            desControles.Add(monLabel);
            unEmplacement.Y += monLabel.Height;

            maComboBox.Location = unEmplacement;
            desControles.Add(maComboBox);
            unEmplacement.Y += maComboBox.Height + 10;

            return unEmplacement;
        }

        private Point AddListBox(XmlNode unNoeud, Control.ControlCollection desControles, Point unEmplacement, string tag)
        {
            // Création d'un contrôle ListBox.
            ListBox maListBox = new ListBox();

            // Valeur de l'attribut "name" de la balise <question> en cours
            if (unNoeud.Attributes["name"] != null)
                maListBox.Name = unNoeud.Attributes["name"].Value;

            maListBox.Tag = tag;
            maListBox.Width = LARGEUR_CONTROLES;
            
            // Activer la selection multiple
            maListBox.SelectionMode = SelectionMode.MultiSimple;


            // Création d'une collection ordonnée de noeuds <reponse>
            XmlNodeList lesReponses;
            lesReponses = unNoeud.SelectNodes("reponses/reponse");
            foreach (XmlNode uneReponse in lesReponses)
            {
                if (uneReponse != null)
                {
                    // Ajouter chaque réponse à la ListBox
                    string reponseText = uneReponse.InnerText;
                    maListBox.Items.Add(reponseText);

                    // Vérifier si cette réponse est sélectionnée par défaut
                    XmlAttribute defaultAttribute = uneReponse.Attributes["default"];
                    if (defaultAttribute != null && defaultAttribute.Value.ToLower() == "true")
                    {
                        // Trouver l'index de la réponse et la sélectionner
                        int selectedIndex = maListBox.Items.IndexOf(reponseText);
                        if (selectedIndex != -1)
                        {
                            maListBox.SetSelected(selectedIndex, true);
                        }
                    }
                }
            }


            // Création d'un Label
            Label monLabel = new Label();
            monLabel.Name = maListBox.Name + "Label";
            if (unNoeud.SelectSingleNode("text") != null)
                monLabel.Text = unNoeud.SelectSingleNode("text").InnerText;

            monLabel.Width = LARGEUR_CONTROLES;

            // Ajout à la collection
            monLabel.Location = unEmplacement;
            desControles.Add(monLabel);
            unEmplacement.Y += monLabel.Height;

            maListBox.Location = unEmplacement;
            desControles.Add(maListBox);
            unEmplacement.Y += maListBox.Height + 10;

            return unEmplacement;
        }



        private Point AddTextBox(XmlNode unNoeud, Control.ControlCollection desControles, Point unEmplacement, string tag)
        {
            // Création d'un contrôle TextBox.
            TextBox maTextBox = new TextBox();

            // Il y a-t-il une réponse par défaut ? 
            if (unNoeud.SelectSingleNode("defaultreponse") != null)
                maTextBox.Text = unNoeud.SelectSingleNode("defaultreponse").InnerText;

            // Valeur de l'attribut "name" de la balise <question> en cours
            if (unNoeud.Attributes["name"] != null)
                maTextBox.Name = unNoeud.Attributes["name"].Value;

            maTextBox.Tag = tag;
            maTextBox.Width = LARGEUR_CONTROLES;

            // Il y a-t-il un nombre maximal de caractères ? 
            if (unNoeud.SelectSingleNode("maxCharacters") != null)
                maTextBox.MaxLength = int.Parse(unNoeud.SelectSingleNode("maxCharacters").InnerText);

            // Calculer le nombre de lignes qui devront être affichées
            if (maTextBox.MaxLength > 0)
            {
                int numLines = (maTextBox.MaxLength / CARACTERES_PAR_LIGNE) + 1;

                // Calculer la largeur de la TextBox, et par conséquent s'il y a lieu
                // d'avoir des barres de défilement
                if (numLines == 1)
                    maTextBox.Multiline = false;
                else
                {
                    if (numLines >= 4)
                    {
                        maTextBox.Multiline = true;
                        maTextBox.Height = 4 * HAUTEUR_PAR_LIGNE;
                        maTextBox.ScrollBars = ScrollBars.Vertical;
                    }
                    else
                    {
                        maTextBox.Multiline = true;
                        maTextBox.Height = numLines * HAUTEUR_PAR_LIGNE;
                        maTextBox.ScrollBars = ScrollBars.None;
                    }
                }
            }

            // Création d'un Label
            Label monLabel = new Label();
            monLabel.Name = maTextBox.Name + "Label";
            if (unNoeud.SelectSingleNode("text") != null)
                monLabel.Text = unNoeud.SelectSingleNode("text").InnerText;

            monLabel.Width = LARGEUR_CONTROLES;

            // Ajout à la collection
            monLabel.Location = unEmplacement;
            desControles.Add(monLabel);
            unEmplacement.Y += monLabel.Height;

            maTextBox.Location = unEmplacement;
            desControles.Add(maTextBox);
            unEmplacement.Y += maTextBox.Height + 10;

            return unEmplacement;
        }
    }
}