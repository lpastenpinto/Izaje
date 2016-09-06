using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Excel=Microsoft.Office.Interop.Excel;
using System.Web.Mvc;
using WebIzaje.Models;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebIzaje.Controllers
{
    public class EstadoPagoController : Controller
    {
        //
        // GET: /EstadoPago/

        static int flag = -1;
        string tipo_equipo_aux = "";
        double total_dhorometro,total_dreloj,total_contador,total_valordia,total_valormlp,total_valord,_costo_hora,uf,total_saltos_horometro;
        
        List<double>ultimo_horometro=new List<double>();
        double contadorAnterior=0;
        [HttpGet]
        public ActionResult Generarestadopago()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin")))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
       
        [HttpGet]
        public ActionResult Mostrareresumenestadopago()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                // aki obtenemos todoslos servicios que existen en la bd
                ResumenestadopagoSelect resumenes = new ResumenestadopagoSelect();
                ViewBag.success = flag;
                flag = -1;
                return View(resumenes.obtener_resumen_estadopago());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }           
            
        }
        // view que muestra el detalle del estado de pago se debe obtener los datos deacuerdo ala ID
        [HttpGet]
        public ActionResult Listadetalleestadopago(string id_pago_tipo)
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                DetalleEstadoPagoSelect datos_resumen = new DetalleEstadoPagoSelect();
                List<DetalleEstadoPago> lista_datos = new List<DetalleEstadoPago>();
                ResumenEstadoPago datos_id = new ResumenEstadoPago();
                ResumenestadopagoSelect seleccionar = new ResumenestadopagoSelect();
                datos_id = seleccionar.obtener_resumen_estadopago_id(id_pago_tipo);

                //obtener todos los id de detalleestadopago por id_pago
                lista_datos = datos_resumen.listadetallesestadopago(datos_id.id_estadopagotipo);
                // calculo rigger
                List<string> lista_auxiliar = new List<string>();

                RiggerDiasTrabajados datos_rigger = new RiggerDiasTrabajados();
                // obtener datos del rigger segun id_estado pago
                RiggerTotales datos_totales_rigger = new RiggerTotales();
                List<RiggerTotales> lista_totales_rigger = new List<RiggerTotales>();
                lista_totales_rigger = datos_totales_rigger.calcular_totales(id_pago_tipo);
                List<RiggerDiasTrabajados> lista_rigger_para_mostrar = new List<RiggerDiasTrabajados>();
                lista_rigger_para_mostrar = datos_rigger.obtene_riggers_por_idestadopago(id_pago_tipo);

                ViewData["Riggers"] = lista_rigger_para_mostrar;
                ViewData["TotalesRiggers"] = lista_totales_rigger;
                // estos los obtendremos de el modelo de resumenestadopago
                ViewData["id"] = datos_id.id_estadopago;
                ViewData["uf"] = datos_id.uf;
                ViewData["total_valor_dia"] = datos_id.total_valor_dia.Substring(0,8);
                ViewData["total_valor_mlp"] = datos_id.total_valor_mlp.Substring(0,8);
                ViewData["total_valor_distribucion"] = datos_id.total_valor_distribucion.Substring(0,8);
                ViewData["costo_hora"] = datos_id.costo_hora;
                ViewData["total_delta_reloj"] = datos_id.total_delta_reloj;
                ViewData["total_delta_horometro"] = datos_id.total_delta_horometro;
                ViewData["total_contador"] = datos_id.total_contador;
                ViewData["total_saltos_horometro"] = datos_id.total_saltos_horometro;


                return View(lista_datos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        // caluclo de valores de los servicios cuando se genera un nuevo estado de pago
        
        public void calculo_estadodepago(FormCollection form)
        {
            // genaremos un lista de estados de pago para almacenarlos agrupados por typo de equipo
            
            // obtener minimo garantizado desde el typo de equipo que sea
            DateTime fecha_actual = DateTime.Now;
            uf = double.Parse(form["Unidad_fomento"]);
            if(form["seleccion"].Equals("entre fechas"))
            {
              GetServicios servicios = new GetServicios();
                // obtenemos una lista contodos los servicios entre las fechas indicadas
              List<Servicios> lista_servicios=servicios.todos_servicios_entrefechas(form["fecha_inicio_e"],form["fecha_fin_e"]);
               // creamos un  lista de detalles estado pago para insertar en la bd el detalle con los valores calculados
             
               //obtener tipos de equipo
              List<TiposEquipos> lista_tipo_equipo = new List<TiposEquipos>();
              TipoEquipoSelect obtener_equipos= new TipoEquipoSelect();
              comprobarCautivo comprobar_cautivo = new comprobarCautivo();
              lista_tipo_equipo = obtener_equipos.lista_tipos_equipo();
            foreach (TiposEquipos tipo_equipo in lista_tipo_equipo)
            {
                List<DetalleEstadoPago> lista_detalle_estado = new List<DetalleEstadoPago>();
                // se crea variable resumen estado pago para guardar los datos generales
                ResumenEstadoPago datos_resumenestadopago = new ResumenEstadoPago();
                //deberia estar afuera
                string id_estado_pago = id_autogenerado_estadopago(tipo_equipo.subt_tipo);
                // debemos agrupar por tipoequipo
                foreach (Servicios datos in lista_servicios)
                { 
                    if(tipo_equipo.subt_tipo.Equals(datos.tipo_equipo) &&(comprobar_cautivo.comprobar(datos.tag_equipo)==false))
                    {
                        // setear el tipo_equipo_aux para los calculos de resumenestadopago
                        tipo_equipo_aux = tipo_equipo.subt_tipo;
                        new actualizarCalculado().actualizarCalculadoEnDatosFin(datos.idSolicitud);

                    DetalleEstadoPago detalle_Estadopago= new DetalleEstadoPago();
                    // parametros que se pasan de servicio a detalle_estado_pago
                    detalle_Estadopago.idEstadoPago = id_estado_pago;
                    detalle_Estadopago.idSolicitud = datos.idSolicitud;
                    detalle_Estadopago.fecha = new DateTime(datos.fecha.Year, datos.fecha.Month, datos.fecha.Day);
                    detalle_Estadopago.operador = datos.rut_operador;
                    detalle_Estadopago.tagequipo = datos.tag_equipo;
                    detalle_Estadopago.area = datos.nombre_area;
                    detalle_Estadopago.responsable = datos.jefe_area;
                    detalle_Estadopago.empresa = datos.empresa_propietaria;
                    detalle_Estadopago.centro_costo = datos.codigo_centro_costo;
                    detalle_Estadopago.i_horometro = datos.hora_inicio_horometro;
                    detalle_Estadopago.f_horometro = datos.hora_fin_horometro;
                    detalle_Estadopago.i_reloj = datos.hora_inicio_reloj;
                    detalle_Estadopago.f_reloj = datos.hora_fin_reloj;
                    // se igualan todos los parametros y se agregan los calculados
                    detalle_Estadopago.d_horometro=calculo_delta_horometro(datos.hora_inicio_horometro,datos.hora_fin_horometro).ToString();
                
                    //sumando para calcular el total
                    total_dhorometro +=calculo_delta_horometro(datos.hora_inicio_horometro,datos.hora_fin_horometro);
                
                    detalle_Estadopago.d_reloj=calculo_delta_hora(datos.hora_inicio_reloj,datos.hora_fin_reloj).ToString();

                    //total_dreloj +=double.Parse( detalle_Estadopago.d_reloj);
                    total_dreloj += calculo_delta_hora(datos.hora_inicio_reloj, datos.hora_fin_reloj);

                    contadorAnterior = total_contador;
                    total_contador += double.Parse(detalle_Estadopago.d_horometro);
                    
                    //total_contador += calculo_contador(double.Parse(detalle_Estadopago.d_reloj));
                    detalle_Estadopago.contador_deltareloj = total_contador.ToString();
                    //detalle_Estadopago.contador_deltareloj = calculo_contador(double.Parse(detalle_Estadopago.d_reloj)).ToString();

                    //calculo de valor dia y mlp
                    detalle_Estadopago.valor_dia = calculo_valordia(tipo_equipo.minimo_garantizado, double.Parse(detalle_Estadopago.d_horometro), uf, tipo_equipo.costo_hora_equipo, tipo_equipo.hora_extra, double.Parse(detalle_Estadopago.d_horometro), double.Parse(detalle_Estadopago.contador_deltareloj)).ToString();
                    if (datos.empresa_propietaria.Equals("ameco", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        detalle_Estadopago.valor_mlp = "0";
                    }
                    else
                    {
                        
                        detalle_Estadopago.valor_mlp = calculo_valordia(tipo_equipo.minimo_garantizado, double.Parse(detalle_Estadopago.d_horometro), uf, tipo_equipo.costo_hora_equipo, tipo_equipo.hora_extra, double.Parse(detalle_Estadopago.d_horometro), double.Parse(detalle_Estadopago.contador_deltareloj)).ToString();
                    }
                    total_valordia += double.Parse(detalle_Estadopago.valor_dia);
                    total_valormlp += double.Parse(detalle_Estadopago.valor_mlp);
                    //almacenamos en la lista de detalles estado pago
                    lista_detalle_estado.Add(detalle_Estadopago);
                    
                    //calculo rigger
                    //obtener_datos_rigger(id_estado_pago, datos.idSolicitud);
                    }
                    
                }

                if (tipo_equipo.subt_tipo.Equals(tipo_equipo_aux))
                {
                    // un vez terminado el foreach debemos pasar la lista de detallesestado de pago para calcular el valor de distribucion
                    calculo_valordistribucion(lista_detalle_estado);
                    //calculo salto horometro
                    calculo_salto_horometro(lista_detalle_estado);
              
                    //--------------------------- Ingreso de datos al Resumen----------------------------------------------
                    // ingreso id estado pago autogenerado
                    datos_resumenestadopago.id_estadopago = id_estado_pago;
                    datos_resumenestadopago.uf = uf.ToString();
                    // fecha donde se genera el estado de pago 
                    datos_resumenestadopago.fecha_generado = fecha_actual.ToShortDateString();
                    // guardamos el tipo de equipo que se recupera desde bd
                    datos_resumenestadopago.tipo_equipo = tipo_equipo.subt_tipo;
                    datos_resumenestadopago.total_valor_dia = total_valordia.ToString();
                    datos_resumenestadopago.total_valor_mlp = total_valormlp.ToString();
                    datos_resumenestadopago.total_valor_distribucion = total_valord.ToString();
                    datos_resumenestadopago.costo_hora = _costo_hora.ToString();
                    datos_resumenestadopago.total_delta_horometro = total_dhorometro.ToString();
                    datos_resumenestadopago.total_delta_reloj = total_dreloj.ToString();
                    datos_resumenestadopago.total_contador = total_contador.ToString();
                    datos_resumenestadopago.total_saltos_horometro = total_saltos_horometro.ToString();

                    // inserta en la tabla resumen el id, y datos globlaes
                    ResumenestadopagoInsert resumeestadopago = new ResumenestadopagoInsert();
                    resumeestadopago.saveresumen(datos_resumenestadopago);
                    // insertar en la bd el detalle del estado de pago
                    DetalleEstadoPagoInsert detalleestadopago = new DetalleEstadoPagoInsert();
                    detalleestadopago.savedetalleestadopago(lista_detalle_estado);
                }
            }
                
            }

            if(form["seleccion"].Equals("ala fecha"))
            {
                GetServicios servicios = new GetServicios();
                
               
                List<TiposEquipos> lista_tipo_equipo = new List<TiposEquipos>();
                comprobarCautivo comprobar_cautivo= new comprobarCautivo();
                TipoEquipoSelect obtener_equipos = new TipoEquipoSelect();
                lista_tipo_equipo = obtener_equipos.lista_tipos_equipo();

                string id_estado_pago = id_autogenerado_general();
                
                foreach (TiposEquipos tipo_equipo in lista_tipo_equipo)
                {

                    // creamos un  lista de detalles estado pago para insertar en la bd el detalle con los valores calculados
                    List<DetalleEstadoPago> lista_detalle_estado = new List<DetalleEstadoPago>();
                    //generamos id estado de apgo por equipo
                    string id_estado_pago_por_tipo = id_autogenerado_estadopago(tipo_equipo.subt_tipo);     

                // debemos agrupar por tipoequipo
                    //obtener servicios ala fecha y por tipo de equipo comprobando que este en false el campo calculado
                    List<Servicios> lista_servicios = servicios.todos_servicios_alafecha(form["fecha_fin_a"],tipo_equipo.subt_tipo);    
                    
                    //comprobamos que la lista no este vacia
                    if (lista_servicios.Count > 0)
                    {
                        //setear valores globales
                        setear_variables_globales();
                        
                        foreach (Servicios datos in lista_servicios)
                        {

                            
                                // setear el tipo_equipo_aux para los calculos de resumenestadopago
                                tipo_equipo_aux = tipo_equipo.subt_tipo;
                                new actualizarCalculado().actualizarCalculadoEnDatosFin(datos.idSolicitud);

                                DetalleEstadoPago detalle_Estadopago = new DetalleEstadoPago();
                                // parametros que se pasan de servicio a detalle_estado_pago
                                detalle_Estadopago.idEstadoPago = id_estado_pago;
                                //se le asgina el id estado de pago por tipo
                                detalle_Estadopago.idEstadoPagoTipo = id_estado_pago_por_tipo;
                                detalle_Estadopago.idSolicitud = datos.idSolicitud;
                                detalle_Estadopago.fecha = new DateTime(datos.fecha.Year, datos.fecha.Month, datos.fecha.Day);
                                detalle_Estadopago.operador = datos.rut_operador;
                                detalle_Estadopago.tagequipo = datos.tag_equipo;
                                detalle_Estadopago.tipoequipo = datos.tipo_equipo;
                                detalle_Estadopago.area = datos.nombre_area;
                                detalle_Estadopago.responsable = datos.jefe_area;
                                detalle_Estadopago.empresa = datos.empresa_propietaria;
                                detalle_Estadopago.centro_costo = datos.codigo_centro_costo;
                                detalle_Estadopago.i_horometro = datos.hora_inicio_horometro;
                                detalle_Estadopago.f_horometro = datos.hora_fin_horometro;
                                detalle_Estadopago.i_reloj = datos.hora_inicio_reloj;
                                detalle_Estadopago.f_reloj = datos.hora_fin_reloj;
                                //------------------------- Datos calculados-------------------
                                // se igualan todos los parametros y se agregan los calculados
                                detalle_Estadopago.d_horometro = calculo_delta_horometro(datos.hora_inicio_horometro, datos.hora_fin_horometro).ToString();

                                //sumando para calcular el total
                                total_dhorometro += calculo_delta_horometro(datos.hora_inicio_horometro, datos.hora_fin_horometro);

                                detalle_Estadopago.d_reloj = calculo_delta_hora(datos.hora_inicio_reloj, datos.hora_fin_reloj).ToString();

                                total_dreloj += double.Parse(detalle_Estadopago.d_reloj);
                                //total_dreloj += calculo_delta_hora(datos.hora_inicio_reloj, datos.hora_fin_reloj);

                                contadorAnterior = total_contador;
                                total_contador += double.Parse(detalle_Estadopago.d_horometro);
                                //total_contador += calculo_contador(double.Parse(detalle_Estadopago.d_reloj));
                                detalle_Estadopago.contador_deltareloj = total_contador.ToString();
                                // detalle_Estadopago.contador_deltareloj = calculo_contador(double.Parse(detalle_Estadopago.d_reloj)).ToString();


                                //total_contador += double.Parse(detalle_Estadopago.contador_deltareloj);


                                //calculo de valor dia y mlp

                                detalle_Estadopago.valor_dia = calculo_valordia(tipo_equipo.minimo_garantizado, double.Parse(detalle_Estadopago.d_reloj), uf, tipo_equipo.costo_hora_equipo, tipo_equipo.hora_extra, double.Parse(detalle_Estadopago.d_horometro), double.Parse(detalle_Estadopago.contador_deltareloj)).ToString();
                               
                                if (datos.empresa_propietaria.Equals("ameco", StringComparison.OrdinalIgnoreCase))
                                {

                                    detalle_Estadopago.valor_mlp = "0";
                                }
                                else
                                {

                                    detalle_Estadopago.valor_mlp = calculo_valordia(tipo_equipo.minimo_garantizado, double.Parse(detalle_Estadopago.d_reloj), uf, tipo_equipo.costo_hora_equipo, tipo_equipo.hora_extra, double.Parse(detalle_Estadopago.d_horometro), double.Parse(detalle_Estadopago.contador_deltareloj)).ToString();
                               

                                }
                                total_valordia += double.Parse(detalle_Estadopago.valor_dia);
                                total_valormlp += double.Parse(detalle_Estadopago.valor_mlp);

                                lista_detalle_estado.Add(detalle_Estadopago);
                                //total_valormlp += datos.valor_mlp;
                                //total_valord += datos.valor_distribucion;

                                //calculo rigger
                                obtener_datos_rigger(id_estado_pago,id_estado_pago_por_tipo, datos.idSolicitud);
                            

                        }// fin foreach servicios
                    }//fin if comprobar lista
                    if (!String.IsNullOrEmpty(tipo_equipo_aux))
                    {
                        // se crea variable resumen estado pago para guardar los datos generales
                        ResumenEstadoPago datos_resumenestadopago = new ResumenEstadoPago();
                        // un vez terminado el foreach debemos pasar la lista de detallesestado de pago para calcular el valor de distribucion
                        calculo_valordistribucion(lista_detalle_estado);
                        //calculo salto horometro
                        calculo_salto_horometro(lista_detalle_estado);
                        //--------------------------- Ingreso de datos al Resumen----------------------------------------------
                        // ingreso id estado pago autogenerado


                        datos_resumenestadopago.id_estadopago = id_estado_pago;
                        //generamos el segun id que es por tipo de equipo
                        datos_resumenestadopago.id_estadopagotipo = id_estado_pago_por_tipo;
                        // fecha donde se genera el estado de pago 
                        datos_resumenestadopago.fecha_generado = fecha_actual.ToShortDateString();
                        // guardamos el tipo de equipo que se recupera desde bd
                        datos_resumenestadopago.uf = uf.ToString();
                        datos_resumenestadopago.tipo_equipo = tipo_equipo_aux;
                        datos_resumenestadopago.total_valor_dia = total_valordia.ToString();
                        datos_resumenestadopago.total_valor_mlp = total_valormlp.ToString();
                        datos_resumenestadopago.total_valor_distribucion = total_valord.ToString();
                        datos_resumenestadopago.costo_hora = _costo_hora.ToString();
                        datos_resumenestadopago.total_delta_horometro = total_dhorometro.ToString();
                        datos_resumenestadopago.total_delta_reloj = total_dreloj.ToString();
                        datos_resumenestadopago.total_contador = total_contador.ToString();
                        datos_resumenestadopago.total_saltos_horometro = total_saltos_horometro.ToString();
                        // inserta en la tabla resumen el id, y datos globlaes
                        ResumenestadopagoInsert resumeestadopago = new ResumenestadopagoInsert();
                        resumeestadopago.saveresumen(datos_resumenestadopago);
                        // insertar en la bd el detalle del estado de pago
                        DetalleEstadoPagoInsert detalleestadopago = new DetalleEstadoPagoInsert();
                        detalleestadopago.savedetalleestadopago(lista_detalle_estado);
                        //seteamos el tipo equipo antes de terminar el if
                        tipo_equipo_aux = "";
                    }
                }
               
                
            }
        }

        public void setear_variables_globales()
        {
            tipo_equipo_aux = "";
            total_dhorometro=0;
            total_dreloj = 0; total_contador = 0; total_valordia = 0; total_valormlp = 0; total_valord = 0; _costo_hora = 0; total_saltos_horometro = 0; 

            ultimo_horometro = new List<double>();
            contadorAnterior = 0;
        }
        public string id_autogenerado_general()
        {
            DateTime fecha_generado = DateTime.Now;

            string fecha_corta = fecha_generado.ToShortDateString();

            string tiempo = fecha_generado.Hour.ToString() + fecha_generado.Minute.ToString();

            
            string id = fecha_generado.Date.Year.ToString() + fecha_generado.Date.Month.ToString() + fecha_generado.Date.Day.ToString() + tiempo ;
            return id;
        }
        public string id_autogenerado_estadopago(string tipo_equipo)
        {
            DateTime fecha_generado = DateTime.Now;
            
            string fecha_corta = fecha_generado.ToShortDateString();

            string tiempo = fecha_generado.Hour.ToString() + fecha_generado.Minute.ToString();
            
            string[] nombre_tipo = tipo_equipo.Split(' ');
            string tipo = "";
            
            foreach (string cadena in nombre_tipo)
            {

                if (IsNumeric(cadena))
                {
                    tipo += cadena;
                }
                else
                {
                    tipo += cadena.Substring(0, 1);
                }
            }
            string id = fecha_generado.Date.Year.ToString() + fecha_generado.Date.Month.ToString() +fecha_generado.Date.Day.ToString()+tiempo+tipo;
            return id;
        }
        public bool IsNumeric(object Expression)
        {

            bool isNum;

            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;

        }
        public void calculo_salto_horometro(List<DetalleEstadoPago> lista)
        {
            double aux_ultimo_horometro = 0;
            //comprobar si es la segunda iteracion
            int i = 0;
            foreach (DetalleEstadoPago datos in lista)
            {
                
                if (i == 0)
                {
                    aux_ultimo_horometro = double.Parse(datos.f_horometro);
                }
                if (i > 0)
                {
                    double diferencia = double.Parse(datos.i_horometro) - aux_ultimo_horometro;
                    datos.salto_horometro = diferencia.ToString();
                    aux_ultimo_horometro = double.Parse(datos.f_horometro);
                    total_saltos_horometro += double.Parse(datos.salto_horometro);
                }
                i++;
            }

        }
        public double calculo_delta_horometro(string horometro_inicio,string horometro_fin)
        {
            double total = double.Parse(horometro_fin) - double.Parse(horometro_inicio);
            return total;
        }
        //public double calculo_delta_hora(string hora_inicio, string hora_fin)
        public double calculo_delta_hora(string horaInicio, string horaFin)
        {
            
            /*double total = 0 ;
            double descuento_colaccion =0;
            
            DateTime H_inicio = DateTime.Parse(hora_inicio);
            DateTime H_fin = DateTime.Parse(hora_fin);
            double hora_i = double.Parse(H_inicio.Hour.ToString());
            double hora_f = double.Parse(H_fin.Hour.ToString());
            if (hora_i < 8 && hora_f > 9)
            {
                descuento_colaccion = 1;
            }
            if (hora_i >= 9 && hora_i < 14 && hora_f >= 15)
            {
                descuento_colaccion = 1;
            }
            
            TimeSpan diferencia = H_fin.Subtract(H_inicio);
            double totalhoras = double.Parse(diferencia.Hours.ToString());
            double totalminutos = double.Parse(diferencia.Minutes.ToString());
            if (totalminutos == 30)
            {
                totalminutos = 0.5;
            }
          
            if (totalminutos != null)
            {
                total = (totalhoras + totalminutos)-descuento_colaccion;
            }
            else
            {
                total = totalhoras-descuento_colaccion;
            }
            return total;//*/
            double retorno = 0;

            double inicio = double.Parse(horaInicio.Split(':')[0]);
            if (horaInicio.Split(':')[1].Equals("30")) inicio += 0.5;

            double fin = double.Parse(horaFin.Split(':')[0]);
            if (horaFin.Split(':')[1].Equals("30")) fin += 0.5;

            if ((inicio <= 9 && fin >= 11) || (inicio <= 12 && fin >= 14))
                fin--;

            retorno = (fin - inicio);
            return retorno;
        }
        
        public double calculo_valordia(double minimo_garantizado,double delta_reloj,double valor_uf,double valor_hora,double valor_hora_extra,double delta_horometro,double contador)
        {
            double retorno = 0;
            if (contador < minimo_garantizado)
            {
                retorno = (delta_horometro * valor_uf * valor_hora);
            }
            else
            {
                if (contadorAnterior < minimo_garantizado) 
                {
                    retorno = ((minimo_garantizado - contadorAnterior) * valor_uf * valor_hora) + ((contador - minimo_garantizado) * valor_uf * valor_hora_extra);
                }
                else
                {
                    retorno = (delta_horometro * valor_uf * valor_hora_extra);
                }
            }
            return retorno;
        }

        public void calculo_valordistribucion(List<DetalleEstadoPago> lista)
        {
            // hay que sacar todas las horas deltahorometro internas
            double resta_valores_ameco = 0;
            foreach (DetalleEstadoPago aux in lista)
            {
                if (aux.empresa.Equals("ameco", StringComparison.OrdinalIgnoreCase))
                {
                    resta_valores_ameco += double.Parse(aux.d_horometro);
                }
            }
            //calculo del costo hora
            //double costo_hora = total_valormlp / total_dreloj;
            //se utiliza ahora el total_dhorometro y el valor dia
            double divisor_corregido = total_dhorometro - resta_valores_ameco;
            double costo_hora = total_valormlp / divisor_corregido;
            _costo_hora = costo_hora;
            foreach (DetalleEstadoPago datos in lista)
            {
                if (datos.valor_mlp != "0")
                {
                    datos.valor_distribucion = (double.Parse(datos.d_horometro) * costo_hora).ToString();
                    total_valord += double.Parse(datos.valor_distribucion);
                }
                else
                {
                    datos.valor_distribucion = "0";
                }
            }
        }
        [HttpPost]
        // accion donde se dirigen los datos al realizar el submit desde generar estado de pago
        public ActionResult Guardar_estadopagoGenerado(FormCollection datos_estadopago)
        {
            
            flag = 1;
            calculo_estadodepago(datos_estadopago);
           
            // antes de dirigirlo a ver los estados de pago se deben hacer todos los calculos de valor dia y demases
            return RedirectToAction("Mostrareresumenestadopago");
        }

        public void Generar_Excel(string id)
        {
            ExportToExcel(id);
        }
        public void ExportToExcel(string id)
        {
            List<DetalleEstadoPago> lista = new List<DetalleEstadoPago>();
            DetalleEstadoPagoSelect dato = new DetalleEstadoPagoSelect();
            lista = dato.listadetallesestadopago(id);
            ResumenestadopagoSelect resumen_datos = new ResumenestadopagoSelect();
            ResumenEstadoPago resumen = new ResumenEstadoPago();
            resumen = resumen_datos.obtener_resumen_estadopago_id(id);
            var grid = new GridView();
            //grid.AutoGenerateColumns = false;

            var products = new System.Data.DataTable("teste");

            products.Columns.Add("Id Solicitud", typeof(string));
            products.Columns.Add("Fecha", typeof(string));
            products.Columns.Add("Operador", typeof(string));
            products.Columns.Add("Tag Equipo", typeof(string));
            products.Columns.Add("Area", typeof(string));
            products.Columns.Add("Responsable", typeof(string));
            products.Columns.Add("Empresa", typeof(string));
            products.Columns.Add("Centro Costo", typeof(string));
            products.Columns.Add("Inicio Horometro", typeof(string));
            products.Columns.Add("Fin Horometro", typeof(string));
            products.Columns.Add("Delta Horometro", typeof(string));
            products.Columns.Add("Salto Horometro", typeof(string));
            products.Columns.Add("Inicio Reloj", typeof(string));
            products.Columns.Add("Fin Reloj", typeof(string));
            products.Columns.Add("Delta Reloj", typeof(string));
            products.Columns.Add("Contador", typeof(string));
            products.Columns.Add("Valor Dia", typeof(string));
            products.Columns.Add("Valor MLP", typeof(string));
            products.Columns.Add("Valor Distribucion", typeof(string));

            foreach (DetalleEstadoPago datos in lista)
            {
                products.Rows.Add(datos.idSolicitud, datos.fecha, datos.operador
                    , datos.tagequipo, datos.area, datos.responsable, datos.empresa
                    , datos.centro_costo, datos.i_horometro, datos.f_horometro
                    , datos.d_horometro,datos.salto_horometro, datos.i_reloj, datos.f_reloj, datos.d_reloj, datos.contador_deltareloj
                    , datos.valor_dia, datos.valor_mlp, datos.valor_distribucion);

            }

            products.Columns.Add("UF");
            products.Columns.Add("Costo Hora Distribucion");

            products.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            products.Rows.Add("", "", "", "", "", "", "", "", "", "","Total", "Total", "", "", "Total", "", "Total", "Total", "Total", "Total", "Total");
            products.Rows.Add("", "", "", "", "", "", "", "", "", "", resumen.total_delta_horometro, resumen.total_saltos_horometro, "","", resumen.total_delta_reloj,"", resumen.total_valor_dia, resumen.total_valor_mlp, resumen.total_valor_distribucion, resumen.uf, resumen.costo_hora);


            grid.DataSource = products;
            grid.BackColor = System.Drawing.Color.Lavender;

            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + id + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

        }

        //metodos para el calculo de dias del rigger

        public void obtener_datos_rigger(string id_estadopago_general,string id_estadopago_tipo,string id_solicitud)
        {
            //primero obtenemos los dtos del rigger deacuerdo al id de la solicitud
            DatosRigger datos_riger = new DatosRigger();
            RiggerDiasTrabajados trabajados_rigger=new RiggerDiasTrabajados();
            datos_riger = datos_riger.obtener_datos_rigger(id_solicitud);
            if (datos_riger.rut_rigger1 != "--")
            { 
              //comprobamos que el rigger no ha sido calculado con la id_solcitud
                if (!trabajados_rigger.comprobar_rigger_idsolicitud(datos_riger.rut_rigger1,id_solicitud))
                {
                    //calculaos los dias trabajadas
                    int calculo_dias = calculo_dias_rigger(datos_riger.fecha_inicio, datos_riger.fecha_termino);

                    string nombre_rigger = trabajados_rigger.obtener_nombre_rigger(datos_riger.rut_rigger1);

                    trabajados_rigger.id_estado_pago = id_estadopago_general;
                    trabajados_rigger.id_estado_pago_tipo = id_estadopago_tipo;
                    trabajados_rigger.id_solicitud = id_solicitud;
                    trabajados_rigger.nombre_rigger = nombre_rigger;
                    trabajados_rigger.rut_rigger = datos_riger.rut_rigger1;
                    trabajados_rigger.fecha_inicio = datos_riger.fecha_inicio;
                    trabajados_rigger.fecha_fin = datos_riger.fecha_termino;
                    trabajados_rigger.dias_trabajados = calculo_dias;
                    trabajados_rigger.insertar_datos_rigger(trabajados_rigger);
                }
                //actualizamos la tabla en bd
            }
            if (datos_riger.rut_rigger2 != "--")
            {
                //calculaos los dias trabajadas
                if (!trabajados_rigger.comprobar_rigger_idsolicitud(datos_riger.rut_rigger1, id_solicitud))
                {
                    int calculo_dias = calculo_dias_rigger(datos_riger.fecha_inicio, datos_riger.fecha_termino);

                    string nombre_rigger = trabajados_rigger.obtener_nombre_rigger(datos_riger.rut_rigger2);


                    trabajados_rigger.id_estado_pago = id_estadopago_general;
                    trabajados_rigger.id_estado_pago_tipo = id_estadopago_tipo;

                    trabajados_rigger.nombre_rigger = nombre_rigger;
                    trabajados_rigger.rut_rigger = datos_riger.rut_rigger2;
                    trabajados_rigger.dias_trabajados = calculo_dias;
                    trabajados_rigger.insertar_datos_rigger(trabajados_rigger);
                }
                //actualizamos la tabla en bd
            }

        }
        public int calculo_dias_rigger(DateTime fecha_inicio, DateTime fecha_fin)
        {

            DateTime fecha_inicio_acortada = new DateTime(fecha_inicio.Year, fecha_inicio.Month, fecha_inicio.Day);
            DateTime fecha_fin_acortada = new DateTime(fecha_fin.Year, fecha_fin.Month, fecha_fin.Day);
            // variable utilizada para la resta entre fecha
            TimeSpan diferencia = fecha_fin_acortada - fecha_inicio_acortada;
            int dias = diferencia.Days + 1;
            return dias;
        }
        #region Filtro Rigger

        public ActionResult RiggerTrabajados()
        {
            if (Session["rol"] != null && (Session["rol"].Equals("admin") || Session["rol"].Equals("izaje")))
            {
                ResumenestadopagoSelect id_generales = new ResumenestadopagoSelect();
                ViewData["id_estado_pago_general"] = id_generales.obtener_id_estado_pago_generales();

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        public ActionResult DetalleRigger(string idGeneral)
        {
         
            RiggerDiasTrabajados datos_rigger = new RiggerDiasTrabajados();
           List<RiggerDiasTrabajados> lista_datos_rigger = new List<RiggerDiasTrabajados>();
           //obtenemos la lista con todo los rigger calculados por id estado pago general
            lista_datos_rigger = datos_rigger.obtener_rigger_id_pago_general(idGeneral);
            // Declaraciones para obtener el los totales de dias trabajados para cada rigger hasta la fecha 
            RiggerTotales datos_total_rigger = new RiggerTotales();
            List<RiggerTotales> lista_datos_total_rigger = new List<RiggerTotales>();
            lista_datos_total_rigger=datos_total_rigger.calcular_totales_id_general(idGeneral);
            ViewData["TotalesRiggers"] = lista_datos_total_rigger;
            return View(lista_datos_rigger);
        }

        public string rescatar_id_tipo(string id_general)
        {
            ResumenestadopagoSelect id_tipos = new ResumenestadopagoSelect();
            return id_tipos.obtener_id_tipo_por_id_general(id_general);
        }
        #endregion
    }
}

