using Final_project.Clases;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.IO;

namespace Final_project
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void vaciar_campos()
        {
            Textbox_codigo.Text = "";
            Textbox_nombre.Text = "";
            Textbox_autor.Text = "";
            Textbox_ano.Text = "";
            Textbox_ingreso.Text = "";
            Textbox_cantidad.Text = "";
            Textbox_filtro.Text = "";
            Textbox_consultaI.Text = "";
            Textbox_consultaII.Text = "";
            Textbox_añoI.Text = "";
            Textbox_añoII.Text = "";
        }

        private void Btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            string cod = Textbox_codigo.Text;
            MySqlDataReader rdr = null;

            string query = $"SELECT * FROM tb_libros WHERE codigo LIKE '%{cod}%';";
            MySqlConnection connectionDB = ClsConexion.conexion();
            connectionDB.Open();

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, connectionDB);
                rdr = mySqlCommand.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Textbox_codigo.Text = rdr.GetString(0);
                        Textbox_nombre.Text = rdr.GetString(1);
                        Textbox_autor.Text = rdr.GetString(2);
                        Textbox_ano.Text = rdr.GetString(3);
                        Textbox_ingreso.Text = rdr.GetString(4);
                        Textbox_cantidad.Text = rdr.GetString(5);
                    }
                }
            }
            catch (MySqlException error)
            {
                MessageBox.Show("No se pudo encontrar por: " + error.Message);
            }
            finally
            {
                connectionDB.Close();
            }
        }

        private void Btn_agregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string codigo = Textbox_codigo.Text;
                string nombre = Textbox_nombre.Text;
                string autor = Textbox_autor.Text;
                string ano = Textbox_ano.Text;
                string ingreso = Textbox_ingreso.Text;
                string cantidad = Textbox_cantidad.Text;

                if (codigo != "" && nombre != "" && autor != "" && ano != "" && ingreso != "" && cantidad != "")
                {
                    string query = $"INSERT INTO tb_libros(codigo, nombre, autor, año, ingreso, cantidad) VALUES ('{codigo}', '{nombre}', '{autor}', '{ano}', '{ingreso}', '{cantidad}');";

                    MySqlConnection mySqlConnection = ClsConexion.conexion();
                    mySqlConnection.Open();

                    try
                    {
                        MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                        mySqlCommand.ExecuteNonQuery();
                        MessageBox.Show("¡¡GUARDADO!! :)");
                        vaciar_campos();
                    }
                    catch (MySqlException error)
                    {
                        MessageBox.Show("NO SE PUDO GUARDAR :( POR:" + error.Message);
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("NO DEJE CAMPOS EN BLANCO");
                }
            }
            catch (FormatException error_en_datos)
            {
                MessageBox.Show("Ingrese los datos que le piden: " + error_en_datos.Message);
            }
        }

        private void Btn_actualizar_Click(object sender, RoutedEventArgs e)
        {
            string codigo = Textbox_codigo.Text;
            string nombre = Textbox_nombre.Text;
            string autor = Textbox_autor.Text;
            string ano = Textbox_ano.Text;
            string cantidad = Textbox_cantidad.Text;

            string query = $"UPDATE tb_libros SET nombre = '{nombre}', autor = '{autor}', año = '{ano}', cantidad = {cantidad} WHERE codigo = '{codigo}';";

            MySqlConnection mySqlConnection = ClsConexion.conexion();
            mySqlConnection.Open();

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                MessageBox.Show("¡¡REGISTRO ACTUALIZADO!!");
                vaciar_campos();
            }
            catch (MySqlException error)
            {
                MessageBox.Show("No se pudo actualizar:( por: " + error.Message);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void Btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            string codigo = Textbox_codigo.Text;
            string query = $"DELETE FROM tb_libros WHERE codigo = '{codigo}';";
            MessageBoxResult resul = MessageBox.Show("¿Está seguro que desea eliminar el registro seleccionado ?", "Confirmación", MessageBoxButton.YesNo);

            if (resul == MessageBoxResult.Yes)
            {
                MySqlConnection mySqlConnection = ClsConexion.conexion();
                mySqlConnection.Open();
                try
                {
                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    mySqlCommand.ExecuteNonQuery();
                    MessageBox.Show("¡¡REGISTRO ELIMINADO!!");
                    vaciar_campos();
                }
                catch (MySqlException error)
                {
                    MessageBox.Show("No se pudo eliminar por: " + error.Message);
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("REGISTRO INTACTO:)");
            }
        }

        private void Btn_guardar_csv_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM tb_libros";
            MySqlConnection cn = ClsConexion.conexion();
            cn.Open();
            try
            {
                MySqlDataAdapter adaptador = new MySqlDataAdapter(query, cn);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                List<string> lineas = new List<string>(), columnas = new List<string>();
                foreach (DataColumn colum in tabla.Columns) columnas.Add(colum.ColumnName);

                lineas.Add(string.Join(";", columnas));
                foreach (DataRow fila in tabla.Rows)
                {
                    List<string> celdas = new List<string>();
                    foreach (object celda in fila.ItemArray) celdas.Add(celda.ToString());
                    lineas.Add(string.Join(";", celdas));
                }
                string ruta = @"C:\Users\Win10\proyecto.csv";
                File.WriteAllLines(ruta, lineas);
                MessageBox.Show("¡¡TABLA GUARDADA EN ARCHIVO CSV!!");
            }
            catch (MySqlException error)
            {
                MessageBox.Show(error.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void Btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            vaciar_campos();
        }

        private void Btn_CargarTabla_Click(object sender, RoutedEventArgs e)
        {
            string fecha_1 = Textbox_consultaI.Text;
            string fecha_2 = Textbox_consultaII.Text;
            MySqlConnection mySqlConnection = ClsConexion.conexion();
            try
            {
                mySqlConnection.Open();
                string query = $"SELECT * FROM tb_libros WHERE ingreso BETWEEN '{fecha_1}' AND '{fecha_2}'";
                MySqlCommand comm = new MySqlCommand(query, mySqlConnection);
                comm.ExecuteNonQuery();

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable("tb_libros");
                dataAdapter.Fill(dt);
                DataGrid.ItemsSource = dt.DefaultView;
                dataAdapter.Update(dt);

                vaciar_campos();
                mySqlConnection.Close();
            }
            catch (MySqlException error)
            {
                MessageBox.Show("Error: " + error);
            }

        }


        private void Textbox_consultaI_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_CargarTabla_Click_1(object sender, RoutedEventArgs e)
        {
            MySqlConnection mySqlConnection = ClsConexion.conexion();
            try
            {
                mySqlConnection.Open();
                string query = $"SELECT * FROM tb_libros;";
                MySqlCommand comm = new MySqlCommand(query, mySqlConnection);
                comm.ExecuteNonQuery();

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable("tb_libros");
                dataAdapter.Fill(dt);
                DataGrid.ItemsSource = dt.DefaultView;
                dataAdapter.Update(dt);

                mySqlConnection.Close();
            }
            catch (MySqlException error)
            {
                MessageBox.Show("Error: " + error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string año_1 = Textbox_añoI.Text;
            string año_2 = Textbox_añoII.Text;
            MySqlConnection mySqlConnection = ClsConexion.conexion();
            try
            {
                mySqlConnection.Open();
                string query = $"SELECT * FROM tb_libros WHERE año BETWEEN '{año_1}' AND '{año_2}'";
                MySqlCommand comm = new MySqlCommand(query, mySqlConnection);
                comm.ExecuteNonQuery();

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable("tb_libros");
                dataAdapter.Fill(dt);
                DataGrid.ItemsSource = dt.DefaultView;
                dataAdapter.Update(dt);

                vaciar_campos();
                mySqlConnection.Close();
            }
            catch (MySqlException error)
            {
                MessageBox.Show("Error: " + error);
            }
        }

        private void Btn_filtro_Click(object sender, RoutedEventArgs e)
        {
            string filtro = Textbox_filtro.Text;
            MySqlConnection mySqlConnection = ClsConexion.conexion();
            try
            {
                mySqlConnection.Open();
                string query = $"SELECT * FROM tb_libros WHERE nombre LIKE '%{filtro}%';";
                MySqlCommand comm = new MySqlCommand(query, mySqlConnection);
                comm.ExecuteNonQuery();

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable("tb_libros");
                dataAdapter.Fill(dt);
                DataGrid.ItemsSource = dt.DefaultView;
                dataAdapter.Update(dt);

                vaciar_campos();
                mySqlConnection.Close();
            }
            catch (MySqlException error)
            {
                MessageBox.Show("Error: " + error);
            }
        }
    }
}
