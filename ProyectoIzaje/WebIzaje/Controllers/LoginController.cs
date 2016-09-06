using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIzaje.Models;

namespace WebIzaje.Controllers
{
    public class LoginController : Controller
    {
        static int flag = -1;
        //Gerencias
        public ActionResult Index()
        {
            ViewBag.success = flag;
            flag = -1;
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(FormCollection post)
        {
            if (new conexion().revisarUsuarioPassword(post["nombre"], post["password"]))
            {
                Session["nombre"] = post["nombre"];
                Session["rol"] = new conexion().obtenerRol(post["nombre"]);
                Session["area"] = new conexion().obtenerAreaJefeArea(post["nombre"]);

                return RedirectToAction("Index", "Home");
            }
            else { flag = 0; }
            return RedirectToAction("Index");
        }

        public ActionResult LogOut()
        {
            Session.RemoveAll();

            return RedirectToAction("Index", "Home");
        }
    }
}