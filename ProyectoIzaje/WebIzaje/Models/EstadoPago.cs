using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace WebIzaje.Models
{
    public class Servicios
    {
        public string idSolicitud { get; set; }
        public string tag_equipo {get;set; }
        public DateTime fecha{get;set;}
        public string hora_inicio_reloj{get;set;}
        public string hora_fin_reloj { get; set; }
        public string hora_inicio_horometro { get; set; }
        public string hora_fin_horometro { get; set; }
        public string rut_operador { get; set; }
        public string nombre_area { get; set; }
        public string jefe_area { get; set; }
        public string codigo_centro_costo { get; set; }
        public string tipo_equipo { get; set; }
        public string empresa_propietaria { get; set; }
    }
    public class GetServicios
    {
        public List<Servicios> todos_servicios_entrefechas(string fecha_inicio, string fecha_final)
        {
            Servicios servicios = new Servicios();
            List<Servicios> lista_servicios = new conexion().obtener_Servicios_entrefecha(fecha_inicio, fecha_final);
            // se obtienen de la bd todos los servicios con la consulta entre fechas

            //se almacenan en el objeto servicio luego enla lista
            return lista_servicios;
        }
        public List<Servicios> todos_servicios_alafecha(string fecha_final,string tipo_equipo)
        {
            Servicios servicios = new Servicios();
            List<Servicios> lista_servicios = new conexion().obtener_Servicios_alafecha(fecha_final,tipo_equipo);
            // se obtienen de la bd todos los servicios
            //se almacenan en el objeto servicio luego enla lista
            // llamar ala clase de conexion

            return lista_servicios;
        }        
       
    }    
    public class DetalleEstadoPago
    {
        public string idEstadoPago { get; set; }
        public string idEstadoPagoTipo { get; set; }
        public string idSolicitud { get; set; }
        public DateTime fecha { get; set; }
        public string operador { get; set; }
        public string tagequipo { get; set; }
        public string tipoequipo { get; set; }
        public string area { get; set; }
        public string responsable { get; set; }
        public string empresa { get; set; }
        public string centro_costo { get; set; }
        public string i_horometro { get; set; }
        public string f_horometro { get; set; }
        public string d_horometro { get; set; }
        public string i_reloj { get; set; }
        public string f_reloj { get; set; }
        public string d_reloj { get; set; }
        public string contador_deltareloj { get; set; }
        public string valor_dia { get; set; }
        public string valor_mlp { get; set; }
        public string valor_distribucion { get; set; }
        public string salto_horometro { get; set; }

    }
    public class DetalleEstadoPagoInsert
    {
        public void savedetalleestadopago(List<DetalleEstadoPago> lista_detalles)
        {
            //Insertar en la Bd
            for(int i=0;i<lista_detalles.Count;i++)
                new conexion().guardardetalleestadopago(lista_detalles[i]);
            
        }      
    }
    public class DetalleEstadoPagoSelect
    {
        public List<DetalleEstadoPago> listadetallesestadopago(string id)
        {
            return (new conexion().obtenerdetalleestadopago(id));
        }        
    }    
        
   // estos son los estados de pago generados a modo de resumen para poder mostrarlos en el show y luego en base a id acceder a todo el detalle 
       public class ResumenEstadoPago
        {
            public string id_estadopago { get; set; }
            public string id_estadopagotipo { get; set; }
            public string fecha_generado { get; set; }
            public string tipo_equipo { get; set; }
            public string uf { get; set; }
            public string total_valor_dia { get; set; }
            public string total_valor_mlp { get; set; }
            public string costo_hora { get; set; }
            public string total_valor_distribucion { get; set; }
            public string total_delta_horometro { get; set; }
            public string total_delta_reloj { get; set; }
            public string total_contador { get; set; }
            public string total_saltos_horometro { get; set; }
       }       
     public class ResumenestadopagoInsert
     {
            public void saveresumen(ResumenEstadoPago resumenestadopago)
            {
                new conexion().guardarresumenestadopago(resumenestadopago);
            }

      }
     public class ResumenestadopagoSelect
     {
         public List<ResumenEstadoPago> obtener_resumen_estadopago()
         {
             List<ResumenEstadoPago> lista_resumen = new conexion().obtener_resumenes();
             return lista_resumen;
         }
         public ResumenEstadoPago obtener_resumen_estadopago_id(string id)
         {
             ResumenEstadoPago resumen_por_id = new conexion().obtener_resumenes_id(id);
             return resumen_por_id;
         }
         public string[] obtener_id_estado_pago_generales()
         {
             return new conexion().obtener_id_estado_pago_general();
         }
         public string obtener_id_tipo_por_id_general(string id_general)
         {
             return new conexion().obtener_id_tipo_por_id_general(id_general);
         }
     }
     public class TiposEquipos
     { 
        public string subt_tipo{get; set;}
        public string familia { get; set; }
        public double minimo_garantizado { get; set; }
        public double costo_hora_equipo { get; set; }
        public double hora_extra { get; set; }
            
     }
     public class TipoEquipoSelect
     {

        public List<TiposEquipos> lista_tipos_equipo()
         {
             return (new conexion().obtener_tipo_equipo());
         }
     }
     public class actualizarCalculado 
     {
         public void actualizarCalculadoEnDatosFin(string id) 
         {
             new conexion().cambiarCalculadoDatosFin(id);
         }
     }
     public class comprobarCautivo
     {
         public bool comprobar(string tag_equipo)
         {
             bool retorno = new conexion().comprobar_cautivo(tag_equipo);
             return retorno;
         }
     }
     public class DatosRigger
     {
         
         public string rut_rigger1 { get; set; }
         public string rut_rigger2 { get; set; }
         public DateTime fecha_inicio { get; set; }
         public DateTime fecha_termino { get; set; }
         

         public DatosRigger obtener_datos_rigger(string idsolicitud)
         {
             DatosRigger datos = new conexion().obtener_datos_rigger(idsolicitud);
             return datos;
         }
     }
     public class RiggerDiasTrabajados
     {
         public string id_estado_pago { get; set; }
         public string id_estado_pago_tipo { get; set; }
         public string id_solicitud { get; set; }
         public string nombre_rigger { get; set; }
         public string rut_rigger { get; set; }
         public DateTime fecha_inicio { get; set; }
         public DateTime fecha_fin { get; set; }
          public int dias_trabajados { get; set; }
         
         
         //metodos
        
         public void insertar_datos_rigger(RiggerDiasTrabajados datos)
         {
             new conexion().guardar_datos_rigger(datos);
         }
        
        
         public string obtener_nombre_rigger(string rut)
         {
             string nombre = new conexion().obtener_nombre_rigger(rut);
             return nombre;
         }
         public bool comprobar_rigger_idsolicitud(string rut_rigger,string id_solicitud)
         {
             bool retorno = new conexion().comprobar_rigger_id_solicitud(rut_rigger,id_solicitud);
             return retorno;
         }
         public List<RiggerDiasTrabajados> obtene_riggers_por_idestadopago(string id_estado_pago)
         {
             List<RiggerDiasTrabajados> datos = new List<RiggerDiasTrabajados>();
             datos = new conexion().obtener_rigger_por_estadopago(id_estado_pago);
             return datos;
         }

         public List<RiggerDiasTrabajados> obtener_rigger_id_pago_general(string id_general)
         {
             List<RiggerDiasTrabajados> lista = new conexion().obtener_rigger_id_general(id_general);
             return lista;
         }
     }
     public class RiggerTotales
     {
         public string rut_rigger { get; set; }
         public string nombre_rigger { get; set; }
         public int total_dias_trabajados { get; set; }

         public List<RiggerTotales> calcular_totales(string id_estado_pago)
         {
             List<RiggerTotales> totales_rigger = new List<RiggerTotales>();
             totales_rigger = new conexion().calculo_totales_rigger(id_estado_pago);
             return totales_rigger;
         }

         public List<RiggerTotales> calcular_totales_ala_fecha(DateTime fechatope)
         {
             List<RiggerTotales> lista = new conexion().calcular_totales_rigger_ala_fecha(fechatope);
                 return lista;

         }
         public List<RiggerTotales> calcular_totales_id_general(string id_general)
         {
             List<RiggerTotales> lista = new conexion().calcular_totales_rigger_id_general(id_general);
             return lista;

         }
     }
  
}