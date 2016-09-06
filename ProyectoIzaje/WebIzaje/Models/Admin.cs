using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIzaje.Models
{
    public class Jefe_Area
    {
        public string usuario { get; set; }
        public string nombre { get; set; }
        public string fono { get; set; }
        public string email { get; set; }
        public string area { get; set; }

        public bool deleteJefe_Area(string jefe)
        {
            try
            {
                if (new Delete().deleteJefe_Area(jefe))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertJefe_Area(Jefe_Area jefe)
        {
            try
            {

                new Insert().InsertJefe_Area(jefe);
                return true;

            }
            catch (Exception)
            {

            }
            return false;
        }

        public Jefe_Area GetJefe_Area(string nombre)
        {
            try
            {
                return new Get().GetJefe_Area(nombre);
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
    public class Usuarios
    {
        public string identificador { get; set; }
        public string nombres { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string email { get; set; }
        public string rol { get; set; }
        public string password { get; set; }


        public List<Usuarios> obtenerUsuarios()
        {
            List<Usuarios> retorno = new List<Usuarios>();

            retorno = new conexion().obtenerListaUsuarios();

            return retorno;
        }
        public bool eliminarUsuario(string id)
        {
            bool retorno = false;
            retorno = new conexion().deleteUsuario(id);
            return retorno;
        }
        public string comprobarUsuario(string nombre)
        {
            string retorno = "";
            if (new conexion().ExisteUsuario(nombre))
            {
                retorno = "True";
            }
            else
            {
                retorno = "False";
            }
            return retorno;
        }
        public bool guardarUsuario(Usuarios user)
        {
            bool retorno = new conexion().saveUsuario(user);
            return retorno;
        }
        public Usuarios obtenerdatos(string nombre)
        {
            Usuarios user = new Usuarios();
            user = new conexion().getUsuario(nombre);
            return user;
        }
        public bool actualizarUsuario(Usuarios datos)
        {
            bool retorno = new conexion().updateUsuario(datos);
            return retorno;
        }
    }


    public class Certificacion
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }

        public bool deleteCertificacion(string nombre)
        {
            try
            {
                if (new Delete().deleteCertificacion(nombre))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool updateCertificacion(string nombre, string item)
        {
            try
            {
                if (new Update().updateCertificacion(nombre, item))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertCertificacion(Certificacion certificacion)
        {
            try
            {
                new Insert().InsertCertificacion(certificacion);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public List<Certificacion> GetCertificacion()
        {
            try
            {
                return new Get().GetCertificacion();
            }
            catch (Exception)
            { }
            return null;
        }

        public Certificacion GetCertificacion(string id)
        {
            try
            {
                return new Get().GetCertificacion(id);
            }
            catch (Exception)
            { }
            return null;
        }
    }
    public class Empresa
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }

        public bool deleteEmpresa(string nombre)
        {
            try
            {
                if (new Delete().deleteEmpresa(nombre))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool updateEmpresa(string nombre, string item)
        {
            try
            {
                if (new Update().updateEmpresa(nombre, item))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool InsertEmpresa(Empresa empresa)
        {
            try
            {
                new Insert().InsertEmpresa(empresa);
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        public List<Empresa> GetEmpresa()
        {
            try
            {
                return new Get().GetEmpresa();
            }
            catch (Exception)
            { }
            return null;
        }
    }
    public class Familia_Equipo
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }

        public bool deleteFamilia_Equipo(string nombre)
        {
            try
            {

                if (new Delete().deleteFamilia_Equipo(nombre))
                    return true;

            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool updateFamilia_Equipo(string nombre, string item)
        {
            try
            {
                if (new Update().updateFamilia_Equipo(nombre, item))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificiarFamilia_Equipo(string nombre)
        {
            try
            {
                if (new Get().existFamilia(nombre))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool InsertFamilia_Equipo(Familia_Equipo familia)
        {
            try
            {
                new Insert().InsertFamilia_Equipo(familia);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public List<Familia_Equipo> GetFamilia_Equipo()
        {
            try
            {
                return new Get().GetFamilia_Equipo();
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
    public class Marca_Equipo
    {
        public string marca { get; set; }

        public bool deleteMarca_Equipo(string marca)
        {
            try
            {
                if (!new Get().existMarca(marca))
                {
                    if (new Delete().deleteMarca_Equipo(marca))
                        return true;
                }

            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertMarca_Equipo(Marca_Equipo marca)
        {
            try
            {
                new Insert().InsertMarca_Equipo(marca);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificarMarca_Equipo(string marca)
        {
            try
            {
                if (new Get().existMarca(marca))
                {
                    return true;
                }

            }
            catch (Exception)
            {

            }
            return false;
        }

        public List<Marca_Equipo> GetMarca_Equipo()
        {
            try
            {
                return new Get().GetMarca_Equipo();
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
    public class Modelo_Equipo
    {
        public string modelo { get; set; }
        public string marca { get; set; }
        public string tipo { get; set; }

        public bool deleteModelo_Equipo(string modelo)
        {
            try
            {
                if (new Delete().deleteModelo_Equipo(modelo))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool updateModelo_Equipo(string nombre, string marca, string tipo)
        {
            try
            {
                if (new Update().updateModelo_Equipo(nombre, marca, tipo))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertModelo_Equipo(Modelo_Equipo modelo)
        {
            try
            {
                new Insert().InsertModelo_Equipo(modelo);
                return true;

            }
            catch (Exception)
            {

            }
            return false;
        }

        public List<Modelo_Equipo> GetModelo_Equipo()
        {
            try
            {
                return new Get().GetModelo_Equipo(); ;
            }
            catch (Exception)
            {

            }
            return null;
        }
    }


    public class gerencia
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }

        public bool eliminar(string nombre)
        {
            try
            {
                if (new conexion().eliminarGerencia(nombre))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool update(string nombre, string descripcion)
        {
            try
            {

                if (new conexion().updateGerencia(nombre, descripcion))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificarExist(string nombre)
        {
            try
            {
                if (new conexion().verificarGerencia(nombre))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarGerencia(this.nombre, this.descripcion))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public static List<gerencia> obtenerGerencias()
        {
            List<gerencia> retorno = new List<gerencia>();

            retorno = new conexion().obtenerListaGerencias();

            return retorno;
        }
        public static gerencia obtenerGerencias(string id)
        {
            gerencia retorno = new gerencia();
            retorno = new conexion().obtenerListaGerencias(id);
            return retorno;
        }
    }
    public class superintendencia
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string gerencia { get; set; }

        public static List<superintendencia> obtenerSuperintendencias()
        {
            List<superintendencia> retorno = new List<superintendencia>();

            retorno = new conexion().obtenerListaSuperintendencias();

            return retorno;
        }
        public static superintendencia obtenerSuperintendencias(string id)
        {
            superintendencia retorno = new superintendencia();

            retorno = new conexion().obtenerListaSuperintendencias(id);

            return retorno;
        }

        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarSuperintendencia(this.nombre, this.descripcion, this.gerencia))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool eliminar(string id)
        {
            try
            {

                if (new conexion().eliminarSuperintendencia(id))
                    return true;

            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool update(string nombre, string descripcion, string gerencia)
        {
            try
            {

                if (new conexion().updateSuperintendencia(nombre, descripcion, gerencia))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificarSuperIntendencia(string nombre)
        {
            try
            {
                if (new conexion().verificarSuperintendencia(nombre))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

    }
    public class area
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string superintendencia { get; set; }

        public static List<area> obtenerAreas()
        {
            List<area> retorno = new List<area>();

            retorno = new conexion().obtenerListaAreas();

            return retorno;
        }
        public static area obtenerAreas(string id)
        {
            area retorno = new area();

            retorno = new conexion().obtenerListaAreas(id);

            return retorno;
        }

        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarArea(this.nombre, this.descripcion, this.superintendencia))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool eliminar(string nombre)
        {
            try
            {

                if (new conexion().eliminarArea(nombre))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool update(string nombre, string descripcion, string super)
        {
            try
            {

                if (new conexion().updateAreas(nombre, descripcion, super))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificarArea(string nombre)
        {
            try
            {
                if (new conexion().verificarArea(nombre))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
    }
    public class centro_costo
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string area { get; set; }

        public static List<centro_costo> obtenerCentrosCosto()
        {
            List<centro_costo> retorno = new List<centro_costo>();

            retorno = new conexion().obtenerListaCentrosCosto();

            return retorno;
        }
        public static centro_costo obtenerCentrosCosto(string id)
        {
            centro_costo retorno = new centro_costo();

            retorno = new conexion().obtenerListaCentrosCosto(id);

            return retorno;
        }

        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarCentroCosto(this.nombre, this.codigo, this.area))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool eliminar(string codigo)
        {
            try
            {
                if (new conexion().eliminarCentroCosto(codigo))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool update(string nombre, string descripcion, string area)
        {
            try
            {

                if (new conexion().updateCentrocosto(nombre, descripcion, area))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool verificarCentro(string id)
        {
            try
            {
                if (new Get().existCentro(id))
                {
                    return true;
                }

            }
            catch (Exception)
            {

            }
            return false;
        }
    }
    public class criticidad
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }

        public static List<criticidad> obtenerCriticidades()
        {
            List<criticidad> retorno = new List<criticidad>();

            retorno = new conexion().obtenerListaCriticidad();

            return retorno;
        }
        public static criticidad obtenerCriticidades(string id)
        {
            criticidad retorno = new criticidad();

            retorno = new conexion().obtenerListaCriticidad(id);

            return retorno;
        }

        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarCriticidad(this.nombre, this.descripcion))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool eliminar(string nombre)
        {
            try
            {
                if (new conexion().eliminarCriticidad(nombre))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool update(string nombre, string descripcion)
        {
            try
            {

                if (new conexion().updateCriticidad(nombre, descripcion))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

    }
    public class tipoEquipo
    {
        public string tipo { get; set; }
        public string familia { get; set; }
        public int minimoGarantizado { get; set; }
        public double costo_hora_normal { get; set; }
        public double costo_hora_extra { get; set; }

        public static List<tipoEquipo> obtenerTiposEquipo()
        {
            List<tipoEquipo> retorno = new List<tipoEquipo>();

            retorno = new conexion().obtenerListaTipoEquipo();

            return retorno;
        }
        public static tipoEquipo obtenerTiposEquipo(string id)
        {
            tipoEquipo retorno = new tipoEquipo();

            retorno = new conexion().obtenerListaTipoEquipo(id);

            return retorno;
        }
        public bool agregarBD()
        {
            try
            {
                if (new conexion().agregarTipoEquipo(this.tipo, this.familia,
                    this.minimoGarantizado + "", this.costo_hora_normal, this.costo_hora_extra))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool eliminar(string tipo)
        {
            try
            {

                if (new conexion().eliminarTipoEquipo(tipo))
                    return true;

            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool update(string nombre, string familia, int minimo, double costo, double costoe)
        {
            try
            {

                if (new conexion().updateTipoEquipo(nombre, familia, minimo, costo, costoe))
                    return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool verificarArea(string nombre)
        {
            try
            {
                if (new conexion().verificarTipoEquipo(nombre))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
    }


}
