using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using WebIzaje.Code;

namespace WebIzaje.Models
{

        public class DatosEquipo
        {
            public string familia_equipo { get; set; }
            public string tipo_equipo { get; set; }
            public string marca { get; set; }
            public string modelo { get; set; }
            public string año_fabricacion { get; set; }
            public string capacidad { get; set; }
            public string estado { get; set; }
            public string empresa_propietaria { get; set; }
            public string tag { get; set; }
            public List<EquipoCertificados> datos_certificados{get;set;}
            public string cautivo { get; set; }
            public string nocautivo { get; set; }
            public EquipoCautivo datos_cautivo { get; set; }
            public EquipoNoCautivo datos_nocautivo { get; set; }
            public string fecha_ingreso_faena { get; set; }
            public string odometro { get; set; }
            public string horas_horometro { get; set; }
            public string url_imagen { get; set; }
        }
       
         public class EquipoCautivo
         {
             public string tag_equipo{get;set;}
             public string costo_fijo { get; set; }
             public string area_trabajo { get; set; }
         
         }

         #region seleccion y insercion equipo cautivo
         public class EquipoCautivoInsert
         {
             public void saveEquipocautivo(EquipoCautivo dato_cautivo)
             {
                 new conexion().guardar_Equipocautivo(dato_cautivo);
             }
             public void updateEquipocautivo(EquipoCautivo dato_cautivo)
             {
                 new conexion().update_Equipocautivo(dato_cautivo);
             }
             
         }
         public class EquipoCautivoSelect
         {
             public EquipoCautivo obtener_cautivo(string tag)
             {
                 EquipoCautivo datos=new EquipoCautivo();
                 datos=new conexion().obtener_datos_cautivo(tag);
                 return datos;
             }
             
         }
        #endregion
         public class EquipoNoCautivo
         {
             public string tag_equipo { get; set; }
             public string tipo_equipo { get; set; }
             public string costo_hora { get; set; }
             public string costo_hora_extra { get; set; }
             public string minimo_garantizado { get; set; }
         
         }

         #region seleccion  en tabla tipoequipo de equipos nocautivos
         public class EquipoNoCautivoSelect
         {
             public EquipoNoCautivo obtener_Nocautivo(string tipo_equipo)
             {
                 EquipoNoCautivo datos = new EquipoNoCautivo();
                 datos = new conexion().obtener_datos_Nocautivo(tipo_equipo);
                 return datos;
             }
             
         }
         
        #endregion
         #region seleccion y inserccion de certificados
         public class EquipoCertificados
         {
             public string tag_equipo { get; set; }
             public string fecha_vencimiento { get; set; }
             public string nombre_certificado { get; set; }
             public string url { get; set; }

         }
         public class EquipoCertificadosInsert
         {
             public bool saveCertificados(EquipoCertificados lista_certificados)
             {
                 return new conexion().guardar_certificados(lista_certificados);

             }
            
         }
         public class EquipoCerticadosDelete
         {
             public bool delete_certificados(string tag)
             {
                 return new conexion().eliminar_certificados(tag);
             }
            
         }
         public class EquipoDelete
         {
             public bool delete_equipo(string tag)
             {
               return  new conexion().eliminar_equipo(tag);
             }
            
         }
         public class EquipoCertificadoSelect
         {
             public List<EquipoCertificados> lista_certificados(string tag)
             {
                 List<EquipoCertificados> lista_datos = new List<EquipoCertificados>();
                 lista_datos = new conexion().obtener_certificados(tag);
                 return lista_datos;
             }
           
         }
         #endregion
         public class EquipoInsert
         {

             public bool savedatosequipo(DatosEquipo equipo)
             {
                 return new conexion().guardar_datosequipo(equipo);
             }
           

         }
         public class EquipoSelect
         {
             public DatosEquipo obtener_equipos(string tag)
             {
                 DatosEquipo datos= new DatosEquipo();
                 datos=new conexion().obtener_datos_equipo(tag);
                 return datos;
             }
             


         }
         public class ListaEquipos
         {
             #region metodo para mostrar lista de equipos
             public List<DatosEquipo> mostrar_lista_datos()
             {
                 // se consulta en la bd por todos los equipos disponibles
                 List<DatosEquipo> lista_datos = new List<DatosEquipo>();
                 lista_datos = new conexion().obtener_lista_equipos();
                 return lista_datos;
             }
             
             #endregion
         }         
        public class ObtenerDatosTablasFijas
        {

            #region datos que se obtien de tablas sin pasar parametros
            public string[] obtener_datos_familia()
            {
                return new conexion().datos_familia();
            }
            
             public string[] obtener_datos_marca()
            {
                return new conexion().datos_marca();
            }
             
             public string[] obtener_datos_empresa()
             {
                 return new conexion().datos_empresa();
             }
            
             public string[] obtener_datos_area()
             {
                 return new conexion().datos_area();
             }
             
            #endregion

             #region datos que se obtiene pasando parmetros
             public string[] obtener_datos_tipo_equipo(string familia)
             {
                 return new conexion().datos_tipo_equipo(familia);
             }
             
             public string[] obtener_datos_modelo(string marca, string tipo)
             {
                 return new conexion().datos_modelo(marca,tipo);
             }

             public string[] obtener_datos_marcas(string tipo)
             {
                 return new conexion().datos_marcas(tipo);
             }
            
             #endregion
        }
        public class MetodosReportesEquipo
        {
            public List<DatosEquipo> mostrar_lista_datos_por_fecha(string fechaingreso)
            {
                // se consulta en la bd por todos los equipos disponibles
                List<DatosEquipo> lista_datos = new List<DatosEquipo>();
                lista_datos = new conexion().obtener_equipos_por_fecha(fechaingreso);
                return lista_datos;
            }
        }
}
    
