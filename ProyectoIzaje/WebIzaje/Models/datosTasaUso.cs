using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIzaje.Models
{
    public class datosTasaUso
    {
        public string idSolicitud { get; set; }
        public string area { get; set; }
        public string centroCosto { get; set; }
        public string empresa { get; set; }
        public string fecha { get; set; }
        public string horaRelojInicio { get; set; }
        public string horaRelojFin { get; set; }
        public string horaHorometroInicio { get; set; }
        public string horaHorometroFin { get; set; }
        public string deltaHorasReloj { get; set; }
        public string deltaHorasHorometro { get; set; }
        public double tasaEfectivaUso { get; set; }
        public string equipo { get; set; }
        public string operador { get; set; }
        public string rigger { get; set; }

        public static List<datosTasaUso> solicitudesAdatosTasaUso(List <Solicitud> solicitudes)
        {
            List<datosTasaUso> retorno = new List<datosTasaUso>();

            for (int i = 0; i < solicitudes.Count; i++) 
            {
                for (int j = 0; j < solicitudes[i].fechas.Count; j++)
                {
                    datosTasaUso dato = new datosTasaUso();

                    dato.idSolicitud = solicitudes[i].idSolicitud;
                    dato.area = solicitudes[i].area;
                    dato.empresa = solicitudes[i].empresa;
                    dato.centroCosto = solicitudes[i].centroCosto;
                    dato.fecha = solicitudes[i].fechas[j];
                    dato.horaRelojInicio = solicitudes[i].horaRelojInicial1[j];
                    dato.horaRelojFin = solicitudes[i].horaRelojFinal1[j];
                    dato.horaHorometroInicio = solicitudes[i].horaHorometroInicial1[j];
                    dato.horaHorometroFin = solicitudes[i].horaHorometroFinal1[j];
                    dato.deltaHorasReloj = obtenerDiferenciaReloj(solicitudes[i].horaRelojInicial1[j], solicitudes[i].horaRelojFinal1[j]);
                    dato.deltaHorasHorometro = obtenerDiferenciaHorometro(solicitudes[i].horaHorometroInicial1[j], solicitudes[i].horaHorometroFinal1[j]);
                    dato.tasaEfectivaUso = Math.Truncate((double.Parse(dato.deltaHorasHorometro) / double.Parse(dato.deltaHorasReloj)*100));

                    DatosEquipo equipo = new EquipoSelect().obtener_equipos(solicitudes[i].idEquipo1);
                    dato.equipo = solicitudes[i].idEquipo1 + "/" + equipo.marca + " " + equipo.modelo;

                    TrabajadorDatos trabajador = new TrabajadorGet().trabajador(solicitudes[i].idOperador1);
                    dato.operador = solicitudes[i].idOperador1 + "/" + trabajador.nombre + " " + trabajador.apellidoP + " " + trabajador.apellidoM;

                    trabajador = new TrabajadorGet().trabajador(solicitudes[i].idRigger1);
                    dato.rigger = solicitudes[i].idRigger1 + "/" + trabajador.nombre + " " + trabajador.apellidoP + " " + trabajador.apellidoM;

                    retorno.Add(dato);

                    if (!solicitudes[i].idEquipo2.Equals("--")) 
                    {
                        dato = new datosTasaUso();

                        dato.idSolicitud = solicitudes[i].idSolicitud;
                        dato.fecha = solicitudes[i].fechas[j];
                        dato.horaRelojInicio = solicitudes[i].horaRelojInicial2[j];
                        dato.horaRelojFin = solicitudes[i].horaRelojFinal2[j];
                        dato.horaHorometroInicio = solicitudes[i].horaHorometroInicial2[j];
                        dato.horaHorometroFin = solicitudes[i].horaHorometroFinal2[j];
                        dato.deltaHorasReloj = obtenerDiferenciaReloj(solicitudes[i].horaRelojInicial2[j], solicitudes[i].horaRelojFinal2[j]);
                        dato.deltaHorasHorometro = obtenerDiferenciaHorometro(solicitudes[i].horaHorometroInicial2[j], solicitudes[i].horaHorometroFinal2[j]);
                        dato.tasaEfectivaUso = Math.Truncate((double.Parse(dato.deltaHorasHorometro) / double.Parse(dato.deltaHorasReloj) * 100));

                        equipo = new EquipoSelect().obtener_equipos(solicitudes[i].idEquipo2);
                        dato.equipo = solicitudes[i].idEquipo2 + "/" + equipo.marca + " " + equipo.modelo;

                        trabajador = new TrabajadorGet().trabajador(solicitudes[i].idOperador2);
                        dato.operador = solicitudes[i].idOperador2 + "/" + trabajador.nombre + " " + trabajador.apellidoP + " " + trabajador.apellidoM;

                        trabajador = new TrabajadorGet().trabajador(solicitudes[i].idRigger2);
                        dato.rigger = solicitudes[i].idRigger2 + "/" + trabajador.nombre + " " + trabajador.apellidoP + " " + trabajador.apellidoM;

                        retorno.Add(dato);
                    }
                }
            }
            return retorno;
        }
        static string obtenerDiferenciaReloj(string horaInicio, string horaFin) 
        {
            string retorno = "";

            double inicio = double.Parse(horaInicio.Split(':')[0]);
            if (horaInicio.Split(':')[1].Equals("30")) inicio += 0.5;

            double fin = double.Parse(horaFin.Split(':')[0]);
            if (horaFin.Split(':')[1].Equals("30")) fin += 0.5;

            if ((inicio <= 9 && fin >= 11) || (inicio <= 12 && fin >= 14 || inicio <= 0 && fin >= 8))
                fin--;

            retorno = (fin - inicio).ToString();
            return retorno;
        }
        static string obtenerDiferenciaHorometro(string horometroInicio, string horometroFin)
        {
            string retorno = "";
            double inicio = double.Parse(horometroInicio);
            double fin = double.Parse(horometroFin);
            retorno = (fin - inicio).ToString();
            return retorno;
        }
        public static double obtenerDiferenciaRelojPublic(string horaInicio, string horaFin)
        {
            double retorno = 0;

            double inicio = double.Parse(horaInicio.Split(':')[0]);
            if (horaInicio.Split(':')[1].Equals("30")) inicio += 0.5;

            double fin = double.Parse(horaFin.Split(':')[0]);
            if (horaFin.Split(':')[1].Equals("30")) fin += 0.5;

            if ((inicio <= 9 && fin >= 11) || (inicio <= 12 && fin >= 14 || inicio <= 0 && fin >= 8))
                fin--;

            retorno = (fin - inicio);
            return retorno;
        }
    }
}