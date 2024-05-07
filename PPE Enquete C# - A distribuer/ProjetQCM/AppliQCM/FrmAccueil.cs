using AppliQCM;
using BiblioOutils;
using System;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Xml;

namespace AppliQCM
{
    public partial class FrmAccueil : Form
    {
        //---------------------------------------------
        // Propriétés : fenêtres pouvant être affichées
        //---------------------------------------------
        FrmQuestionnaire fenQuestionnaire;

        public FrmAccueil()
        {
            InitializeComponent();
        }

        private void mnuOuvrir_Click(object sender, EventArgs e)
        {
            try
            {
                // Paramétrage des propriétés de la boîte de dialogue
                openFileDialog1.FileName = "";
                openFileDialog1.InitialDirectory = "d:\\";
                openFileDialog1.Filter = "xml files (*.xml)|*.xml";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                // Ouverture et test du bouton cliqué. Si oui, récupérer le nom du fichier
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string documentXML = openFileDialog1.FileName;
                    // Création et affichage d'un objet de classe "questionnaire"
                    fenQuestionnaire = new FrmQuestionnaire(documentXML, this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Questionnaire", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuFermer_Click(object sender, EventArgs e)
        {
            // On récupère la fenêtre fille active
            Form fenFille = this.ActiveMdiChild;
            if (fenFille != null)
            {
                string content = string.Empty;

                // Parcourir les contrôles de la fenêtre fille
                foreach (Control control in fenFille.Controls)
                {
                    // Si le contrôle est une ListBox, ajouter son libellé et les éléments sélectionnés
                    if (control is ListBox)
                    {
                        ListBox listBox = control as ListBox;
                        content += listBox.Name + ":" + "\n";
                        foreach (object selectedItem in listBox.SelectedItems)
                        {
                            content += "- " + selectedItem.ToString() + "\n";
                        }
                    }
                    // Si le contrôle est une TextBox, ajouter son libellé et son texte
                    else if (control is TextBox)
                    {
                        TextBox textBox = control as TextBox;
                        content += textBox.Name + ": " + textBox.Text + "\n";
                    }
                    // Si le contrôle est une ComboBox, ajouter son libellé et l'élément sélectionné
                    else if (control is ComboBox)
                    {
                        ComboBox comboBox = control as ComboBox;
                        content += comboBox.Name + ": " + comboBox.SelectedItem.ToString() + "\n";
                    }
                }

                // Afficher le contenu
                MessageBox.Show(content, fenFille.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Fermer la fenêtre fille
                fenFille.Close();
            }
        }

        private void mnuQuitter_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuHorizontale_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void mnuVerticale_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void mnuApropos_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "QCM Dynamique C#.", "À propos...", MessageBoxButtons.OK);
        }

        private void validerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // On récupère la fenêtre fille active
                Form fenFille = this.ActiveMdiChild;
                // Vérifier si la fenêtre active est une instance de FrmQuestionnaire
                if (fenFille != null && fenFille is FrmQuestionnaire fenQuestionnaire)
                {
                    string cleQuestionnaire = fenQuestionnaire.GetCleXML();

                    ConnectionADO cnx = new ConnectionADO("localhost", "bdEnquete", "root", "");

                    // Parcourir les contrôles de la fenêtre fille
                    foreach (Control control in fenFille.Controls)
                    {
                        string reponse = string.Empty;
                        // Si le contrôle est une ListBox, ajouter son libellé et les éléments sélectionnés
                        if (control is ListBox)
                        {
                            ListBox listBox = control as ListBox;
                            foreach (string selectedItem in listBox.SelectedItems)
                            {
                                reponse = listBox.Name + ":" + selectedItem;
                                cnx.RequeteInsertDeleteUpdate($"CALL AjouterReponse('{cleQuestionnaire}', '{reponse}')");
                            }
                        }
                        // Si le contrôle est une TextBox, ajouter son libellé et son texte
                        else if (control is TextBox)
                        {
                            TextBox textBox = control as TextBox;
                            reponse = textBox.Name + ": " + textBox.Text;
                            cnx.RequeteInsertDeleteUpdate($"CALL AjouterReponse('{cleQuestionnaire}', '{reponse}')");
                        }
                        // Si le contrôle est une ComboBox, ajouter son libellé et l'élément sélectionné
                        else if (control is ComboBox)
                        {
                            ComboBox comboBox = control as ComboBox;
                            reponse = comboBox.Name + ": " + comboBox.SelectedItem.ToString();
                            cnx.RequeteInsertDeleteUpdate($"CALL AjouterReponse('{cleQuestionnaire}', '{reponse}')");
                        }
                    }
                    MessageBox.Show("Ajouts terminés.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Fermer la fenêtre fille
                    fenFille.Close();
                }
                else
                {
                    MessageBox.Show("Aucune fenêtre active ou la fenêtre active n'est pas un questionnaire.");
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}