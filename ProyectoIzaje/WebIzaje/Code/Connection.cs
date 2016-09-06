using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WebIzaje.Code
{
    public class Connection
    {
        SqlConnection connect;

        private void start()
        {//Se establece cadena de conexion, se entregan los valores_de la base de datos_y servidor

            string server = "PEZZ-PC\\SQLEXPRESS";
            string BD = "Izaje";
            bool seguridad = true;
            connect = new SqlConnection("Data Source=" + server + ";Initial Catalog=" + BD + ";Integrated Security=" + seguridad);
        }

        public void open()
        {//Abre la conexion
            connect.Open();
        }

        public void close()
        {//cierra conexion
            connect.Close();
        }

        private SqlConnection getString()
        {//devuelve la cadena de conexion
            return connect;
        }

        public SqlConnection connection()
        {//Establece conexion
            try
            {
                start();
                open();
                return getString();
            }
            catch (Exception)
            {
                close();
            }
            return null;
        }
    }
}