using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WebIzaje.Models
{
    public class Notificaciones
    {
        public string id { get; set; }
        public string categoria { get; set; }
        public string tipo { get; set; }
        public string documento { get; set; }
        public DateTime fecha { get; set; }
    }
    public class NotificacionesGet
    {
        public List<Notificaciones> licencias()
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<Notificaciones> tdatos = new List<Notificaciones>();
            DateTime fecha = DateTime.Now.AddDays(14);
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select rut,nombre,fecha ";
                sqlcmd += "from trabajadorlicencias ";
                sqlcmd += "where fecha <= '"+ fecha.ToShortDateString() +"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Notificaciones tnotif = new Notificaciones();
                    tnotif.id = (string)dr["rut"];
                    tnotif.categoria = "Trabajador";
                    tnotif.tipo = "Licencia";
                    tnotif.documento = (string)dr["nombre"];
                    DateTime date = DateTime.Parse(dr["fecha"].ToString());
                    tnotif.fecha = new DateTime(date.Year,date.Month,date.Day);
                    tdatos.Add(tnotif);
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return tdatos;
        }

        public List<Notificaciones> certificados()
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<Notificaciones> tdatos = new List<Notificaciones>();
            DateTime fecha = DateTime.Now.AddDays(14);
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select rut,nombre,fecha ";
                sqlcmd += "from trabajadorcertificados ";
                sqlcmd += "where fecha <= '" + fecha.ToShortDateString() + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Notificaciones tnotif = new Notificaciones();
                    tnotif.id = (string)dr["rut"];
                    tnotif.categoria = "Trabajador";
                    tnotif.tipo = "Certificación";
                    tnotif.documento = (string)dr["nombre"];

                    DateTime date = DateTime.Parse(dr["fecha"].ToString());
                    tnotif.fecha = new DateTime(date.Year,date.Month,date.Day);
                    tdatos.Add(tnotif);
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return tdatos;
        }

        public List<Notificaciones> equipos()
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<Notificaciones> tdatos = new List<Notificaciones>();
            DateTime fecha = DateTime.Now.AddDays(14);
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select rut,marca,modelo,capacidad,fecha ";
                sqlcmd += "from trabajadorequipos ";
                sqlcmd += "where fecha <= '" + fecha.ToShortDateString() + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Notificaciones tnotif = new Notificaciones();
                    tnotif.id = (string)dr["rut"];
                    tnotif.categoria = "Trabajador";
                    tnotif.tipo = "Equipo";
                    tnotif.documento = (string)dr["marca"] + '/' + (string)dr["modelo"] + '/' + (string)dr["capacidad"];

                    DateTime date = DateTime.Parse(dr["fecha"].ToString());
                    tnotif.fecha = new DateTime(date.Year, date.Month, date.Day);
                    tdatos.Add(tnotif);
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return tdatos;
        }

        public List<Notificaciones> equipos_certificados()
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<Notificaciones> tdatos = new List<Notificaciones>();
            DateTime fecha = DateTime.Now.AddDays(14);
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select tag_equipo,nombre_certificado,fecha_vencimiento ";
                sqlcmd += "from certificados_equipo ";
                sqlcmd += "where fecha_vencimiento <= '" + fecha.ToShortDateString() + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Notificaciones tnotif = new Notificaciones();
                    tnotif.id = (string)dr["tag_equipo"];
                    tnotif.categoria = "Equipo";
                    tnotif.tipo = "Certificación";
                    tnotif.documento = (string)dr["nombre_certificado"];
                    if (! string.IsNullOrEmpty(dr["fecha_vencimiento"].ToString()) )
                    {
                        DateTime date = DateTime.Parse(dr["fecha_vencimiento"].ToString());
                        tnotif.fecha = new DateTime(date.Year, date.Month, date.Day);
                    }
                    else
                    {
                        tnotif.fecha = new DateTime(1900,01,01);
                    }
                    
                    tdatos.Add(tnotif);
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return tdatos;
        }
    }
}