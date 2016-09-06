using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class InfoController : Controller
    {
        //
        // GET: /Info/

        [HttpGet]
        public ActionResult All(string id)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
            string item1 = string.Empty;
            string item2 = string.Empty;
            string item3 = string.Empty;
            string item4 = string.Empty;

            if (!string.IsNullOrEmpty(id) && id.Contains("item1") && id.Contains("item2") && id.Contains("item3") && id.Contains("item4"))
            {
                string[] item = id.Split(';');
                item1 = item[0].Split('=')[1];
                item2= item[1].Split('=')[1];
                item3 = item[2].Split('=')[1];
                item4 = item[3].Split('=')[1];
                string[] date_in = item3.Split('-');
                DateTime date_inicio = new DateTime(int.Parse(date_in[2]),int.Parse(date_in[1]),int.Parse(date_in[0]));
                string[] date_out = item4.Split('-');
                DateTime date_fin = new DateTime(int.Parse(date_out[2]), int.Parse(date_out[1]), int.Parse(date_out[0]));
                

                if (item1.Equals("all")) item1 = string.Empty;
                if (item2.Equals("all")) item2 = string.Empty;

                ViewBag.item1 = item1;
                ViewBag.item2 = item2;
                ViewBag.item3 = item3;
                ViewBag.item4 = item4;

                return View(Solicitud.obtenerTodas_info(item1.Trim(), item2.Trim(), date_inicio, date_fin));
            }

            ViewBag.item1 = item1;
            ViewBag.item2 = item2;
            ViewBag.item3 = item3;
            ViewBag.item4 = item4;

            return View(Solicitud.obtenerTodas_info(DateTime.Now));

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public string GetTrabajadores(){
            string cadena = string.Empty;
            foreach (TrabajadorDatos x in new TrabajadorGet().AllDatos())
            {
                cadena += x.rut + " : " + x.nombre + " " + x.apellidoP + " " + x.apellidoM + ",";
            }
            cadena = cadena.TrimEnd(',');
            return cadena;
        }

        public string GetEquipos()
        {
            string cadena = string.Empty;
            foreach (DatosEquipo x in new conexion().obtener_lista_equipos())
            {
                
                cadena += x.tag + " : " + x.modelo + ",";
            }
            cadena = cadena.TrimEnd(',');
            return cadena;
        }
	}
}