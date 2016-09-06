using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIzaje.Models
{
    public class Mail
    {
        public string id_solicitud { get; set; }
        public string usuario_creador { get; set; }
        public string estado { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string correo_usuario { get; set; }

        public void guardar_datos_solicitud(Mail datos_solicitud)
        {
            new conexion().save_datos_solicitud(datos_solicitud);
        }
        public string verificar_id(string id,string estado)
        {
            string retorno = "";
            retorno = new conexion().verificar_id(id, estado);
            return retorno;
        }

        public void actualizar_estado(string id, string estado)
        {
            new conexion().actualizar_estado_email_solicitud(id, estado);
        }
        public string[] ver_pendientes(DateTime fecha_inicio_comprobacion, string estado, string area)
        {
            string[] retorno = new conexion().obtener_pendientes(fecha_inicio_comprobacion,estado,area);
            return retorno;
        }
        public string[] obtener_todas_area()
        {
            
            
            string[]    retorno = new conexion().obtener_areas();
                return retorno;
        }
        public string[] mail_usuarios(string estado,string area)
        { 
            string rol="";
            string[] retorno;
            if(estado.Equals("NUEVA") || estado.Equals("AUTORIZADA"))
            {
                rol="Izaje";
                 retorno = new conexion().obtener_mail_usuario_por_rol(rol);
            }
            else
            {
                rol="jefeArea";
                string[] usuarios_jefes_area = new conexion().obtener_nombres_jefes_area(area);
                string mails = "";
                foreach (string nombre in usuarios_jefes_area)
                {
                    mails += new conexion().obtener_mail_usuario_por_nombre(nombre) + ",";
                   
                }
                mails = mails.TrimEnd(',');
                retorno = mails.Split(',');
            }
            
            return retorno;
        }
        public DateTime ultimo_aviso(string estado)
        {
            DateTime retorno = new conexion().ultimo_aviso(estado);
            return retorno;
        }
        public void actualizar_fecha_ultimo_aviso(DateTime fecha, string tipo_aviso)
        {
            new conexion().actualizar_fecha_ultimo_aviso(fecha,tipo_aviso);
        }
    }
}