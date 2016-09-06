using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Code;
using WebIzaje.Models;
using System.Text;
using System.Xml;
using System.Web.UI;

namespace WebIzaje.Controllers
{
    public class EquipoController : Controller
    {
        //
        // GET: /Equipo/
        // para validar errores eliminacion y guardados
        static int flag = -1;
       [HttpGet]
        public ActionResult Eliminar(string eliminar)
        {
            if ((Session["rol"] != null) && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                EquipoDelete eliminar_equipo = new EquipoDelete();
                EquipoCerticadosDelete eliminar_certificados = new EquipoCerticadosDelete();
                bool respuesta_equipo = eliminar_equipo.delete_equipo(eliminar);
                bool respuesta_certificado = eliminar_certificados.delete_certificados(eliminar);
                bool respuesta_directorio = deleteDiretory(eliminar);
                if ((respuesta_equipo == true) && (respuesta_certificado == true) && (respuesta_directorio == true))
                {
                    flag = 1;
                }
                else
                {
                    flag = 0;
                }
                return RedirectToAction("Todos");
            }
            else
            { return RedirectToAction("Index", "Home"); }
        }
        [HttpGet]
        public ActionResult Todos()
        {
            if ((Session["rol"] != null) && (Session["rol"].Equals("admin") || (Session["rol"].Equals("izaje"))))
            {
                ViewBag.success = flag;
                flag = -1;
                ListaEquipos datos = new ListaEquipos();
                return View(datos.mostrar_lista_datos());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult Nuevo()
        {
            if ( Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                // consultas a la bd para obtener los datos estaticos de las tablas
                ObtenerDatosTablasFijas datos_tablas_fijas = new ObtenerDatosTablasFijas();
                string[] estado = new string[] { "Disponible", "NoDisponible" };
                ViewData["familia_equipo"] = datos_tablas_fijas.obtener_datos_familia();
                //ViewData["marca"] = datos_tablas_fijas.obtener_datos_marca();
                ViewData["empresa"] = datos_tablas_fijas.obtener_datos_empresa();
                ViewData["area"] = datos_tablas_fijas.obtener_datos_area(); ;
                ViewData["estado"] = estado;
                return View();
            }
            else
            {
               return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult Editar(string tag)
        {
            if ((Session["rol"] != null) && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                // datos a obtener de la base de datos
                ObtenerDatosTablasFijas datos_tablas_fijas = new ObtenerDatosTablasFijas();
                string[] estado = new string[] { "Disponible", "NoDisponible" };
                ViewData["familia_equipo"] = datos_tablas_fijas.obtener_datos_familia();
                //ViewData["marca"] = datos_tablas_fijas.obtener_datos_marca();
                ViewData["empresa"] = datos_tablas_fijas.obtener_datos_empresa();
                ViewData["area"] = datos_tablas_fijas.obtener_datos_area(); ;
                ViewData["estado"] = estado;
                DatosEquipo datos_equipo = new DatosEquipo();
                EquipoSelect seleccionar_equipo = new EquipoSelect();
                datos_equipo = seleccionar_equipo.obtener_equipos(tag);
                if (datos_equipo.nocautivo.Equals("True"))
                {
                    EquipoNoCautivoSelect seleccionar_nocautivo = new EquipoNoCautivoSelect();
                    EquipoNoCautivo dato = new EquipoNoCautivo();
                    dato = seleccionar_nocautivo.obtener_Nocautivo(datos_equipo.tipo_equipo);
                    datos_equipo.datos_nocautivo = dato;
                }
                else
                {
                    EquipoCautivoSelect seleccionar_cautivo = new EquipoCautivoSelect();
                    EquipoCautivo dato = new EquipoCautivo();
                    dato = seleccionar_cautivo.obtener_cautivo(tag);
                    datos_equipo.datos_cautivo = dato;
                }
                EquipoCertificadoSelect seleccionar_certificados = new EquipoCertificadoSelect();
                List<EquipoCertificados> dato_certificado = new List<EquipoCertificados>();
                dato_certificado = seleccionar_certificados.lista_certificados(tag);
                datos_equipo.datos_certificados = dato_certificado;

                return View(datos_equipo);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
         }

      
       
        [HttpPost]
        // el httppostedfilebase permite pasar los documentos o imagenes con el unico requerimiento de que el name sea el mismo qe la variable de entrada
        public ActionResult guardarNuevo(FormCollection datos_form, HttpPostedFileBase imagen, IEnumerable<HttpPostedFileBase> file_)
        {
            // insertar estos datos en la Bd
            DatosEquipo datos_equipo = new DatosEquipo();
            EquipoInsert insertar_equipo = new EquipoInsert();
            EquipoCertificadosInsert insertar_certificado = new EquipoCertificadosInsert();
            
                
            // se crea la carpeta con el tag ingresado
                if (this.createFolder(datos_form["tag"]))
                {
                    //insertar Datos en la bd
                    datos_equipo=saveDatos(datos_form,imagen,"save");
                   bool retorno_guardar= insertar_equipo.savedatosequipo(datos_equipo);
                        //inserta certificados en bd
                    saveCertificados(datos_form, file_);
                    if (retorno_guardar == true)
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = 0;
                    }
                }

                return RedirectToAction("Todos");
        }
        [HttpPost]
        public ActionResult guardarEditar(FormCollection form, HttpPostedFileBase imagen, IEnumerable<HttpPostedFileBase> file_)
        {
            flag = 1;
            string id = form["tag"];//new
            string id_old = form["tagOld"];//old
            
            //elimina todos los certificados que se hallan eliminado en el formulario
            if (!form["clear_c"].Equals(""))
            {
                string[] clear_c = form["clear_c"].TrimEnd(';').Split(';');
                foreach (string x in clear_c)
                {
                    this.deleteFile(x);
                }
            }
            //si se realiza el cambio de nombre del directory
            if (renameFolder(id, id_old))
            {
                //update datos
                EquipoDelete eliminar_equipo = new EquipoDelete();
                if (eliminar_equipo.delete_equipo(id_old))
                {
                    EquipoInsert insertar_nuevo_equipo = new EquipoInsert();
                    if (!insertar_nuevo_equipo.savedatosequipo(saveDatos(form, imagen,"update"))) flag = 0;
                }
                else flag = 0;

                //delete BD
                EquipoCerticadosDelete eliminar_certificados = new EquipoCerticadosDelete();
                if (eliminar_certificados.delete_certificados(id_old))
                {
                    //Update Certificados
                    if (!saveCertificados(form, file_)) flag = 0;
                }
                else flag = 0;
            }
            else flag = 0;

            return RedirectToAction("Todos");
        }

        private DatosEquipo saveDatos(FormCollection datos_form, HttpPostedFileBase imagen,string tipo_accion)
        {
            string tag = datos_form["tag"];

            DatosEquipo datos_equipo = new DatosEquipo();
            EquipoCautivo equipocautivo = new EquipoCautivo();
            EquipoCautivoInsert equipocautivoInsert = new EquipoCautivoInsert();

            datos_equipo.familia_equipo = datos_form["familiaE"];
            datos_equipo.tipo_equipo = datos_form["tipoE"];
            datos_equipo.marca = datos_form["marcaE"];
            datos_equipo.modelo = datos_form["modeloE"];
            datos_equipo.año_fabricacion = datos_form["añof"];
            datos_equipo.capacidad = datos_form["capacidadE"];
            datos_equipo.estado = datos_form["EstadoE"];
            datos_equipo.empresa_propietaria = datos_form["empresaPE"];
            datos_equipo.tag = datos_form["tag"];

            if (datos_form["seleccion"].Equals("ECautivo"))
            {
                datos_equipo.cautivo = "True";
                datos_equipo.nocautivo = "False";
                equipocautivo.tag_equipo = datos_form["tag"];
                equipocautivo.area_trabajo = datos_form["areaT"];
                equipocautivo.costo_fijo = datos_form["costoFijo"];
                
                    equipocautivoInsert.saveEquipocautivo(equipocautivo);
                
            }
            else
            {
                datos_equipo.nocautivo = "True";
                datos_equipo.cautivo = "False";
            }
            string[] fecha_ingreso = datos_form["fechaIngreso"].Split('/');

            datos_equipo.fecha_ingreso_faena = fecha_ingreso[2] + "-" + fecha_ingreso[1] + "-" + fecha_ingreso[0];
            datos_equipo.odometro = datos_form["odometro"];
            datos_equipo.horas_horometro = datos_form["horasHO"];
            //agregar url al modelo?
            datos_equipo.url_imagen = saveImagen(datos_form, imagen, tag);

            return datos_equipo;
        }

        // metodo para guardar la imagen
        private string saveImagen(FormCollection form, HttpPostedFileBase imagen, string tag)
        {
            // se utliza para el editar
            if (!existSearch(form, "imagen_url"))
            {
                if (imagen != null)
                {
                    string url_imagen = this.uploadImagen(imagen, tag);
                    return url_imagen;
                }
            }
            else
            {
                if (imagen == null)
                {
                    if (!form["imagen_url"].Equals("-1"))
                    {
                        return form["imagen_url"];
                    }
                    else
                    {
                        string url_imagen = this.uploadImagen(imagen, tag);
                        return url_imagen;
                    }
                }
                else
                {
                    string url_imagen = this.uploadImagen(imagen, tag);
                    return url_imagen;
                }
            }
            return "-1";
        }

        private bool saveCertificados(FormCollection form, IEnumerable<HttpPostedFileBase> certificado)
        {
            List<EquipoCertificados> lista_certificados = new List<EquipoCertificados>();
            string id = form["tag"];
            
            // se crea la ruta para almacena archivo, ruta
            
            string[] certificado_nombres = { "Certificado Inspeccion","Certificado Accesorios", "Certificado Pluma"};
            //for (int i = 0; i < certificado_nombres.Length; i++)
            int i = 0;
            EquipoCertificadosInsert insert = new EquipoCertificadosInsert();
            foreach (HttpPostedFileBase file in certificado)
            {
                string url_l = upload(file, id + "/Certificados");
                EquipoCertificados certificados = new EquipoCertificados();
                certificados.tag_equipo = id;
                certificados.nombre_certificado = certificado_nombres[i];
                string[] fecha_certificados = form["fecha_" + i].Split('/');
                certificados.fecha_vencimiento = fecha_certificados[2]+"-"+fecha_certificados[1]+"-"+fecha_certificados[0];

                // se utiliza para el editar busca si existe la etiqueta en los datos que se pasan por form
                if (this.existSearch(form, "file_url"))
                {
                    
                    if (form["file_url" + i] != string.Empty)
                    {
                        certificados.url = form["file_url" + i];
                    }
                    else
                    {
                        certificados.url = url_l;
                    }
                }
                else
                {
                    certificados.url = url_l;
                }

                if (!insert.saveCertificados(certificados)) return false;
                i++;
            }
            return true;
        }

        private string upload(HttpPostedFileBase files, string ruta)
        {
            string cadena = string.Empty;
            
            if (files != null)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Document/" + ruta + "/"), fileName);
                files.SaveAs(path);
                cadena += "~/Document/" + ruta + "/" + fileName ;
            }
            else
            {
                cadena += "-1";
            }

            return cadena;
        }

        private string uploadImagen(HttpPostedFileBase file, string ruta)
        {
            string cadena = string.Empty;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Document/" + ruta + "/"), fileName);
                    file.SaveAs(path);
                    cadena = "~/Document/" + ruta + "/" + fileName;
                }
                else
                {
                    return "-1";
                }
            }
            else
            {
                return "-1";
            }
            return cadena;
        }

        private bool existSearch(FormCollection form, string search)
        {
            foreach (var x in form.AllKeys)
            {
                if (x.Contains(search))
                {
                    return true;
                }
            }
            return false;
        }
        
        // metodo el cual se encarga de crear la carpeta deacuerdo al tag
        private bool createFolder(string tag)
        {
            try
            {
                var path = Server.MapPath("~/Document/" + tag);
                if (!Directory.Exists(path))
                {
                    DirectoryInfo ruta = Directory.CreateDirectory(path);
                    DirectoryInfo rutaC = Directory.CreateDirectory(path + "/Certificados");
                   

                    return true;
                }
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            return false;
        }

        // metodo para descargar el archivo cuando se edita el equipo
        public GetFile DownloadFile(string url)
        {
            var path = Server.MapPath(url);
            string[] urls = url.Split('/');
            if (System.IO.File.Exists(path))
            {
                return new GetFile
                {
                    FileName = urls[4],
                    Path = url

                };
            }
            return null;
        }

        private bool diff(string itemA, string itemB)
        {
            if (itemA.Equals(itemB))
            {
                return false;
            }
            return true;
        }
        
        // metodo para eliminar los certificados desde el directorio
        private void deleteFile(string url)
        {
            var path = Server.MapPath(url);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        private bool renameFolder(string id, string id_old)
        {
            try
            {
                var path_old = Server.MapPath("~/Document/" + id_old);
                var path = Server.MapPath("~/Document/" + id);

                if (Directory.Exists(path_old))
                {
                    if (!id_old.Equals(id))
                    {
                        Directory.Move(path_old, path);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            return true;
        }
        
        //metodo para eliminar la carpeta del directorio
        private bool deleteDiretory(string id)
        {
            try
            {
                var path = Server.MapPath("~/Document/" + id + "/");
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path, true);
                }
                return true;
            }
            catch (DirectoryNotFoundException)
            {
            }
            return false;
        }
        #region metodos accedidos por ajax
        public string rescatar_datos_tipoequipo(string valor_seleccion)
        {
            string retorno = "";
            ObtenerDatosTablasFijas datos_tablas_fijas = new ObtenerDatosTablasFijas();
            string[] tipos_equipo = datos_tablas_fijas.obtener_datos_tipo_equipo(valor_seleccion);
            for (int i = 0; i < tipos_equipo.Length; i++)
            {
                retorno += tipos_equipo[i] + ",";
            }
            retorno = retorno.TrimEnd(',');
            return retorno;
        }
        public string rescatar_datos_modelo(string valor_seleccion, string valor_tipo)
            {
            string retorno = "";
            ObtenerDatosTablasFijas datos_tablas_fijas = new ObtenerDatosTablasFijas();
            string[] modelos = datos_tablas_fijas.obtener_datos_modelo(valor_seleccion,valor_tipo);
            for (int i = 0; i < modelos.Length; i++)
            {
                retorno += modelos[i] + ",";
            }
            retorno = retorno.TrimEnd(',');
            return retorno;
        }
        public string rescatar_datos_marcas(string valor_seleccion)
        {
            string retorno = "";
            ObtenerDatosTablasFijas datos_tablas_fijas = new ObtenerDatosTablasFijas();
            string[] marcas = datos_tablas_fijas.obtener_datos_marcas(valor_seleccion);
            for (int i = 0; i < marcas.Length; i++)
            {
                retorno += marcas[i] + ",";
            }
            retorno = retorno.TrimEnd(',');
            return retorno;
        }
        
        public string rescatar_Datos_nocautivo(string valor_seleccion)
        {
            // se debe consultar por el  tipo de equipo en bd y devolver costo hora costo hora extra y minimo garantizado
            string retorno = "";
            EquipoNoCautivo datos_equipo = new EquipoNoCautivo();
            EquipoNoCautivoSelect datos_nocautivo = new EquipoNoCautivoSelect();
            datos_equipo= datos_nocautivo.obtener_Nocautivo(valor_seleccion);
            string[] datos = new string[] { datos_equipo.costo_hora, datos_equipo.costo_hora_extra, datos_equipo.minimo_garantizado };
            for (int i = 0; i < datos.Length; i++)
            {
                retorno += datos[i] + ";";
            }
            return retorno;


        }

        public string comprobar_tag(string tag)
        {
            string retorno ="";
            DatosEquipo datos = new DatosEquipo();
            EquipoSelect seleccionar_equipo = new EquipoSelect();
            datos=seleccionar_equipo.obtener_equipos(tag);
            if (datos.tag != null)
            {
                retorno = "True";
            }
            else
            {
                retorno = "False";

            }
            return retorno;
        }
        #endregion
    }
}
