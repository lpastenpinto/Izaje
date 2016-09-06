using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace WebIzaje.Models
{
    public class TrabajadorDatos
    {
        public string rut { get; set; }
        public string nombre { get; set; }
        public string apellidoP { get; set; }
        public string apellidoM { get; set; }
        public string fono { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string rol { get; set; }
        public string estado { get; set; }
        public string url { get; set; }
        public List<TrabajadorLicencias> licencia { get; set; }
        public List<TrabajadorCertificados> certificado { get; set; }
        public List<TrabajadorEquipos> equipo { get; set; }
    }
    public class TrabajadorLicencias
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string fecha { get; set; }
        public string url { get; set; }
    }
    public class TrabajadorCertificados
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string fecha { get; set; }
        public string url { get; set; }
    }
    public class TrabajadorEquipos
    {
        public string id { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string capacidad { get; set; }
        public string fecha { get; set; }
        public string url { get; set; }
    }

    public class TrabajadorInsert
    {
        
        public bool datos(TrabajadorDatos user)
        {
            SqlConnection cnx = new conexion().crearConexion();
            bool exit = false;
            try
            {
                SqlCommand cmd = new SqlCommand(); 
                cmd.Connection = cnx;

                string sqlcmd = string.Empty;
                sqlcmd = "insert into ";
                sqlcmd += "TrabajadorDatos values( ";
                sqlcmd += "'" + user.rut + "',";
                sqlcmd += "'" + user.nombre + "',";
                sqlcmd += "'" + user.apellidoP + "',";
                sqlcmd += "'" + user.apellidoM + "',";
                sqlcmd += "'" + user.fono + "',";
                sqlcmd += "'" + user.email + "',";
                sqlcmd += "'" + user.direccion + "',";
                sqlcmd += "'" + user.rol+ "',";
                sqlcmd += "'" + user.estado + "',";
                sqlcmd += "'" + user.url + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                exit =  true;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            cnx.Close();
            return exit;
        }

        public bool licencias(TrabajadorLicencias licencia)
        {
            SqlConnection cnx = new conexion().crearConexion();
            bool exit = false;
            try
            {
                SqlCommand cmd = new SqlCommand(); 
                cmd.Connection = cnx;

                string sqlcmd = string.Empty;
                sqlcmd = "insert into ";
                sqlcmd += "TrabajadorLicencias values( ";
                sqlcmd += "'" + licencia.id + "',";
                sqlcmd += "'" + licencia.nombre + "',";

                if (!string.IsNullOrEmpty(licencia.fecha))
                {
                    //string[] fecha = licencia.fecha.Split('/');
                    sqlcmd +="'"+licencia.fecha+"'," ;
                }
                else
                {
                    sqlcmd += "'',";
                }
                sqlcmd += "'" + licencia.url + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                
                exit = true;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            cnx.Close();
            return exit;
        }

        public bool certificados(TrabajadorCertificados certificado)
        {
            SqlConnection cnx = new conexion().crearConexion();
            bool exit = false;
            try
            {
                SqlCommand cmd = new SqlCommand(); 
                cmd.Connection = cnx;

                string sqlcmd = string.Empty;
                sqlcmd = "Insert Into ";
                sqlcmd += "TrabajadorCertificados values( ";
                sqlcmd += "'" + certificado.id + "',";
                sqlcmd += "'" + certificado.nombre + "',";
                if (!string.IsNullOrEmpty(certificado.fecha))
                {
                    string[] fecha = certificado.fecha.Split('/');
                    sqlcmd += "'"+certificado.fecha+"',";
                }
                else
                {
                    sqlcmd += "'',";
                }
                sqlcmd += "'" + certificado.url + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                exit= true;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            cnx.Close();
            return exit;
        }

        public bool equipos(TrabajadorEquipos equipo)
        {
            SqlConnection cnx = new conexion().crearConexion();
            bool exit = false;
            try
            {
                SqlCommand cmd = new SqlCommand(); 
                cmd.Connection = cnx;

                string sqlcmd = string.Empty;
                sqlcmd = "Insert Into ";
                sqlcmd += "TrabajadorEquipos values( ";
                sqlcmd += "'" + equipo.id + "',";
                sqlcmd += "'" + equipo.marca + "',";
                sqlcmd += "'" + equipo.modelo + "',";
                sqlcmd += "'" + equipo.capacidad + "',";
                if (!string.IsNullOrEmpty(equipo.fecha))
                {
                    string[] fecha = equipo.fecha.Split('/');
                    sqlcmd +="'"+equipo.fecha+"'," ;
                }
                else
                {
                    sqlcmd += "'',";
                }
                sqlcmd += "'" + equipo.url + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                exit = true;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            cnx.Close();
            return exit;
        }
    }

    public class TrabajadorDelete
    {

        public bool datos(string id)
        {
            SqlConnection cnx = new conexion().crearConexion();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "Delete From ";
                sqlcmd += "TrabajadorDatos ";
                sqlcmd += "where (rut= '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                cnx.Close();
            }
            return false;
        }
        public bool licencias(string id)
        {
            SqlConnection cnx = new conexion().crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = null;
                sqlcmd = "Delete From ";
                sqlcmd += "TrabajadorLicencias ";
                sqlcmd += "where (rut= '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                cnx.Close();
            }
            return false;
        }

        public bool certificados(string id)
        {
            SqlConnection cnx = new conexion().crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = null;
                sqlcmd = "Delete From ";
                sqlcmd += "TrabajadorCertificados ";
                sqlcmd += "where (rut= '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                cnx.Close();
            }
            return false;
        }

        public bool equipos(string id)
        {
            SqlConnection cnx = new conexion().crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = null;
                sqlcmd = "Delete From ";
                sqlcmd += "TrabajadorEquipos ";
                sqlcmd += "where (rut= '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                cnx.Close();
            }
            return false;
        }
    }

    public class TrabajadorGet{

        public bool existRut(string num)
        {
            SqlConnection cnx = new conexion().crearConexion();
            bool exit = false;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorDatos ";
                sqlcmd += "where (rut = '" + num + "')";                

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    exit = true;
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return exit;
        }

        public List<TrabajadorDatos> AllDatos()
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<TrabajadorDatos> tdatos = new List<TrabajadorDatos>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorDatos";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TrabajadorDatos tdato = new TrabajadorDatos();
                    tdato.rut = (string)dr["rut"];
                    tdato.nombre = (string)dr["nombre"];
                    tdato.apellidoP = (string)dr["apellidoP"];
                    tdato.apellidoM = (string)dr["apellidoM"];
                    tdato.fono = (string)dr["fono"];
                    tdato.email = (string)dr["email"];
                    tdato.direccion = (string)dr["direccion"];
                    tdato.rol = (string)dr["rol"];
                    tdato.estado = (string)dr["estado"];
                    tdato.url = (string)dr["url"];
                    tdatos.Add(tdato);
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

        public TrabajadorDatos trabajador(string num)
        {
            SqlConnection cnx = new conexion().crearConexion();
            TrabajadorDatos tdato = new TrabajadorDatos();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorDatos ";
                sqlcmd += "where ( rut = '" + num +"')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    tdato.rut = (string)dr["rut"];
                    tdato.nombre = (string)dr["nombre"];
                    tdato.apellidoP = (string)dr["apellidoP"];
                    tdato.apellidoM = (string)dr["apellidoM"];
                    tdato.fono = (string)dr["fono"];
                    tdato.email = (string)dr["email"];
                    tdato.direccion = (string)dr["direccion"];
                    tdato.rol = (string)dr["rol"];
                    tdato.estado = (string)dr["estado"];
                    tdato.url = (string)dr["url"];
                }
                tdato.licencia = this.licencias(num);
                tdato.certificado = this.certificados(num);
                tdato.equipo = this.equipos(num);

                dr.Close();
            }
            catch (Exception)
            {
            }
            cnx.Close();
            return tdato;
        }

        public List<TrabajadorLicencias> licencias(string num)
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<TrabajadorLicencias> tlicencias = new List<TrabajadorLicencias>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorLicencias ";
                sqlcmd += "where ( rut = '" + num + "')";
                
                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
               
                SqlDataReader dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    TrabajadorLicencias tlicencia = new TrabajadorLicencias();
                    tlicencia.id = (string)dr["rut"];
                    tlicencia.nombre = (string)dr["nombre"];

                    DateTime fecha=new DateTime();
                    if (!dr["fecha"].GetType().Equals(typeof(System.DBNull)))
                    {

                        fecha = (DateTime)dr["fecha"];
                        string año = fecha.Year + "";
                        string mes = fecha.Month + "";
                        if (mes.Length == 1) mes = "0" + mes;
                        string dia = fecha.Day + "";
                        if (dia.Length == 1) dia = "0" + dia;
                        tlicencia.fecha = dia + "/" + mes + "/" + año;
                    }
                    else tlicencia.fecha = string.Empty;

                    tlicencia.url = (string)dr["url"];
                    tlicencias.Add(tlicencia);
                }
                
                dr.Close();
            }
            catch (Exception)
            {
            }
            cnx.Close();
            return tlicencias;
        }

        public List<TrabajadorCertificados> certificados(string num)
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<TrabajadorCertificados> tcertificados = new List<TrabajadorCertificados>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorCertificados ";
                sqlcmd += "where ( rut = '" + num + "')";
                
                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TrabajadorCertificados tcertificado = new TrabajadorCertificados();
                    tcertificado.id = (string)dr["rut"];
                    tcertificado.nombre = (string)dr["nombre"];

                    DateTime fecha = new DateTime();
                    if (!dr["fecha"].GetType().Equals(typeof(System.DBNull)))
                    {
                        fecha = (DateTime)dr["fecha"];
                        string año = fecha.Year + "";
                        string mes = fecha.Month + "";
                        if (mes.Length == 1) mes = "0" + mes;
                        string dia = fecha.Day + "";
                        if (dia.Length == 1) dia = "0" + dia;
                        tcertificado.fecha = dia + "/" + mes + "/" + año;
                    }
                    else tcertificado.fecha = string.Empty;

                    tcertificado.url = (string)dr["url"];
                    tcertificados.Add(tcertificado);
                }
                dr.Close();
            }
            catch (Exception)
            {
            }
            cnx.Close();
            return tcertificados;
        }

        public List<TrabajadorEquipos> equipos(string num)
        {
            SqlConnection cnx = new conexion().crearConexion();
            List<TrabajadorEquipos> tequipos = new List<TrabajadorEquipos>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd = "select * ";
                sqlcmd += "from TrabajadorEquipos ";
                sqlcmd += "where ( rut = '" + num + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TrabajadorEquipos tequipo = new TrabajadorEquipos();
                    tequipo.id = (string)dr["rut"];
                    tequipo.marca = (string)dr["marca"];
                    tequipo.modelo = (string)dr["modelo"];
                    tequipo.capacidad = (string)dr["capacidad"];


                    DateTime fecha = new DateTime();
                    if (!dr["fecha"].GetType().Equals(typeof(System.DBNull)))
                    {
                        fecha = (DateTime)dr["fecha"];
                        string año = fecha.Year + "";
                        string mes = fecha.Month + "";
                        if (mes.Length == 1) mes = "0" + mes;
                        string dia = fecha.Day + "";
                        if (dia.Length == 1) dia = "0" + dia;
                        tequipo.fecha = dia + "/" + mes + "/" + año;

                    }
                    else tequipo.fecha = string.Empty;

                    tequipo.url = (string)dr["url"];
                    tequipos.Add(tequipo);
                }
                dr.Close();
                
            }
            catch (Exception)
            {
            }
            cnx.Close();
            return tequipos;
        }

    }
}