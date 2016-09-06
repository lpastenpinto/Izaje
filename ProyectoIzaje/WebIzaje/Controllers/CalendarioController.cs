using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class CalendarioController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public string getEventos()
        {
            List<Solicitud> solicitudes = Solicitud.obtenerTodas();
            string json = "[";
           
            foreach(Solicitud solicitud in solicitudes){
                if (!solicitud.estado.Equals("NUEVA")) 
                {

                    string hora = solicitud.inicioCorregido.Hour.ToString();
                    if (hora.Length == 1) hora = "0" + hora;
                    string minuto = solicitud.inicioCorregido.Minute.ToString();
                    if (minuto.Length == 1) minuto = "0" + minuto;

                    DateTime start = solicitud.inicioCorregido;
                    DateTime end = solicitud.finCorregido;

                    json += "{";
                    json += "\"id\": \"" + solicitud.idSolicitud + "\",";
                    json += "\"title\": \"" + hora + ":" 
                        + minuto + " - " + solicitud.descripcionLugar 
                        + " - " + solicitud.criticidadCorregida + "\",";
                    if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
                    {
                        json += "\"url\": \"" + @Url.Action("verSolicitud", "Solicitud", new { idSolicitud = solicitud.idSolicitud }) + "\",";
                    }
                    else
                    {
                        json += "\"url\": \"" + @Url.Action("Index", "Home") + "\",";
                    }

                    if (solicitud.estado.Equals("FINALIZADA") || solicitud.estado.Equals("CONFIRMADA")) json += "\"class\": \"event-inverse\",";
                    else if (solicitud.estado.Equals("PLANIFICADA")) json += "\"class\": \"event-warning\",";
                    else if (solicitud.estado.Equals("AUTORIZADA")) json += "\"class\": \"event-important\",";

                    json += "\"start\":"+(new DateTime(start.Year,start.Month,start.Day,start.Hour,start.Minute,start.Millisecond).ToUniversalTime().Ticks - 621355968000000000) / 10000+",";
                    json += "\"end\":" + (new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Millisecond).ToUniversalTime().Ticks - 621355968000000000) / 10000;                    
                    json += "},";
                }                
            }
            /*

            {
                json += "{";
                json += "\"id\": \" 123 \",";
                json += "\"title\": \" titulos:\",";
                json += "\"class\": \"event-important\",";

                json += "\"start\":" + (new DateTime(2014,10,13,16,00,00,00).ToUniversalTime().Ticks - 621355968000000000) / 10000 + ",";
                json += "\"end\":" + (new DateTime(2014, 10, 13, 18, 00, 00, 00).ToUniversalTime().Ticks - 621355968000000000) / 10000;
                json += "},";
            }           
            */ 
            json=json.TrimEnd(',');
            json += "]";

            return json; 
        }
    }
}