using MySql.Data.MySqlClient;
using System;

namespace BiblioOutils
{
    public class ConnectionADO
    {
        private string server;
        private string database;
        private string username;
        private string password;

        private MySqlConnection cnx;

        public ConnectionADO(string server, string database, string username, string password)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;

            Cnx = new MySqlConnection($"SERVER={Server};DATABASE={Database};UID={Username};password={Password}");
        }

        public string Server { get => server; set => server = value; }
        public string Database { get => database; set => database = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public MySqlConnection Cnx { get => cnx; set => cnx = value; }

        public void SeConnecter()
        {
            Cnx.Open();
        }

        public void SeDeconnecter()
        {
            Cnx.Close();
        }

        public MySqlDataReader RequeteSelect(string req)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(req, Cnx);
                MySqlDataReader res = cmd.ExecuteReader();
                return res;
            }
            catch (Exception ex)
            {
                // Handle SELECT query execution error 
                throw new Exception("Error executing SELECT query.", ex);
            }
        }

        public void RequeteInsertDeleteUpdate(string req)
        {
            SeConnecter();
            try
            {
                MySqlCommand cmd = new MySqlCommand(req, Cnx);
                cmd.ExecuteNonQuery(); // Execute INSERT, UPDATE, or DELETE query
            }
            catch (Exception ex)
            {
                // Handle INSERT, UPDATE, or DELETE query execution error 
                throw new Exception("Error executing INSERT, UPDATE, or DELETE query.", ex);
            }
            finally
            {
                SeDeconnecter();
            }
        }

        public int reqGroupe(string req)
        {
            SeConnecter();
            try
            {
                MySqlCommand cmd = new MySqlCommand(req, Cnx);
                int res = Convert.ToInt32(cmd.ExecuteScalar());
                return res;
            }
            catch (Exception ex)
            {
                // Handle query execution error
                throw new Exception("Error executing query.", ex);
            }
            finally
            {
                SeDeconnecter();
            }
        }
    }
}