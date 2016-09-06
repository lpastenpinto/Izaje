using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class NotificacionesController : Controller
    {
        //
        // GET: /Notificaciones/
        [HttpGet]
        public ActionResult All()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                //recuperar all Notificaciones
                List<Notificaciones> listaL = new NotificacionesGet().licencias();
                List<Notificaciones> listaC = new NotificacionesGet().certificados();
                List<Notificaciones> listaE = new NotificacionesGet().equipos();
                List<Notificaciones> listaEc = new NotificacionesGet().equipos_certificados();

                listaL = listaL.Concat(listaC).ToList();
                listaL = listaL.Concat(listaE).ToList();
                listaL = listaL.Concat(listaEc).ToList();

                return View(listaL);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public string load_notificaciones()
        {
            List<Notificaciones> listaL = new NotificacionesGet().licencias();
            List<Notificaciones> listaC = new NotificacionesGet().certificados();
            List<Notificaciones> listaE = new NotificacionesGet().equipos();
            List<Notificaciones> listaEc = new NotificacionesGet().equipos_certificados();
  
            string cadena = string.Empty;
            foreach (Notificaciones item in listaL)
            {
                string fecha_temp = item.fecha.Year.ToString() + '/' + item.fecha.Month.ToString() + '/' + item.fecha.Day.ToString();
                cadena += item.id + ',' + item.categoria + ',' + item.tipo + ',' + item.documento + ',' + fecha_temp + ';';
            }
            foreach (Notificaciones item in listaC)
            {
                string fecha_temp = item.fecha.Year.ToString() + '/' + item.fecha.Month.ToString() + '/' + item.fecha.Day.ToString();
                cadena += item.id + ',' + item.categoria + ',' + item.tipo + ',' + item.documento + ',' + fecha_temp + ';';
            }
            foreach (Notificaciones item in listaE)
            {
                string fecha_temp = item.fecha.Year.ToString() + '/' + item.fecha.Month.ToString() + '/' + item.fecha.Day.ToString();
                cadena += item.id + ',' + item.categoria + ',' + item.tipo + ',' + item.documento + ',' + fecha_temp + ';';
            }
            foreach (Notificaciones item in listaEc)
            {
                string fecha_temp = item.fecha.Year.ToString() + '/' + item.fecha.Month.ToString() + '/' + item.fecha.Day.ToString();
                cadena += item.id + ',' + item.categoria + ',' + item.tipo + ',' + item.documento + ',' + fecha_temp + ';';
            }
            return cadena.TrimEnd(';');
        }
	}
}