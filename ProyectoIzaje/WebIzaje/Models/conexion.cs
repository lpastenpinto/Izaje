
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
namespace WebIzaje.Models
{
    public class conexion
    {
        public SqlConnection crearConexion()
        {
            //string conexion sql server
            var fileName = Path.GetFileName("conexion.txt");
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Document/"), fileName);

            StreamReader streamReader = new StreamReader(path);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            string sqldb = text;
            SqlConnection conn = new SqlConnection(sqldb);
           
            conn.Open();
            return conn;
        }
        //Para control de usuarios y grupos
        public bool revisarUsuarioPassword(string usuario, string password)
        {
            SqlConnection conexion = new conexion().crearConexion();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT password from usuarios where identificador='" + usuario + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            string pass = "";

            while (dr.Read())
            {
                pass = (string)dr["password"];
            }

            conexion.Close();
            if (pass == password) return true;
            return false;
        }
        public string obtenerRol(string usuario)
        {
            SqlConnection conexion = new conexion().crearConexion();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT rol from usuarios where identificador='" + usuario + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            string rol = "";

            while (dr.Read())
            {
                rol = (string)dr["rol"];
            }

            conexion.Close();
            return rol;
        }
        public string obtenerAreaJefeArea(string usuario)
        {
            SqlConnection conexion = new conexion().crearConexion();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "SELECT area from jefe_area where nombre='" + usuario + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            string jefe_area = "";

            while (dr.Read())
            {
                jefe_area = (string)dr["area"];
            }

            conexion.Close();
            return jefe_area;
        }
        //Para Solicitud:
        bool verificarSiExisteSolicitud(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from solicitudes where id='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            conexion.Close();

            return retorno;
        }
        bool verificarSiExistenDatosDeFinSolicitud(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from datos_fin_solicitud where id_solicitud='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            conexion.Close();

            return retorno;
        }
        public Solicitud obtenerSolicitudPorID(string id)
        {
            Solicitud sol = new Solicitud();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from solicitudes where id='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read()) return sol;
            sol.idSolicitud = (string)dr["id"];

            if (dr["planificada_por"].GetType() != typeof(DBNull)) sol.planificadaPor = (string)dr["planificada_por"];
            if (dr["autorizada_por"].GetType() != typeof(DBNull)) sol.autorizadaPor = (string)dr["autorizada_por"];
            if (dr["finalizada_por"].GetType() != typeof(DBNull)) sol.finalizadaPor = (string)dr["finalizada_por"];
            if (dr["confirmada_por"].GetType() != typeof(DBNull)) sol.confirmadaPor = (string)dr["confirmada_por"];

            sol.fechaCreacion = (System.DateTime)dr["fecha_creacion"];

            sol.estado = (string)dr["estado"];
            sol.nombre = (string)dr["solicitante"];
            sol.empresa = (string)dr["empresa_trabajador"];
            sol.correo = (string)dr["mail_solicitante"];
            if (!(dr["telefono"]).GetType().Equals(typeof(System.DBNull)))
            {
                sol.telefono = (string)dr["telefono"];
            }
            else 
            {
                sol.telefono = "";
            }
            sol.celular = (string)dr["celular"];

            sol.gerencia = (string)dr["gerencia"];
            sol.superintendencia = (string)dr["superintendencia"];
            sol.area = (string)dr["area"];
            sol.centroCosto = (string)dr["centro_costo"];
            sol.descripcionLugar = (string)dr["descripcion_lugar"];

            sol.inicio = (System.DateTime)dr["fecha_inicio"];
            sol.fin = (System.DateTime)dr["fecha_termino"];

            int tiempoEstimado = (int)dr["tiempo_estimado_operacion"];
            sol.tiempoEstimadoOperacion = tiempoEstimado;

            sol.criticidad = (string)dr["criticidad"];

            int peso = (int)dr["peso"];
            sol.peso = peso;

            int largo = (int)dr["largo"];
            sol.largo = largo;

            int ancho = (int)dr["ancho"];
            sol.ancho = ancho;

            int alto = (int)dr["alto"];
            sol.alto = alto;

            sol.turno = (string)dr["turno"];
            if (dr["ruta_imagen"].GetType().Equals(typeof(DBNull)))
            {
                sol.rutaImagen = "";
            }
            else
            {
                sol.rutaImagen = (string)dr["ruta_imagen"];
            }
            sol.descripcionCarga = (string)dr["descripcion_carga"];
            sol.descripcionServicio = (string)dr["descripcion_servicio"];

            if ((string)dr["solicitado_anteriormente"] == "true") sol.solicitadoAnteriormente = true;
            else sol.solicitadoAnteriormente = false;

            if ((string)dr["requiere_maniobra"] == "true") sol.requiereManiobra = true;
            else sol.requiereManiobra = false;

            if ((string)dr["requiere_rigger"] == "true") sol.requiereRigger = true;
            else sol.requiereRigger = false;

            if ((string)dr["cuenta_con_maniobra"] == "true") sol.cuentaConManiobra = true;
            else sol.cuentaConManiobra = false;

            if (!sol.estado.Equals("NUEVA"))
            {
                sol.criticidadCorregida = (string)dr["criticidad_corregida"];

                int tiempoEstimadoCorregido = (int)dr["tiempo_estimado_corregido"];
                sol.tiempoEstimadoOperacionCorregido =tiempoEstimadoCorregido;

                int pesoCorregido = (int)dr["peso_corregido"];
                sol.pesoCorregido = pesoCorregido;

                sol.turnoCorregido = (string)dr["turno_corregido"];

                int altoCorregido = (int)dr["alto_corregido"];
                sol.altoCorregido = altoCorregido;

                int anchoCorregido = (int)dr["ancho_corregido"];
                sol.anchoCorregido = anchoCorregido;

                int largoCorregido = (int)dr["largo_corregido"];
                sol.largoCorregido = largoCorregido;

                sol.descripcionCargaCorregida = (string)dr["descripcion_carga_corregida"];
                sol.descripcionServicioCorregida = (string)dr["descripcion_servicio_corregida"];
                sol.inicioCorregido = (System.DateTime)dr["fecha_inicio_corregida"];
                sol.finCorregido = (System.DateTime)dr["fecha_termino_corregida"];

                sol.idEquipo1 = (string)dr["id_equipo_1"];
                sol.idOperador1 = (string)dr["id_operador_1"];
                sol.idRigger1 = (string)dr["id_rigger_1"];
                sol.idEquipo2 = (string)dr["id_equipo_2"];
                sol.idOperador2 = (string)dr["id_operador_2"];
                sol.idRigger2 = (string)dr["id_rigger_2"];
            }

            if (sol.estado.Equals("FINALIZADA") || sol.estado.Equals("CONFIRMADA"))
            {
                sol.calculado = obtenerCalculadoSolicitud(sol.idSolicitud);
                sol.estadoPagoCalculado = obtenerEstadoPagoCalculadoSolicitud(sol.idSolicitud);

                sol.horaRelojInicial1 = new List<string>();
                sol.horaRelojFinal1 = new List<string>();
                sol.horaHorometroInicial1 = new List<string>();
                sol.horaHorometroFinal1 = new List<string>();
                sol.horaRelojInicial2 = new List<string>();
                sol.horaRelojFinal2 = new List<string>();
                sol.horaHorometroInicial2 = new List<string>();
                sol.horaHorometroFinal2 = new List<string>();

                sol.fechas = obtenerFechas(sol.idSolicitud, sol.idEquipo1);
                for (int i = 0; i < sol.fechas.Count; i++)
                {
                    sol.horaRelojInicial1.Add(obtenerRelojInicial(sol.fechas[i], sol.idSolicitud, sol.idEquipo1));
                    sol.horaRelojFinal1.Add(obtenerRelojFinal(sol.fechas[i], sol.idSolicitud, sol.idEquipo1));
                    sol.horaHorometroInicial1.Add(obtenerHorometroInicial(sol.fechas[i], sol.idSolicitud, sol.idEquipo1));
                    sol.horaHorometroFinal1.Add(obtenerHorometroFinal(sol.fechas[i], sol.idSolicitud, sol.idEquipo1));
                    if (!(sol.idEquipo2==null) && !sol.idEquipo2.Equals("--")) 
                    {
                        sol.horaRelojInicial2.Add(obtenerRelojInicial(sol.fechas[i], sol.idSolicitud, sol.idEquipo2));
                        sol.horaRelojFinal2.Add(obtenerRelojFinal(sol.fechas[i], sol.idSolicitud, sol.idEquipo2));
                        sol.horaHorometroInicial2.Add(obtenerHorometroInicial(sol.fechas[i], sol.idSolicitud, sol.idEquipo2));
                        sol.horaHorometroFinal2.Add(obtenerHorometroFinal(sol.fechas[i], sol.idSolicitud, sol.idEquipo2));
                    }
                }
            }//*/

            conexion.Close();

            return sol;
        }
        List<string> obtenerFechas(string id, string idEquipo)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select fecha from datos_fin_solicitud where id_solicitud='" + id
                + "' AND tag_equipo='" + idEquipo + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                DateTime fecha = (DateTime)dr["fecha"];
                string año = fecha.Year + "";
                string mes = fecha.Month + "";
                if (mes.Length == 1) mes = "0" + mes;
                string dia = fecha.Day + "";
                if (dia.Length == 1) dia = "0" + dia;
                retorno.Add(dia + "-" + mes + "-" + año);
            }

