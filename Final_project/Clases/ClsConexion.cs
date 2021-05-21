using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_project.Clases
{
    class ClsConexion
    {
        public static MySqlConnection conexion()
        {
            string servidor = "localhost";
            string bd = "biblioteca";
            string usuario = "root";
            string password = "progra1";

            string CadenaConexion = "Database=" + bd + "; Data Source=" + servidor + "; User Id =" + usuario + "; Password =" + password + "; port =3306";

            try
            {
                MySqlConnection conexionDB = new MySqlConnection(CadenaConexion);
                return conexionDB;
            }
            catch (MySqlException error)
            {
                Console.WriteLine("Error:" + error.Message);
                return null;
            }
        }
    }
}
