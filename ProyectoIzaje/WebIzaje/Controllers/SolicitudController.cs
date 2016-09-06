using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;
using System.Web.Hosting;
using System.IO;
using Postal;

namespace WebIzaje.Controllers
{
    public class SolicitudController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult agregarSolicitud()
        {
            ViewData["gerencias"] = obtenerGerencias().ToArray();
            ViewData["criticidad"] = new conexion().obtenerCriticidad();
            return View();
        }
        public ActionResult editarSolicitud(string idSolicitud)
        {
            Solicitud dato = Solicitud.obtenerSolicitud(idSolicitud);

            ViewData["criticidad"] = new conexion().obtenerCriticidad();

            ViewData["turnoDia"] = "turno dia";
            ViewData["turnoNoche"] = "turno noche";

            ViewData["gerencias"] = obtenerGerencias().ToArray();
            ViewData["superintendencias"] = obtenerSuperintendencias(dato.gerencia).ToArray();
            ViewData["areas"] = obtenerAreas(dato.superintendencia).ToArray();
            ViewData["centrosDeCosto"] = obtenerCentroCosto(dato.area).ToArray();
            
            return View(dato);
        }
        public ActionResult verSolicitud(string idSolicitud)
        {
            Solicitud dato = Solicitud.obtenerSolicitud(idSolicitud);
            
            return View(dato);
        }
        public ActionResult planificarSolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                var dato = Solicitud.obtenerSolicitud(idSolicitud);

                ViewData["criticidad"] = new conexion().obtenerCriticidad();

                ViewData["turnoDia"] = "turno dia";
                ViewData["turnoNoche"] = "turno noche";

                ViewData["familiasEquipo"] = obtenerFamiliasEquipo();

                return View(dato);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult replanificarSolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                var dato = Solicitud.obtenerSolicitud(idSolicitud);

                ViewData["criticidad"] = new conexion().obtenerCriticidad();

                ViewData["turnoDia"] = "turno dia";
                ViewData["turnoNoche"] = "turno noche";

                ViewData["familiasEquipo"] = obtenerFamiliasEquipo();