            conexion.Close();
            return retorno;
        }
        bool obtenerCalculadoSolicitud(string id) 
        {
            bool retorno=false;
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select calculado from datos_fin_solicitud where id_solicitud='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                retorno = bool.Parse((string)dr["calculado"]);
            }
            conexion.Close();
            return retorno;
        }
        string obtenerEstadoPagoCalculadoSolicitud(string id) 
        {
            string retorno="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_estado_pago from detalle_estado_pago where id_solicitud='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                retorno = (string)dr["id_estado_pago"];
            }
            conexion.Close();
            return retorno;
        }
        string obtenerRelojInicial(string fecha, string id, string idEquipo)
        {
            string retorno = "";

            string dia = fecha.Split('-')[0];
            string mes = fecha.Split('-')[1];
            string año = fecha.Split('-')[2];
            int diaInt = int.Parse(dia);
            int mesInt = int.Parse(mes);
            int añoInt = int.Parse(año);

            string fechaString = año + mes + dia;

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select hora_reloj_inicio from datos_fin_solicitud where id_solicitud='"
                + id + "' AND tag_equipo='" + idEquipo + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                retorno = (string)dr["hora_reloj_inicio"];
            }
            conexion.Close();
            return retorno;
        }
        string obtenerRelojFinal(string fecha, string id, string idEquipo)
        {
            string retorno = "";

            string dia = fecha.Split('-')[0];
            string mes = fecha.Split('-')[1];
            string año = fecha.Split('-')[2];
            int diaInt = int.Parse(dia);
            int mesInt = int.Parse(mes);
            int añoInt = int.Parse(año);

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select hora_reloj_fin from datos_fin_solicitud where id_solicitud='"
                + id + "' AND tag_equipo='" + idEquipo + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                retorno=(string)dr["hora_reloj_fin"];
            }
            conexion.Close();
            return retorno;
        }
        string obtenerHorometroInicial(string fecha, string id, string idEquipo)
        {
            string retorno = "";
            string dia = fecha.Split('-')[0];
            string mes = fecha.Split('-')[1];
            string año = fecha.Split('-')[2];
            int diaInt = int.Parse(dia);
            int mesInt = int.Parse(mes);
            int añoInt = int.Parse(año);

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select hora_horometro_inicio from datos_fin_solicitud where id_solicitud='"
                + id + "' AND tag_equipo='" + idEquipo + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                int valor = (int)dr["hora_horometro_inicio"];
                retorno=valor.ToString();
            }
            conexion.Close();
            return retorno;
        }
        string obtenerHorometroFinal(string fecha, string id, string idEquipo)
        {
            string retorno = "";

            string dia = fecha.Split('-')[0];
            string mes = fecha.Split('-')[1];
            string año = fecha.Split('-')[2];
            int diaInt = int.Parse(dia);
            int mesInt = int.Parse(mes);
            int añoInt = int.Parse(año);

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select hora_horometro_fin from datos_fin_solicitud where id_solicitud='"
                + id + "' AND tag_equipo='" + idEquipo + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) +"'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                int valor = (int)dr["hora_horometro_fin"];
                retorno=valor.ToString();
            }
            conexion.Close();
            return retorno;
        }
        public void guardarSolicitudEnBD(Solicitud sol, string jefeDeArea)
        {
            SqlConnection conexion = crearConexion();

            if (verificarSiExisteSolicitud(sol.idSolicitud))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;

                string añoInicio = sol.inicio.Year + "";
                string mesInicio = sol.inicio.Month + "";
                if (mesInicio.Length == 1) mesInicio = "0" + mesInicio;
                string diaInicio = sol.inicio.Day + "";
                if (diaInicio.Length == 1) diaInicio = "0" + diaInicio;
                string horaInicio = sol.inicio.Hour + "";
                if (horaInicio.Length == 1) horaInicio = "0" + horaInicio;
                string minutoInicio = sol.inicio.Minute + "";
                if (minutoInicio.Length == 1) minutoInicio = "0" + minutoInicio;

                string añoFin = sol.fin.Year + "";
                string mesFin = sol.fin.Month + "";
                if (mesFin.Length == 1) mesFin = "0" + mesFin;
                string diaFin = sol.fin.Day + "";
                if (diaFin.Length == 1) diaFin = "0" + diaFin;
                string horaFin = sol.fin.Hour + "";
                if (horaFin.Length == 1) horaFin = "0" + horaFin;
                string minutoFin = sol.fin.Minute + "";
                if (minutoFin.Length == 1) minutoFin = "0" + minutoFin;

                string solicitadoAnteriormente;
                if (sol.solicitadoAnteriormente) solicitadoAnteriormente = "true";
                else solicitadoAnteriormente = "false";

                string requiereRigger;
                if (sol.requiereRigger) requiereRigger = "true";
                else requiereRigger = "false";

                string requiereManiobra;
                if (sol.requiereManiobra) requiereManiobra = "true";
                else requiereManiobra = "false";

                string cuentaConManiobra;
                if (sol.cuentaConManiobra) cuentaConManiobra = "true";
                else cuentaConManiobra = "false";

                string añoInicioCorregido = sol.inicioCorregido.Year + "";
                string mesInicioCorregido = sol.inicioCorregido.Month + "";
                if (mesInicioCorregido.Length == 1) mesInicioCorregido = "0" + mesInicioCorregido;
                string diaInicioCorregido = sol.inicioCorregido.Day + "";
                if (diaInicioCorregido.Length == 1) diaInicioCorregido = "0" + diaInicioCorregido;
                string horaInicioCorregido = sol.inicioCorregido.Hour + "";
                if (horaInicioCorregido.Length == 1) horaInicioCorregido = "0" + horaInicioCorregido;
                string minutoInicioCorregido = sol.inicioCorregido.Minute + "";
                if (minutoInicioCorregido.Length == 1) minutoInicioCorregido = "0" + minutoInicioCorregido;

                string añoFinCorregido = sol.finCorregido.Year + "";
                string mesFinCorregido = sol.finCorregido.Month + "";
                if (mesFinCorregido.Length == 1) mesFinCorregido = "0" + mesFinCorregido;
                string diaFinCorregido = sol.finCorregido.Day + "";
                if (diaFinCorregido.Length == 1) diaFinCorregido = "0" + diaFinCorregido;
                string horaFinCorregido = sol.finCorregido.Hour + "";
                if (horaFinCorregido.Length == 1) horaFinCorregido = "0" + horaFinCorregido;
                string minutoFinCorregido = sol.finCorregido.Minute + "";
                if (minutoFinCorregido.Length == 1) minutoFinCorregido = "0" + minutoFinCorregido;

                cmd.CommandText = "update solicitudes set empresa_trabajador='" + sol.empresa
                    + "', solicitante='" + sol.nombre
                    + "', mail_solicitante='" + sol.correo
                    + "', area='" + sol.area
                    + "', centro_costo='" + sol.centroCosto
                    + "', descripcion_lugar='" + sol.descripcionLugar
                    + "', fecha_inicio='"+sol.inicio
                    + "', fecha_termino='"+sol.fin
                    + "', tiempo_estimado_operacion='" + sol.tiempoEstimadoOperacion
                    + "', telefono='" + sol.telefono
                    + "', celular='" + sol.celular
                    + "', gerencia='" + sol.gerencia
                    + "', superintendencia='" + sol.superintendencia
                    + "', criticidad='" + sol.criticidad
                    + "', peso='" + sol.peso
                    + "', largo='" + sol.largo
                    + "', ancho='" + sol.ancho
                    + "', alto='" + sol.alto
                    + "', turno='" + sol.turno
                    + "', ruta_imagen='" + sol.rutaImagen
                    + "', descripcion_carga='" + sol.descripcionCarga
                    + "', descripcion_servicio='" + sol.descripcionServicio
                    + "', solicitado_anteriormente='" + solicitadoAnteriormente
                    + "', requiere_rigger='" + requiereRigger
                    + "', requiere_maniobra='" + requiereManiobra
                    + "', cuenta_con_maniobra='" + cuentaConManiobra
                    + "', estado='" + sol.estado
                    + "', criticidad_corregida='" + sol.criticidadCorregida
                    + "', tiempo_estimado_corregido='" + sol.tiempoEstimadoOperacionCorregido
                    + "', peso_corregido='" + sol.pesoCorregido
                    + "', turno_corregido='" + sol.turnoCorregido
                    + "', alto_corregido='" + sol.altoCorregido
                    + "', ancho_corregido='" + sol.anchoCorregido
                    + "', largo_corregido='" + sol.largoCorregido
                    + "', descripcion_carga_corregida='" + sol.descripcionCargaCorregida
                    + "', descripcion_servicio_corregida='" + sol.descripcionServicioCorregida
                    + "', fecha_inicio_corregida='" + sol.inicio
                    + "', fecha_termino_corregida='" + sol.fin
                    + "', id_equipo_1='" + sol.idEquipo1
                    + "', id_operador_1='" + sol.idOperador1
                    + "', id_rigger_1='" + sol.idRigger1
                    + "', id_equipo_2='" + sol.idEquipo2
                    + "', id_operador_2='" + sol.idOperador2
                    + "', id_rigger_2='" + sol.idRigger2
                    + "', planificada_por='" + sol.planificadaPor
                    + "', autorizada_por='" + sol.autorizadaPor
                    + "', finalizada_por='" + sol.finalizadaPor
                    + "', confirmada_por='" + sol.confirmadaPor
                    + "'  where id='" + sol.idSolicitud + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                //Se guardan los datos de fin de solicitud en la tabla datos_fin_solicitud si el estado es finalizada o confirmada
                if (sol.estado.Equals("FINALIZADA") || sol.estado.Equals("CONFIRMADA"))
                {
                    guardarDatosFinSolicitud(sol, jefeDeArea);
                }

            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;

                string añoInicio = sol.inicio.Year + "";
                string mesInicio = sol.inicio.Month + "";
                if (mesInicio.Length == 1) mesInicio = "0" + mesInicio;
                string diaInicio = sol.inicio.Day + "";
                if (diaInicio.Length == 1) diaInicio = "0" + diaInicio;
                string horaInicio = sol.inicio.Hour + "";
                if (horaInicio.Length == 1) horaInicio = "0" + horaInicio;
                string minutoInicio = sol.inicio.Minute + "";
                if (minutoInicio.Length == 1) minutoInicio = "0" + minutoInicio;

                string añoFin = sol.fin.Year + "";
                string mesFin = sol.fin.Month + "";
                if (mesFin.Length == 1) mesFin = "0" + mesFin;
                string diaFin = sol.fin.Day + "";
                if (diaFin.Length == 1) diaFin = "0" + diaFin;
                string horaFin = sol.fin.Hour + "";
                if (horaFin.Length == 1) horaFin = "0" + horaFin;
                string minutoFin = sol.fin.Minute + "";
                if (minutoFin.Length == 1) minutoFin = "0" + minutoFin;

                string solicitadoAnteriormente;
                if (sol.solicitadoAnteriormente) solicitadoAnteriormente = "true";
                else solicitadoAnteriormente = "false";

                string requiereRigger;
                if (sol.requiereRigger) requiereRigger = "true";
                else requiereRigger = "false";

                string requiereManiobra;
                if (sol.requiereManiobra) requiereManiobra = "true";
                else requiereManiobra = "false";

                string cuentaConManiobra;
                if (sol.cuentaConManiobra) cuentaConManiobra = "true";
                else cuentaConManiobra = "false";

                //(id, empresa_trabajador, solicitante, mail_solicitante, area, centro_costo, descripcion_lugar, fecha_inicio)
                cmd.CommandText = "insert into solicitudes(id, empresa_trabajador, solicitante, mail_solicitante, area, centro_costo, descripcion_lugar, fecha_inicio, fecha_termino, tiempo_estimado_operacion, " +
                    "telefono, celular, gerencia, superintendencia, criticidad, peso, largo, ancho, alto, turno, ruta_imagen, descripcion_carga, descripcion_servicio, " +
                    "solicitado_anteriormente, requiere_rigger, requiere_maniobra, cuenta_con_maniobra, estado, fecha_creacion) values('"
                    + sol.idSolicitud + "','"
                    + sol.empresa + "','"
                    + sol.nombre + "','"
                    + sol.correo + "','"
                    + sol.area + "','"
                    + sol.centroCosto + "','"
                    + sol.descripcionLugar+"','"
                    + sol.inicio+ "','"
                    + sol.fin+"','"
                     + sol.tiempoEstimadoOperacion + "','"
                     + sol.telefono + "','"
                     + sol.celular + "','"
                     + sol.gerencia + "','"
                     + sol.superintendencia + "','"
                     + sol.criticidad + "','"
                     + sol.peso + "','"
                     + sol.largo + "','"
                     + sol.ancho + "','"
                     + sol.alto + "','"
                     + sol.turno + "','"
                     + sol.rutaImagen + "','"
                     + sol.descripcionCarga + "','"
                     + sol.descripcionServicio + "','" +
                     solicitadoAnteriormente + "','"
                     + requiereRigger + "','"
                     + requiereManiobra + "','"
                     + cuentaConManiobra + "','"
                     + sol.estado + "','" 
                     + sol.fechaCreacion + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            conexion.Close();
        }
        void guardarDatosFinSolicitud(Solicitud sol, string jefeDeArea)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            if (verificarSiExistenDatosDeFinSolicitud(sol.idSolicitud))
            {
                for (int i = 0; i < sol.fechas.Count; i++)
                {
                    string dia = sol.fechas[i].Split('-')[0];
                    string mes = sol.fechas[i].Split('-')[1];
                    string año = sol.fechas[i].Split('-')[2];
                    int diaInt = int.Parse(dia);
                    int mesInt = int.Parse(mes);
                    int añoInt = int.Parse(año);

                    cmd.CommandText = "update datos_fin_solicitud"
                    + " set hora_reloj_inicio='" + sol.horaRelojInicial1[i]
                    + "', hora_reloj_fin='" + sol.horaRelojFinal1[i]
                    + "', hora_horometro_inicio='" + sol.horaHorometroInicial1[i]
                    + "', hora_horometro_fin='" + sol.horaHorometroFinal1[i]
                    + "'  where id_solicitud='" + sol.idSolicitud
                    + "' AND tag_equipo='" + sol.idEquipo1
                    + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    if (!sol.idEquipo2.Equals("--"))
                    {
                        cmd.CommandText = "update datos_fin_solicitud"
                            + " set hora_reloj_inicio='" + sol.horaRelojInicial2[i]
                            + "', hora_reloj_fin='" + sol.horaRelojFinal2[i]
                            + "', hora_horometro_inicio='" + sol.horaHorometroInicial2[i]
                            + "', hora_horometro_fin='" + sol.horaHorometroFinal2[i]
                            + "'  where id_solicitud='" + sol.idSolicitud
                            + "' AND tag_equipo='" + sol.idEquipo2
                            + "' AND fecha='" + new DateTime(añoInt, mesInt, diaInt) + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                string jefeArea = jefeDeArea;
                string tipoEquipo1 = obtenerTipoEquipo(sol.idEquipo1);
                string tipoEquipo2 = obtenerTipoEquipo(sol.idEquipo2);
                string empresaPropietaria1 = sol.empresa;
                string empresaPropietaria2 = sol.empresa;

                for (int i = 0; i < sol.fechas.Count; i++)
                {
                    string dia = sol.fechas[i].Split('-')[0];
                    string mes = sol.fechas[i].Split('-')[1];
                    string año = sol.fechas[i].Split('-')[2];

                    int diaInt = int.Parse(dia);
                    int mesInt = int.Parse(mes);
                    int añoInt = int.Parse(año);

                    //Equipo 1
                    cmd.CommandText = "insert into datos_fin_solicitud values('"
                        + sol.idSolicitud + "', '"
                        + sol.idEquipo1 + "', '"
                        + new DateTime(añoInt, mesInt, diaInt) +"','"
                        + sol.horaRelojInicial1[i] + "','"
                        + sol.horaRelojFinal1[i] + "','"
                        + sol.horaHorometroInicial1[i] + "','"
                        + sol.horaHorometroFinal1[i] + "','"
                        + sol.idOperador1 + "','"
                        + sol.area + "','"
                        + jefeArea + "','"
                        + sol.centroCosto + "','"
                        + tipoEquipo1 + "','"
                        + empresaPropietaria1 + "','"
                        + "false" + "')";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    if (!sol.idEquipo2.Equals("--"))
                    {
                        //Equipo 2
                        cmd.CommandText = "insert into datos_fin_solicitud values('"
                            + sol.idSolicitud + "', '"
                            + sol.idEquipo2 + "', '"
                            + sol.fechas[i] + "','"
                            + sol.horaRelojInicial2[i] + "','"
                            + sol.horaRelojFinal2[i] + "','"
                            + sol.horaHorometroInicial2[i] + "','"
                            + sol.horaHorometroFinal2[i] + "','"
                            + sol.idOperador2 + "','"
                            + sol.area + "','"
                            + jefeArea + "','"
                            + sol.centroCosto + "','"
                            + tipoEquipo2 + "','"
                            + empresaPropietaria2 + "','"
                            + "false" + "')";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            conexion.Close();
        }        
        string obtenerTipoEquipo(string equipo)
        {
            string retorno = "";

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tipo_equipo from equipos where tag='" + equipo + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                retorno = (string)dr["tipo_equipo"];
            }

            conexion.Close();

            return retorno;
        }
        string obtenerEmpresaPropietaria(string equipo)
        {
            string retorno = "";

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select empresa_propietaria from equipos where tag='" + equipo + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                retorno = (string)dr["empresa_propietaria"];
            }

            conexion.Close();

            return retorno;
        }        
        public List<Solicitud> obtenerSolicitudesNuevas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where estado='NUEVA'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            //sol.idSolicitud = (string)dr["id"];


            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerSolicitudesPlanificadas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where estado='PLANIFICADA'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerSolicitudesAutorizadas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where estado='AUTORIZADA'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerSolicitudesFinalizadas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where estado='FINALIZADA'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerSolicitudesConfirmadas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where estado='CONFIRMADA'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerTodas()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerTodasResumida()
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id, fecha_inicio, fecha_termino, estado, planificada_por, autorizada_por, finalizada_por, confirmada_por from solicitudes";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Solicitud temp = new Solicitud();
                temp.idSolicitud = (string)dr["id"];
                temp.inicio = (DateTime)dr["fecha_inicio"];
                temp.fin = (DateTime)dr["fecha_termino"];
                temp.estado = (string)dr["estado"];
                if (!dr["planificada_por"].GetType().Equals(typeof(System.DBNull)))
                {
                    temp.planificadaPor = (string)dr["planificada_por"];
                }
                else
                {
                    temp.planificadaPor = "";
                }
                if (!dr["autorizada_por"].GetType().Equals(typeof(System.DBNull)))
                {
                    temp.autorizadaPor = (string)dr["autorizada_por"];
                }
                else
                {
                    temp.autorizadaPor = "";
                }
                if (!dr["finalizada_por"].GetType().Equals(typeof(System.DBNull)))
                {
                    temp.finalizadaPor = (string)dr["finalizada_por"];
                }
                else
                {
                    temp.finalizadaPor = "";
                }
                if (!dr["confirmada_por"].GetType().Equals(typeof(System.DBNull)))
                {
                    temp.confirmadaPor = (string)dr["confirmada_por"];
                }
                else
                {
                    temp.confirmadaPor = "";
                }

                sol.Add(temp);
            }

            conexion.Close();

            return sol;
        }
        public List<Solicitud> obtenerTodas(string equipo, string trabajador, DateTime fecha_in, DateTime fecha_out)
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            string cadena = "select id from solicitudes where ( fecha_inicio_corregida>=@inicio and fecha_inicio_corregida<=@fin ";
            cadena += "and (id_equipo_1 Like '%" + equipo + "%' or id_equipo_2 like '%" + equipo + "%') ";
            cadena += "and ((id_operador_1 like '%" + trabajador + "%' or id_operador_2 like '%" + trabajador + "%') ";
            cadena += "or (id_rigger_1 like '%" + trabajador + "%' or id_rigger_2 like '%" + trabajador + "%')))";

            cmd.Parameters.Add("@inicio", SqlDbType.DateTime).Value = fecha_in;
            cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = fecha_out.AddDays(1);

            cmd.CommandText = cadena;
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }

        public List<Solicitud> obtenerTodas(DateTime fecha)
        {
            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            string cadena = "select id from solicitudes where (YEAR(fecha_inicio_corregida) = '" + fecha.Year + "' AND MONTH(fecha_inicio_corregida) = '" + fecha.Month + "')";
            cmd.CommandText = cadena;
            cmd.CommandType = CommandType.Text;


            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string id = (string)dr["id"];
                sol.Add(this.obtenerSolicitudPorID(id));
            }

            conexion.Close();

            return sol;
        }

        public void eliminarSolicitud(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "delete from solicitudes where id='" + id + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conexion.Close();

            conexion = crearConexion();
            cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "delete from datos_fin_solicitud where [id_solicitud]='" + id + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conexion.Close();
        }

        //Para Gerencia, Superintendencia, Área y Centro Costo:
        public List<string> obtenerGerencias()
        {
            List<string> sol = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from gerencia";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add((string)dr["nombre"]);
            }

            conexion.Close();

            return sol;
        }
        public List<string> obtenerSuperintendencia(string gerencia)
        {
            List<string> sol = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from superintendencia where gerencia='" + gerencia + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add((string)dr["nombre"]);
            }

            conexion.Close();

            return sol;
        }
        public List<string> obtenerArea(string superintendencia)
        {
            List<string> sol = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from area where superintendencia='" + superintendencia + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add((string)dr["nombre"]);
            }

            conexion.Close();

            return sol;
        }
        public List<string> obtenerCentroCosto(string area)
        {
            List<string> sol = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select codigo from centro_costo where area='" + area + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add((string)dr["codigo"]);
            }

            conexion.Close();

            return sol;
        }

        //Para la criticidad
        public List<string> obtenerCriticidad()
        {
            List<string> sol = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from tipo_criticidad";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add((string)dr["nombre"]);
            }

            conexion.Close();

            return sol;
        }

        //Para estado pago

        #region Estado de Pago


        //modificadas
        public List<DetalleEstadoPago> obtenerdetalleestadopago(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from detalle_estado_pago where id_estado_pago_tipo='" + id + "' order by fecha asc";
            cmd.CommandType = CommandType.Text;
            List<DetalleEstadoPago> lista_detalle = new List<DetalleEstadoPago>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DetalleEstadoPago detalle = new DetalleEstadoPago();

                detalle.idEstadoPago = (string)dr["id_estado_pago"];
                detalle.idEstadoPagoTipo = (string)dr["id_estado_pago_tipo"];
                detalle.idSolicitud = (string)dr["id_solicitud"];
                detalle.fecha = (DateTime)dr["fecha"];
                detalle.operador = (string)dr["operador"];
                detalle.tagequipo = (string)dr["tag_equipo"];
                // detalle.tipoequipo = (string)dr["tipo_equipo"];
                detalle.area = (string)dr["area"];
                detalle.responsable = (string)dr["responsable"];
                detalle.empresa = (string)dr["empresa"];
                detalle.centro_costo = (string)dr["centro_costo"];
                detalle.i_horometro = (string)dr["inicio_horometro"];
                detalle.f_horometro = (string)dr["fin_horometro"];
                detalle.d_horometro = (string)dr["delta_horometro"];
                detalle.salto_horometro = (string)dr["salto_horometro"];
                detalle.i_reloj = (string)dr["inicio_reloj"];
                detalle.f_reloj = (string)dr["fin_reloj"];
                detalle.d_reloj = (string)dr["delta_reloj"];
                detalle.contador_deltareloj = (string)dr["contador_delta_reloj"];
                detalle.valor_dia = (string)dr["valor_dia"];
                detalle.valor_mlp = (string)dr["valor_mlp"];
                detalle.valor_distribucion = (string)dr["valor_distribucion"];
                lista_detalle.Add(detalle);

            }
            conexion.Close();
            return lista_detalle;
        }
        public void guardardetalleestadopago(DetalleEstadoPago lista_detalles)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "insert into detalle_estado_pago(id_estado_pago,id_estado_pago_tipo, id_solicitud, fecha, operador, tag_equipo,"
                + "tipo_equipo,area,responsable,empresa,centro_costo,inicio_horometro,fin_horometro"
                + ",delta_horometro,salto_horometro,inicio_reloj,fin_reloj,delta_reloj,contador_delta_reloj,valor_dia,valor_mlp,valor_distribucion) values('"
               + lista_detalles.idEstadoPago
               + "', '" + lista_detalles.idEstadoPagoTipo
               + "', '" + lista_detalles.idSolicitud
               + "', '" + lista_detalles.fecha
               + "','" + lista_detalles.operador
               + "','" + lista_detalles.tagequipo
               + "','" + lista_detalles.tipoequipo
               + "','" + lista_detalles.area
               + "','" + lista_detalles.responsable
               + "','" + lista_detalles.empresa
               + "','" + lista_detalles.centro_costo
               + "','" + lista_detalles.i_horometro
               + "','" + lista_detalles.f_horometro
               + "','" + lista_detalles.d_horometro
               + "','" + lista_detalles.salto_horometro
               + "','" + lista_detalles.i_reloj
               + "','" + lista_detalles.f_reloj
               + "','" + lista_detalles.d_reloj
               + "','" + lista_detalles.contador_deltareloj
               + "','" + lista_detalles.valor_dia
               + "','" + lista_detalles.valor_mlp
               + "','" + lista_detalles.valor_distribucion + "')";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conexion.Close();
        }
        public void guardarresumenestadopago(ResumenEstadoPago resumenestadopago)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "insert into resumenestadopago(id_estado_pago,id_estado_pago_tipo, fecha_generado, tipo_equipo, uf, total_valor_dia,"
                + " total_valor_mlp,costo_hora,total_valor_distribucion,total_delta_horometro,total_salto_horometro,total_delta_reloj,total_contador) values('"
               + resumenestadopago.id_estadopago + "', '"
               + resumenestadopago.id_estadopagotipo + "', '"
               + resumenestadopago.fecha_generado + "','"
               + resumenestadopago.tipo_equipo + "','"
               + resumenestadopago.uf + "','"
               + resumenestadopago.total_valor_dia + "','"
               + resumenestadopago.total_valor_mlp + "','"
               + resumenestadopago.costo_hora + "','"
               + resumenestadopago.total_valor_distribucion + "','"
               + resumenestadopago.total_delta_horometro + "','"
               + resumenestadopago.total_saltos_horometro + "','"
               + resumenestadopago.total_delta_reloj + "','"
               + resumenestadopago.total_contador + "')";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conexion.Close();
        }
        public List<Servicios> obtener_Servicios_alafecha(string fecha_final, string tipo_equipo)
        {

            string año = fecha_final.Split('/')[2];
            string mes = fecha_final.Split('/')[1];
            string dia = fecha_final.Split('/')[0];
            int añoInt = int.Parse(año);
            int mesInt = int.Parse(mes);
            int diaInt = int.Parse(dia);

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from datos_fin_solicitud where fecha<='"
                + new DateTime(añoInt, mesInt, diaInt) + "' AND calculado='false' AND subtipo_equipo='" + tipo_equipo + "' order by fecha asc";
            cmd.CommandType = CommandType.Text;
            List<Servicios> lista_servicios = new List<Servicios>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Servicios servicio = new Servicios();
                servicio.idSolicitud = (string)dr["id_solicitud"];
                servicio.tag_equipo = (string)dr["tag_equipo"];
                servicio.fecha = (DateTime)dr["fecha"];
                servicio.hora_inicio_reloj = (string)dr["hora_reloj_inicio"];
                servicio.hora_fin_reloj = (string)dr["hora_reloj_fin"];

                int horaHorometroInicio = (int)dr["hora_horometro_inicio"];
                servicio.hora_inicio_horometro = horaHorometroInicio.ToString();

                int horaHorometroFin = (int)dr["hora_horometro_fin"];
                servicio.hora_fin_horometro = horaHorometroFin.ToString();

                servicio.rut_operador = (string)dr["identificador_operador"];
                servicio.nombre_area = (string)dr["area"];
                servicio.jefe_area = (string)dr["jefe_area"];
                servicio.codigo_centro_costo = (string)dr["codigo_centro_costos"];
                servicio.tipo_equipo = (string)dr["subtipo_equipo"];
                servicio.empresa_propietaria = (string)dr["nombre_empresa_propietaria"];
                lista_servicios.Add(servicio);
            }
            conexion.Close();
            return lista_servicios;
        }
        public List<Servicios> obtener_Servicios_entrefecha(string fecha_inicio, string fecha_final)
        {

            string añoFinal = fecha_final.Split('/')[2];
            string mesFinal = fecha_final.Split('/')[1];
            string diaFinal = fecha_final.Split('/')[0];
            int añoIntFinal = int.Parse(añoFinal);
            int mesIntFinal = int.Parse(mesFinal);
            int diaIntFinal = int.Parse(diaFinal);

            string añoInicial = fecha_inicio.Split('/')[2];
            string mesInicial = fecha_inicio.Split('/')[1];
            string diaInicial = fecha_inicio.Split('/')[0];
            int añoIntInicial = int.Parse(añoInicial);
            int mesIntInicial = int.Parse(mesInicial);
            int diaIntInicial = int.Parse(diaInicial);

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from datos_fin_solicitud where fecha Between '"
                + new DateTime(añoIntInicial, mesIntInicial, diaIntInicial) + "' and '"
                + new DateTime(añoIntFinal, mesIntFinal, diaIntFinal)
                + "' AND calculado='false' order by fecha asc";
            cmd.CommandType = CommandType.Text;
            List<Servicios> lista_servicios = new List<Servicios>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Servicios servicio = new Servicios();
                if (!dr.Read()) return lista_servicios;
                servicio.idSolicitud = (string)dr["id_solicitud"];
                servicio.tag_equipo = (string)dr["tag_equipo"];
                servicio.fecha = (DateTime)dr["fecha"];
                servicio.hora_inicio_reloj = (string)dr["hora_reloj_inicio"];
                servicio.hora_fin_reloj = (string)dr["hora_reloj_fin"];

                int horaHorometroInicio = (int)dr["hora_horometro_inicio"];
                servicio.hora_inicio_horometro = horaHorometroInicio.ToString();

                int horaHorometroFin = (int)dr["hora_horometro_fin"];
                servicio.hora_fin_horometro = horaHorometroFin.ToString();

                servicio.rut_operador = (string)dr["identificador_operador"];
                servicio.nombre_area = (string)dr["area"];
                servicio.jefe_area = (string)dr["jefe_area"];
                servicio.codigo_centro_costo = (string)dr["codigo_centro_costos"];
                servicio.tipo_equipo = (string)dr["subtipo_equipo"];
                servicio.empresa_propietaria = (string)dr["nombre_empresa_propietaria"];
                lista_servicios.Add(servicio);
            }
            conexion.Close();
            return lista_servicios;
        }
        public List<ResumenEstadoPago> obtener_resumenes()
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from resumenestadopago";
            cmd.CommandType = CommandType.Text;
            List<ResumenEstadoPago> lista_resumen = new List<ResumenEstadoPago>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ResumenEstadoPago resumen = new ResumenEstadoPago();

                resumen.id_estadopago = (string)dr["id_estado_pago"];
                resumen.id_estadopagotipo = (string)dr["id_estado_pago_tipo"];
                resumen.fecha_generado = (string)dr["fecha_generado"];
                resumen.tipo_equipo = (string)dr["tipo_equipo"];
                resumen.uf = (string)dr["uf"];
                resumen.total_valor_dia = (string)dr["total_valor_dia"];
                resumen.total_valor_mlp = (string)dr["total_valor_mlp"];
                resumen.costo_hora = (string)dr["costo_hora"];
                resumen.total_valor_distribucion = (string)dr["total_valor_distribucion"];

                lista_resumen.Add(resumen);
            }
            conexion.Close();
            return lista_resumen;
        }
        public ResumenEstadoPago obtener_resumenes_id(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from resumenestadopago where id_estado_pago_tipo='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();
            ResumenEstadoPago resumen = new ResumenEstadoPago();
            if (!dr.Read()) return resumen;
            resumen.id_estadopago = (string)dr["id_estado_pago"];
            resumen.id_estadopagotipo = (string)dr["id_estado_pago_tipo"];
            resumen.fecha_generado = (string)dr["fecha_generado"];
            resumen.tipo_equipo = (string)dr["tipo_equipo"];
            resumen.uf = (string)dr["uf"];
            resumen.total_valor_dia = (string)dr["total_valor_dia"];
            resumen.total_valor_mlp = (string)dr["total_valor_mlp"];
            resumen.costo_hora = (string)dr["costo_hora"];
            resumen.total_valor_distribucion = (string)dr["total_valor_distribucion"];
            resumen.total_delta_horometro = (string)dr["total_delta_horometro"];
            resumen.total_saltos_horometro = (string)dr["total_salto_horometro"];
            resumen.total_delta_reloj = (string)dr["total_delta_reloj"];
            resumen.total_contador = (string)dr["total_contador"];

            conexion.Close();
            return resumen;
        }

        public string[] obtener_id_estado_pago_general()
        {
            string texto_id = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct id_estado_pago from resumenestadopago";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_id += (string)dr["id_estado_pago"] + ",";
            }
            texto_id = texto_id.TrimEnd(',');
            string[] id = texto_id.Split(',');
            conexion.Close();
            return id;
        }

        public string obtener_id_tipo_por_id_general(string id_general)
        {
            string texto_id = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select  id_estado_pago_tipo from resumenestadopago where id_estado_pago='" + id_general + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_id += (string)dr["id_estado_pago_tipo"] + ",";
            }
            texto_id = texto_id.TrimEnd(',');

            conexion.Close();
            return texto_id;
        }
        #endregion

        public List<TiposEquipos> obtener_tipo_equipo()
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from tipo_equipo";
            cmd.CommandType = CommandType.Text;
            List<TiposEquipos> lista_tipos = new List<TiposEquipos>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                TiposEquipos equipos = new TiposEquipos();

                equipos.subt_tipo = (string)dr["tipo"];
                equipos.familia = (string)dr["familia"];
                int minimoGarantizado = (int)dr["minimo_garantizado"];
                equipos.minimo_garantizado = minimoGarantizado;

                double costo_hora = (double)dr["costo_hora"];
                equipos.costo_hora_equipo = costo_hora;

                double costo_hora_extra = (double)dr["costo_hora_extra"];
                equipos.hora_extra = costo_hora_extra;

                lista_tipos.Add(equipos);
            }
            conexion.Close();

            return lista_tipos;
        }        
        public bool comprobar_cautivo(string tag_equipo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from equipos_cautivo where tag_equipo='"+tag_equipo+"'";
            cmd.CommandType = CommandType.Text;
            List<TiposEquipos> lista_tipos = new List<TiposEquipos>();
            SqlDataReader dr = cmd.ExecuteReader();
            

            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }
        public void cambiarCalculadoDatosFin(string id) {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = crearConexion();

            cmd.CommandText = "UPDATE datos_fin_solicitud set calculado='true' where id_solicitud='"+id+"'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        //Para Gerencia
        public List<gerencia> obtenerListaGerencias()
        {
            List<gerencia> retorno = new List<gerencia>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion from gerencia";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                gerencia gerencia = new gerencia();
                gerencia.nombre = (string)dr["nombre"];
                gerencia.descripcion = (string)dr["descripcion"];
                retorno.Add(gerencia);
            }

            conexion.Close();

            return retorno;
        }
        public gerencia obtenerListaGerencias(string id)
        {
            gerencia item = new gerencia();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion from gerencia where nombre='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                item.nombre = (string)dr["nombre"];
                item.descripcion = (string)dr["descripcion"];
            }
            conexion.Close();
            return item;
        }
        public bool agregarGerencia(string nombre, string descripcion)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();
                cmd.CommandText = "insert into gerencia values('" + nombre + "','" + descripcion + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarGerencia(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from gerencia where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool updateGerencia(string nombre, string descripcion)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update gerencia set descripcion='" + descripcion + "' where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool verificarGerencia(string gerencia)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select gerencia from superintendencia where gerencia='" + gerencia + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            cmd.Connection.Close();

            return retorno;
        }

        //Para Superintendencia        
        public bool agregarSuperintendencia(string nombre, string descripcion, string gerencia)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();
                cmd.CommandText = "insert into superintendencia values('" + nombre + "','" + descripcion + "', '" + gerencia + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarSuperintendencia(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from superintendencia where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;

        }
        public bool updateSuperintendencia(string nombre, string descripcion, string gerencia)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update superintendencia set descripcion='" + descripcion + "', gerencia ='" + gerencia + "'where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public List<superintendencia> obtenerListaSuperintendencias()
        {
            List<superintendencia> retorno = new List<superintendencia>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion, gerencia from superintendencia";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                superintendencia superintendencia = new superintendencia();
                superintendencia.nombre = (string)dr["nombre"];
                superintendencia.descripcion = (string)dr["descripcion"];
                superintendencia.gerencia = (string)dr["gerencia"];
                retorno.Add(superintendencia);
            }

            conexion.Close();

            return retorno;
        }
        public superintendencia obtenerListaSuperintendencias(string id)
        {
            superintendencia item = new superintendencia();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion, gerencia from superintendencia where nombre='" + id + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                item.nombre = (string)dr["nombre"];
                item.descripcion = (string)dr["descripcion"];
                item.gerencia = (string)dr["gerencia"];
            }
            conexion.Close();
            return item;
        }
        public bool verificarSuperintendencia(string superintendencia)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select superintendencia from area where superintendencia='" + superintendencia + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }

        //Para Areas
        public bool agregarArea(string nombre, string descripcion, string superintendencia)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();
                cmd.CommandText = "insert into area values('" + nombre + "','" + descripcion + "', '" + superintendencia + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarArea(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from area where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool updateAreas(string nombre, string descripcion, string superintendencia)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update area set descripcion='" + descripcion + "',superintendencia='" + superintendencia + "' where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public List<area> obtenerListaAreas()
        {
            List<area> retorno = new List<area>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion, superintendencia from area";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                area area = new area();
                area.nombre = (string)dr["nombre"];
                area.descripcion = (string)dr["descripcion"];
                area.superintendencia = (string)dr["superintendencia"];
                retorno.Add(area);
            }

            conexion.Close();

            return retorno;
        }
        public area obtenerListaAreas(string id)
        {
            area item = new area();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion, superintendencia from area where nombre='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                item.nombre = (string)dr["nombre"];
                item.descripcion = (string)dr["descripcion"];
                item.superintendencia = (string)dr["superintendencia"];
            }
            conexion.Close();
            return item;
        }
        public bool verificarArea(string area)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select area from centro_costo where area='" + area + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }

        /*
        //Para Areas
        public void agregarArea(string nombre, string descripcion, string superintendencia)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = crearConexion();

            cmd.CommandText = "insert into area values('" + nombre + "','" + descripcion + "', '" + superintendencia + "')";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        public void eliminarArea(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "delete from area where nombre='" + nombre + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        public List<area> obtenerListaAreas()
        {
            List<area> retorno = new List<area>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion, superintendencia from area";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                area area = new area();
                area.nombre = (string)dr["nombre"];
                area.descripcion = (string)dr["descripcion"];
                area.superintendencia = (string)dr["superintendencia"];
                retorno.Add(area);
            }

            conexion.Close();

            return retorno;
        }
        public void eliminarCentroCosto(string codigo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "delete from centro_costo where codigo='" + codigo + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        public bool verificarArea(string area)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select area from centro_costo where area='" + area + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }
        //*/

        //Para centro costo
        public List<centro_costo> obtenerListaCentrosCosto()
        {
            List<centro_costo> retorno = new List<centro_costo>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, codigo, area from centro_costo";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                centro_costo cCosto = new centro_costo();
                cCosto.nombre = (string)dr["nombre"];
                cCosto.codigo = (string)dr["codigo"];
                cCosto.area = (string)dr["area"];
                retorno.Add(cCosto);
            }

            conexion.Close();

            return retorno;
        }
        public centro_costo obtenerListaCentrosCosto(string id)
        {
            centro_costo item = new centro_costo();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, codigo, area from centro_costo where codigo='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                item.nombre = (string)dr["nombre"];
                item.codigo = (string)dr["codigo"];
                item.area = (string)dr["area"];
            }
            conexion.Close();
            return item;
        }
        public bool agregarCentroCosto(string nombre, string codigo, string area)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();

                cmd.CommandText = "insert into centro_costo values('" + codigo + "','" + nombre + "', '" + area + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarCentroCosto(string codigo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from centro_costo where codigo='" + codigo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool updateCentrocosto(string codigo, string nombre, string area)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update centro_costo set nombre='" + nombre + "', area='" + area + "' where codigo='" + codigo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        
        //Para la criticidad
        public List<criticidad> obtenerListaCriticidad()
        {
            List<criticidad> retorno = new List<criticidad>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion from tipo_criticidad";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                criticidad criticidad = new criticidad();
                criticidad.nombre = (string)dr["nombre"];
                criticidad.descripcion = (string)dr["descripcion"];
                retorno.Add(criticidad);
            }

            conexion.Close();

            return retorno;
        }
        public criticidad obtenerListaCriticidad(string id)
        {
            criticidad item = new criticidad();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre, descripcion from tipo_criticidad where nombre='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                item.nombre = (string)dr["nombre"];
                item.descripcion = (string)dr["descripcion"];
            }

            conexion.Close();
            return item;
        }
        public bool agregarCriticidad(string nombre, string descripcion)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();
                cmd.CommandText = "insert into tipo_criticidad values('" + nombre + "','" + descripcion + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarCriticidad(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from tipo_criticidad where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool updateCriticidad(string nombre, string descripcion)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update tipo_criticidad set descripcion='" + descripcion + "' where nombre='" + nombre + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }

        //Para Tipo Equipo
        public List<tipoEquipo> obtenerListaTipoEquipo()
        {
            List<tipoEquipo> retorno = new List<tipoEquipo>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tipo, familia, minimo_garantizado, costo_hora, costo_hora_extra from tipo_equipo";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                tipoEquipo tEquipo = new tipoEquipo();
                tEquipo.tipo = (string)dr["tipo"];
                tEquipo.familia = (string)dr["familia"];

                tEquipo.minimoGarantizado = (int)dr["minimo_garantizado"];
                tEquipo.costo_hora_normal = (double)dr["costo_hora"];
                tEquipo.costo_hora_extra = (double)dr["costo_hora_extra"];
                retorno.Add(tEquipo);
            }

            conexion.Close();

            return retorno;
        }
        public tipoEquipo obtenerListaTipoEquipo(string id)
        {
            tipoEquipo item = new tipoEquipo();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tipo, familia, minimo_garantizado, costo_hora, costo_hora_extra from tipo_equipo where tipo ='" + id + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                item.tipo = (string)dr["tipo"];
                item.familia = (string)dr["familia"];

                item.minimoGarantizado = (int)dr["minimo_garantizado"];
                item.costo_hora_normal = (double)dr["costo_hora"];
                item.costo_hora_extra = (double)dr["costo_hora_extra"];
            }

            conexion.Close();
            return item;
        }
        public bool agregarTipoEquipo(string tipo, string familia, string minimoGarantizado, double costo_hora_normal, double costo_hora_extra)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = crearConexion();

                cmd.CommandText = "insert into tipo_equipo values('" + tipo + "','" + familia + "', '" + minimoGarantizado
                    + "',@costo_hora_normal,@costo_hora_extra)";

                cmd.Parameters.Add("@costo_hora_normal", SqlDbType.Float).Value = costo_hora_normal;
                cmd.Parameters.Add("@costo_hora_extra", SqlDbType.Float).Value = costo_hora_extra;

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool eliminarTipoEquipo(string tipo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "delete from tipo_equipo where tipo='" + tipo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool updateTipoEquipo(string tipo, string familia, int minimo, double coston, double costoe)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;
                cmd.CommandText = "update tipo_equipo set familia='" + familia + "',minimo_garantizado=" + minimo
                    + ",costo_hora=@costoHoraNormal,costo_hora_extra=@costoHoraExtra where tipo='" + tipo + "'";
                cmd.Parameters.Add("@costoHoraNormal", SqlDbType.Float).Value = coston;
                cmd.Parameters.Add("@costoHoraExtra", SqlDbType.Float).Value = costoe;
                
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return true;
            }
            catch (Exception)
            {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return false;
        }
        public bool verificarTipoEquipo(string tipo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tipo from modelo_equipo where tipo='" + tipo + "'";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            conexion.Close();
            return retorno;
        }

        

        // usuarios
        public List<Usuarios> obtenerListaUsuarios()
        {
            List<Usuarios> retorno = new List<Usuarios>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from usuarios";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Usuarios user = new Usuarios();
                user.identificador = (string)dr["identificador"];
                user.nombres = (string)dr["nombres"];
                user.apellido_paterno = (string)dr["apellido_paterno"];
                user.apellido_materno = (string)dr["apellido_materno"];
                user.email = (string)dr["mail"];
                user.rol = (string)dr["rol"];
                user.password = (string)dr["password"];
                retorno.Add(user);
            }

            conexion.Close();

            return retorno;
        }
        public bool deleteUsuario(string id)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conexion;

                cmd.CommandText = "delete from usuarios where identificador='" + id + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }
        }
        public bool ExisteUsuario(string nombre)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from usuarios where identificador='" + nombre + "'";
            cmd.CommandType = CommandType.Text;
            List<TiposEquipos> lista_tipos = new List<TiposEquipos>();
            SqlDataReader dr = cmd.ExecuteReader();


            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }
        public bool saveUsuario(Usuarios user)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            try
            {
                cmd.CommandText = "insert into usuarios(identificador,nombres,apellido_paterno,apellido_materno,mail,rol,password) values('"
                   + user.identificador + "', '"
                   + user.nombres + "', '"
                   + user.apellido_paterno + "', '"
                   + user.apellido_materno + "', '"
                   + user.email + "', '"
                   + user.rol + "','"
                   + user.password + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }


        }
        public Usuarios getUsuario(string nombre)
        {
            Usuarios user = new Usuarios();
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from usuarios where identificador='" + nombre + "'";
            cmd.CommandType = CommandType.Text;
            List<EquipoCertificados> lista_certificados = new List<EquipoCertificados>();
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return user;
            user.identificador = (string)dr["identificador"];
            user.nombres = (string)dr["nombres"];
            user.apellido_paterno = (string)dr["apellido_paterno"];
            user.apellido_materno = (string)dr["apellido_materno"];
            user.email = (string)dr["mail"];
            user.rol = (string)dr["rol"];
            user.password = (string)dr["password"];
            conexion.Close();
            return user;
        }
        public bool updateUsuario(Usuarios datos)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            try
            {
                cmd.CommandText = "update usuarios set nombres='" + datos.nombres
                        + "', apellido_paterno='" + datos.apellido_paterno
                        + "', apellido_materno='" + datos.apellido_materno
                        + "', mail='" + datos.email
                        + "', rol='" + datos.rol
                        + "', password='" + datos.password
                        + "'  where identificador='" + datos.identificador + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;

            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }
        }

        //Para estado pago

        //Para obtener equipos disponibles entre fechas, de un tipo específico
        public List<string> obtenerEquiposPorTipo(string tipo)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select tag from equipos where tipo_equipo='" + tipo + "' AND estado='Disponible'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                retorno.Add((string)dr["tag"]);
            }
            conexion.Close();

            return retorno;
        }
        public List<string> obtenerEquiposNoDisponiblesSegunFechas(string diaInicio, string mesInicio, string añoInicio,
            string horaInicio, string minutoInicio, string diaFin, string mesFin, string añoFin, string horaFin, string minutoFin, string idSol)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select id_equipo_1, id_equipo_2 from solicitudes where fecha_inicio_corregida>'" + añoInicio + mesInicio
                + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_inicio_corregida<'"
                + añoFin + mesFin + diaFin + " " + horaFin + ":" + minutoFin + "' AND estado<>'NUEVA' AND id<>'"+idSol+"' OR fecha_inicio_corregida<'"
                + añoInicio + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_termino_corregida>'"
                + añoInicio + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND estado<>'NUEVA' AND id<>'" + idSol + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                retorno.Add((string)dr["id_equipo_1"]);
                if (!((string)dr["id_equipo_2"]).Equals("--") && ((string)dr["id_equipo_2"]).Equals(""))
                {
                    retorno.Add((string)dr["id_equipo_2"]);
                }
            }
            conexion.Close();

            return retorno;
        }

        public List<string> obtenerOperadoresPorTag(string tagEquipo)
        {
            List<string> retorno = new List<string>();
            string modelo = "", marca = "";

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select marca, modelo from equipos where tag='" + tagEquipo + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                modelo = (string)dr["modelo"];
                marca = (string)dr["marca"];
            }

            cmd = new SqlCommand();
            SqlConnection conexion2 = crearConexion();
            cmd.Connection = conexion2;

            cmd.CommandText = "select rut from trabajadorequipos where marca='" + marca + "' AND modelo='" + modelo + "'";
            cmd.CommandType = CommandType.Text;
            
            SqlDataReader dr2 = cmd.ExecuteReader();

            while (dr2.Read())
            {
                retorno.Add(dr2["rut"].ToString());
            }

            conexion.Close();
            conexion2.Close();
            return retorno;
        }

        public List<string> obtenerOperadoresNoDisponiblesSegunFechas(string diaInicio, string mesInicio, string añoInicio,
            string horaInicio, string minutoInicio, string diaFin, string mesFin, string añoFin, string horaFin, 
            string minutoFin, string turno, string idSol)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select id_operador_1, id_operador_2 from solicitudes where fecha_inicio_corregida>'" + añoInicio + mesInicio
                + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_inicio_corregida<'"
                + añoFin + mesFin + diaFin + " " + horaFin + ":" + minutoFin + "' AND estado<>'NUEVA' AND id<>'"+idSol+"' OR fecha_inicio_corregida<'"
                + añoInicio + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_termino_corregida>'"
                + añoInicio + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND estado<>'NUEVA' AND id<>'" + idSol + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                retorno.Add((string)dr["id_operador_1"]);
                if (!((string)dr["id_operador_2"]).Equals("--") && ((string)dr["id_operador_2"]).Equals(""))
                {
                    retorno.Add((string)dr["id_operador_2"]);
                }
            }

            cmd = new SqlCommand();
            SqlConnection conexion2 = crearConexion();
            cmd.Connection = conexion2;

            cmd.CommandText = "select rut from trabajadordatos where estado<>'" + turno + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                retorno.Add((string)dr2["rut"]);
            }

            conexion.Close();

            return retorno;
        }

        public List<string> obtenerRiggersPorTag(string tagEquipo)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select rut from trabajadordatos where rol='rigger'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                retorno.Add((string)dr["rut"]);
            }

            conexion.Close();
            return retorno;
        }
        public List<string> obtenerRiggersNoDisponiblesSegunFechas(string diaInicio, string mesInicio, string añoInicio,
            string horaInicio, string minutoInicio, string diaFin, string mesFin, string añoFin, string horaFin, string minutoFin, string turno, string idSol)
        {
            List<string> retorno = new List<string>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;

            cmd.CommandText = "select id_rigger_1, id_rigger_2 from solicitudes where fecha_inicio_corregida>='" + añoInicio 
                + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_inicio_corregida<='"
                + añoFin + mesFin + diaFin + " " + horaFin + ":" + minutoFin + "' AND estado<>'NUEVA' AND id<>'" + idSol + "' OR fecha_inicio_corregida<='"
                + añoInicio + mesInicio + diaInicio + " " + horaInicio + ":" + minutoInicio + "' AND fecha_termino_corregida>='"
                + añoFin + mesFin + diaFin + " " + horaFin + ":" + minutoFin + "' AND estado<>'NUEVA' AND id<>'" + idSol + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                retorno.Add((string)dr["id_rigger_1"]);
                if (!((string)dr["id_rigger_2"]).Equals("--") && ((string)dr["id_rigger_2"]).Equals(""))
                {
                    retorno.Add((string)dr["id_rigger_2"]);
                }
            }

            cmd = new SqlCommand();
            SqlConnection conexion2 = crearConexion();

            cmd.Connection = conexion2;

            cmd.CommandText = "select rut from trabajadordatos where estado<>'" + turno + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                retorno.Add((string)dr2["rut"]);
            }

            conexion.Close();
            return retorno;
        }

        #region Datos Rigger
        //modificados
        public List<RiggerTotales> calculo_totales_rigger(string id_estado_pago_tipo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct nombre_rigger,rut_rigger from trabajados_rigger where id_estado_pago_tipo='" + id_estado_pago_tipo + "'";
            cmd.CommandType = CommandType.Text;
            List<RiggerTotales> lista_totales = new List<RiggerTotales>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if ((string)dr["nombre_rigger"] != "")
                {
                    RiggerTotales rigger = new RiggerTotales();
                    rigger.nombre_rigger = (string)dr["nombre_rigger"];
                    rigger.rut_rigger = (string)dr["rut_rigger"];
                    List<string> id_solicitudes = obtener_idsolicitudes_por_rigger_estadopago(rigger.nombre_rigger, id_estado_pago_tipo);
                    rigger.total_dias_trabajados = total_dias_trabajados_rigger_idestadopago(id_solicitudes);
                    lista_totales.Add(rigger);
                }
            }

            conexion.Close();
            return lista_totales;
        }
        //nuevo metodo
        public List<RiggerTotales> calcular_totales_rigger_id_general(string id_general)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct nombre_rigger,rut_rigger from trabajados_rigger where id_estado_pago ='" + id_general + "'";
            cmd.CommandType = CommandType.Text;
            List<RiggerTotales> lista_totales = new List<RiggerTotales>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if ((string)dr["nombre_rigger"] != "")
                {
                    RiggerTotales rigger = new RiggerTotales();
                    rigger.nombre_rigger = (string)dr["nombre_rigger"];
                    rigger.rut_rigger = (string)dr["rut_rigger"];
                    List<string> id_solicitudes = obtener_idsolicitudes_por_rigger_id_general(rigger.rut_rigger, id_general);
                    rigger.total_dias_trabajados = total_dias_trabajados_rigger_idsolicitudes(id_solicitudes);
                    lista_totales.Add(rigger);
                }
            }

            conexion.Close();
            return lista_totales;
        }
        public List<RiggerTotales> calcular_totales_rigger_ala_fecha(DateTime fechatope)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct nombre_rigger,rut_rigger from trabajados_rigger where fecha_termino <='" + fechatope + "'";
            cmd.CommandType = CommandType.Text;
            List<RiggerTotales> lista_totales = new List<RiggerTotales>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if ((string)dr["nombre_rigger"] != "")
                {
                    RiggerTotales rigger = new RiggerTotales();
                    rigger.nombre_rigger = (string)dr["nombre_rigger"];
                    rigger.rut_rigger = (string)dr["rut_rigger"];
                    List<string> id_solicitudes = obtener_idsolicitudes_por_rigger_rut(rigger.rut_rigger, fechatope);
                    rigger.total_dias_trabajados = total_dias_trabajados_rigger_idsolicitudes(id_solicitudes);
                    lista_totales.Add(rigger);
                }
            }

            conexion.Close();
            return lista_totales;
        }

        public List<string> obtener_idsolicitudes_por_rigger_id_general(string rut, string id_general)
        {

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_solicitud from trabajados_rigger where (rut_rigger='" + rut + "' and id_estado_pago='" + id_general + "')";
            cmd.CommandType = CommandType.Text;
            List<string> id_solicitudes = new List<string>();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id_solicitudes.Add((string)dr["id_solicitud"]);
            }


            conexion.Close();
            return id_solicitudes;
        }
        public List<string> obtener_idsolicitudes_por_rigger_rut(string rut, DateTime fechatope)
        {

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_solicitud from trabajados_rigger where (rut_rigger='" + rut + "' and fecha_termino<='" + fechatope + "')";
            cmd.CommandType = CommandType.Text;
            List<string> id_solicitudes = new List<string>();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id_solicitudes.Add((string)dr["id_solicitud"]);
            }


            conexion.Close();
            return id_solicitudes;
        }

        public int total_dias_trabajados_rigger_idsolicitudes(List<string> id_solicitudes)
        {

            List<DateTime> fechas_norepetidas = new List<DateTime>();
            foreach (string id in id_solicitudes)
            {
                SqlConnection conexion = crearConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "select fecha from detalle_estado_pago where  id_solicitud='" + id + "'";
                cmd.CommandType = CommandType.Text;


                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!fechas_norepetidas.Contains((DateTime)dr["fecha"]))
                    {
                        fechas_norepetidas.Add((DateTime)dr["fecha"]);
                    }
                }
                conexion.Close();
            }
            int total = fechas_norepetidas.Count;
            return total;


        }
        //--------------------------
        public List<string> obtener_idsolicitudes_por_rigger_estadopago(string nombre_rigger, string id_estado_pago_tipo)
        {

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_solicitud from trabajados_rigger where (id_estado_pago_tipo='" + id_estado_pago_tipo + "' and nombre_rigger='" + nombre_rigger + "')";
            cmd.CommandType = CommandType.Text;
            List<string> id_solicitudes = new List<string>();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id_solicitudes.Add((string)dr["id_solicitud"]);
            }


            conexion.Close();
            return id_solicitudes;
        }
        public int total_dias_trabajados_rigger_idestadopago(List<string> id_solicitudes)
        {

            List<DateTime> fechas_norepetidas = new List<DateTime>();
            foreach (string id in id_solicitudes)
            {
                SqlConnection conexion = crearConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "select fecha from detalle_estado_pago where id_solicitud='" + id + "'";
                cmd.CommandType = CommandType.Text;


                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!fechas_norepetidas.Contains((DateTime)dr["fecha"]))
                    {
                        fechas_norepetidas.Add((DateTime)dr["fecha"]);
                    }
                }
                conexion.Close();
            }
            int total = fechas_norepetidas.Count;
            return total;


        }
        public List<RiggerDiasTrabajados> obtener_rigger_por_estadopago(string id_estado_pago_tipo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_solicitud,nombre_rigger,rut_rigger,fecha_inicio,fecha_termino,dias_trabajados from trabajados_rigger where id_estado_pago_tipo='" + id_estado_pago_tipo + "'";
            cmd.CommandType = CommandType.Text;
            List<RiggerDiasTrabajados> datos = new List<RiggerDiasTrabajados>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                RiggerDiasTrabajados datos_rigger = new RiggerDiasTrabajados();

                datos_rigger.id_solicitud = (string)dr["id_solicitud"];
                datos_rigger.nombre_rigger = (string)dr["nombre_rigger"];
                datos_rigger.rut_rigger = (string)dr["rut_rigger"];
                datos_rigger.fecha_inicio = (DateTime)dr["fecha_inicio"];
                datos_rigger.fecha_fin = (DateTime)dr["fecha_termino"];
                datos_rigger.dias_trabajados = (int)dr["dias_trabajados"];
                datos.Add(datos_rigger);

            }
            conexion.Close();
            return datos;
        }
        //nuevo metodo
        public List<RiggerDiasTrabajados> obtener_rigger_id_general(string id_general)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_solicitud,nombre_rigger,rut_rigger,fecha_inicio,fecha_termino,dias_trabajados from trabajados_rigger where id_estado_pago='" + id_general + "'";
            cmd.CommandType = CommandType.Text;
            List<RiggerDiasTrabajados> datos = new List<RiggerDiasTrabajados>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                RiggerDiasTrabajados datos_rigger = new RiggerDiasTrabajados();

                datos_rigger.id_solicitud = (string)dr["id_solicitud"];
                datos_rigger.nombre_rigger = (string)dr["nombre_rigger"];
                datos_rigger.rut_rigger = (string)dr["rut_rigger"];
                datos_rigger.fecha_inicio = (DateTime)dr["fecha_inicio"];
                datos_rigger.fecha_fin = (DateTime)dr["fecha_termino"];
                datos_rigger.dias_trabajados = (int)dr["dias_trabajados"];
                datos.Add(datos_rigger);

            }
            conexion.Close();
            return datos;
        }
        public bool comprobar_rigger_id_solicitud(string rut_rigger, string id_solicitud)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre_rigger from trabajados_rigger where( id_solicitud='" + id_solicitud + "' and rut_rigger='" + rut_rigger + "')";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            bool retorno = dr.HasRows;

            cmd.Connection.Close();
            return retorno;
        }

        //modificado
        public void guardar_datos_rigger(RiggerDiasTrabajados datos)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "insert into trabajados_rigger(id_estado_pago,id_estado_pago_tipo,id_solicitud,nombre_rigger,rut_rigger,fecha_inicio,fecha_termino,dias_trabajados) values('"
               + datos.id_estado_pago + "','"
               + datos.id_estado_pago_tipo + "','"
               + datos.id_solicitud + "','"
               + datos.nombre_rigger + "','"
               + datos.rut_rigger + "','"
               + datos.fecha_inicio + "','"
               + datos.fecha_fin + "','"
               + datos.dias_trabajados + "')";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            conexion.Close();
        }
        public DatosRigger obtener_datos_rigger(string id_solicitud)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id_rigger_1,id_rigger_2,fecha_inicio_corregida,fecha_termino_corregida from solicitudes where id='" + id_solicitud + "' and estado='CONFIRMADA'";
            cmd.CommandType = CommandType.Text;
            DatosRigger datos_rigger = new DatosRigger();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {

                datos_rigger.rut_rigger1 = (string)dr["id_rigger_1"];
                datos_rigger.rut_rigger2 = (string)dr["id_rigger_2"];
                datos_rigger.fecha_inicio = (DateTime)dr["fecha_inicio_corregida"];
                datos_rigger.fecha_termino = (DateTime)dr["fecha_termino_corregida"];

            }
            conexion.Close();
            return datos_rigger;
        }
        public int calcular_total_dias_trabajados(string id_estado_pago_tipo, string rut_rigger)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select dias_trabajados from trabajados_rigger where id_estado_pago_tipo='" + id_estado_pago_tipo + "' and rut_rigger='" + rut_rigger + "'";
            cmd.CommandType = CommandType.Text;
            int dias = 0;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {


                dias += (int)dr["dias_trabajados"];
            }

            conexion.Close();
            return dias;

        }
        public string obtener_nombre_rigger(string rut)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre,apellidop,apellidom from trabajadordatos where rut='" + rut + "'";
            cmd.CommandType = CommandType.Text;
            string nombre_completo = "";
            string nombre = "";
            string apellidoP = "";
            string apellidoM = "";
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return nombre;


            nombre = (string)dr["nombre"];
            apellidoP = (string)dr["apellidop"];
            apellidoM = (string)dr["apellidom"];

            nombre_completo = nombre + " " + apellidoP + " " + apellidoM;
            conexion.Close();
            return nombre_completo;
        }
        #endregion

        #region Equipo(Insertar y Eliminar)
        public void guardar_Equipocautivo(EquipoCautivo dato_cautivo)
        {
            bool seleccion_accion = comprobar_cautivo(dato_cautivo.tag_equipo);
            if (!seleccion_accion)
            {

                SqlConnection conexion = crearConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "insert into equipos_cautivo(tag_equipo,costo_fijo,area_trabajo) values('"
                   + dato_cautivo.tag_equipo + "', '"
                   + dato_cautivo.costo_fijo + "','"
                   + dato_cautivo.area_trabajo + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
            }
            else
            {
                SqlConnection conexion = crearConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "update equipos_cautivo set costo_fijo='" + dato_cautivo.costo_fijo + "', area_trabajo='" + dato_cautivo.area_trabajo + "' where tag_equipo='" + dato_cautivo.tag_equipo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                conexion.Close();
            }
        }
        public EquipoCautivo obtener_datos_cautivo(string tag)
        {
            EquipoCautivo datos = new EquipoCautivo();
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from equipos_cautivo where tag_equipo='" + tag + "'";
            cmd.CommandType = CommandType.Text;
            List<EquipoCertificados> lista_certificados = new List<EquipoCertificados>();
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return datos;
            datos.costo_fijo = (string)dr["costo_fijo"];
            datos.area_trabajo = (string)dr["area_trabajo"];
            conexion.Close();
            return datos;
        }
        public void update_Equipocautivo(EquipoCautivo dato_cautivo)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "update equipos_cautivo set costo_fijo='" + dato_cautivo.costo_fijo + "', area_trabajo='" + dato_cautivo.area_trabajo + "' where tag_equipo='" + dato_cautivo.tag_equipo + "'";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            conexion.Close();
        }
        public EquipoNoCautivo obtener_datos_Nocautivo(string tipo_equipo)
        {

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from tipo_equipo where tipo='" + tipo_equipo + "'";
            cmd.CommandType = CommandType.Text;
            EquipoNoCautivo datos = new EquipoNoCautivo();
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return datos;

            double minimoGarantizado = (int)dr["minimo_garantizado"];
            datos.minimo_garantizado = minimoGarantizado.ToString();
            double costoHora = (double)dr["costo_hora"];
            datos.costo_hora = costoHora.ToString();
            double costoHoraExtra = (double)dr["costo_hora_extra"];
            datos.costo_hora_extra = costoHoraExtra.ToString();
            conexion.Close();
            return datos;
        }
        public bool guardar_certificados(EquipoCertificados certificados)
        {
            try
            {
                SqlConnection conexion = crearConexion();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;

                string fecha = "'"+string.Empty+"'";
               
                if (!string.IsNullOrEmpty(certificados.fecha_vencimiento))
                {
                     
                }

                cmd.CommandText = "insert into certificados_equipo(tag_equipo,nombre_certificado,fecha_vencimiento,url) values('"
                   + certificados.tag_equipo + "', '"
                   + certificados.nombre_certificado + "','"
                   + certificados.fecha_vencimiento +"','"
                   + certificados.url + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool eliminar_certificados(string tag)
        {
            SqlConnection conexion = crearConexion();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "delete from certificados_equipo where tag_equipo='" + tag + "'";
                cmd.CommandType = CommandType.Text;

                // cambiar por el comando que coresponde
                cmd.ExecuteNonQuery();

                conexion.Close();
                return true;

            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }
        }
        public bool eliminar_equipo(string tag)
        {

            SqlConnection conexion = crearConexion();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "delete from equipos where tag='" + tag + "'";
                cmd.CommandType = CommandType.Text;

                // cambiar por el comando que coresponde
                cmd.ExecuteNonQuery();

                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }
        }
        public List<EquipoCertificados> obtener_certificados(string tag)
        {
            List<EquipoCertificados> lista_Datos = new List<EquipoCertificados>();
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from certificados_equipo where tag_equipo='" + tag + "'";
            cmd.CommandType = CommandType.Text;
            List<EquipoCertificados> lista_certificados = new List<EquipoCertificados>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                EquipoCertificados certificado = new EquipoCertificados();

                certificado.nombre_certificado = (string)dr["nombre_certificado"];

                DateTime fecha;
                if ( !string.IsNullOrEmpty( dr["fecha_vencimiento"].ToString() ))
                {
                    string[] fecha_temp = dr["fecha_vencimiento"].ToString().Split('-');
                    fecha = new DateTime(int.Parse(fecha_temp[0]), int.Parse(fecha_temp[1]), int.Parse(fecha_temp[2]));

                    string año = fecha.Year + "";
                    string mes = fecha.Month + "";
                    if (mes.Length == 1) mes = "0" + mes;
                    string dia = fecha.Day + "";
                    if (dia.Length == 1) dia = "0" + dia;

                    certificado.fecha_vencimiento = dia + "/" + mes + "/" + año;
                }
                else certificado.fecha_vencimiento = string.Empty;

                certificado.url = (string)dr["url"];

                lista_certificados.Add(certificado);
            }
            conexion.Close();
            return lista_certificados;
        }
        public bool guardar_datosequipo(DatosEquipo datos)
        {
            SqlConnection conexion = crearConexion();
            try
            {
                SqlCommand cmd = new SqlCommand();

                string añoString = datos.fecha_ingreso_faena.Split('-')[0];
                string mesString = datos.fecha_ingreso_faena.Split('-')[1];
                string diaString = datos.fecha_ingreso_faena.Split('-')[2];

                int año = int.Parse(añoString);
                int mes = int.Parse(mesString);
                int dia = int.Parse(diaString);

               // if (datos.url_imagen == null || datos.url_imagen.Equals("")) datos.url_imagen = "VACÍA";

                cmd.Connection = conexion;
                cmd.CommandText = "insert into equipos(familia_equipo,tipo_equipo,marca,modelo,ano_fabricacion,"
                    + "capacidad,estado,empresa_propietaria,tag,"
                    + " equipo_cautivo,equipo_nocautivo,fecha_ingreso_faena,odometro,hora_horometro,url) values('"
                   + datos.familia_equipo + "', '"
                   + datos.tipo_equipo + "','"
                   + datos.marca + "','"
                   + datos.modelo + "','"
                   + datos.año_fabricacion + "','"
                   + datos.capacidad + "','"
                   + datos.estado + "','"
                   + datos.empresa_propietaria + "','"
                   + datos.tag + "','"
                   + datos.cautivo + "','"
                   + datos.nocautivo + "','"
                   + new DateTime(año,mes,dia)+"','"
                   + datos.odometro + "','"
                   + datos.horas_horometro + "','"
                   + datos.url_imagen + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception)
            {
                conexion.Close();
                return false;
            }
        }
        public DatosEquipo obtener_datos_equipo(string tag)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from equipos where tag='" + tag + "'";
            cmd.CommandType = CommandType.Text;
            DatosEquipo datos = new DatosEquipo();
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return datos;
            datos.familia_equipo = (string)dr["familia_equipo"];
            datos.tipo_equipo = (string)dr["tipo_equipo"];
            datos.marca = (string)dr["marca"];
            datos.modelo = (string)dr["modelo"];
            datos.año_fabricacion = (string)dr["ano_fabricacion"];
            datos.capacidad = (string)dr["capacidad"];
            datos.estado = (string)dr["estado"];
            datos.empresa_propietaria = (string)dr["empresa_propietaria"];
            datos.tag = (string)dr["tag"];
            datos.cautivo = (string)dr["equipo_cautivo"];
            datos.nocautivo = (string)dr["equipo_nocautivo"];

            DateTime fechaIngreso = new DateTime();
            if (!dr["fecha_ingreso_faena"].GetType().Equals(typeof(System.DBNull)))
            {
                fechaIngreso = (DateTime)dr["fecha_ingreso_faena"];
                string año = fechaIngreso.Year + "";
                string mes = fechaIngreso.Month + "";
                if (mes.Length == 1) mes = "0" + mes;
                string dia = fechaIngreso.Day + "";
                if (dia.Length == 1) dia = "0" + dia;
                datos.fecha_ingreso_faena = dia + "/" + mes + "/" + año;
            }
            else datos.fecha_ingreso_faena = string.Empty;

            datos.odometro = (string)dr["odometro"];
            datos.horas_horometro = (string)dr["hora_horometro"];
            datos.url_imagen = (string)dr["url"];
            conexion.Close();
            return datos;
        }
        public List<DatosEquipo> obtener_lista_equipos()
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tag,familia_equipo,tipo_equipo,modelo,ano_fabricacion,capacidad,estado from Equipos";
            cmd.CommandType = CommandType.Text;
            List<DatosEquipo> lista_Equipos = new List<DatosEquipo>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DatosEquipo equipos = new DatosEquipo();

                equipos.tag = (string)dr["tag"];
                equipos.familia_equipo = (string)dr["familia_equipo"];
                equipos.tipo_equipo = (string)dr["tipo_equipo"];
                equipos.modelo = (string)dr["modelo"];
                equipos.año_fabricacion = (string)dr["ano_fabricacion"];
                equipos.capacidad = (string)dr["capacidad"];
                equipos.estado = (string)dr["estado"];
                lista_Equipos.Add(equipos);
            }
            conexion.Close();
            return lista_Equipos;
        }
        public string[] datos_familia()
        {
            string texto_familia="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from familia_equipo";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_familia += (string)dr["nombre"] + ",";
            }
            texto_familia = texto_familia.TrimEnd(',');
            string[] familia = texto_familia.Split(',');
            conexion.Close();
            return familia;
        }
        public string[] datos_marca()
        {
            string texto_marca="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from marca_equipo";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_marca += (string)dr["marca"] + ",";
            }
            texto_marca = texto_marca.TrimEnd(',');
            string[] marcas = texto_marca.Split(',');
            conexion.Close();
            return marcas;
        }
        public string[] datos_empresa()
        {
            string texto_empresa="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from empresa";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_empresa += (string)dr["nombre"] + ",";
            }
            texto_empresa = texto_empresa.TrimEnd(',');
            string[] empresas = texto_empresa.Split(',');
            conexion.Close();
            return empresas;
        }
        public string[] datos_area()
        {
            string texto_area="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from area";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_area += (string)dr["nombre"] + ",";
            }
            texto_area = texto_area.TrimEnd(',');
            string[] empresas = texto_area.Split(',');
            conexion.Close();
            return empresas;
        }
        public string[] datos_tipo_equipo(string familia)
        {
            string texto_tipo_equipo="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from tipo_equipo where familia='" + familia + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_tipo_equipo += (string)dr["tipo"] + ",";
            }
            texto_tipo_equipo = texto_tipo_equipo.TrimEnd(',');
            string[] tipo_equipo = texto_tipo_equipo.Split(',');
            conexion.Close();
            return tipo_equipo;
        }
        public string[] datos_modelo(string marca, string tipo)
        {
            string texto_modelo="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select * from modelo_equipo where marca='" + marca + "' and tipo='"+tipo+"'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_modelo += (string)dr["modelo"] + ",";
            }
            texto_modelo = texto_modelo.TrimEnd(',');
            string[] modelo = texto_modelo.Split(',');
            conexion.Close();
            return modelo;
        }
        public string[] datos_marcas(string tipo)
        {
            string texto_modelo="";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select DISTINCT marca from modelo_equipo where tipo='" + tipo + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_modelo += (string)dr["marca"] + ",";
            }
            texto_modelo = texto_modelo.TrimEnd(',');
            string[] modelo = texto_modelo.Split(',');
            conexion.Close();
            return modelo;
        }

        public string[] getMarcas()
        {
            string texto_marca = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct marca from equipos";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_marca += (string)dr["marca"] + ",";
            }
            texto_marca = texto_marca.TrimEnd(',');
            string[] marcas = texto_marca.Split(',');
            conexion.Close();
            return marcas;
        }

        public string[] getModelos(string marca)
        {
            string texto_marca = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct modelo from equipos where(marca = '" + marca + "')";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_marca += (string)dr["modelo"] + ",";
            }
            texto_marca = texto_marca.TrimEnd(',');
            string[] marcas = texto_marca.Split(',');
            conexion.Close();
            return marcas;
        }

        public string[] getCapacidad(string marca, string  modelo)
        {
            string texto_marca = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select distinct capacidad from equipos where(marca = '" + marca + "' and modelo = '" + modelo + "')";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                texto_marca += (string)dr["capacidad"] + ",";
            }
            texto_marca = texto_marca.TrimEnd(',');
            string[] marcas = texto_marca.Split(',');
            conexion.Close();
            return marcas;
        }
        #endregion

        #region Email

        public void save_datos_solicitud(Mail datos)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            try
            {
                cmd.CommandText = "insert into email_seguimiento_solicitudes(id_solicitud,usuario_creador,estado_solicitud,fecha_creacion,correo) values('"
                   + datos.id_solicitud + "', '"
                   + datos.usuario_creador + "', '"
                   + datos.estado + "', '"
                   + datos.fecha_creacion + "', '"
                   + datos.correo_usuario + "')";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception)
            { }
        }
        public string verificar_id(string id, string estado)
        {
            SqlConnection conexion = new conexion().crearConexion();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conexion;
            cmd.CommandText = "SELECT correo from email_seguimiento_solicitudes where(id_solicitud='" + id + "' and estado_solicitud='" + estado + "')";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();

            string correo = "";

            if (!dr.Read()) return correo;

            correo = (string)dr["correo"];


            conexion.Close();
            return correo;
        }
        public void actualizar_estado_email_solicitud(string id, string estado)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            try
            {
                cmd.CommandText = "update email_seguimiento_solicitudes set estado_solicitud ='" + estado + "'  where id_solicitud='" + id + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception)
            {
                conexion.Close();
            }


        }
        public string[] obtener_pendientes(DateTime fecha_incio_comprobacion, string estado, string area)
        {
            string texto_condicion = "";
            if (estado.Equals("NUEVA"))
            {
                texto_condicion = "fecha_creacion";
            }
            else
            {
                texto_condicion = "fecha_inicio_corregida";
            }
            DateTime fecha_convertida = new DateTime(fecha_incio_comprobacion.Year, fecha_incio_comprobacion.Month, fecha_incio_comprobacion.Day, fecha_incio_comprobacion.Hour, fecha_incio_comprobacion.Minute, fecha_incio_comprobacion.Second);
            string texto_id = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where(" + texto_condicion + " >'" + fecha_convertida + "' and estado='" + estado + "' and area='" + area + "')";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_id += (string)dr["id"] + ",";
            }
            texto_id = texto_id.TrimEnd(',');
            string[] id = texto_id.Split(',');
            conexion.Close();
            return id;
        }
        public string[] obtener_nombres_jefes_area(string area)
        {
            string usuarios_Area = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from jefe_area where area='" + area + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                usuarios_Area += (string)dr["nombre"] + ",";
            }
            usuarios_Area = usuarios_Area.TrimEnd(',');
            string[] nombres = usuarios_Area.Split(',');
            conexion.Close();
            return nombres;
        }
        public string[] obtener_mail_usuario_por_rol(string rol)
        {
            string texto_emails = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select mail from usuarios where rol='" + rol + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                texto_emails += (string)dr["mail"] + ",";
            }
            texto_emails = texto_emails.TrimEnd(',');
            string[] emails = texto_emails.Split(',');
            conexion.Close();
            return emails;
        }
        public string obtener_mail_usuario_por_nombre(string nombre_usuario)
        {
            string mail = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select mail from usuarios where identificador='" + nombre_usuario + "'";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return mail;

            mail = (string)dr["mail"] + ",";

            conexion.Close();
            return mail;
        }
        public DateTime ultimo_aviso(string estado)
        {
            SqlConnection conexion = new conexion().crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            DateTime ultimo_av = new DateTime();
            if (estado.Equals("NUEVA"))
            {
                cmd.CommandText = "SELECT fecha_ultimo_aviso_planificar from pendientes";
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return ultimo_av;

                ultimo_av = (DateTime)dr["fecha_ultimo_aviso_planificar"];


                conexion.Close();

            }
            if (estado.Equals("PLANIFICADA"))
            {
                cmd.CommandText = "SELECT fecha_ultimo_aviso_autorizar from pendientes";
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return ultimo_av;

                ultimo_av = (DateTime)dr["fecha_ultimo_aviso_autorizar"];


                conexion.Close();

            }
            if (estado.Equals("AUTORIZADA"))
            {
                cmd.CommandText = "SELECT fecha_ultimo_aviso_finalizar from pendientes";
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();



                if (!dr.Read()) return ultimo_av;

                ultimo_av = (DateTime)dr["fecha_ultimo_aviso_finalizar"];


                conexion.Close();

            }
            if (estado.Equals("FINALIZADA"))
            {
                cmd.CommandText = "SELECT fecha_ultimo_aviso_confirmar from pendientes";
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();



                if (!dr.Read()) return ultimo_av;

                ultimo_av = (DateTime)dr["fecha_ultimo_aviso_confirmar"];


                conexion.Close();

            }
            return ultimo_av;
        }
        public void actualizar_fecha_ultimo_aviso(DateTime fecha, string estado)
        {
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            DateTime fecha_corregida = new DateTime(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, fecha.Second);
            if (estado.Equals("NUEVA"))
            {
                try
                {
                    cmd.CommandText = "update pendientes set fecha_ultimo_aviso_planificar ='" + fecha_corregida + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                }
                catch (Exception)
                {
                    conexion.Close();
                }
            }
            if (estado.Equals("PLANIFICADA"))
            {
                try
                {
                    cmd.CommandText = "update pendientes set fecha_ultimo_aviso_autorizar ='" + fecha_corregida + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                }
                catch (Exception)
                {
                    conexion.Close();
                }
            }
            if (estado.Equals("AUTORIZADA"))
            {
                try
                {
                    cmd.CommandText = "update pendientes set fecha_ultimo_aviso_finalizar ='" + fecha_corregida + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                }
                catch (Exception)
                {
                    conexion.Close();
                }
            }
            if (estado.Equals("CONFIRMADA"))
            {
                try
                {
                    cmd.CommandText = "update pendientes set fecha_ultimo_aviso_confirmar ='" + fecha_corregida + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                }
                catch (Exception)
                {
                    conexion.Close();
                }
            }
        }
        public string[] obtener_areas()
        {
            string area = "";
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select nombre from area ";
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                area = (string)dr["nombre"] + ",";
            }
            area = area.TrimEnd(',');
            string[] areas = area.Split(',');
            conexion.Close();
            return areas;
        }
        #endregion

        #region metodos para  REPORTES

        public List<DatosEquipo> obtener_equipos_por_fecha(string fechaIngreso)
        {
            string[] fecha_separada = fechaIngreso.Split('/');
            DateTime fecha_corregida = new DateTime(int.Parse(fecha_separada[2]), int.Parse(fecha_separada[1]), int.Parse(fecha_separada[0]));
            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select tag,familia_equipo,tipo_equipo,modelo,ano_fabricacion,capacidad,estado from Equipos where fecha_ingreso_faena<'" + fecha_corregida + "'";
            cmd.CommandType = CommandType.Text;
            List<DatosEquipo> lista_Equipos = new List<DatosEquipo>();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DatosEquipo equipos = new DatosEquipo();

                equipos.tag = (string)dr["tag"];
                equipos.familia_equipo = (string)dr["familia_equipo"];
                equipos.tipo_equipo = (string)dr["tipo_equipo"];
                equipos.modelo = (string)dr["modelo"];
                equipos.año_fabricacion = (string)dr["ano_fabricacion"];
                equipos.capacidad = (string)dr["capacidad"];
                equipos.estado = (string)dr["estado"];
                lista_Equipos.Add(equipos);
            }
            conexion.Close();
            return lista_Equipos;

        }
        public List<Solicitud> obtener_confirmadas_por_fecha(string fechaInicio, string fechaTermino)
        {
            string[] fecha_separada_inicio = fechaInicio.Split('/');
            string[] fecha_separada_termino = fechaTermino.Split('/');
            DateTime fecha_corregida_inicio = new DateTime(int.Parse(fecha_separada_inicio[2]), int.Parse(fecha_separada_inicio[1]), int.Parse(fecha_separada_inicio[0]));
            DateTime fecha_corregida_fin = new DateTime(int.Parse(fecha_separada_termino[2]), int.Parse(fecha_separada_termino[1]), int.Parse(fecha_separada_termino[0]));
            fecha_corregida_fin = fecha_corregida_fin.AddDays(1);

            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where (estado='CONFIRMADA' and fecha_inicio_corregida>=@inicio and fecha_inicio_corregida<@fin)";

            cmd.Parameters.Add("@inicio", SqlDbType.DateTime).Value = fecha_corregida_inicio;
            cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = fecha_corregida_fin;

            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }

        public List<Solicitud> obtener_solicitudes_por_fecha(string fechaInicio, string fechaTermino)
        {
            string[] fecha_separada_inicio = fechaInicio.Split('/');
            string[] fecha_separada_termino = fechaTermino.Split('/');
            DateTime fecha_corregida_inicio = new DateTime(int.Parse(fecha_separada_inicio[2]), int.Parse(fecha_separada_inicio[1]), int.Parse(fecha_separada_inicio[0]));
            DateTime fecha_corregida_fin = new DateTime(int.Parse(fecha_separada_termino[2]), int.Parse(fecha_separada_termino[1]), int.Parse(fecha_separada_termino[0]));
            fecha_corregida_fin = fecha_corregida_fin.AddDays(1);

            List<Solicitud> sol = new List<Solicitud>();

            SqlConnection conexion = crearConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandText = "select id from solicitudes where (fecha_inicio_corregida>=@inicio and fecha_inicio_corregida<@fin)";

            cmd.Parameters.Add("@inicio", SqlDbType.DateTime).Value = fecha_corregida_inicio;
            cmd.Parameters.Add("@fin", SqlDbType.DateTime).Value = fecha_corregida_fin;

            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sol.Add(this.obtenerSolicitudPorID((string)dr["id"]));
            }

            conexion.Close();

            return sol;
        }

        #endregion
    }
    public class Delete
    {
        SqlConnection cnx = new conexion().crearConexion();
        SqlCommand cmd = new SqlCommand();

        public bool deleteCertificacion(string nombre)
        {
            try
            {
                cmd.Connection = cnx;
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "Certificacion where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool deleteEmpresa(string nombre)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "Empresa where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool deleteFamilia_Equipo(string nombre)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "Familia_Equipo where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool deleteMarca_Equipo(string marca)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "marca where marca = '" + marca + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool deleteModelo_Equipo(string modelo)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "Modelo_Equipo where modelo = '" + modelo + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool deleteJefe_Area(string jefe)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "delete from ";
                sqlcmd += "Jefe_Area where nombre = '" + jefe + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
    }
    public class Insert
    {
        SqlConnection cnx = new conexion().crearConexion();
        SqlCommand cmd = new SqlCommand();
        public bool InsertCertificacion(Certificacion certificacion)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "Certificacion values(";
                sqlcmd += "'" + certificacion.nombre + "',";
                sqlcmd += "'" + certificacion.descripcion + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool InsertEmpresa(Empresa empresa)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "Empresa values(";
                sqlcmd += "'" + empresa.nombre + "',";
                sqlcmd += "'" + empresa.descripcion + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool InsertFamilia_Equipo(Familia_Equipo familia)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "Familia_Equipo values(";
                sqlcmd += "'" + familia.nombre + "',";
                sqlcmd += "'" + familia.descripcion + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool InsertMarca_Equipo(Marca_Equipo marca)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "marca values(";
                sqlcmd += "'" + marca.marca + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool InsertModelo_Equipo(Modelo_Equipo modelo)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "Modelo_Equipo values(";
                sqlcmd += "'" + modelo.modelo + "',";
                sqlcmd += "'" + modelo.marca + "',";
                sqlcmd += "'" + modelo.tipo + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool InsertJefe_Area(Jefe_Area jefe)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "insert into ";
                sqlcmd += "Jefe_Area values(";
                sqlcmd += "'" + jefe.nombre + "',";
                sqlcmd += "'" + jefe.area + "'";
                sqlcmd += ")";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }


    }
    public class Get
    {
        SqlConnection cnx = new conexion().crearConexion();
        SqlCommand cmd = new SqlCommand();

        public List<Certificacion> GetCertificacion()
        {
            List<Certificacion> certificaciones = new List<Certificacion>();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Certificacion";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Certificacion certificacion = new Certificacion();
                    certificacion.nombre = (string)dr["nombre"];
                    certificacion.descripcion = (string)dr["descripcion"];
                    certificaciones.Add(certificacion);
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return certificaciones;
        }
        public Certificacion GetCertificacion(string id)
        {
            Certificacion certificacion = new Certificacion();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Certificacion ";
                sqlcmd += "where nombre = '"+ id +"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    certificacion.nombre = (string)dr["nombre"];
                    certificacion.descripcion = (string)dr["descripcion"];
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return certificacion;
        }
        public List<Empresa> GetEmpresa()
        {
            List<Empresa> empresas = new List<Empresa>();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Empresa";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    Empresa empresa = new Empresa();
                    empresa.nombre = (string)dr["nombre"];
                    empresa.descripcion = (string)dr["descripcion"];
                    empresas.Add(empresa);
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return empresas;
        }
        public Empresa GetEmpresa(string id)
        {
            Empresa empresa = new Empresa();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Empresa ";
                sqlcmd += "where nombre = '"+ id +"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    empresa.nombre = (string)dr["nombre"];
                    empresa.descripcion = (string)dr["descripcion"];
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return empresa;
        }
        public List<Familia_Equipo> GetFamilia_Equipo()
        {
            List<Familia_Equipo> familias = new List<Familia_Equipo>();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Familia_Equipo";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Familia_Equipo familia = new Familia_Equipo();
                    familia.nombre = (string)dr["nombre"];
                    familia.descripcion = (string)dr["descripcion"];
                    familias.Add(familia);
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return familias;
        }
        public Familia_Equipo GetFamilia_Equipo(string id)
        {
            Familia_Equipo familia = new Familia_Equipo();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Familia_Equipo ";
                sqlcmd += "where nombre = '"+ id +"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    familia.nombre = (string)dr["nombre"];
                    familia.descripcion = (string)dr["descripcion"];
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return familia;
        }
        public List<Marca_Equipo> GetMarca_Equipo()
        {
            List<Marca_Equipo> marcas = new List<Marca_Equipo>();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select *";
                sqlcmd += "from Marca";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Marca_Equipo marca = new Marca_Equipo();
                    marca.marca = (string)dr["marca"];
                    marcas.Add(marca);
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return marcas;
        }
        public Marca_Equipo GetMarca_Equipo(string id)
        {
            Marca_Equipo marca = new Marca_Equipo();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select *";
                sqlcmd += "from Marca ";
                sqlcmd += "where marca ='"+ id+"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    marca.marca = (string)dr["marca"];
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return marca;
        }
        public List<Modelo_Equipo> GetModelo_Equipo()
        {
            List<Modelo_Equipo> modelos = new List<Modelo_Equipo>();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Modelo_Equipo";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    Modelo_Equipo modelo = new Modelo_Equipo();
                    modelo.modelo = (string)dr["modelo"];
                    modelo.marca = (string)dr["marca"];
                    modelo.tipo = (string)dr["tipo"];
                    modelos.Add(modelo);
                }
                dr.Close();

            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return modelos;
        }
        public Modelo_Equipo GetModelo_Equipo(string id)
        {
            Modelo_Equipo modelo = new Modelo_Equipo();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Modelo_Equipo ";
                sqlcmd += "where modelo = '"+ id +"'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    modelo.modelo = (string)dr["modelo"];
                    modelo.marca = (string)dr["marca"];
                    modelo.tipo = (string)dr["tipo"];
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return modelo;
        }
        public bool existCentro(string id)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from solicitudes ";
                sqlcmd += "where ( centro_costo = '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    cnx.Close();
                    return true;
                }

                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool existMarca(string id)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Modelo_Equipo ";
                sqlcmd += "where ( marca = '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    cnx.Close();
                    return true;
                }

                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool existFamilia(string id)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from tipo_equipo ";
                sqlcmd += "where ( familia = '" + id + "')";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    cnx.Close();
                    return true;
                }

                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public Jefe_Area GetJefe_Area(string nombre)
        {
            Jefe_Area jefe = new Jefe_Area();
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "select * ";
                sqlcmd += "from Jefe_Area where nombre='" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        jefe.nombre = (string)dr["nombre"];
                        jefe.area = (string)dr["area"];
                    }
                }
                else
                {
                    jefe.nombre = string.Empty;
                    jefe.area = string.Empty;
                }
                dr.Close();
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return jefe;
        }
    }
    public class Update
    {
        SqlConnection cnx = new conexion().crearConexion();
        SqlCommand cmd = new SqlCommand();
        public bool updateCertificacion(string nombre, string descripcion)
        {
            try
            {
                cmd.Connection = cnx;
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "update Certificacion ";
                sqlcmd += "set descripcion='" + descripcion + "' where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool updateEmpresa(string nombre, string descripcion)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "update Empresa ";
                sqlcmd += "set descripcion='" + descripcion + "' where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool updateFamilia_Equipo(string nombre, string descripcion)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "update Familia_Equipo ";
                sqlcmd += "set descripcion='" + descripcion + "' where nombre = '" + nombre + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
        public bool updateModelo_Equipo(string modelo, string marca, string tipo)
        {
            try
            {
                cmd.Connection = cnx;
                string sqlcmd = string.Empty;
                sqlcmd += "update Modelo_Equipo ";
                sqlcmd += "set marca='" + marca + "', tipo='" + tipo + "' where modelo = '" + modelo + "'";

                cmd.CommandText = sqlcmd;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception)
            {
                cnx.Close();
            }
            cnx.Close();
            return false;
        }
    }

}
