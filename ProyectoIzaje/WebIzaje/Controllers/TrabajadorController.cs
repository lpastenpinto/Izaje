using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;
using WebIzaje.Code;
using System.IO;
using WebIzaje.Controllers;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Hosting;


namespace WebIzaje.Controllers
{
    public class TrabajadorController : Controller
    {
        //
        // GET: /Trabajador/
        static int flag = -1;

        [HttpGet]
        public ActionResult Nuevo()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                flag = -1;
                return View();
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public ActionResult Editar(string id)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                flag = -1;

                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    TrabajadorGet getTrabajador = new TrabajadorGet();
                    TrabajadorDatos tdato = getTrabajador.trabajador(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return RedirectToAction("Todos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Todos()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                //recuperar all trabajadores
                TrabajadorGet tget = new TrabajadorGet();
                List<TrabajadorDatos> tdatos = tget.AllDatos();

                ViewBag.success = flag;
                flag = -1;
                return View(tdatos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        [HttpPost]
        public ActionResult Guardar(FormCollection form, HttpPostedFileBase imagen, IEnumerable<HttpPostedFileBase> lnew_file, IEnumerable<HttpPostedFileBase> cnew_file, IEnumerable<HttpPostedFileBase> enew_file)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                flag = 1;
                string id = form["rut"];
                //si no existe el ID
                if (!new TrabajadorGet().existRut(id))
                {
                    //si se crea el directorio
                    if (this.createFolder(id))
                    {
                        //insertar Datos
                        if (new TrabajadorInsert().datos(this.saveDatos(form, imagen)))
                        {
                            //insertar licencias
                            foreach (TrabajadorLicencias item in saveLicencias(form, lnew_file))
                            {
                                if (!new TrabajadorInsert().licencias(item)) flag = 0;
                            }
                            //insertar certificados
                            foreach (TrabajadorCertificados item in saveCertificados(form, cnew_file))
                            {
                                if (!new TrabajadorInsert().certificados(item)) flag = 0;
                            }

                            //insertar equipos
                            foreach (TrabajadorEquipos item in saveEquipos(form, enew_file))
                            {
                                if (!new TrabajadorInsert().equipos(item)) flag = 0;
                            }

                            if (flag == 0)
                            {
                                this.Delete(id);
                            }
                        }
                        else { flag = 0; }
                    }
                    else { flag = 0; }
                }
                else { flag = 2; }//Si ya existe el Rut 

                return RedirectToAction("Todos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        [HttpPost]
        public ActionResult GuardarEditar(FormCollection form, HttpPostedFileBase imagen, IEnumerable<HttpPostedFileBase> lnew_file, IEnumerable<HttpPostedFileBase> cnew_file, IEnumerable<HttpPostedFileBase> enew_file)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                flag = 1;
                string id = form["rut"];//new
                string id_old = form["rutOld"];//old

                bool existId = false;
                //si existen diferencias de ID
                if (diff(id, id_old))
                {
                    //verificar existencia de ID
                    existId = new TrabajadorGet().existRut(id);
                }

                if (!existId)
                {
                    //Delete Licencia
                    if (!string.IsNullOrEmpty(form["clear_l"]))
                    {
                        string[] clear_l = form["clear_l"].TrimEnd(';').Split(';');
                        //eliminar los files
                        foreach (string x in clear_l)
                        {
                            this.deleteFile(x);
                        }
                    }

                    //Delete Certificados
                    if (!string.IsNullOrEmpty(form["clear_c"]))
                    {
                        string[] clear_c = form["clear_c"].TrimEnd(';').Split(';');
                        foreach (string x in clear_c)
                        {
                            this.deleteFile(x);
                        }
                    }

                    //Delete Equipos
                    if (!string.IsNullOrEmpty(form["clear_e"]))
                    {
                        string[] clear_e = form["clear_e"].TrimEnd(';').Split(';');
                        foreach (string x in clear_e)
                        {
                            this.deleteFile(x);
                        }
                    }

                    //si se realiza el cambio de nombre del directory
                    if (renameFolder(id, id_old))
                    {
                        //update datos
                        if (new TrabajadorDelete().datos(id_old))
                        {
                            if (new TrabajadorInsert().datos(saveDatos(form, imagen)))
                            {
                                //delete BD
                                if (new TrabajadorDelete().licencias(id_old))
                                {
                                    //Update Licencias
                                    foreach (TrabajadorLicencias licencia in saveLicencias(form, lnew_file))
                                    {
                                        if (!new TrabajadorInsert().licencias(licencia)) flag = 0;
                                    }
                                }
                                else flag = 0;

                                //delete BD
                                if (new TrabajadorDelete().certificados(id_old))
                                {
                                    //Update Certificados
                                    foreach (TrabajadorCertificados certificado in saveCertificados(form, cnew_file))
                                    {
                                        if (!new TrabajadorInsert().certificados(certificado)) flag = 0;
                                    }
                                }
                                else flag = 0;

                                //delete BD
                                if (new TrabajadorDelete().equipos(id_old))
                                {
                                    //Update Equipos
                                    foreach (TrabajadorEquipos equipo in saveEquipos(form, enew_file))
                                    {
                                        if (!new TrabajadorInsert().equipos(equipo)) flag = 0;
                                    }
                                }
                                else flag = 0;
                            }
                            else flag = 0;
                        }
                        else flag = 0;

                    }
                    else flag = 0;
                }
                else flag = 2;//Si ya existe el Rut

                return RedirectToAction("Todos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                flag = 1;
                if (this.deleteDiretory(id))
                {
                    if (!new TrabajadorDelete().equipos(id)) flag = 0;
                    if (!new TrabajadorDelete().certificados(id)) flag = 0;
                    if (!new TrabajadorDelete().licencias(id)) flag = 0;
                    if (!new TrabajadorDelete().datos(id)) flag = 0;
                }
                else flag = 0;

                return RedirectToAction("Todos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private TrabajadorDatos saveDatos(FormCollection form, HttpPostedFileBase imagen){
            string id = form["rut"];
            TrabajadorDatos trabajador = new TrabajadorDatos();
            trabajador.rut = id;
            trabajador.nombre = form["nombre"];
            trabajador.apellidoP = form["apellidoP"];
            trabajador.apellidoM = form["apellidoM"];
            trabajador.fono = form["fono"];
            trabajador.email = form["email"];
            trabajador.direccion = form["direccion"];
            trabajador.rol = form["rol"];
            trabajador.estado = form["estado"];
            trabajador.url = saveImagen(form, imagen, id);

            return trabajador;
        }

        private string saveImagen(FormCollection form, HttpPostedFileBase imagen, string id)
        {
            if (!existSearch(form, "imagen_url"))
            {
                if (imagen != null)
                {
                    string url_imagen = this.uploadImagen(imagen, id + "/");
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
                        string url_imagen = this.uploadImagen(imagen, id + "/");
                        return url_imagen;
                    }
                } 
                else
                {
                    string url_imagen = this.uploadImagen(imagen, id + "/");
                    return url_imagen;
                }
            }
            return "-1";
        }

        private List<TrabajadorLicencias> saveLicencias(FormCollection form, IEnumerable<HttpPostedFileBase> lnew_file)
        {
            List<TrabajadorLicencias> licencias = new List<TrabajadorLicencias>();
            string id = form["rut"];
            string[] url_l = this.upload(lnew_file, id + "/Licencias");
            string[] licencia_nombres = { "Examen Psicosensometrico", "Curso Manejo a la Defensa", "Licencia Municipal", "Hoja de Vida del Conductor", "Examen Preocupacional", "Inducción" };
            for (int i = 0; i < licencia_nombres.Length; i++)
            {
                TrabajadorLicencias licencia = new TrabajadorLicencias();
                licencia.id = id;
                licencia.nombre = licencia_nombres[i];
                if (form["lnew_fecha" + i].Equals(""))
                {
                    licencia.fecha = null;
                }
                else
                {
                    string[] fecha = form["lnew_fecha" + i].Split('/');
                    DateTime date = new DateTime(int.Parse(fecha[2]), int.Parse(fecha[1]), int.Parse(fecha[0]));
                    licencia.fecha = date.ToShortDateString();
                }
               
                
                if (this.existSearch(form,"lnew_url"))
                {
                    if (!string.IsNullOrEmpty(form["lnew_url" + i] ))
                    {
                        licencia.url = form["lnew_url" + i];
                    }
                    else
                    {
                        licencia.url = url_l[i];
                    }
                }
                else
                {
                    licencia.url = url_l[i];
                }

                licencias.Add(licencia);
            }

            return licencias;
        }

        private List<TrabajadorCertificados> saveCertificados(FormCollection form, IEnumerable<HttpPostedFileBase> cnew_file)
        {
            List<TrabajadorCertificados> certificados = new List<TrabajadorCertificados>();
            string id = form["rut"];
            string[] url_c = this.upload(cnew_file, id + "/Certificados");
            string[] index_c = this.indexGet(form, "cnew_fecha");
            for (int i = 0; i < url_c.Length; i++)
            {
                TrabajadorCertificados certificado = new TrabajadorCertificados();
                certificado.id = id;
                certificado.nombre = form["cnew_name" + Convert.ToInt32(index_c[i])];
                //certificado.fecha = form["cnew_fecha" + Convert.ToInt32(index_c[i])];
                if (form["cnew_fecha" + Convert.ToInt32(index_c[i])].Equals(""))
                {
                    certificado.fecha = null;
                }
                else
                {
                    string[] fecha = form["cnew_fecha" + Convert.ToInt32(index_c[i])].Split('/');
                    DateTime date = new DateTime(int.Parse(fecha[2]), int.Parse(fecha[1]), int.Parse(fecha[0]));
                    certificado.fecha = date.ToShortDateString();
                }

                if (this.existSearch(form, "cnew_url"))
                {
                    if (!string.IsNullOrEmpty(form["cnew_url" + Convert.ToInt32(index_c[i])]))
                    {
                        certificado.url = form["cnew_url" + Convert.ToInt32(index_c[i])];
                    }
                    else
                    {
                        certificado.url = url_c[i];
                    }
                }
                else
                {
                    certificado.url = url_c[i];
                }
                certificados.Add(certificado);
            }
            return certificados;
        }

        private List<TrabajadorEquipos> saveEquipos(FormCollection form, IEnumerable<HttpPostedFileBase> enew_file)
        {
            List<TrabajadorEquipos> equipos = new List<TrabajadorEquipos>();
            string id = form["rut"];
            string[] url_e = this.upload(enew_file, id + "/Equipos");
            string[] index_e = this.indexGet(form, "enew_fecha");
            for (int i = 0; i < url_e.Length; i++)
            {
                TrabajadorEquipos equipo = new TrabajadorEquipos();
                equipo.id = id;
                equipo.marca = form["enew_marca" + Convert.ToInt32(index_e[i])];
                equipo.modelo = form["enew_modelo" + Convert.ToInt32(index_e[i])];
                equipo.capacidad = form["enew_capacidad" + Convert.ToInt32(index_e[i])];
                //equipo.fecha = form["enew_fecha" + Convert.ToInt32(index_e[i])];
                if (form["enew_fecha" + Convert.ToInt32(index_e[i])].Equals(""))
                {
                    equipo.fecha = null;
                }
                else
                {
                    string[] fecha = form["enew_fecha" + Convert.ToInt32(index_e[i])].Split('/');
                    DateTime date = new DateTime(int.Parse(fecha[2]), int.Parse(fecha[1]), int.Parse(fecha[0]));
                    equipo.fecha = date.ToShortDateString();
                }

                if (this.existSearch(form, "enew_url"))
                {
                    if (!string.IsNullOrEmpty(form["enew_url" + Convert.ToInt32(index_e[i])]))
                    {
                        equipo.url = form["enew_url" + Convert.ToInt32(index_e[i])];
                    }
                    else
                    {
                        equipo.url = url_e[i];
                    }
                }
                else
                {
                    equipo.url = url_e[i];
                }
                equipos.Add(equipo);
            }
            return equipos;
        }

        private string[] upload(IEnumerable<HttpPostedFileBase> files, string ruta)
        {
            string cadena = string.Empty;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Document/" + ruta + "/"), fileName);
                        file.SaveAs(path);
                        cadena += "~/Document/" + ruta + "/"+ fileName + ";";
                    }
                    else
                    {
                        cadena += "-1;";
                    }
                }
                string item = cadena.TrimEnd(';');
                return item.Split(';');
            }
            return new string[0];
            
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

        private string[] indexGet(FormCollection form, string nombre)
        {
            string index = string.Empty;
            foreach (var x in form.AllKeys)
            {
                if (x.Contains(nombre))
                {
                    string num = x.Replace(nombre, "");
                    index += num + ",";
                }
            }
            if (string.IsNullOrEmpty(index))
            {
                return new string[0];
            }
            index = index.TrimEnd(',');
            return index.Split(',');
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

        private bool createFolder(string id){
            try
            {
                var path = Server.MapPath("~/Document/" + id);
                if (!Directory.Exists(path))
                {
                    DirectoryInfo ruta = Directory.CreateDirectory(path);
                    DirectoryInfo rutaL = Directory.CreateDirectory(path + "/Licencias");
                    DirectoryInfo rutaC = Directory.CreateDirectory(path + "/Certificados");
                    DirectoryInfo rutaE = Directory.CreateDirectory(path + "/Equipos");

                    return true;
                }
            }catch(DirectoryNotFoundException ){
                return false;
            }
            return false;
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

        private bool deleteFile(string url)
        {
            try
            {
                var path = Server.MapPath(url);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return true;
            }
            catch (FileLoadException)
            { }
            return false;
        }

        private bool deleteDiretory(string id)
        {
            try
            {
                var path = Server.MapPath("~/Document/" + id +"/");
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path,true);
                }
                return true;
            }catch(DirectoryNotFoundException){
            }
            return false;
        }

        public string GetMarca()
        {
            string marca = string.Empty;
            foreach( string x in new conexion().getMarcas())
            {
                marca += x + ",";
            }
            marca = marca.TrimEnd(',');
            return marca;
        }
        public string GetModelo(string marca){
            string modelo = string.Empty;
            foreach (string x in new conexion().getModelos(marca))
            {
                modelo += x + ",";
            }
            modelo = modelo.TrimEnd(',');
            return modelo;
        }
        public string GetCapacidad(string marca, string modelo)
        {
            string capacidad = string.Empty;
            foreach (string x in new conexion().getCapacidad(marca, modelo))
            {
                capacidad += x + ",";
            }
            capacidad = capacidad.TrimEnd(',');
            return capacidad;
        }
        public string GetCertificado()
        {
            string certificado = string.Empty;
            foreach (Certificacion x in new Certificacion().GetCertificacion())
            {
                certificado += x.nombre + ",";
            }
            certificado = certificado.TrimEnd(',');
            return certificado;
        }

        public bool rutValidar(string rut){
            try
            {
                if ((rut.Length == 10 || rut.Length == 9) && rut.Contains("-"))
                {
                    string[] ruts = rut.Split('-');
                    if (ruts[1].Length == 1 && ruts[0].Length > 0)
                    {
                        int contador = 2;
                        int suma = 0;
                        string[] array = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "k", "K" };
                        if ((ruts[0].Length == 8 || ruts[0].Length == 7) && (array.Contains(ruts[1])))
                        {
                            //verificar existencia de ID
                            if (new TrabajadorGet().existRut(rut))
                            {
                                return false;
                            }

                            for (int i = (ruts[0].Length - 1); i >= 0; i--)
                            {
                                string rutNum = ruts[0];
                                string dig = rutNum.Substring(i, 1);
                                int digito = Convert.ToInt32(dig);
                                suma = suma + (digito * contador);

                                contador++;
                                if (contador == 8) contador = 2;
                            }
                        }
                        else { return false; }
                        suma = suma % 11;
                        suma = 11 - suma;

                        if (suma == 11)
                        {
                            suma = 0;
                        }

                        //Set Digito Verificador
                        int verificador = 0;
                        if (ruts[1].Equals("k") || ruts[1].Equals("K"))
                        {
                            verificador = 10;
                        }
                        else
                        {
                            verificador = Convert.ToInt32(ruts[1]);
                        }

                        //Match Digito Verificador con Resto de Division
                        if (suma == verificador)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return false; }
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
    }
}
