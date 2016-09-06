using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class ReportesController : Controller
    {
        //
        // GET: /Reportes/
        public ActionResult Index()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Todos()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                string[] reportes = new string[] { "Responsables", "Servicios por Area", "Servicios por Centro de Costo", "Servicios por Criticidad", "Estados Equipos" };
                ViewData["Reportes"] = reportes;
                return View();
            }
            else 
            {
                return RedirectToAction("Index","Home");
            }
        }
        public ActionResult tasaUso()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult indicadores(string FechaInicio, string FechaFin, string FechaHoy)
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                DateTime hoy;
                List<Solicitud> solicitudes;

                if (string.IsNullOrEmpty(FechaInicio) || string.IsNullOrEmpty(FechaFin) || string.IsNullOrEmpty(FechaHoy)) 
                {                    
                    hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime Inicio = DateTime.Now.AddMonths(-1);
                    DateTime Fin = DateTime.Now.AddMonths(1);

                    string diaInicio = Inicio.Day.ToString();
                    if (diaInicio.Length == 1) diaInicio = "0" + diaInicio;

                    string mesInicio = Inicio.Month.ToString();
                    if (mesInicio.Length == 1) mesInicio = "0" + mesInicio;

                    string añoInicio = Inicio.Year.ToString();

                    string diaFin = Fin.Day.ToString();
                    if (diaFin.Length == 1) diaFin = "0" + diaFin;

                    string mesFin = Fin.Month.ToString();
                    if (mesFin.Length == 1) mesFin = "0" + mesFin;

                    string añoFin = Fin.Year.ToString();

                    string inicio = diaInicio + "/" + mesInicio + "/" + añoInicio;
                    string fin = diaFin + "/" + mesFin + "/" + añoFin;

                    solicitudes = new MetodosReportesSolicitud().mostrar_todas_solicitudes_por_fecha(inicio, fin);
                }
                else 
                {
                    int año = int.Parse(FechaHoy.Split('/')[2]);
                    int mes = int.Parse(FechaHoy.Split('/')[1]);
                    int dia = int.Parse(FechaHoy.Split('/')[0]);

                    hoy = new DateTime(año, mes, dia);
                    solicitudes = new MetodosReportesSolicitud().mostrar_todas_solicitudes_por_fecha(FechaInicio, FechaFin);
                }
                

                int cantidadParaMasDeSieteDias=0;
                int cantidadPlanificadasFuturas = 0;
                int cantidadFuturas = 0;
                int cantidadConfirmadas=0;
                double cantidadHorasEstimadas = 0;
                double cantidadRealHoras = 0;

                                
                for (int i = 0; i < solicitudes.Count; i++) 
                {
                    if (solicitudes[i].inicio.CompareTo(hoy) > 0)
                    {
                        //Se cuentan las solicitudes futuras
                        cantidadFuturas++;

                        //Se cuentan las solicitudes planificadas con antelación
                        DateTime sieteDias = solicitudes[i].inicio.AddDays(-7);
                        if (sieteDias.CompareTo(solicitudes[i].fechaCreacion) > 0)
                        {
                            cantidadParaMasDeSieteDias++;
                        }

                        //Se cuentan las planificadas futuras
                        if (!solicitudes[i].estado.Equals("NUEVA"))
                            cantidadPlanificadasFuturas++;
                    }
                    else
                    {
                        if (solicitudes[i].estado.Equals("CONFIRMADA")) cantidadConfirmadas++;
                        if (solicitudes[i].estado.Equals("CONFIRMADA") || solicitudes[i].estado.Equals("FINALIZADA"))
                        {
                            cantidadHorasEstimadas += solicitudes[i].tiempoEstimadoOperacionCorregido;
                            for (int j = 0; j < solicitudes[i].fechas.Count; j++)
                            {
                                cantidadRealHoras += datosTasaUso.obtenerDiferenciaRelojPublic(
                                    solicitudes[i].horaRelojInicial1[j], solicitudes[i].horaRelojFinal1[j]);

                                if (!solicitudes[i].idEquipo2.Equals("--")) 
                                {
                                    cantidadRealHoras += datosTasaUso.obtenerDiferenciaRelojPublic(
                                        solicitudes[i].horaRelojInicial2[j], solicitudes[i].horaRelojFinal2[j]);
                                }
                            }
                        }
                    }                
                }

                int cantidadNoConfirmadas = solicitudes.Count - cantidadFuturas - cantidadConfirmadas;

                double tasaSolicitadasConAnterioridad = 0;
                double tasaPlanificadasFuturas = 0;
                double tasaConfirmadas = 0;
                double tasaNoConfirmadas = 0;
                double tasaExactitudEstimacionHoras = 0;
                if (cantidadFuturas != 0)
                {
                    tasaSolicitadasConAnterioridad = (double)cantidadParaMasDeSieteDias / cantidadFuturas;
                    tasaPlanificadasFuturas = (double)cantidadPlanificadasFuturas / cantidadFuturas;
                }

                if (solicitudes.Count > 0)
                {
                    tasaConfirmadas = (double)cantidadConfirmadas / (solicitudes.Count-cantidadFuturas);
                    tasaNoConfirmadas = (double)cantidadNoConfirmadas / (solicitudes.Count - cantidadFuturas);
                }
                if (cantidadHorasEstimadas != 0) 
                {
                    tasaExactitudEstimacionHoras = (double)cantidadRealHoras / cantidadHorasEstimadas;
                }

                ViewData["cantidadSolicitadasConAnterioridad"]=cantidadParaMasDeSieteDias;
                ViewData["tasaSolicitadasConAnterioridad"] = Math.Round(tasaSolicitadasConAnterioridad * 100, 2);
                
                ViewData["cantidadPlanificadas"] = cantidadPlanificadasFuturas;
                ViewData["tasaPlanificadas"] = Math.Round(tasaPlanificadasFuturas * 100, 2);
                
                ViewData["cantidadConfirmadas"] = cantidadConfirmadas;
                ViewData["cantidadNoConfirmadas"] = cantidadNoConfirmadas;
                ViewData["tasaConfirmadas"] = Math.Round(tasaConfirmadas * 100, 2);
                ViewData["tasaNoConfirmadas"] = Math.Round(tasaNoConfirmadas * 100, 2);
                
                ViewData["cantidadHorasReales"] = cantidadRealHoras;
                ViewData["cantidadHorasEstimadas"] = cantidadHorasEstimadas;
                ViewData["tasaExactitudEstimacionHoras"] = Math.Round(tasaExactitudEstimacionHoras * 100, 2);

                ViewData["totalFuturas"] = cantidadFuturas;
                ViewData["totalPrevias"] = solicitudes.Count - cantidadFuturas;

                
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Servicio_Area()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Servicio_CentroCosto()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Responsables()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Estado_Equipos()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Servicio_Criticidad()
        {
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        #region Reportes que Trabajan con Modelo Equipo
        public FileContentResult verReporteEquipo(string FechaIngreso)
        {
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            List<DatosEquipo> lista_datos = new List<DatosEquipo>();
            MetodosReportesEquipo equipos = new MetodosReportesEquipo();
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            LocalReport reporte_local = new LocalReport();
            // pasa la ruta donde se encuentra el reporte
            

                reporte_local.ReportPath = Server.MapPath("~/Report/Estado_Equipos.rdlc");

            
            // creamos un recurso de datos del tipo report
            ReportDataSource conjunto_datos = new ReportDataSource();

            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                lista_datos = equipos.mostrar_lista_datos_por_fecha(FechaIngreso);
            }
            // le asginamos al conjuto de datos el nombre del datasource del reporte
            conjunto_datos.Name = "DataSet1";
            // se le asigna el datasource el conjunto de datos desde el modelo
            conjunto_datos.Value = lista_datos;
            // se agrega el conjunto de datos del tipo report al reporte local
            reporte_local.DataSources.Add(conjunto_datos);
            // datos para renderizar como se mostrara el reporte
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
                 "  <OutputFormat>jpeg</OutputFormat>" +
                 "  <PageWidth>11in</PageWidth>" +
                 "  <PageHeight>8.5in</PageHeight>" +
                 "  <MarginTop>0.5in</MarginTop>" +
                 "  <MarginLeft>1in</MarginLeft>" +
                 "  <MarginRight>1in</MarginRight>" +
                 "  <MarginBottom>0.5in</MarginBottom>" +
                 "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Se renderiza el reporte            
            renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // el reporte es mostrado como una imagen
            return File(renderedBytes, mimeType);

        }
        public void GuardarReporte_Equipo(string FechaIngreso)
        {

            string Extension = "PDF";
            List<DatosEquipo> lista_datos = new List<DatosEquipo>();
            MetodosReportesEquipo equipos = new MetodosReportesEquipo();
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            LocalReport reporte_local = new LocalReport();
            // pasa la ruta donde se encuentra el reporte
            
            
                reporte_local.ReportPath = Server.MapPath("~/Report/Estado_Equipos.rdlc");

            
            // creamos un recurso de datos del tipo report
            ReportDataSource conjunto_datos = new ReportDataSource();
            // le asginamos al conjuto de datos el nombre del datasource del reporte
            conjunto_datos.Name = "DataSet1";
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                lista_datos = equipos.mostrar_lista_datos_por_fecha(FechaIngreso);
            }
            // se le asigna el datasource el conjunto de datos desde el modelo
            conjunto_datos.Value = lista_datos;
            // se agrega el conjunto de datos del tipo report al reporte local
            reporte_local.DataSources.Add(conjunto_datos);
            // datos para renderizar como se mostrara el reporte
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
                 "  <OutputFormat>jpeg</OutputFormat>" +
                 "  <PageWidth>11in</PageWidth>" +
                 "  <PageHeight>8.5in</PageHeight>" +
                 "  <MarginTop>0.5in</MarginTop>" +
                 "  <MarginLeft>1in</MarginLeft>" +
                 "  <MarginRight>1in</MarginRight>" +
                 "  <MarginBottom>0.5in</MarginBottom>" +
                 "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Se renderiza el reporte            
            renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // el reporte es mostrado como una imagen
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=Estados Equipos." + Extension);
            Response.BinaryWrite(renderedBytes); // create the file
            Response.Flush(); // send it to the client to download


        }
        #endregion

        #region Reportes que trabajan con Modelo Solicitud
        public FileContentResult VistaReporte_Solicitud(string FechaInicio, string FechaFin, string Tipo_reporte)
        {
                // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
                List<Solicitud> lista_datos = new List<Solicitud>();
                MetodosReportesSolicitud datos = new MetodosReportesSolicitud();
                // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
                LocalReport reporte_local = new LocalReport();
                // pasa la ruta donde se encuentra el reporte
                if (Tipo_reporte.Equals("Servicios por Area"))
                {
                    reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_Area.rdlc");

                }
                if (Tipo_reporte.Equals("Servicios por Centro de Costo"))
                {
                    reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_CentroCosto.rdlc");

                }
                if (Tipo_reporte.Equals("Responsables"))
                {
                    reporte_local.ReportPath = Server.MapPath("~/Report/Responsables.rdlc");

                }
                if (Tipo_reporte.Equals("Servicios por Criticidad"))
                {
                    reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_Criticidad.rdlc");
                }
                // creamos un recurso de datos del tipo report
                ReportDataSource conjunto_datos = new ReportDataSource();
                // le asginamos al conjuto de datos el nombre del datasource del reporte
                conjunto_datos.Name = "DataSet1";
                if (Session["rol"] != null && Session["rol"].Equals("admin"))
                {
                    lista_datos = datos.mostrar_solicitudes_por_fecha(FechaInicio, FechaFin);
                }
                // se le asigna el datasource el conjunto de datos desde el modelo
                conjunto_datos.Value = lista_datos;
                // se agrega el conjunto de datos del tipo report al reporte local
                reporte_local.DataSources.Add(conjunto_datos);
                // datos para renderizar como se mostrara el reporte
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo = "<DeviceInfo>" +
                     "  <OutputFormat>jpeg</OutputFormat>" +
                     "  <PageWidth>11in</PageWidth>" +
                     "  <PageHeight>8.5in</PageHeight>" +
                     "  <MarginTop>0.5in</MarginTop>" +
                     "  <MarginLeft>1in</MarginLeft>" +
                     "  <MarginRight>1in</MarginRight>" +
                     "  <MarginBottom>0.5in</MarginBottom>" +
                     "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                //Se renderiza el reporte            
                renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                // el reporte es mostrado como una imagen
                return File(renderedBytes, mimeType);
        }
        public void GuardarReporte_Solicitud(string FechaInicio, string FechaFin, string Tipo_reporte)
        {
            
            string Extension = "PDF";
            List<Solicitud> lista_datos = new List<Solicitud>();
            MetodosReportesSolicitud datos = new MetodosReportesSolicitud();
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            LocalReport reporte_local = new LocalReport();
            // pasa la ruta donde se encuentra el reporte
            if (Tipo_reporte.Equals("Servicios por Area"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_Area.rdlc");

            }
            if (Tipo_reporte.Equals("Servicios por Centro de Costo"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_CentroCosto.rdlc");

            }
            if (Tipo_reporte.Equals("Responsables"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/Responsables.rdlc");

            }
            if (Tipo_reporte.Equals("Servicios por Criticidad"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/Servicio_Criticidad.rdlc");
            }
            // creamos un recurso de datos del tipo report
            ReportDataSource conjunto_datos = new ReportDataSource();
            // le asginamos al conjuto de datos el nombre del datasource del reporte
            conjunto_datos.Name = "DataSet1";
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                lista_datos = datos.mostrar_solicitudes_por_fecha(FechaInicio, FechaFin);
            }
            // se le asigna el datasource el conjunto de datos desde el modelo
            conjunto_datos.Value = lista_datos;
            // se agrega el conjunto de datos del tipo report al reporte local
            reporte_local.DataSources.Add(conjunto_datos);
            // datos para renderizar como se mostrara el reporte
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
                 "  <OutputFormat>jpeg</OutputFormat>" +
                 "  <PageWidth>11in</PageWidth>" +
                 "  <PageHeight>8.5in</PageHeight>" +
                 "  <MarginTop>0.5in</MarginTop>" +
                 "  <MarginLeft>1in</MarginLeft>" +
                 "  <MarginRight>1in</MarginRight>" +
                 "  <MarginBottom>0.5in</MarginBottom>" +
                 "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Se renderiza el reporte            
            renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // el reporte es mostrado como una imagen
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + Tipo_reporte + "." + Extension);
            Response.BinaryWrite(renderedBytes); // create the file
            Response.Flush(); // send it to the client to download


        }
        #endregion

        #region reportes de tasa de uso efectivo
        public FileContentResult VistaReporte_TasaUso(string FechaInicio, string FechaFin, string Tipo_reporte)
        {
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            List<Solicitud> lista_datos = new List<Solicitud>();
            MetodosReportesSolicitud datos = new MetodosReportesSolicitud();
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            LocalReport reporte_local = new LocalReport();
            // pasa la ruta donde se encuentra el reporte
            if (Tipo_reporte.Equals("todos"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Todos.rdlc");

            }
            else if (Tipo_reporte.Equals("equipo"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Equipo.rdlc");

            }
            else if (Tipo_reporte.Equals("area"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Area.rdlc");

            }
            else if (Tipo_reporte.Equals("centro de costo"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_CentroCosto.rdlc");

            }
            else if (Tipo_reporte.Equals("empresa"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Empresa.rdlc");
            }
            // creamos un recurso de datos del tipo report
            ReportDataSource conjunto_datos = new ReportDataSource();
            // le asginamos al conjuto de datos el nombre del datasource del reporte
            conjunto_datos.Name = "DataSet1";
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                lista_datos = datos.mostrar_solicitudes_por_fecha(FechaInicio, FechaFin);
            }
            //Se transforman los datos a datos de tasa efectiva de uso
            List<datosTasaUso> datosTasaEfectivaUso = datosTasaUso.solicitudesAdatosTasaUso(lista_datos);
            // se le asigna el datasource el conjunto de datos desde el modelo
            conjunto_datos.Value = datosTasaEfectivaUso;
            // se agrega el conjunto de datos del tipo report al reporte local
            reporte_local.DataSources.Add(conjunto_datos);
            // datos para renderizar como se mostrara el reporte
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
                 "  <OutputFormat>jpeg</OutputFormat>" +
                 "  <PageWidth>12in</PageWidth>" +
                 "  <PageHeight>10in</PageHeight>" +
                 "  <MarginTop>0.5in</MarginTop>" +
                 "  <MarginLeft>1in</MarginLeft>" +
                 "  <MarginRight>1in</MarginRight>" +
                 "  <MarginBottom>0.5in</MarginBottom>" +
                 "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Se renderiza el reporte            
            renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // el reporte es mostrado como una imagen
            return File(renderedBytes, mimeType);
        }
        public void GuardarReporte_TasaUso(string FechaInicio, string FechaFin, string Tipo_reporte)
        {
            string Extension = "PDF";
            List<Solicitud> lista_datos = new List<Solicitud>();
            MetodosReportesSolicitud datos = new MetodosReportesSolicitud();
            // Nota los datos creados en el dataset deben ser con el mismo nombre que tengan los Datos del Modelo
            LocalReport reporte_local = new LocalReport();
            // pasa la ruta donde se encuentra el reporte
            // pasa la ruta donde se encuentra el reporte
            if (Tipo_reporte.Equals("todos"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Todos.rdlc");

            }
            if (Tipo_reporte.Equals("area"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Area.rdlc");

            }
            if (Tipo_reporte.Equals("centro de costo"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_CentroCosto.rdlc");

            }
            if (Tipo_reporte.Equals("empresa"))
            {
                reporte_local.ReportPath = Server.MapPath("~/Report/tasaUso_Empresa.rdlc");
            }
            // creamos un recurso de datos del tipo report
            ReportDataSource conjunto_datos = new ReportDataSource();
            // le asginamos al conjuto de datos el nombre del datasource del reporte
            conjunto_datos.Name = "DataSet1";
            if (Session["rol"] != null && Session["rol"].Equals("admin"))
            {
                lista_datos = datos.mostrar_solicitudes_por_fecha(FechaInicio, FechaFin);
            }
            //Se transforman los datos a datos de tasa efectiva de uso
            List<datosTasaUso> datosTasaEfectivaUso = datosTasaUso.solicitudesAdatosTasaUso(lista_datos);
            // se le asigna el datasource el conjunto de datos desde el modelo
            conjunto_datos.Value = datosTasaEfectivaUso;
            // se agrega el conjunto de datos del tipo report al reporte local
            reporte_local.DataSources.Add(conjunto_datos);
            // datos para renderizar como se mostrara el reporte
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
                 "  <OutputFormat>jpeg</OutputFormat>" +
                 "  <PageWidth>12in</PageWidth>" +
                 "  <PageHeight>10in</PageHeight>" +
                 "  <MarginTop>0.5in</MarginTop>" +
                 "  <MarginLeft>1in</MarginLeft>" +
                 "  <MarginRight>1in</MarginRight>" +
                 "  <MarginBottom>0.5in</MarginBottom>" +
                 "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Se renderiza el reporte            
            renderedBytes = reporte_local.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // el reporte es mostrado como una imagen
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + Tipo_reporte + "." + Extension);
            Response.BinaryWrite(renderedBytes); // create the file
            Response.Flush(); // send it to the client to download


        }
        #endregion
    }
}