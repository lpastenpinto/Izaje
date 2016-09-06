using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class AdminController : Controller
    {
        static int flag = -1;

        #region Gerencias
        public ActionResult gerencias()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(gerencia.obtenerGerencias());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult agregarGerencia()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult editarGerencia(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    gerencia tdato = gerencia.obtenerGerencias(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return RedirectToAction("gerencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarGerencia(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                gerencia item = new gerencia();
                item.nombre = form["nombre"];
                item.descripcion = form["descripcion"];

                if (gerencia.obtenerGerencias(item.nombre).nombre == null)
                {
                    if (! item.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("gerencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult AgregarEditarGerencia(FormCollection form)
        {

            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                gerencia item = new gerencia();
                string id = form["nombre"];
                item.nombre = form["nombre_nuevo"];
                item.descripcion = form["descripcion"];

                if (id.Equals(item.nombre))
                {
                    if (!item.update(id, item.descripcion))
                    {
                        flag = 0;
                    }
                }
                else
                {
                    if (gerencia.obtenerGerencias(item.nombre).nombre == null)
                    {
                        if (!item.verificarExist(id))
                        {
                            if (item.eliminar(id))
                            {
                                if (!item.agregarBD())
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            if (!item.update(id, item.descripcion))
                            {
                                flag = 0;
                            }
                            else { flag = 2; }
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }

                return RedirectToAction("gerencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult eliminarGerencia(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new gerencia().verificarExist(id))
                {
                    if (!new gerencia().eliminar(id))
                    {
                        flag = 0;
                    }
                }
                else
                {
                    flag = 2;
                }
                return RedirectToAction("gerencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Superintendencias
        public ActionResult superintendencias()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(superintendencia.obtenerSuperintendencias());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult agregarSuperintendencia()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                ViewData["gerencias"] = new conexion().obtenerGerencias();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult editarSuperintendencia(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    superintendencia tdato = superintendencia.obtenerSuperintendencias(id);
                    if (tdato.nombre != null)
                    {
                        ViewBag.gerencias = new conexion().obtenerGerencias();
                        return View(tdato);
                    }
                }
                return RedirectToAction("gerencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarSuperintendencia(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                superintendencia item = new superintendencia();
                item.nombre = form["nombre"];
                item.descripcion = form["descripcion"];
                item.gerencia = form["gerencia"];

                if (superintendencia.obtenerSuperintendencias(item.nombre).nombre == null)
                {
                    if (!item.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("superintendencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarEditarSuperintendencia(FormCollection form)
        {

            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                superintendencia item = new superintendencia();
                string id = form["nombre"];
                item.nombre = form["nombre_nuevo"];
                item.descripcion = form["descripcion"];
                item.gerencia = form["gerencia"];

                if (id.Equals(item.nombre))
                {
                    if (!item.update(id,item.descripcion,item.gerencia))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (superintendencia.obtenerSuperintendencias(item.nombre).nombre == null)
                    {
                        if(!item.verificarSuperIntendencia(id)){
                            if (item.eliminar(id))
                            {
                                if (!item.agregarBD())
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            if (!item.update(id, item.descripcion,item.gerencia))
                            {
                                flag = 0;
                            }
                            else { flag = 2; }
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("superintendencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult eliminarSuperintendencia(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new superintendencia().verificarSuperIntendencia(id))
                {
                    if (!new superintendencia().eliminar(id))
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("superintendencias");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Areas
        public ActionResult areas()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(area.obtenerAreas());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult agregarArea()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                ViewData["superintendencias"] = new conexion().obtenerListaSuperintendencias();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult editarArea(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    area tdato = area.obtenerAreas(id);
                    if (tdato.nombre != null)
                    {
                        ViewBag.superintendencias = new conexion().obtenerListaSuperintendencias();
                        return View(tdato);
                    }
                }
                return RedirectToAction("areas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarArea(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                area item = new area();
                item.nombre = form["nombre"];
                item.descripcion = form["descripcion"];
                item.superintendencia = form["superintendencia"];

                if (superintendencia.obtenerSuperintendencias(item.nombre).nombre == null)
                {
                    if (!item.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("areas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarEditarArea(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                area item = new area();
                string id = form["nombre"];
                item.nombre = form["nombre_nuevo"];
                item.descripcion = form["descripcion"];
                item.superintendencia = form["superintendencia"];

                if (id.Equals(item.nombre))
                {
                    if (!item.update(id,item.descripcion,item.superintendencia))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (area.obtenerAreas(item.nombre).nombre == null)
                    {
                        if (!item.verificarArea(id))
                        {
                            if (item.eliminar(id))
                            {
                                if (!item.agregarBD())
                                    flag = 0;
                            }
                            else
                            {
                                flag = 2;
                            }
                        }
                        else
                        {
                            if (!item.update(id, item.descripcion, item.superintendencia))
                            {
                                flag = 0;
                            }
                            else
                            {
                                flag = 2;
                            }
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("areas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult eliminarArea(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {

                flag = 1;
                if (!new area().verificarArea(id))
                {
                    if (!new area().eliminar(id))
                        flag = 0;
                }
                else
                {
                    flag = 2;
                }

                return RedirectToAction("areas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }        
        #endregion

        #region Centro de costo
        public ActionResult centrosCosto()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(centro_costo.obtenerCentrosCosto());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult agregarCentroCosto()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                ViewData["centrosCosto"] = new conexion().obtenerListaAreas();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult editarCentroCosto(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    centro_costo tdato = centro_costo.obtenerCentrosCosto(id);
                    if (tdato.nombre != null)
                    {
                        ViewBag.centrosCosto = new conexion().obtenerListaAreas();
                        return View(tdato);
                    }
                }
                return RedirectToAction("centrosCosto");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarCentroCosto(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                centro_costo item = new centro_costo();
                item.nombre = form["nombre"];
                item.codigo = form["codigo"];
                item.area = form["area"];

                if (centro_costo.obtenerCentrosCosto(item.codigo).codigo == null)
                {
                    if (!item.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("centrosCosto");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarEditarCentroCosto(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                centro_costo item = new centro_costo();
                string id = form["nombre"];
                item.nombre = form["nombre_nuevo"];
                item.codigo = form["codigo"];
                item.area = form["area"];

                if (id.Equals(item.codigo))
                {
                    if (!item.update(id, item.nombre, item.area))
                    {
                        flag = 0;
                    }
                }
                else
                {
                    if (centro_costo.obtenerCentrosCosto(item.codigo).codigo == null)
                    {
                        if (!item.verificarCentro(id))
                        {
                            if (item.eliminar(id))
                            {
                                if (!item.agregarBD())
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else { flag = 2; }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("centrosCosto");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult eliminarCentroCosto(string id)
        {

            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new centro_costo().verificarCentro(id))
                {
                    if (!new centro_costo().eliminar(id))
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("centrosCosto");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Criticidad
        public ActionResult criticidades()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(criticidad.obtenerCriticidades());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult agregarCriticidad()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult editarCriticidad(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    criticidad tdato = criticidad.obtenerCriticidades(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return RedirectToAction("criticidades");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarCriticidad(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                criticidad item = new criticidad();
                item.nombre = form["nombre"];
                item.descripcion = form["descripcion"];

                if (criticidad.obtenerCriticidades(item.nombre).nombre == null)
                {
                    if (!item.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("criticidades");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarEditarCriticidad(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                criticidad item = new criticidad();
                string id = form["nombre"];
                item.nombre = form["nombre_nuevo"];
                item.descripcion = form["descripcion"];

                if (id.Equals(item.nombre))
                {
                    if (!item.update(id,item.descripcion))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (criticidad.obtenerCriticidades(item.nombre).nombre == null)
                    {
                        if (item.eliminar(id))
                        {
                            if (!item.agregarBD())
                                flag = 0;
                        }
                        else
                        {
                            flag = 0;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("criticidades");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult eliminarCriticidad(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                //criticidad.eliminar(nombre);
                flag = 1;
                if (!new criticidad().eliminar(id))
                    flag = 0;

                return RedirectToAction("criticidades");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Tipos de equipo
        public ActionResult tiposEquipo()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(tipoEquipo.obtenerTiposEquipo());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult agregarTipoEquipo(){
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewData["familias"] = new Familia_Equipo().GetFamilia_Equipo();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult editarTipoEquipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    tipoEquipo tdato = tipoEquipo.obtenerTiposEquipo(id);
                    if (tdato.tipo != null)
                    {
                        ViewBag.familias = new Familia_Equipo().GetFamilia_Equipo();
                        return View(tdato);
                    }
                }
                return RedirectToAction("tiposEquipo");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarTipoEquipo(FormCollection post)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                tipoEquipo nuevo = new tipoEquipo();

                //Agregar el método para generar una id única
                nuevo.tipo = post["tipo"];
                nuevo.familia = post["familia"];
                
                nuevo.minimoGarantizado = int.Parse((string)post["minimoGarantizado"]);

                string cadenaCostoHoraNormal = (string)post["costoHoraNormal"];
                cadenaCostoHoraNormal = cadenaCostoHoraNormal.Replace(".", ",");

                string parteEntera=cadenaCostoHoraNormal.Split(',')[0];
                string parteDecimal=cadenaCostoHoraNormal.Split(',')[1];

                double dparteEntera=double.Parse(parteEntera);
                double dparteDecimal=double.Parse(parteDecimal);
                for(int i=0;i<parteDecimal.Length;i++)
                    dparteDecimal/=10;

                nuevo.costo_hora_normal = dparteEntera + dparteDecimal;
                
                string cadenaCostoHoraExtra=(string)post["costoHoraExtra"];
                cadenaCostoHoraExtra = cadenaCostoHoraExtra.Replace(".", ",");

                parteEntera = cadenaCostoHoraExtra.Split(',')[0];
                parteDecimal = cadenaCostoHoraExtra.Split(',')[1];

                dparteEntera = double.Parse(parteEntera);
                dparteDecimal = double.Parse(parteDecimal);
                for (int i = 0; i < parteDecimal.Length; i++)
                    dparteDecimal /= 10;

                nuevo.costo_hora_extra = dparteEntera + dparteDecimal;

                if(tipoEquipo.obtenerTiposEquipo(nuevo.tipo).tipo == null)
                {
                    if (!nuevo.agregarBD())
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("tiposEquipo");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AgregarEditarTipoEquipo(FormCollection post)
        {

            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                tipoEquipo nuevo = new tipoEquipo();

                string id = post["tipo"];

                //Agregar el método para generar una id única
                nuevo.tipo = post["tipo_nuevo"];
                nuevo.familia = post["familia"];

                nuevo.minimoGarantizado = int.Parse((string)post["minimoGarantizado"]);

                string cadenaCostoHoraNormal = (string)post["costoHoraNormal"];
                cadenaCostoHoraNormal = cadenaCostoHoraNormal.Replace(".", ",");

                string parteEntera = cadenaCostoHoraNormal.Split(',')[0];
                string parteDecimal = cadenaCostoHoraNormal.Split(',')[1];

                double dparteEntera = double.Parse(parteEntera);
                double dparteDecimal = double.Parse(parteDecimal);
                for (int i = 0; i < parteDecimal.Length; i++)
                    dparteDecimal /= 10;

                nuevo.costo_hora_normal = dparteEntera + dparteDecimal;

                string cadenaCostoHoraExtra = (string)post["costoHoraExtra"];
                cadenaCostoHoraExtra = cadenaCostoHoraExtra.Replace(".", ",");

                parteEntera = cadenaCostoHoraExtra.Split(',')[0];
                parteDecimal = cadenaCostoHoraExtra.Split(',')[1];

                dparteEntera = double.Parse(parteEntera);
                dparteDecimal = double.Parse(parteDecimal);
                for (int i = 0; i < parteDecimal.Length; i++)
                    dparteDecimal /= 10;

                nuevo.costo_hora_extra = dparteEntera + dparteDecimal;

                if (id.Equals(nuevo.tipo))
                {
                    if (! nuevo.update(id,nuevo.familia,nuevo.minimoGarantizado,nuevo.costo_hora_normal,nuevo.costo_hora_extra))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (tipoEquipo.obtenerTiposEquipo(nuevo.tipo).tipo == null)
                    {
                        if (!nuevo.verificarArea(id))
                        {
                            if (nuevo.eliminar(id))
                            {
                                if (!nuevo.agregarBD())
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            flag = 2;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }

                return RedirectToAction("tiposEquipo");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public ActionResult eliminarTipoEquipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                //criticidad.eliminar(nombre);
                flag = 1;
                if (!new tipoEquipo().verificarArea(id))
                {
                    if (!new tipoEquipo().eliminar(id))
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("tiposEquipo");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion



        #region Certificaciones
        [HttpGet]
        public ActionResult Certificaciones()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(new Certificacion().GetCertificacion());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Nueva_Certificacion()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
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
        public ActionResult Editar_Certificacion(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    Get getTrabajador = new Get();
                    Certificacion tdato = new Certificacion().GetCertificacion(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return RedirectToAction("Certificaciones");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Eliminar_Certificacion(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new Certificacion().deleteCertificacion(id)) flag = 0;

                return RedirectToAction("Certificaciones");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Guardar_Certificacion(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Certificacion certificacion = new Certificacion();
                certificacion.nombre = form["nombre"];
                certificacion.descripcion = form["descripcion"];

                if (new Get().GetCertificacion(certificacion.nombre).nombre == null)
                {
                    if (!new Certificacion().InsertCertificacion(certificacion))
                        flag = 0;
                }
                else { flag = 2; }

                return RedirectToAction("Certificaciones");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Guardar_Editar_Certificacion(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Certificacion certificacion = new Certificacion();
                string id = form["nombre"];
                certificacion.nombre = form["nombre_nuevo"];
                certificacion.descripcion = form["descripcion"];

                if (id.Equals(certificacion.nombre))
                {
                    if (!new Certificacion().updateCertificacion(id,certificacion.descripcion))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (new Get().GetCertificacion(certificacion.nombre).nombre == null)
                    {
                        if (new Certificacion().deleteCertificacion(id))
                        {
                            if (!new Certificacion().InsertCertificacion(certificacion))
                                flag = 0;
                        }
                        else
                        {
                            flag = 0;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                
                return RedirectToAction("Certificaciones");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Empresa
        [HttpGet]
        public ActionResult Empresas()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(new Empresa().GetEmpresa());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Nueva_Empresa()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
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
        public ActionResult Editar_Empresa(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    Empresa tdato = new Get().GetEmpresa(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return View("Empresas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public ActionResult Eliminar_Empresa(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new Empresa().deleteEmpresa(id)) flag = 0;

                return RedirectToAction("Empresas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Empresa(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Empresa empresa = new Empresa();
                empresa.nombre = form["nombre"];
                empresa.descripcion = form["descripcion"];
                if (new Get().GetEmpresa(empresa.nombre).nombre == null)
                {
                    if (!new Empresa().InsertEmpresa(empresa)) flag = 0;
                }
                else
                {
                    flag = 2;
                }

                return RedirectToAction("Empresas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Editar_Empresa(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Empresa empresa = new Empresa();
                string id = form["nombre"];
                empresa.nombre = form["nombre_nuevo"];
                empresa.descripcion = form["descripcion"];

                if (id.Equals(empresa.nombre))
                {
                    if (!new Empresa().updateEmpresa(id,empresa.descripcion))
                    {
                      
                            flag = 0;
                    }
                }
                else
                {
                    if (new Get().GetEmpresa(empresa.nombre).nombre == null)
                    {
                        if (new Empresa().deleteEmpresa(id))
                        {
                            if (!new Empresa().InsertEmpresa(empresa))
                                flag = 0;
                        }
                        else
                        {
                            flag = 0;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }

                return RedirectToAction("Empresas");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Familia_Equipo
        [HttpGet]
        public ActionResult Familia_Equipos()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(new Familia_Equipo().GetFamilia_Equipo());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Nueva_Familia_Equipo()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
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
        public ActionResult Editar_Familia_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    Familia_Equipo tdato = new Get().GetFamilia_Equipo(id);
                    if (tdato.nombre != null)
                    {
                        return View(tdato);
                    }
                }
                return View("Familia_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Eliminar_Familia_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new Familia_Equipo().verificiarFamilia_Equipo(id))
                {
                    if (!new Familia_Equipo().deleteFamilia_Equipo(id)) flag = 0;//Esta en uso en otra tabla
                }
                else { flag = 2; }

                return RedirectToAction("Familia_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Familia_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Familia_Equipo familia = new Familia_Equipo();
                familia.nombre = form["nombre"];
                familia.descripcion = form["descripcion"];
                if (new Get().GetFamilia_Equipo(familia.nombre).nombre == null)
                {
                    if (!new Familia_Equipo().InsertFamilia_Equipo(familia)) flag = 0;
                }
                else
                {
                    flag = 2;
                }
                return RedirectToAction("Familia_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Editar_Familia_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Familia_Equipo familia = new Familia_Equipo();
                string id = form["nombre"];
                familia.nombre = form["nombre_nuevo"];
                familia.descripcion = form["descripcion"];

                if (id.Equals(familia.nombre))
                {
                    if (!new Familia_Equipo().updateFamilia_Equipo(id,familia.descripcion))
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (new Get().GetFamilia_Equipo(familia.nombre).nombre == null)
                    {
                        if (!new Familia_Equipo().verificiarFamilia_Equipo(id))
                        {
                            if (new Familia_Equipo().deleteFamilia_Equipo(id))
                            {
                                if (!new Familia_Equipo().InsertFamilia_Equipo(familia))
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            if (!new Familia_Equipo().updateFamilia_Equipo(id, familia.descripcion))
                            {
                                flag = 0;
                            }
                            else
                            {
                                flag = 2;
                            }
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("Familia_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region Marca_Equipo
        [HttpGet]
        public ActionResult Marca_Equipos()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(new Marca_Equipo().GetMarca_Equipo());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Nueva_Marca_Equipo()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
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
        public ActionResult Editar_Marca_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    Marca_Equipo tdato = new Get().GetMarca_Equipo(id);
                    if (tdato.marca != null)
                    {
                        return View(tdato);
                    }
                }
                return View("Marca_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public ActionResult Eliminar_Marca_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new Marca_Equipo().verificarMarca_Equipo(id))
                {
                    if (!new Marca_Equipo().deleteMarca_Equipo(id)) flag = 0;//Está en uso en otra tabla
                }
                else
                {
                    flag = 2;
                }

                return RedirectToAction("Marca_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Marca_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                Marca_Equipo marca = new Marca_Equipo();
                marca.marca = form["marca"];
                if (new Get().GetMarca_Equipo(marca.marca).marca == null)
                {
                    if (!new Marca_Equipo().InsertMarca_Equipo(marca)) flag = 0;
                }
                else
                {
                    flag = 2;
                }
                return RedirectToAction("Marca_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Editar_Marca_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                Marca_Equipo marca = new Marca_Equipo();
                string id = form["nombre"];
                marca.marca = form["nombre_nuevo"];

                if (id.Equals(marca.marca))
                {
                    if (!new Marca_Equipo().verificarMarca_Equipo(id))
                    {
                        if (new Marca_Equipo().deleteMarca_Equipo(id))
                        {
                            if (!new Marca_Equipo().InsertMarca_Equipo(marca))
                                flag = 0;
                        }
                    }
                }
                else
                {
                    if (new Get().GetMarca_Equipo(marca.marca).marca == null)
                    {
                        if (!new Marca_Equipo().verificarMarca_Equipo(id))
                        {
                            if (new Marca_Equipo().deleteMarca_Equipo(id))
                            {
                                if (!new Marca_Equipo().InsertMarca_Equipo(marca))
                                    flag = 0;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            flag = 2;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                return RedirectToAction("Marca_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region Modelo_Equipo
        [HttpGet]
        public ActionResult Modelo_Equipos()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                return View(new Modelo_Equipo().GetModelo_Equipo());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Nuevo_Modelo_Equipo()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;
                ViewBag.marca = new Marca_Equipo().GetMarca_Equipo();
                ViewBag.tipo = tipoEquipo.obtenerTiposEquipo();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public ActionResult Editar_Modelo_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = -1;



                if (!string.IsNullOrEmpty(id))
                {
                    //recuperar datos con id{rut}
                    Modelo_Equipo tdato = new Get().GetModelo_Equipo(id);
                    if (tdato.marca != null)
                    {
                        ViewBag.marca = new Marca_Equipo().GetMarca_Equipo();
                        ViewBag.tipo = tipoEquipo.obtenerTiposEquipo();
                        return View(tdato);
                    }
                }
                return View("Modelo_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public ActionResult Eliminar_Modelo_Equipo(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                if (!new Modelo_Equipo().deleteModelo_Equipo(id)) flag = 0;

                return RedirectToAction("Modelo_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Guardar_Modelo_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;
                Modelo_Equipo modelo = new Modelo_Equipo();
                modelo.modelo = form["modelo"];
                modelo.marca = form["marca"];
                modelo.tipo = form["tipo"];
                if (new Get().GetModelo_Equipo(modelo.modelo).modelo == null)
                {
                    if (!new Modelo_Equipo().InsertModelo_Equipo(modelo)) flag = 0;
                }
                else
                {
                    flag = 2;
                }
                return RedirectToAction("Modelo_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult Guardar_Editar_Modelo_Equipo(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                flag = 1;

                string id = form["modelo"];
                Modelo_Equipo modelo = new Modelo_Equipo();
                modelo.modelo = form["modelo_nuevo"];
                modelo.marca = form["marca"];
                modelo.tipo = form["tipo"];

                if (id.Equals(modelo.modelo))
                {
                    if (!new Modelo_Equipo().updateModelo_Equipo(id,modelo.marca,modelo.tipo) )
                    {
                            flag = 0;
                    }
                }
                else
                {
                    if (new Get().GetModelo_Equipo(modelo.modelo).modelo == null)
                    {
                        
                        if (new Modelo_Equipo().deleteModelo_Equipo(id))
                        {
                            if (!new Modelo_Equipo().InsertModelo_Equipo(modelo))
                                flag = 0;
                        }
                        else
                        {
                            flag = 0;
                        }
                    }
                    else
                    {
                        flag = 2;
                    }
 
                }
                return RedirectToAction("Modelo_Equipos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        #endregion


        #region Jefe_Area
        [HttpGet]
        public ActionResult Nuevo_Jefe_Area(string id)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                flag = -1;
                ViewBag.nombre = id;
                ViewBag.areas = new conexion().obtenerListaAreas();
                ViewBag.area = new Jefe_Area().GetJefe_Area(id).area;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Guardar_Jefe_Area(FormCollection form)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                flag = 1;
                Jefe_Area jefe = new Jefe_Area();
                jefe.nombre = form["nombre"];
                jefe.area = form["area"];


                if (new Jefe_Area().deleteJefe_Area(jefe.nombre))
                {
                    if (!new Jefe_Area().InsertJefe_Area(jefe)) flag = 0;
                }


                return RedirectToAction("todoUsuarios", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        #endregion

        #region Control de Usuarios
        [HttpGet]
        public ActionResult todoUsuarios()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                ViewBag.success = flag;
                flag = -1;
                Usuarios user = new Usuarios();
                return View(user.obtenerUsuarios());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult agregarUsuario()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                string[] roles = new string[] { "jefeArea", "izaje", "admin" };
                ViewData["roles"] = roles;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult eliminarUsuario(string id)
        {

            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {

                if (new Jefe_Area().deleteJefe_Area(id) && new Usuarios().eliminarUsuario(id))
                {
                    flag = 1;
                    return RedirectToAction("todoUsuarios");
                }
                else
                {
                    flag = 0;
                    return RedirectToAction("todoUsuarios");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public string comprobar_usuario(string nombre)
        {
            Usuarios user = new Usuarios();
            string retorno = user.comprobarUsuario(nombre);
            return retorno;
        }
        [HttpPost]
        public ActionResult metodoAgregarUsuario(FormCollection datos)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                Usuarios user = new Usuarios();
                user.nombres = datos["nombre_completo"].ToString();
                user.apellido_paterno = datos["apellido_Paterno"].ToString();
                user.apellido_materno = datos["apellido_Materno"].ToString();
                user.email = datos["email"].ToString();
                user.identificador = datos["identificador"].ToString();
                user.rol = datos["rol"].ToString();
                user.password = datos["password"].ToString();
                if (user.guardarUsuario(user))
                {
                    flag = 1;
                    return RedirectToAction("todoUsuarios", "Admin");
                }
                else
                {
                    flag = 0;
                    return RedirectToAction("todoUsuarios", "Admin");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult editarUsuario(string nombre)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                string[] roles = new string[] { "jefeArea", "izaje", "admin" };
                ViewData["roles"] = roles;
                Usuarios user = new Usuarios();
                return View(user.obtenerdatos(nombre));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult metodoEditarGuardarUsuario(FormCollection datos)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin") && Session["nombre"].Equals("admin"))
            {
                Usuarios user = new Usuarios();
                user.nombres = datos["nombre_completo"].ToString();
                user.apellido_paterno = datos["apellido_Paterno"].ToString();
                user.apellido_materno = datos["apellido_Materno"].ToString();
                user.email = datos["email"].ToString();
                user.identificador = datos["identificador"].ToString();

                user.rol = datos["rol"].ToString();
                user.password = datos["password"].ToString();
                if (user.actualizarUsuario(user))
                {
                    flag = 1;
                    return RedirectToAction("todoUsuarios", "Admin");

                }
                else
                {
                    flag = 0;
                    return RedirectToAction("todoUsuarios", "Admin");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

    }
}