                return View(dato);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult autorizarSolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("jefeArea")))
            {
                var dato = Solicitud.obtenerSolicitud(idSolicitud);

                //string fecha= dato.inicio.Date.ToString().Split(' ')[0];
                ViewData["fecha"] = dato.inicio.Date;

                ViewData["medianamentecritica"] = "Medianamiente crítica";
                ViewData["critica"] = "Crítica";

                ViewData["turnoDia"] = "Día";
                ViewData["turnoNoche"] = "Noche";

                return View(dato);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult finalizarSolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                var dato = Solicitud.obtenerSolicitud(idSolicitud);

                //string fecha= dato.inicio.Date.ToString().Split(' ')[0];
                ViewData["fecha"] = dato.inicio.Date;
                
                ViewData["medianamentecritica"] = "Medianamiente crítica";
                ViewData["critica"] = "Crítica";

                ViewData["turnoDia"] = "turno dia";
                ViewData["turnoNoche"] = "turno noche";

                DatosEquipo equipo1= new EquipoSelect().obtener_equipos(dato.idEquipo1);
                equipo1.datos_nocautivo = new EquipoNoCautivoSelect().obtener_Nocautivo(equipo1.tipo_equipo);
                ViewData["uf1"] = equipo1.datos_nocautivo.costo_hora;
                if (!dato.idEquipo2.Equals("--"))
                {
                    DatosEquipo equipo2 = new EquipoSelect().obtener_equipos(dato.idEquipo2);
                    equipo2.datos_nocautivo = new EquipoNoCautivoSelect().obtener_Nocautivo(equipo2.tipo_equipo);
                    ViewData["uf2"] = equipo2.datos_nocautivo.costo_hora;
                }
                else 
                {
                    ViewData["uf2"] = 0;
                }

                //Se obtienen los días que duró el servicio y se envía esta información
                ViewData["dias"] = obtenerDias(dato.inicioCorregido, dato.finCorregido);

                return View(dato);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult corregirDatos(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                var dato = Solicitud.obtenerSolicitud(idSolicitud);

                ViewData["criticidad"] = new conexion().obtenerCriticidad();

                ViewData["turnoDia"] = "turno dia";
                ViewData["turnoNoche"] = "turno noche";

                DatosEquipo equipo1=new EquipoSelect().obtener_equipos(dato.idEquipo1);

                ViewData["familiasEquipo"] = obtenerFamiliasEquipo();
                ViewData["familiaEquipo1"] = equipo1.familia_equipo;
                
                string tiposEquipo1 = new WebIzaje.Controllers.EquipoController().rescatar_datos_tipoequipo(equipo1.familia_equipo);
                string[] tiposEquipo1Arr = tiposEquipo1.Split(',');
                List<string> tiposEquipo1List = new List<string>();
                for (int i = 0; i < tiposEquipo1Arr.Length; i++) 
                {
                    tiposEquipo1List.Add(tiposEquipo1Arr[i]);
                }

                ViewData["tiposEquipo1"] = tiposEquipo1List;
                ViewData["tipoEquipo1"] = equipo1.tipo_equipo;

                List<string> equipos1 = new conexion().obtenerEquiposPorTipo(equipo1.tipo_equipo);
                List<string> equipos1Format = new List<string>();
                for (int i = 0; i < equipos1.Count; i++) 
                {
                    DatosEquipo temp = new EquipoSelect().obtener_equipos(equipos1[i]);
                    equipos1Format.Add(temp.tag + "/" + temp.marca + " " + temp.modelo);
                }
                ViewData["equipos1"] = equipos1Format;

                List<string> operadores1 = new conexion().obtenerOperadoresPorTag(equipo1.tag);
                List<string> operadores1Format = new List<string>();
                for (int i = 0; i < operadores1.Count; i++)
                {
                    TrabajadorDatos temp = new TrabajadorGet().trabajador(operadores1[i]);
                    operadores1Format.Add(temp.rut + "/" + temp.nombre + " " + temp.apellidoP);
                }
                ViewData["operadores1"] = operadores1Format;

                List<string> riggers1 = new conexion().obtenerRiggersPorTag(equipo1.tag);
                List<string> riggers1Format = new List<string>();
                for (int i = 0; i < riggers1.Count; i++)
                {
                    TrabajadorDatos temp = new TrabajadorGet().trabajador(riggers1[i]);
                    riggers1Format.Add(temp.rut + "/" + temp.nombre + " " + temp.apellidoP);
                }
                ViewData["riggers1"] = riggers1Format;


                if (!dato.idEquipo2.Equals("--")) 
                {
                    DatosEquipo equipo2 = new EquipoSelect().obtener_equipos(dato.idEquipo2);

                    ViewData["familiaEquipo2"] = equipo2.familia_equipo;

                    string tiposEquipo2 = new WebIzaje.Controllers.EquipoController().rescatar_datos_tipoequipo(equipo2.familia_equipo);
                    string[] tiposEquipo2Arr = tiposEquipo2.Split(',');
                    List<string> tiposEquipo2List = new List<string>();
                    for (int i = 0; i < tiposEquipo2Arr.Length; i++)
                    {
                        tiposEquipo2List.Add(tiposEquipo2Arr[i]);
                    }

                    ViewData["tiposEquipo2"] = tiposEquipo2List;
                    ViewData["tipoEquipo2"] = equipo2.tipo_equipo;

                    List<string> equipos2 = new conexion().obtenerEquiposPorTipo(equipo2.tipo_equipo);
                    List<string> equipos2Format = new List<string>();
                    for (int i = 0; i < equipos2.Count; i++)
                    {
                        DatosEquipo temp = new EquipoSelect().obtener_equipos(equipos2[i]);
                        equipos2Format.Add(temp.tag + "/" + temp.marca + " " + temp.modelo);
                    }
                    ViewData["equipos2"] = equipos2Format;

                    List<string> operadores2 = new conexion().obtenerOperadoresPorTag(equipo2.tag);
                    List<string> operadores2Format = new List<string>();
                    for (int i = 0; i < operadores2.Count; i++)
                    {
                        TrabajadorDatos temp = new TrabajadorGet().trabajador(operadores2[i]);
                        operadores2Format.Add(temp.rut + "/" + temp.nombre + " " + temp.apellidoP);
                    }
                    ViewData["operadores2"] = operadores2Format;

                    List<string> riggers2 = new conexion().obtenerRiggersPorTag(equipo2.tag);
                    List<string> riggers2Format = new List<string>();
                    for (int i = 0; i < riggers2.Count; i++)
                    {
                        TrabajadorDatos temp = new TrabajadorGet().trabajador(riggers2[i]);
                        riggers2Format.Add(temp.rut + "/" + temp.nombre + " " + temp.apellidoP);
                    }
                    ViewData["riggers2"] = riggers2Format;
                }


                return View(dato);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult verSolicitudes()
        {
            //List<Solicitud> todas = Solicitud.obtenerTodas();
            List<Solicitud> todas = Solicitud.obtenerTodasResumida();
            return View(todas);
        }
        public ActionResult verNuevas()
        {
            List<Solicitud> solicitudes = Solicitud.obtenerNuevas();
            return View(solicitudes);
        }
        public ActionResult verPlanificadas()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje") || Session["rol"].Equals("jefeArea")))
            {
                List<Solicitud> planificadas = Solicitud.obtenerPlanificadas();

                return View(planificadas);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult verAutorizadas()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje") || Session["rol"].Equals("jefeArea")))
            {
                List<Solicitud> autorizadas = Solicitud.obtenerAutorizadas();
                return View(autorizadas);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult verFinalizadas()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje") || Session["rol"].Equals("jefeArea")))
            {
                List<Solicitud> finalizadas = Solicitud.obtenerFinalizadas();
                return View(finalizadas);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult verConfirmadas()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje") || Session["rol"].Equals("jefeArea")))
            {
                List<Solicitud> confirmadas = Solicitud.obtenerConfirmadas();
                return View(confirmadas);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }        
        public ActionResult refinalizarSolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                Solicitud finalizada = Solicitud.obtenerSolicitud(idSolicitud);

                DatosEquipo equipo1 = new EquipoSelect().obtener_equipos(finalizada.idEquipo1);
                equipo1.datos_nocautivo = new EquipoNoCautivoSelect().obtener_Nocautivo(equipo1.tipo_equipo);
                ViewData["uf1"] = equipo1.datos_nocautivo.costo_hora;
                if (!finalizada.idEquipo2.Equals("--"))
                {
                    DatosEquipo equipo2 = new EquipoSelect().obtener_equipos(finalizada.idEquipo2);
                    equipo2.datos_nocautivo = new EquipoNoCautivoSelect().obtener_Nocautivo(equipo2.tipo_equipo);
                    ViewData["uf2"] = equipo2.datos_nocautivo.costo_hora;
                }
                else
                {
                    ViewData["uf2"] = 0;
                }

                return View(finalizada);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Confirmarsolicitud(string idSolicitud)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("jefeArea")))
            {
                Solicitud aConfirmar = Solicitud.obtenerSolicitud(idSolicitud);

                return View(aConfirmar);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public List<datos_confirmar_solicitud> setear_datos_id(string id_solicitud)
        {
            List<datos_confirmar_solicitud> aux = new List<datos_confirmar_solicitud>();
            if (id_solicitud.Equals("123"))
            {
                datos_confirmar_solicitud datos_confirmar = new datos_confirmar_solicitud();

                datos_confirmar.fecha = "22/02/2012";
                datos_confirmar.tag1 = "123";
                datos_confirmar.hora_reloj_inicio1 = "12:00";
                datos_confirmar.hora_reloj_fin1 = "13:00";
                datos_confirmar.hora_horometro_inicio1 = "13:00";
                datos_confirmar.hora_horometro_fin1 = "14:00";
                datos_confirmar.tag2 = "234";
                datos_confirmar.hora_reloj_inicio2 = "13:00";
                datos_confirmar.hora_reloj_fin2 = "14:00";
                datos_confirmar.hora_horometro_incio2 = "14:00";
                datos_confirmar.hora_horometro_fin2 = "15:00";
                datos_confirmar_solicitud datos_confirmar2 = new datos_confirmar_solicitud();

                datos_confirmar2.fecha = "22/02/2012";
                datos_confirmar2.tag1 = "121231233";
                datos_confirmar2.hora_reloj_inicio1 = "12:00";
                datos_confirmar2.hora_reloj_fin1 = "13:00";
                datos_confirmar2.hora_horometro_inicio1 = "13:00";
                datos_confirmar2.hora_horometro_fin1 = "14:00";
                datos_confirmar2.tag2 = "234";
                datos_confirmar2.hora_reloj_inicio2 = "13:00";
                datos_confirmar2.hora_reloj_fin2 = "14:00";
                datos_confirmar2.hora_horometro_incio2 = "14:00";
                datos_confirmar2.hora_horometro_fin2 = "15:00";
                aux.Add(datos_confirmar);
                aux.Add(datos_confirmar2);
            }
            return aux;
        }
        public List<datos_refinalizar_solicitud> setear_datos_refinalizar(string id_solicitud)
        {
            List<datos_refinalizar_solicitud> aux = new List<datos_refinalizar_solicitud>();
            if (id_solicitud.Equals("123"))
            {
                datos_refinalizar_solicitud datos_refinalizar = new datos_refinalizar_solicitud();

                datos_refinalizar.fecha1 = "22/02/2012";
                datos_refinalizar.tag1 = "123";
                datos_refinalizar.rutop1 = "17.074.797-6";
                datos_refinalizar.rutrg1 = "7.737.253-9";
                datos_refinalizar.fecha2 = "22/02/2012";
                datos_refinalizar.rutop2 = "7.483.585-6";
                datos_refinalizar.rutrg2 = "17.368.570-6";

                datos_refinalizar.fecha2 = "22/02/2012";
                datos_refinalizar.tag2 = "1234545";
                datos_refinalizar.rutop2 = "18.074.797-6";
                datos_refinalizar.rutrg2 = "8.737.253-9";
                datos_refinalizar.rutop2 = "8.483.585-6";
                datos_refinalizar.rutrg2 = "18.368.570-6";
                datos_refinalizar_solicitud datos_refinalizar2 = new datos_refinalizar_solicitud();

                datos_refinalizar2.fecha1 = "23/02/2012";
                datos_refinalizar2.tag1 = "111111";
                datos_refinalizar2.rutop1 = "17.074.797-6";
                datos_refinalizar2.rutrg1 = "7.737.253-9";
                datos_refinalizar2.fecha2 = "22/02/2012";
                datos_refinalizar2.rutop2 = "7.483.585-6";
                datos_refinalizar2.rutrg2 = "17.368.570-6";

                datos_refinalizar2.fecha2 = "23/02/2012";
                datos_refinalizar2.tag2 = "222222";
                datos_refinalizar2.rutop2 = "18.074.797-6";
                datos_refinalizar2.rutrg2 = "8.737.253-9";
                datos_refinalizar2.rutop2 = "8.483.585-6";
                datos_refinalizar2.rutrg2 = "18.368.570-6";
                aux.Add(datos_refinalizar);
                aux.Add(datos_refinalizar2);
            }
            return aux;

        }
        public string llenarDatosEditarSolicitud() {
            string retorno = "";
            retorno += "nombre:"+Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";
            
            return retorno;
        }
        public string llenarDatosEliminarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",centroCosto";  
            retorno += ",fecha de inicio";
            retorno += ",fecha de término";

            return retorno;
        }
        public string llenarDatosPlanificarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";

            return retorno;
        }
        public string llenarDatosReplanificarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";

            return retorno;
        }
        public string llenarDatosAutorizarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";

            return retorno;
        }
        public string llenarDatosFinalizarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";

            return retorno;
        }
        public string llenarDatosRefinalizarSolicitud()
        {
            string retorno = "";
            retorno += "nombre:" + Request["valor"];
            retorno += ",empresa";
            retorno += ",correo";
            retorno += ",teléfono";
            retorno += ",celular";

            return retorno;
        }        
        int obtenerDias(DateTime inicio,DateTime fin)
        {
            int retorno=1;

            DateTime a = new DateTime(inicio.Year, inicio.Month, inicio.Day);
            DateTime b = new DateTime(fin.Year, fin.Month, fin.Day);

            while (a.Date < b.Date) 
            {
                retorno++;
                a=a.AddDays(1);
            }

            return retorno;
        }
        public List<string> obtenerGerencias() 
        {
            List<string> retorno = new conexion().obtenerGerencias();
            return retorno;
        }
        public List<string> obtenerSuperintendencias(string gerencia)
        {
            List<string> retorno = new conexion().obtenerSuperintendencia(gerencia);
            return retorno;
        }
        public List<string> obtenerAreas(string superintendencia)
        {
            List<string> retorno = new conexion().obtenerArea(superintendencia);
            return retorno;
        }
        List<string> obtenerCentroCosto(string area)
        {
            List<string> retorno = new conexion().obtenerCentroCosto(area);
            
            return retorno;
        }
        public string obtenerSuperintendenciaJQ()
        {
            List<string> superintendencias = new conexion().obtenerSuperintendencia((string)Request["gerencia"]);

            string retorno = "";
            if (superintendencias.Count > 0) retorno = superintendencias[0];
            for (int i = 1; i < superintendencias.Count; i++)
                retorno += "," + superintendencias[i];
            return retorno;
        }
        public string obtenerAreaJQ()
        {
            List<string> areas = new conexion().obtenerArea((string)Request["superintendencia"]);

            string retorno = "";
            if (areas.Count > 0) retorno = areas[0];
            for (int i = 1; i < areas.Count; i++)
                retorno += "," + areas[i];
            return retorno;
        }
        public string obtenerCentroCostoJQ()
        {
            List<string> centrosDeCosto = new conexion().obtenerCentroCosto((string)Request["area"]);

            string retorno = "";
            if (centrosDeCosto.Count > 0) retorno = centrosDeCosto[0];
            for (int i = 1; i < centrosDeCosto.Count; i++)
                retorno += "," + centrosDeCosto[i];
            return retorno;
        }        
        public List<Familia_Equipo> obtenerFamiliasEquipo() 
        {
            List<Familia_Equipo> retorno = new Familia_Equipo().GetFamilia_Equipo();
            
            return retorno;
        }
        public string obtenerTiposEquipo()
        {
            string retorno = "";

            string familia=(string)Request["familia"];

            List<tipoEquipo> tiposEquipo = tipoEquipo.obtenerTiposEquipo();

            List<string> retornos = new List<string>();
            for (int i = 0; i < tiposEquipo.Count;i++ )
            {
                if (tiposEquipo[i].familia.Equals(familia)) 
                {
                    retornos.Add(tiposEquipo[i].tipo);
                }
            }

            for (int j = 0; j < retornos.Count; j++) 
            {
                if (j != 0) 
                {
                    retorno += "," + retornos[j];
                }
                else 
                {
                    retorno += retornos[j];
                }
            }
            return retorno;
        }
        public string obtenerEquipos()
        {
            string retorno = "";

            string tipoEquipo = (string)Request["tipoEquipo"];
            string inicio = (string)Request["inicio"];
            string fin = (string)Request["fin"];
            string idSol = (string)Request["idSol"];

            string fechaInicio = inicio.Split('/')[0];
            string fechaFin = fin.Split('/')[0];
            string horaCompletaInicio = inicio.Split('/')[1];
            string horaCompletaFin = fin.Split('/')[1];

            string diaInicio = fechaInicio.Split('-')[0];
            string mesInicio = fechaInicio.Split('-')[1];
            string añoInicio = fechaInicio.Split('-')[2];
            string horaInicio = horaCompletaInicio.Split(':')[0];
            string minutoInicio = horaCompletaInicio.Split(':')[1];

            string diaFin = fechaFin.Split('-')[0];
            string mesFin = fechaFin.Split('-')[1];
            string añoFin = fechaFin.Split('-')[2];
            string horaFin = horaCompletaFin.Split(':')[0];
            string minutoFin = horaCompletaFin.Split(':')[1];
            
            List<string> tagsDisponibles=new conexion().obtenerEquiposPorTipo(tipoEquipo);
            List<string> tagsOcupados = new conexion().obtenerEquiposNoDisponiblesSegunFechas(diaInicio, mesInicio, añoInicio,
                horaInicio, minutoInicio, diaFin, mesFin, añoFin, horaFin, minutoFin, idSol);

            for (int i = 0; i < tagsOcupados.Count; i++) 
            {
                if (tagsDisponibles.Contains(tagsOcupados[i]))
                {
                    tagsDisponibles.Remove(tagsOcupados[i]);
                }
            }

            for (int j = 0; j < tagsDisponibles.Count;j++ )
            {
                DatosEquipo equipo = new EquipoSelect().obtener_equipos(tagsDisponibles[j]);

                retorno += equipo.tag + "/" + equipo.marca + " " + equipo.modelo + ",";
            }
            retorno=retorno.TrimEnd(',');

             return retorno;
        }
        public string obtenerEquiposSinFechas()
        {
            string retorno = "";

            string tipoEquipo = (string)Request["tipoEquipo"];

            List<string> tagsDisponibles = new conexion().obtenerEquiposPorTipo(tipoEquipo);            
            
            for (int j = 0; j < tagsDisponibles.Count; j++)
            {
                DatosEquipo equipo = new EquipoSelect().obtener_equipos(tagsDisponibles[j]);

                retorno += equipo.tag + "/" + equipo.marca + " " + equipo.modelo + ",";
            }
            retorno = retorno.TrimEnd(',');

            return retorno;
        }
        public string obtenerOperadorRigger()
        {
            string retorno = "";

            string equipo = (string)Request["equipo"];
            string inicio = (string)Request["inicio"];
            string fin = (string)Request["fin"];
            string turno = (string)Request["turno"];
            string idSol = (string)Request["idSol"];

            string fechaInicio = inicio.Split('/')[0];
            string fechaFin = fin.Split('/')[0];
            string horaCompletaInicio = inicio.Split('/')[1];
            string horaCompletaFin = fin.Split('/')[1];

            string diaInicio = fechaInicio.Split('-')[0];
            string mesInicio = fechaInicio.Split('-')[1];
            string añoInicio = fechaInicio.Split('-')[2];
            string horaInicio = horaCompletaInicio.Split(':')[0];
            string minutoInicio = horaCompletaInicio.Split(':')[1];

            string diaFin = fechaFin.Split('-')[0];
            string mesFin = fechaFin.Split('-')[1];
            string añoFin = fechaFin.Split('-')[2];
            string horaFin = horaCompletaFin.Split(':')[0];
            string minutoFin = horaCompletaFin.Split(':')[1];

            List<string> operadoresDisponibles = new conexion().obtenerOperadoresPorTag(equipo);
            List<string> operadoresOcupados = new conexion().obtenerOperadoresNoDisponiblesSegunFechas(diaInicio, mesInicio, añoInicio,
                horaInicio, minutoInicio, diaFin, mesFin, añoFin, horaFin, minutoFin, turno, idSol);

            for (int i = 0; i < operadoresOcupados.Count; i++)
            {
                if (operadoresDisponibles.Contains(operadoresOcupados[i]))
                {
                    operadoresDisponibles.Remove(operadoresOcupados[i]);
                }
            }

            for (int j = 0; j < operadoresDisponibles.Count; j++)
            {
                TrabajadorDatos trabajador = new TrabajadorGet().trabajador(operadoresDisponibles[j]);
                retorno += trabajador.rut + "/" + trabajador.nombre + " " + trabajador.apellidoP + ",";
            }
            retorno = retorno.TrimEnd(',');
            
            retorno += ";";

            List<string> riggersDisponibles = new conexion().obtenerRiggersPorTag(equipo);
            List<string> riggersOcupados = new conexion().obtenerRiggersNoDisponiblesSegunFechas(diaInicio, mesInicio, añoInicio,
                horaInicio, minutoInicio, diaFin, mesFin, añoFin, horaFin, minutoFin, turno, idSol);

            for (int i = 0; i < riggersOcupados.Count; i++)
            {
                if (riggersDisponibles.Contains(riggersOcupados[i]))
                {
                    riggersDisponibles.Remove(riggersOcupados[i]);
                }
            }

            for (int j = 0; j < riggersDisponibles.Count; j++)
            {
                TrabajadorDatos trabajador = new TrabajadorGet().trabajador(riggersDisponibles[j]);
                retorno += trabajador.rut + "/" + trabajador.nombre + " " + trabajador.apellidoP + ",";
            }
            retorno = retorno.TrimEnd(',');
            
            return retorno;
        }
        public string obtenerOperadorRiggerSinFecha()
        {
            string retorno = "";

            string equipo = (string)Request["equipo"];

            List<string> operadoresDisponibles = new conexion().obtenerOperadoresPorTag(equipo);

            for (int j = 0; j < operadoresDisponibles.Count; j++)
            {
                TrabajadorDatos trabajador = new TrabajadorGet().trabajador(operadoresDisponibles[j]);
                retorno += trabajador.rut + "/" + trabajador.nombre + " " + trabajador.apellidoP + ",";
            }
            retorno = retorno.TrimEnd(',');

            retorno += ";";

            List<string> riggersDisponibles = new conexion().obtenerRiggersPorTag(equipo);
            
            for (int j = 0; j < riggersDisponibles.Count; j++)
            {
                TrabajadorDatos trabajador = new TrabajadorGet().trabajador(riggersDisponibles[j]);
                retorno += trabajador.rut + "/" + trabajador.nombre + " " + trabajador.apellidoP + ",";
            }
            retorno = retorno.TrimEnd(',');

            return retorno;
        }
        private string subirImagen(HttpPostedFileBase imagenCarga, string nombreCarpeta)
        {
            string cadena = "";
            if (imagenCarga != null)
            {
                if (imagenCarga.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imagenCarga.FileName);
                    var path = Path.Combine(Server.MapPath("~/Document/" + nombreCarpeta + "/"), fileName);
                    imagenCarga.SaveAs(path);
                    cadena = "~/Document/" + nombreCarpeta + "/" + fileName;
                }
            }            
            return cadena;
        }
        private bool crearCarpeta(string nombreCarpeta)
        {
            try
            {
                var path = Server.MapPath("~/Document/" + nombreCarpeta);
                if (!Directory.Exists(path))
                {
                    DirectoryInfo ruta = Directory.CreateDirectory(path);                    

                    return true;
                }
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            return false;
        }
        private void eliminarArchivo(string url){
            var path = Server.MapPath(url);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        [HttpPost]
        public ActionResult metodoagregarSolicitud(FormCollection post, HttpPostedFileBase imagenCarga)
        {
            #region agregar solicitud
            Solicitud nueva = new Solicitud();

            //Agregar el método para generar una id única
            nueva.idSolicitud = Solicitud.crearNuevaIDSolicitud(post["nombre"]);
            nueva.fechaCreacion = DateTime.Now;

            nueva.estado = "NUEVA";
            nueva.nombre = post["nombre"];
            nueva.empresa = post["empresa"];
            nueva.correo = post["correo"];
            nueva.telefono = post["telefono"];
            nueva.celular = post["celular"];

            nueva.gerencia = post["gerencia"];
            nueva.superintendencia = post["superintendencia"];
            nueva.area = post["area"];
            nueva.centroCosto = post["centroDeCosto"];
            nueva.descripcionLugar = post["descripcionLugar"];

            string fechainicio = post["fechaInicio"];
            string[] fechaInicioSeparada = fechainicio.Split('/').ToArray();
            int año = int.Parse(fechaInicioSeparada[2]);
            int mes = int.Parse(fechaInicioSeparada[1]);
            int dia = int.Parse(fechaInicioSeparada[0]);
            string horainicio = post["horaInicio"];
            string[] horaInicioSeparada = horainicio.Split(' ')[0].Split(':').ToArray();
            int hora = int.Parse(horaInicioSeparada[0]);
            int minuto = int.Parse(horaInicioSeparada[1]);

            if (hora == 12) hora = 0;
            if (horainicio.Split(' ')[1].Equals("PM"))
            {
                hora += 12;
            }
            nueva.inicio = new DateTime(año, mes, dia, hora, minuto, 0);

            string fechafin = post["fechaTermino"];
            string[] fechaFinSeparada = fechafin.Split('/').ToArray();
            año = int.Parse(fechaFinSeparada[2]);
            mes = int.Parse(fechaFinSeparada[1]);
            dia = int.Parse(fechaFinSeparada[0]);
            string horaFin = post["horaTermino"];
            string[] horaFinSeparada = horaFin.Split(' ')[0].Split(':').ToArray();
            hora = int.Parse(horaFinSeparada[0]);
            minuto = int.Parse(horaFinSeparada[1]);

            if (hora == 12) hora = 0;
            if (horaFin.Split(' ')[1].Equals("PM"))
            {
                hora += 12;
            }
            nueva.fin = new DateTime(año, mes, dia, hora, minuto, 0);

            nueva.criticidad = post["criticidad"];
            nueva.tiempoEstimadoOperacion = int.Parse(post["TiempoEstimadoOperacion"]);
            nueva.peso = int.Parse(post["peso"]);
            nueva.turno = post["turnoSolicitante"];
            nueva.alto = int.Parse(post["altoCarga"]);
            nueva.ancho = int.Parse(post["anchoCarga"]);
            nueva.largo = int.Parse(post["largoCarga"]);

            //Se sube la imagen
            crearCarpeta(nueva.idSolicitud);
            nueva.rutaImagen = subirImagen(imagenCarga, nueva.idSolicitud);

            nueva.descripcionCarga = post["descripcionCarga"];
            nueva.descripcionServicio = post["descripcionServicio"];

            if (post["solicitadoAnteriormente"] == "true")
                nueva.solicitadoAnteriormente = true;
            else nueva.solicitadoAnteriormente = false;
            if (post["riggerCalificado"] == "true")
                nueva.requiereRigger = true;
            else nueva.requiereRigger = false;
            if (post["requiereManiobra"] == "true")
                nueva.requiereManiobra = true;
            else nueva.requiereManiobra = false;
            if (post["cuentaConManiobra"] == "true")
                nueva.cuentaConManiobra = true;
            else nueva.cuentaConManiobra = false;

            if (Session["nombre"] != null) nueva.actualizarEnBD(Session["nombre"].ToString());
            else nueva.actualizarEnBD("");
            #endregion

            try
            {
                #region mail

                string[] datos = new string[6];

                datos[0] = "Nombre del solicitante; " + nueva.nombre;
                datos[1] = "Área en donde se solicita que se realice el servicio; " + nueva.area;
                datos[2] = "Centro de costo asociado al servicio; " + nueva.centroCosto;
                datos[3] = "Fecha y hora de Inicio; " + nueva.inicio.Day + "-" + nueva.inicio.Month + "-" + nueva.inicio.Year + " " + nueva.inicio.Hour + ":" + nueva.inicio.Minute;
                datos[4] = "Fecha y hora de Término; " + nueva.fin.Day + "-" + nueva.fin.Month + "-" + nueva.fin.Year + " " + nueva.fin.Hour + ":" + nueva.fin.Minute;
                datos[5] = "ID de solicitud; " + nueva.idSolicitud;

                dynamic email = new Email("avisos");
                // se envia a ameco            
                email.To = post["correo"];
                email.Asunto = "Se ha creado una nueva Solicitud de Servicios de Izaje a su nombre";
                email.TipoNotificacion = "Se ha creado una Nueva Solicitud con su dirección de correo electrónico";

                email.datosSolicitud = datos;

                DateTime fecha_aux = DateTime.Now;
                email.Fecha = "Fecha: " + fecha_aux;
                email.Pendientes = "";
                email.Cuerpo = "";
                email.Send();

                //verificar todos los estados
                metodo_enviar_pendientes("NUEVA");
                metodo_enviar_pendientes("PLANIFICADA");
                metodo_enviar_pendientes("AUTORIZADA");
                metodo_enviar_pendientes("FINALIZADA");
                #endregion
            }
            catch
            {
                Console.WriteLine("No se puedo enviar el correo");
            }
            
            ViewData["mensaje"] = "Su solicitud fue agregada con éxito";            
           
            List<Solicitud> solicitudes = Solicitud.obtenerNuevas();

            return RedirectToAction("verNuevas");

            //return View("verNuevas", solicitudes); //**UPDATE
        }
        [HttpPost]
        public ActionResult metodoEditarSolicitud(FormCollection post, HttpPostedFileBase imagenCarga)
        {

            #region editarSol

            Solicitud nueva = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);
                        
            nueva.nombre = post["nombre"];
            nueva.empresa = post["empresa"];
            nueva.correo = post["correo"];
            nueva.telefono = post["telefono"];
            nueva.celular = post["celular"];
            
            nueva.gerencia = post["gerencia"];
            nueva.superintendencia = post["superintendencia"];
            nueva.area = post["area"];
            nueva.centroCosto = post["centroDeCosto"];
            nueva.descripcionLugar = post["descripcionLugar"];

            string fechainicio = post["fechaInicio"];
            string[] fechaInicioSeparada = fechainicio.Split('/').ToArray();
            int año = int.Parse(fechaInicioSeparada[2]);
            int mes = int.Parse(fechaInicioSeparada[1]);
            int dia = int.Parse(fechaInicioSeparada[0]);
            string horainicio = post["horaInicio"];
            string[] horaInicioSeparada = horainicio.Split(' ')[0].Split(':').ToArray();
            int hora = int.Parse(horaInicioSeparada[0]);
            int minuto = int.Parse(horaInicioSeparada[1]);

            if (hora == 12) hora = 0;
            if(horainicio.Split(' ')[1].Equals("PM")){
                hora += 12;
            }

            nueva.inicio = new DateTime(año, mes, dia, hora, minuto, 0);

            string fechafin = post["fechaTermino"];
            string[] fechaFinSeparada = fechafin.Split('/').ToArray();
            año = int.Parse(fechaFinSeparada[2]);
            mes = int.Parse(fechaFinSeparada[1]);
            dia = int.Parse(fechaFinSeparada[0]);
            string horaFin = post["horaTermino"];
            string[] horaFinSeparada = horaFin.Split(' ')[0].Split(':').ToArray();
            hora = int.Parse(horaFinSeparada[0]);
            minuto = int.Parse(horaFinSeparada[1]);

            if (hora == 12) hora = 0;
            if (horaFin.Split(' ')[1].Equals("PM")) 
            {
                hora += 12;
            }

            nueva.fin = new DateTime(año, mes, dia, hora, minuto, 0);

            nueva.criticidad = post["criticidad"];
            nueva.tiempoEstimadoOperacion = int.Parse(post["TiempoEstimadoOperacion"]);
            nueva.peso = int.Parse(post["peso"]);
            nueva.turno = post["turnoSolicitante"];
            nueva.alto = int.Parse(post["altoCarga"]);
            nueva.ancho = int.Parse(post["anchoCarga"]);
            nueva.largo = int.Parse(post["largoCarga"]);

            if (imagenCarga != null && imagenCarga.ContentLength != 0)
            {
                //Se elimina la imagen anterior:
                eliminarArchivo(nueva.rutaImagen);

                //Se sube la nueva imagen:
                crearCarpeta(nueva.idSolicitud);
                nueva.rutaImagen = subirImagen(imagenCarga, nueva.idSolicitud);
            }
            

            nueva.descripcionCarga = post["descripcionCarga"];
            nueva.descripcionServicio = post["descripcionServicio"];

            if (post["solicitadoAnteriormente"] == "true")
                nueva.solicitadoAnteriormente = true;
            else nueva.solicitadoAnteriormente = false;
            if (post["riggerCalificado"] == "true")
                nueva.requiereRigger = true;
            else nueva.requiereRigger = false;
            if (post["requiereManiobra"] == "true")
                nueva.requiereManiobra = true;
            else nueva.requiereManiobra = false;
            if (post["cuentaConManiobra"] == "true")
                nueva.cuentaConManiobra = true;
            else nueva.cuentaConManiobra = false;

            if (Session["nombre"] != null) nueva.actualizarEnBD(Session["nombre"].ToString());
            else nueva.actualizarEnBD("");
            
            #endregion

            ViewData["mensaje"] = "Su solicitud ha sido modificada con éxito";

            List<Solicitud> solicitudes = Solicitud.obtenerNuevas();
            
            return RedirectToAction("verNuevas");
            //return View("verNuevas", solicitudes); //**UPDATE
        }
        [HttpPost]
        public ActionResult metodoPlanificarSolicitud(FormCollection post)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {

                #region planificarSol

                Solicitud planificada = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);
                planificada.estado = "PLANIFICADA";

                Usuarios userActual= new Usuarios().obtenerdatos(Session["nombre"].ToString());
                planificada.planificadaPor = userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno;

                planificada.criticidadCorregida = post["criticidadCorreccion"];
                planificada.tiempoEstimadoOperacionCorregido = int.Parse((string)post["TiempoEstimadoOperacionCorreccion"]);
                planificada.pesoCorregido = int.Parse((string)post["pesoCorreccion"]);
                planificada.turnoCorregido = (string)post["turnoSolicitanteCorreccion"];
                planificada.altoCorregido = int.Parse((string)post["altoCargaCorreccion"]);
                planificada.anchoCorregido = int.Parse((string)post["anchoCargaCorreccion"]);
                planificada.altoCorregido = int.Parse((string)post["largoCargaCorreccion"]);
                planificada.descripcionCargaCorregida = post["descripcionCargaCorreccion"];

                planificada.descripcionServicioCorregida = post["descripcionServicioCorreccion"];

                string fechainicioCorregido = post["fechaInicioCorreccion"];
                string[] fechaInicioSeparada = fechainicioCorregido.Split('/').ToArray();
                int año = int.Parse(fechaInicioSeparada[2]);
                int mes = int.Parse(fechaInicioSeparada[1]);
                int dia = int.Parse(fechaInicioSeparada[0]);
                string horainicioCorregido = post["horaInicioCorreccion"];
                string[] horaInicioSeparada = horainicioCorregido.Split(' ')[0].Split(':').ToArray();
                int hora = int.Parse(horaInicioSeparada[0]);
                int minuto = int.Parse(horaInicioSeparada[1]);

                if (hora == 12) hora = 0;
                if (horainicioCorregido.Split(' ')[1].Equals("PM"))
                {
                    hora += 12;
                }

                planificada.inicioCorregido = new DateTime(año, mes, dia, hora, minuto, 0);

                string fechafinCorregido = post["fechaTerminoCorreccion"];
                string[] fechaFinSeparada = fechafinCorregido.Split('/').ToArray();
                año = int.Parse(fechaFinSeparada[2]);
                mes = int.Parse(fechaFinSeparada[1]);
                dia = int.Parse(fechaFinSeparada[0]);
                string horaFinCorregido = post["horaTerminoCorreccion"];
                string[] horaFinSeparada = horaFinCorregido.Split(' ')[0].Split(':').ToArray();
                hora = int.Parse(horaFinSeparada[0]);
                minuto = int.Parse(horaFinSeparada[1]);

                if (hora == 12) hora = 0;
                if (horaFinCorregido.Split(' ')[1].Equals("PM"))
                {
                    hora += 12;
                }

                planificada.finCorregido = new DateTime(año, mes, dia, hora, minuto, 0);

                planificada.idEquipo1 = (string)post["equipo1"];
                planificada.idOperador1 = (string)post["operador1"];
                planificada.idRigger1 = (string)post["rigger1"];
                planificada.idEquipo2 = (string)post["equipo2"];
                planificada.idOperador2 = (string)post["operador2"];
                planificada.idRigger2 = (string)post["rigger2"];

                if (Session["nombre"] != null) planificada.actualizarEnBD(Session["nombre"].ToString());
                else planificada.actualizarEnBD("");

                #endregion

                try
                {
                    #region mail
                    //revisar id de planificacion
                    string mail_usuario = planificada.correo;

                    if (mail_usuario != null)
                    {
                        string[] datos = new string[9];

                        datos[0] = "Nombre del solicitante; " + planificada.nombre;
                        datos[1] = "Área en donde se solicita que se realice el servicio; " + planificada.area;
                        datos[2] = "Centro de costo asociado al servicio; " + planificada.centroCosto;
                        datos[3] = "Fecha y hora de Inicio Solicitada; " + planificada.inicio.Day + "-" + planificada.inicio.Month + "-" + planificada.inicio.Year + " " + planificada.inicio.Hour + ":" + planificada.inicio.Minute;
                        datos[4] = "Fecha y hora de Término Solicitada; " + planificada.fin.Day + "-" + planificada.fin.Month + "-" + planificada.fin.Year + " " + planificada.fin.Hour + ":" + planificada.fin.Minute;
                        datos[5] = "Fecha y hora de Inicio Planificada; " + planificada.inicio.Day + "-" + planificada.inicio.Month + "-" + planificada.inicio.Year + " " + planificada.inicio.Hour + ":" + planificada.inicio.Minute;
                        datos[6] = "Fecha y hora de Término Planificada; " + planificada.fin.Day + "-" + planificada.fin.Month + "-" + planificada.fin.Year + " " + planificada.fin.Hour + ":" + planificada.fin.Minute;
                        datos[7] = "ID de solicitud; " + planificada.idSolicitud;
                        datos[8] = "Planificada por; " + planificada.planificadaPor;

                        dynamic email = new Email("avisos");
                        // se envia a ameco            
                        email.To = planificada.correo;
                        email.Asunto = "Se ha planificado una Solicitud de Servicios de Izaje realizada a su nombre";
                        email.TipoNotificacion = "Ha sido planificada una solicitud de servicios de Izaje creada por usted";

                        email.datosSolicitud = datos;

                        DateTime fecha_aux = DateTime.Now;
                        email.Fecha = "Fecha: " + fecha_aux;
                        email.Pendientes = "";
                        email.Cuerpo = "";
                        email.Send();
                    }

                    // metodo para verificar las nuevas que estan pendientes y enviarle un mail al usuario
                    //se consulta si hay solicitudes nuevas creadas con fecha menor a una semana

                    //verificar todos los estados
                    metodo_enviar_pendientes("NUEVA");
                    metodo_enviar_pendientes("PLANIFICADA");
                    metodo_enviar_pendientes("AUTORIZADA");
                    metodo_enviar_pendientes("FINALIZADA");
                    #endregion
                }
                catch
                {
                    Console.WriteLine("No se puedo enviar el correo");
                }

                ViewData["mensaje"] = "Su solicitud ha sido planificada";

                List<Solicitud> solicitudes = Solicitud.obtenerPlanificadas();

                return RedirectToAction("verPlanificadas");
                //return View("verPlanificadas", solicitudes); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult metodoReplanificarSolicitud(FormCollection post, HttpPostedFileBase imagenCarga)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {

                #region replanificarSol

                Solicitud planificada = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);
                planificada.estado = "PLANIFICADA";

                Usuarios userActual = new Usuarios().obtenerdatos(Session["nombre"].ToString());
                planificada.planificadaPor = userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno;

                planificada.criticidadCorregida = post["criticidadCorreccion"];
                planificada.tiempoEstimadoOperacionCorregido = int.Parse((string)post["TiempoEstimadoOperacionCorreccion"]);
                planificada.pesoCorregido = int.Parse((string)post["pesoCorreccion"]);
                planificada.turnoCorregido = (string)post["turnoSolicitanteCorreccion"];
                planificada.altoCorregido = int.Parse((string)post["altoCargaCorreccion"]);
                planificada.anchoCorregido = int.Parse((string)post["anchoCargaCorreccion"]);
                planificada.altoCorregido = int.Parse((string)post["largoCargaCorreccion"]);
                planificada.descripcionCargaCorregida = post["descripcionCargaCorreccion"];

                planificada.descripcionServicioCorregida = post["descripcionServicioCorreccion"];

                string fechainicioCorregido = post["fechaInicioCorreccion"];
                string[] fechaInicioSeparada = fechainicioCorregido.Split('/').ToArray();
                int año = int.Parse(fechaInicioSeparada[2]);
                int mes = int.Parse(fechaInicioSeparada[1]);
                int dia = int.Parse(fechaInicioSeparada[0]);
                string horainicioCorregido = post["horaInicioCorreccion"];
                string[] horaInicioSeparada = horainicioCorregido.Split(' ')[0].Split(':').ToArray();
                int hora = int.Parse(horaInicioSeparada[0]);
                int minuto = int.Parse(horaInicioSeparada[1]);

                if (hora == 12) hora = 0;
                if (horainicioCorregido.Split(' ')[1].Equals("PM"))
                {
                    hora += 12;
                }

                planificada.inicioCorregido = new DateTime(año, mes, dia, hora, minuto, 0);

                string fechafinCorregido = post["fechaTerminoCorreccion"];
                string[] fechaFinSeparada = fechafinCorregido.Split('/').ToArray();
                año = int.Parse(fechaFinSeparada[2]);
                mes = int.Parse(fechaFinSeparada[1]);
                dia = int.Parse(fechaFinSeparada[0]);
                string horaFinCorregido = post["horaTerminoCorreccion"];
                string[] horaFinSeparada = horaFinCorregido.Split(' ')[0].Split(':').ToArray();
                hora = int.Parse(horaFinSeparada[0]);
                minuto = int.Parse(horaFinSeparada[1]);

                if (hora == 12) hora = 0;
                if (horaFinCorregido.Split(' ')[0].Equals("PM"))
                {
                    hora += 12;
                }

                planificada.finCorregido = new DateTime(año, mes, dia, hora, minuto, 0);

                planificada.idEquipo1 = (string)post["equipo1"];
                planificada.idOperador1 = (string)post["operador1"];
                planificada.idRigger1 = (string)post["rigger1"];
                planificada.idEquipo2 = (string)post["equipo2"];
                planificada.idOperador2 = (string)post["operador2"];
                planificada.idRigger2 = (string)post["rigger2"];

                if (Session["nombre"] != null) planificada.actualizarEnBD(Session["nombre"].ToString());
                else planificada.actualizarEnBD("");

                #endregion

                try
                {
                    #region mail
                    //revisar id de planificacion
                    string mail_usuario = planificada.correo;

                    if (mail_usuario != null)
                    {
                        string[] datos = new string[9];

                        datos[0] = "Nombre del solicitante; " + planificada.nombre;
                        datos[1] = "Área en donde se solicita que se realice el servicio; " + planificada.area;
                        datos[2] = "Centro de costo asociado al servicio; " + planificada.centroCosto;
                        datos[3] = "Fecha y hora de Inicio Solicitada; " + planificada.inicio.Day + "-" + planificada.inicio.Month + "-" + planificada.inicio.Year + " " + planificada.inicio.Hour + ":" + planificada.inicio.Minute;
                        datos[4] = "Fecha y hora de Término Solicitada; " + planificada.fin.Day + "-" + planificada.fin.Month + "-" + planificada.fin.Year + " " + planificada.fin.Hour + ":" + planificada.fin.Minute;
                        datos[5] = "Fecha y hora de Inicio Planificada; " + planificada.inicio.Day + "-" + planificada.inicio.Month + "-" + planificada.inicio.Year + " " + planificada.inicio.Hour + ":" + planificada.inicio.Minute;
                        datos[6] = "Fecha y hora de Término Planificada; " + planificada.fin.Day + "-" + planificada.fin.Month + "-" + planificada.fin.Year + " " + planificada.fin.Hour + ":" + planificada.fin.Minute;
                        datos[7] = "ID de solicitud; " + planificada.idSolicitud;
                        datos[8] = "Planificada por; " + planificada.planificadaPor;

                        dynamic email = new Email("avisos");
                        // se envia a ameco            
                        email.To = planificada.correo;
                        email.Asunto = "Se ha re-planificado una Solicitud de Servicios de Izaje realizada a su nombre";
                        email.TipoNotificacion = "Ha sido re-planificada una solicitud de servicios de Izaje creada por usted";

                        email.datosSolicitud = datos;

                        DateTime fecha_aux = DateTime.Now;
                        email.Fecha = "Fecha: " + fecha_aux;
                        email.Pendientes = "";
                        email.Cuerpo = "";
                        email.Send();
                    }

                    //verificar todos los estados
                    metodo_enviar_pendientes("NUEVA");
                    metodo_enviar_pendientes("PLANIFICADA");
                    metodo_enviar_pendientes("AUTORIZADA");
                    metodo_enviar_pendientes("FINALIZADA");

                    #endregion
                }
                catch
                {
                    Console.WriteLine("No se puedo enviar el correo");
                }

                ViewData["mensaje"] = "Su solicitud ha sido replanificada";

                List<Solicitud> solicitudes = Solicitud.obtenerPlanificadas();

                return RedirectToAction("verPlanificadas");
                //return View("verPlanificadas", solicitudes); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult metodoAutorizarSolicitud(FormCollection post)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("jefeArea")))
            {
                Solicitud autorizada = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);
                autorizada.estado = "AUTORIZADA";
                
                if (Session["nombre"] != null)
                {
                    Usuarios userActual = new Usuarios().obtenerdatos(Session["nombre"].ToString());
                    autorizada.autorizadaPor = userActual.nombres + " "+ userActual.apellido_paterno + " " + userActual.apellido_materno;

                    autorizada.actualizarEnBD(userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno);
                }
                else autorizada.actualizarEnBD("");
                try
                {
                    #region mail
                    //revisar id de planificacion
                    string mail_usuario = autorizada.correo;
                    if (mail_usuario != null)
                    {
                        string[] datos = new string[10];

                        datos[0] = "Nombre del solicitante; " + autorizada.nombre;
                        datos[1] = "Área en donde se solicita que se realice el servicio; " + autorizada.area;
                        datos[2] = "Centro de costo asociado al servicio; " + autorizada.centroCosto;
                        datos[3] = "Fecha y hora de Inicio Solicitada; " + autorizada.inicio.Day + "-" + autorizada.inicio.Month + "-" + autorizada.inicio.Year + " " + autorizada.inicio.Hour + ":" + autorizada.inicio.Minute;
                        datos[4] = "Fecha y hora de Término Solicitada; " + autorizada.fin.Day + "-" + autorizada.fin.Month + "-" + autorizada.fin.Year + " " + autorizada.fin.Hour + ":" + autorizada.fin.Minute;
                        datos[5] = "Fecha y hora de Inicio Planificada; " + autorizada.inicio.Day + "-" + autorizada.inicio.Month + "-" + autorizada.inicio.Year + " " + autorizada.inicio.Hour + ":" + autorizada.inicio.Minute;
                        datos[6] = "Fecha y hora de Término Planificada; " + autorizada.fin.Day + "-" + autorizada.fin.Month + "-" + autorizada.fin.Year + " " + autorizada.fin.Hour + ":" + autorizada.fin.Minute;
                        datos[7] = "ID de solicitud; " + autorizada.idSolicitud;
                        datos[8] = "Planificada por; " + autorizada.planificadaPor;
                        datos[9] = "Autorizada por; " + autorizada.autorizadaPor;

                        dynamic email = new Email("avisos");
                        // se envia a ameco            
                        email.To = autorizada.correo;
                        email.Asunto = "Se ha autorizado una Solicitud de Servicios de Izaje realizada a su nombre";
                        email.TipoNotificacion = "Ha sido autorizada una solicitud de servicios de Izaje creada por usted. Esto significa que la solicitud ha quedado programada para realizarse para la fecha planificada.";

                        email.datosSolicitud = datos;

                        DateTime fecha_aux = DateTime.Now;
                        email.Fecha = "Fecha: " + fecha_aux;
                        email.Pendientes = "";
                        email.Cuerpo = "";
                        email.Send();
                    }
                    // metodo para verificar las nuevas que estan pendientes y enviarle un mail al usuario
                    //se consulta si hay solicitudes nuevas creadas con fecha menor a una semana

                    //verificar todos los estados
                    metodo_enviar_pendientes("NUEVA");
                    metodo_enviar_pendientes("PLANIFICADA");
                    metodo_enviar_pendientes("AUTORIZADA");
                    metodo_enviar_pendientes("FINALIZADA");
                    #endregion
                }
                catch
                {
                    Console.WriteLine("No se puedo enviar el correo");
                }
              
                ViewData["mensaje"] = "Su solicitud ha sido autorizada";

                List<Solicitud> autorizadas = Solicitud.obtenerAutorizadas();

                return RedirectToAction("verAutorizadas");
                //return View("verAutorizadas", autorizadas); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult metodoFinalizarSolicitud(FormCollection post)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                #region finalizarSol

                Solicitud finalizada = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);
                finalizada.estado = "FINALIZADA";

                Usuarios userActual = new Usuarios().obtenerdatos(Session["nombre"].ToString());
                finalizada.finalizadaPor = userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno;

                //Se ingresan las fechas:
                finalizada.fechas = new List<string>();

                for (int i = 0; post["fechaSol" + i] != null; i++)
                {
                    finalizada.fechas.Add(post["fechaSol" + i]);
                }

                //Se ingresan las hora reloj inicial 1
                finalizada.horaRelojInicial1 = new List<string>();

                for (int i = 0; post["horaRelojInicial1" + i] != null; i++)
                {
                    finalizada.horaRelojInicial1.Add(post["horaRelojInicial1" + i]);
                }

                //Se ingresan las hora reloj final 1
                finalizada.horaRelojFinal1 = new List<string>();

                for (int i = 0; post["horaRelojFinal1" + i] != null; i++)
                {
                    finalizada.horaRelojFinal1.Add(post["horaRelojFinal1" + i]);
                }

                //Se ingresan las hora horometro inicial 1
                finalizada.horaHorometroInicial1 = new List<string>();

                for (int i = 0; post["horaHorometroInicio1" + i] != null; i++)
                {
                    finalizada.horaHorometroInicial1.Add(post["horaHorometroInicio1" + i]);
                }

                //Se ingresan las hora horometro final 1
                finalizada.horaHorometroFinal1 = new List<string>();

                for (int i = 0; post["horaHorometroFin1" + i] != null; i++)
                {
                    finalizada.horaHorometroFinal1.Add(post["horaHorometroFin1" + i]);
                }
                if (!finalizada.idEquipo1.Equals("--"))
                {
                    //Se ingresan las hora reloj inicial 2
                    finalizada.horaRelojInicial2 = new List<string>();

                    for (int i = 0; post["horaRelojInicial2" + i] != null; i++)
                    {
                        finalizada.horaRelojInicial2.Add(post["horaRelojInicial2" + i]);
                    }

                    //Se ingresan las hora reloj final 2
                    finalizada.horaRelojFinal2 = new List<string>();

                    for (int i = 0; post["horaRelojFinal2" + i] != null; i++)
                    {
                        finalizada.horaRelojFinal2.Add(post["horaRelojFinal2" + i]);
                    }

                    //Se ingresan las hora horometro inicial 2
                    finalizada.horaHorometroInicial2 = new List<string>();

                    for (int i = 0; post["horaHorometroInicio2" + i] != null; i++)
                    {
                        finalizada.horaHorometroInicial2.Add(post["horaHorometroInicio2" + i]);
                    }

                    //Se ingresan las hora horometro final 2
                    finalizada.horaHorometroFinal2 = new List<string>();

                    for (int i = 0; post["horaHorometroFin2" + i] != null; i++)
                    {
                        finalizada.horaHorometroFinal2.Add(post["horaHorometroFin2" + i]);
                    }
                }

                if (Session["nombre"] != null) finalizada.actualizarEnBD(Session["nombre"].ToString());
                else finalizada.actualizarEnBD("");

                #endregion

                try
                {
                    #region mail

                    //revisar id de planificacion
                    string mail_usuario = finalizada.correo;
                    if (mail_usuario != null)
                    {
                        string[] datos = new string[11];

                        datos[0] = "Nombre del solicitante; " + finalizada.nombre;
                        datos[1] = "Área en donde se solicita que se realice el servicio; " + finalizada.area;
                        datos[2] = "Centro de costo asociado al servicio; " + finalizada.centroCosto;
                        datos[3] = "Fecha y hora de Inicio Solicitada; " + finalizada.inicio.Day + "-" + finalizada.inicio.Month + "-" + finalizada.inicio.Year + " " + finalizada.inicio.Hour + ":" + finalizada.inicio.Minute;
                        datos[4] = "Fecha y hora de Término Solicitada; " + finalizada.fin.Day + "-" + finalizada.fin.Month + "-" + finalizada.fin.Year + " " + finalizada.fin.Hour + ":" + finalizada.fin.Minute;
                        datos[5] = "Fecha y hora de Inicio Planificada; " + finalizada.inicio.Day + "-" + finalizada.inicio.Month + "-" + finalizada.inicio.Year + " " + finalizada.inicio.Hour + ":" + finalizada.inicio.Minute;
                        datos[6] = "Fecha y hora de Término Planificada; " + finalizada.fin.Day + "-" + finalizada.fin.Month + "-" + finalizada.fin.Year + " " + finalizada.fin.Hour + ":" + finalizada.fin.Minute;
                        datos[7] = "ID de solicitud; " + finalizada.idSolicitud;
                        datos[8] = "Planificada por; " + finalizada.planificadaPor;
                        datos[9] = "Autorizada por; " + finalizada.autorizadaPor;
                        datos[10] = "Finalizada por; " + finalizada.finalizadaPor;

                        dynamic email = new Email("avisos");
                        // se envia a ameco            
                        email.To = finalizada.correo;
                        email.Asunto = "Se ha finalizado una Solicitud de Servicios de Izaje realizada a su nombre";
                        email.TipoNotificacion = "Ha sido finalizada una solicitud de servicios de Izaje creada por usted. Esto significa que se han ingresado datos de uso de la máquina durante el servicio para que se calcule el costo del servicio en el siguiente estado de pago a generarse en el sistema.";

                        email.datosSolicitud = datos;

                        DateTime fecha_aux = DateTime.Now;
                        email.Fecha = "Fecha: " + fecha_aux;
                        email.Pendientes = "";
                        email.Cuerpo = "";
                        email.Send();
                    }
                    // metodo para verificar las nuevas que estan pendientes y enviarle un mail al usuario
                    //se consulta si hay solicitudes nuevas creadas con fecha menor a una semana

                    metodo_enviar_pendientes("NUEVA");
                    metodo_enviar_pendientes("PLANIFICADA");
                    metodo_enviar_pendientes("AUTORIZADA");
                    metodo_enviar_pendientes("FINALIZADA");
                    #endregion
                }
                catch
                {
                    Console.WriteLine("No se puedo enviar el correo");
                }
                
                ViewData["mensaje"] = "Su solicitud ha sido finalizada";

                List<Solicitud> finalizadas = Solicitud.obtenerFinalizadas();

                return RedirectToAction("verFinalizadas");
                //return View("verFinalizadas", finalizadas); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult metodoCorregirSolicitud(FormCollection post)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                #region corregirSolicitud

                Solicitud solicitud = Solicitud.obtenerSolicitud((string)post["solicitudElegida"]);

                Usuarios userActual = new Usuarios().obtenerdatos(Session["nombre"].ToString());

                if(solicitud.estado.Equals("AUTORIZADA"))
                {
                    solicitud.idEquipo1=post["equipo1"];
                    solicitud.idOperador1=post["operador1"];
                    solicitud.idRigger1=post["rigger1"];

                    if(!solicitud.idEquipo2.Equals("--"))
                    {
                        solicitud.idEquipo2=post["equipo2"];
                        solicitud.idOperador2=post["operador2"];
                        solicitud.idRigger2=post["rigger2"];
                    }

                    solicitud.actualizarEnBD("");
                }
                else if (solicitud.estado.Equals("FINALIZADA") || solicitud.estado.Equals("CONFIRMADA"))
                {
                    solicitud.idEquipo1=post["equipo1"];
                    solicitud.idOperador1=post["operador1"];
                    solicitud.idRigger1=post["rigger1"];

                    if(!solicitud.idEquipo2.Equals("--"))
                    {
                        solicitud.idEquipo2=post["equipo2"];
                        solicitud.idOperador2=post["operador2"];
                        solicitud.idRigger2=post["rigger2"];
                    }
                    
                    Solicitud.eliminarSolicitud(solicitud.idSolicitud);
                    
                    string estadoSol = solicitud.estado;

                    solicitud.estado = "NUEVA";
                    solicitud.actualizarEnBD("");

                    solicitud.estado = estadoSol;
                    solicitud.actualizarEnBD("");
                }

                else solicitud.actualizarEnBD("");

                #endregion

                List<Solicitud> todas = Solicitud.obtenerTodas();

                return RedirectToAction("verSolicitudes");
                //return View("verSolicitudes", todas); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }        
        [HttpPost]
        public ActionResult metodoConfirmarSolicitud(FormCollection post)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("jefeArea")))
            {
                #region confirmarSol

                Solicitud confirmada = Solicitud.obtenerSolicitud((string)post["idsolicitud"]);
                confirmada.estado = "CONFIRMADA";

                Usuarios userActual = new Usuarios().obtenerdatos(Session["nombre"].ToString());
                confirmada.confirmadaPor = userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno;

                if (Session["nombre"] != null)
                {                    
                    confirmada.actualizarEnBD(userActual.nombres + " " + userActual.apellido_paterno + " " + userActual.apellido_materno);
                }                
                else confirmada.actualizarEnBD("");

                #endregion


                try
                {
                    #region mail
                    //revisar id de planificacion
                    string mail_usuario = confirmada.correo;

                    if (mail_usuario != null)
                    {
                        string[] datos = new string[12];

                        datos[0] = "Nombre del solicitante; " + confirmada.nombre;
                        datos[1] = "Área en donde se solicita que se realice el servicio; " + confirmada.area;
                        datos[2] = "Centro de costo asociado al servicio; " + confirmada.centroCosto;
                        datos[3] = "Fecha y hora de Inicio Solicitada; " + confirmada.inicio.Day + "-" + confirmada.inicio.Month + "-" + confirmada.inicio.Year + " " + confirmada.inicio.Hour + ":" + confirmada.inicio.Minute;
                        datos[4] = "Fecha y hora de Término Solicitada; " + confirmada.fin.Day + "-" + confirmada.fin.Month + "-" + confirmada.fin.Year + " " + confirmada.fin.Hour + ":" + confirmada.fin.Minute;
                        datos[5] = "Fecha y hora de Inicio Planificada; " + confirmada.inicio.Day + "-" + confirmada.inicio.Month + "-" + confirmada.inicio.Year + " " + confirmada.inicio.Hour + ":" + confirmada.inicio.Minute;
                        datos[6] = "Fecha y hora de Término Planificada; " + confirmada.fin.Day + "-" + confirmada.fin.Month + "-" + confirmada.fin.Year + " " + confirmada.fin.Hour + ":" + confirmada.fin.Minute;
                        datos[7] = "ID de solicitud; " + confirmada.idSolicitud;
                        datos[8] = "Planificada por; " + confirmada.planificadaPor;
                        datos[9] = "Autorizada por; " + confirmada.autorizadaPor;
                        datos[10] = "Finalizada por; " + confirmada.finalizadaPor;
                        datos[11] = "Confirmada por; " + confirmada.confirmadaPor;

                        dynamic email = new Email("avisos");
                        // se envia a ameco            
                        email.To = confirmada.correo;
                        email.Asunto = "Se ha confirmado una Solicitud de Servicios de Izaje realizada a su nombre";
                        email.TipoNotificacion = "Ha sido confirmada una solicitud de servicios de Izaje creada por usted. Esto significa que los datos de finalizacion de la misma han sido aceptados para ser contabilizados en el próximo estado de pago a generarse en el sistema.";

                        email.datosSolicitud = datos;

                        DateTime fecha_aux = DateTime.Now;
                        email.Fecha = "Fecha: " + fecha_aux;
                        email.Pendientes = "";
                        email.Cuerpo = "";
                        email.Send();
                    }
                    metodo_enviar_pendientes("NUEVA");
                    metodo_enviar_pendientes("PLANIFICADA");
                    metodo_enviar_pendientes("AUTORIZADA");
                    metodo_enviar_pendientes("FINALIZADA");
                    #endregion
                }
                catch
                {
                    Console.WriteLine("No se puedo enviar el correo");
                }

                ViewData["mensaje"] = "Su solicitud ha sido confirmada";

                List<Solicitud> confirmadas = Solicitud.obtenerConfirmadas();

                return RedirectToAction("verConfirmadas");
                //return View("verConfirmadas", confirmadas); //**UPDATE
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    
        public void metodo_enviar_pendientes(string estado)
        {
            Mail correo = new Mail();
            DateTime fecha_aux = DateTime.Now;
            string texto1 = "";
            string texto2 = "";
            if (estado.Equals("NUEVA"))
            {
                texto1 = "planificar";
                texto2 = "PLANIFICAR";
            }
            if (estado.Equals("PLANIFICADA"))
            {
                texto1 = "autorizar";
                texto2 = "AUTORIZAR";
            }
            if (estado.Equals("AUTORIZADA"))
            {
                texto1 = "finalizar";
                texto2 = "FINALIZAR";
            }
            if (estado.Equals("FINALIZADA"))
            {
                texto1 = "confirmar";
                texto2 = "CONFIRMAR";
            }


            // comprobar cuando fue elultimo envio de aviso pendiente
            DateTime ultimo_aviso = correo.ultimo_aviso(estado);
            //fecha de veirifiacion denull bd
            DateTime fecha_null = new DateTime(2000, 02, 20, 00, 00, 00, 000);
            if (ultimo_aviso != fecha_null)
            {
                ultimo_aviso = ultimo_aviso.AddHours(24);
                if (ultimo_aviso < fecha_aux)
                {

                    //obtenemos todas las areas
                    string[] areas_solicitud = correo.obtener_todas_area();


                    foreach (string area in areas_solicitud)
                    {
                        //obtenemos todos las solicitudes pendientes deacuerdo al estado y al area
                        string[] id = correo.ver_pendientes(fecha_aux.AddDays(-7), estado, area);
                        if (!string.IsNullOrEmpty(id[0]))
                        {
                            // mail donde se debe enviar el pendientes nuevas
                            string[] usuarios = correo.mail_usuarios(estado, area);
                            foreach (string mail in usuarios)
                            {
                                dynamic email_pendientes = new Email("pendientes");
                                email_pendientes.To = mail;
                                email_pendientes.Asunto = "Existen solicitudes de Izaje pendientes por " + texto1;
                                email_pendientes.Cuerpo = "Existen " + id.Length + " solicitudes pendientes por " + texto2 + " :";
                                email_pendientes.Pendientes = id;
                                email_pendientes.Send();
                            }

                            //actualizar fecha ultimo aviso
                            correo.actualizar_fecha_ultimo_aviso(fecha_aux, estado);
                        }
                    }
                }
            }
            else
            {
                // insertar fecha actual como ultimo aviso
                correo.actualizar_fecha_ultimo_aviso(fecha_aux, estado);
            }
        }
    }        
}
