using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;//Input/Output
using System.Runtime.Serialization.Formatters.Binary;

namespace BiblioOutils
{
    public class Serialisation
    {
        //Ecrire et lire des Objects dans un fichier
        public static Object Charger(string nomFichier)
        {
            // 1 - Test de l'existance du fichier
            if (File.Exists(nomFichier))
            {

                // 2 - Declaration et instanciation d'un flux pour l'écriture dans le fichier
                // Mode d'ouverture : Ouverture
                // Accès au flux : Accès lecture
                FileStream flux = new FileStream(nomFichier, FileMode.Open, FileAccess.Read);

                // 3 - Déclaration et instanciation de l'objet responsable pour le formatage en BINAIRE des informations
                BinaryFormatter formatter = new BinaryFormatter();

                // 4 - Récupération de l'objet serialisé
                Object obj = formatter.Deserialize(flux);

                // 5 - Fermeture du flux
                flux.Close();

                return obj;
            }
            else { return null; }
        }

        public static void Enregistrer(string nomFichier, Object objet)
        {
            // 1 - Test de l'existance du fichier
            // s'il existe: le supprimer
            if (File.Exists(nomFichier))
            {
                File.Delete(nomFichier);
            }
            // 2 - Declaration et instanciation d'un flux pour l'écriture dans le fichier
            // Mode d'ouverture : Création
            // Accès au flux : Accès écriture
            FileStream flux = new FileStream(nomFichier, FileMode.Create, FileAccess.Write);

            // 3 - Déclaration et instanciation de l'objet responsable pour le formatage en BINAIRE des informations
            BinaryFormatter formatter = new BinaryFormatter();
            
            // 4 - Sérialisation des objets de la collection
            formatter.Serialize(flux, objet);

            // 5 - Fermeture du flux
            flux.Close();
        }
    }
}
