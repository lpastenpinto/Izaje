using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIzaje.Models
{
    public class Solicitud
    {        
        public bool calculado { get; set; }
        public string estadoPagoCalculado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string idSolicitud { get;set; }
        public string estado { get; set; }

        //Datos del solicitante
        public string nombre { get; set; }
        public string empresa { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }

        //Datos del lugar
        public string gerencia { get; set; }
        public string superintendencia { get; set; }
        public string area { get; set; }
        public string centroCosto { get; set; }
        public string descripcionLugar { get; set; }

        //Datos del servicio
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
        public string criticidad { get; set; }
        public int tiempoEstimadoOperacion { get; set; }
        public int peso { get; set; }
        public string turno { get; set; }
        public int alto { get; set; }
        public int ancho { get; set; }
        public int largo { get; set; }
        public string rutaImagen { get; set; }
        public string descripcionCarga { get; set; }
        public string descripcionServicio { get; set; }
        public bool solicitadoAnteriormente { get; set; }
        public bool requiereRigger { get; set; }
        public bool requiereManiobra { get; set; }
        public bool cuentaConManiobra { get; set; }

        //Datos de servicio Corregidos        
        public string criticidadCorregida { get; set; }
        public int tiempoEstimadoOperacionCorregido { get; set; }
        public int pesoCorregido { get; set; }
        public string turnoCorregido { get; set; }
        public int altoCorregido { get; set; }
        public int anchoCorregido { get; set; }
        public int largoCorregido { get; set; }
        public string descripcionCargaCorregida { get; set; }
        public string descripcionServicioCorregida { get; set; }        

        //Recursos asignados
        public DateTime inicioCorregido { get; set; }
        public DateTime finCorregido { get; set; }
        public string idEquipo1 { get; set; }
        public string idOperador1 { get; set; }
        public string idRigger1 { get; set; }
        public string idEquipo2 { get; set; }
        public string idOperador2 { get; set; }
        public string idRigger2 { get; set; }

        //Operaciones
        public string planificadaPor { get; set; }
        public string autorizadaPor { get; set; }
        public string finalizadaPor { get; set; }
        public string confirmadaPor { get; set; }


        //Datos finalización
        public List<string> fechas { get; set; }
        public List<string> horaRelojInicial1 { get; set; }
        public List<string> horaRelojFinal1 { get; set; }
        public List<string> horaHorometroInicial1 { get; set; }
        public List<string> horaHorometroFinal1 { get; set; }
        public List<string> horaRelojInicial2 { get; set; }
        public List<string> horaRelojFinal2 { get; set; }
        public List<string> horaHorometroInicial2 { get; set; }
        public List<string> horaHorometroFinal2 { get; set; }
        public static string crearNuevaIDSolicitud(string usuario) {
            string id = "";
            string comienzoUsuario = usuario.Split(' ')[0].ToArray()[0] + "";
            string finUsuario = usuario.Split(' ')[1].ToArray()[0] + "";

            DateTime tiempoActual=DateTime.Now;
            id = "" + tiempoActual.Year +
                tiempoActual.Month +
                tiempoActual.Day +
                tiempoActual.Hour +
                tiempoActual.Minute +
                tiempoActual.Second +
                tiempoActual.Millisecond +
                comienzoUsuario + finUsuario;

            //agregar método para saber si existe o no una solicitud con la misma id
            return id;
        }
        public static Solicitud obtenerSolicitud(string id){

            Solicitud retorno = new conexion().obtenerSolicitudPorID(id);
            return retorno;
        }
        public static List<Solicitud> obtenerNuevas() {
            List<Solicitud> retorno = new conexion().obtenerSolicitudesNuevas();
            return retorno;
        }
        public static List<Solicitud> obtenerPlanificadas()
        {
            List<Solicitud> retorno = new conexion().obtenerSolicitudesPlanificadas();
            return retorno;
        }
        public static List<Solicitud> obtenerAutorizadas() 
        {
            List<Solicitud> retorno = new conexion().obtenerSolicitudesAutorizadas();
            return retorno;
        }
        public static List<Solicitud> obtenerFinalizadas() 
        {
            List<Solicitud> retorno = new conexion().obtenerSolicitudesFinalizadas();
            return retorno;
        }
        public static List<Solicitud> obtenerConfirmadas() 
        {
            List<Solicitud> retorno = new conexion().obtenerSolicitudesConfirmadas();
            return retorno;
        }
        public static List<Solicitud> obtenerTodas()
        {
            List<Solicitud> retorno = new conexion().obtenerTodas();
            return retorno;
        }

        public static List<Solicitud> obtenerTodasResumida()
        {
            //List<Solicitud> retorno = new conexion().obtenerTodas();
            List<Solicitud> retorno = new conexion().obtenerTodasResumida();
            return retorno;
        }

        public static List<Solicitud> obtenerTodas_info(DateTime fecha)
        {
            List<Solicitud> retorno = new conexion().obtenerTodas(fecha);
            return retorno;
        }
        public static List<Solicitud> obtenerTodas_info(string equipo, string trabajador, DateTime fecha_in, DateTime fecha_out)
        {
            List<Solicitud> retorno = new conexion().obtenerTodas(equipo, trabajador, fecha_in, fecha_out);
            return retorno;
        }
        public void actualizarEnBD(string jefeDeArea) { 
            //Se actualizan los datos de la solicitud en la BD
            new conexion().guardarSolicitudEnBD(this, jefeDeArea);
        }
        public static void eliminarSolicitud(string idSolicitud) {
            new conexion().eliminarSolicitud(idSolicitud);
        }
    }
    public class datos_confirmar_solicitud
    {

        public string fecha { get; set; }
        public string tag1 { get; set; }
        public string hora_reloj_inicio1 { get; set; }
        public string hora_reloj_fin1 { get; set; }
        public string hora_horometro_inicio1 { get; set; }
        public string hora_horometro_fin1 { get; set; }
        public string tag2 { get; set; }
        public string hora_reloj_inicio2 { get; set; }
        public string hora_reloj_fin2 { get; set; }
        public string hora_horometro_incio2 { get; set; }
        public string hora_horometro_fin2 { get; set; }
        
    }
    public class datos_refinalizar_solicitud
    {
        public string fecha1 { get; set; }
        public string tag1 { get; set; }
        public string rutop1 { get; set; }
        public string rutrg1 { get; set; }
        public string fecha2 { get; set; }
        public string tag2 { get; set; }
        public string rutop2 { get; set; }
        public string rutrg2 { get; set; }

    }
    public class MetodosReportesSolicitud
    {
        public List<Solicitud> mostrar_solicitudes_por_fecha(string fechainicio, string fechafin)
        {
            // se consulta en la bd por todos los equipos disponibles
            List<Solicitud> lista_datos = new List<Solicitud>();
            lista_datos = new conexion().obtener_confirmadas_por_fecha(fechainicio, fechafin);
            return lista_datos;
        }

        public List<Solicitud> mostrar_todas_solicitudes_por_fecha(string fechainicio, string fechafin)
        {
            // se consulta en la bd por todos los equipos disponibles
            List<Solicitud> lista_datos = new List<Solicitud>();
            lista_datos = new conexion().obtener_solicitudes_por_fecha(fechainicio, fechafin);
            return lista_datos;
        }
    }
}