using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Configuration;
using SOREWebService.Model.DAO;
using NLog;
using SOREWebService.Model.VO;
using System.Net;

namespace SOREWebService {
    /// <summary>
    ///     /// <summary>
    /// Servicio Web del proyecto SORE, subproyecto INTERFACES AVANZADAS
    /// Funcionalidades:
    /// 1) Sincronización de datos entre terminales móviles y bases de datos centrales
    /// </summary>
    /// </summary>
    [WebService(Namespace = "http://193.144.80.223/SOREWebService",
                Description = "Servicio web para el proyecto SORE, subproyecto INTERFACES AVANZADAS")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Service1 : System.Web.Services.WebService {

        private SqlConnection connection;
        public SOREWebServiceAutheticationHeader AuthHeader;
        private Logger Log;

        /// <summary>
        /// Devuelve una instancia del servicio web.
        /// Inicializa la conexión SQL con la cadena de conexión guardada en el fichero Web.config
        /// </summary>
        public Service1() {
            this.Log = LogManager.GetLogger("SOREWebServiceLog");
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            this.connection = new SqlConnection();
            this.connection.ConnectionString = (string)appSettingsReader.GetValue("SqlConnection.ConnectionString", typeof(string));
        }

        /// <summary>
        /// Devuelve al cliente un token para autentificación, obtenido a partir de la fecha actual.
        /// Válido durante un minuto.
        /// </summary>
        /// <returns>El token calculado.</returns>
        [WebMethod(Description = "Devuelve un token para autentificación")]
        public string GetToken() {
            try {
                Log.Debug("Ha sido solicitado un token");
                using (WebAuthenticationDAO webAuthDAO = new WebAuthenticationDAO(connection)) {
                    return webAuthDAO.GetToken();
                }
            }
            catch (FormatException e) {
                Log.ErrorException("FormatExeption en SOREWebService.GetToken: " + e.Message, e);
                throw e;
            }
            catch (ArgumentOutOfRangeException e) {
                Log.ErrorException("ArgumentOutOfRangeException en SOREWebService.GetToken: " + e.Message, e);
                throw e;
            }
            catch (ArgumentNullException e) {
                Log.ErrorException("ArgumentNullException en SOREWebService.GetToken: " + e.Message, e);
                throw e;
            }
            catch (ArgumentException e) {
                Log.ErrorException("ArgumentException en SOREWebService.GetToken: " + e.Message, e);
                throw e;
            }
            catch (Exception e) {
                Log.ErrorException("Exception en SOREWebService.GetToken: " + e.Message, e);
                throw e;
            }
        }

        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Comprueba que el usuario tiene acceso al servicio web")]
        public bool CheckUser() {
            try {
                return this.Login(AuthHeader.UserName, AuthHeader.Key);
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.CheckUser: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve todas las tablas de valores TSO de la base de datos.
        /// Las tablas de valores TSO son las siguientes:
        /// <list type="bullet">
        /// <item>CONTEXT_EXTINFO_TYPE_VALUES</item>
        /// <item>CONTEXT_LEVEL_VALUES</item>
        /// <item>CONTEXT_MODE_VALUES</item>
        /// <item>CONTEXT_MSGTYPE_VALUES</item>
        /// <item>CONTEXT_SECLASS_VALUES</item>
        /// <item>CONTEXT_URGENCY_VALUES</item>
        /// <item>EVENT_CASUALTIES_CONTEXT_VALUES</item>
        /// <item>EVENT_CAUSE_VALUES</item>
        /// <item>EVENT_EGEO_STATUS_VALUES</item>
        /// <item>EVENT_EGEO_TYPE_VALUES</item>
        /// <item>EVENT_EGEO_WEATHER_VALUES</item>
        /// <item>EVENT_ETYPE_ACTOR_VALUES</item>
        /// <item>EVENT_ETYPE_CATEGORY_VALUES</item>
        /// <item>EVENT_ETYPE_LOCTYPE_VALUES</item>
        /// <item>EVENT_SOURCE_VALUES</item>
        /// <item>EVENT_STATUS_VALUES</item>
        /// <item>MISSION_STATUS_VALUES</item>
        /// <item>MISSION_TYPE_VALUES</item>
        /// <item>POSITION_COORDSYS_VALUES</item>
        /// <item>POSITION_ETYPE_VALUES</item>
        /// <item>POSITION_HEIGHTROLE_VALUES</item>
        /// <item>POSITION_TYPE_VALUES</item>
        /// <item>RESOURCE_CONTACT_TYPE_VALUES</item>
        /// <item>RESOURCE_RGEO_TYPE_VALUES</item>
        /// <item>RESOURCE_RTYPE_CAPABILITY_VALUES</item>
        /// <item>RESOURCE_RTYPE_CHARACTERISTICS_VALUES</item>
        /// <item>RESOURCE_RTYPE_CLASS_VALUES</item>
        /// <item>RESOURCE_STATUS_VALUES</item>
        /// <item>WIND_DIRECTION_VALUES</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Devuelve todas las tablas de valores TSO de la base de datos.")]
        public DataSet ImportAllTSOValuesTables() {
            try {
                this.Log.Debug("Solicitud de descarga de todas las tablas.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.ConstructAllTSOValuesTablesSet();
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportAllTSOValuesTables: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve todas las tablas de valores TSO de la base de datos en función de la categoría de acceso del usuario.
        /// Las tablas de valores TSO son las siguientes:
        /// <list type="bullet">
        /// <item>CONTEXT_EXTINFO_TYPE_VALUES</item>
        /// <item>CONTEXT_LEVEL_VALUES</item>
        /// <item>CONTEXT_MODE_VALUES</item>
        /// <item>CONTEXT_MSGTYPE_VALUES</item>
        /// <item>CONTEXT_SECLASS_VALUES</item>
        /// <item>CONTEXT_URGENCY_VALUES</item>
        /// <item>EVENT_CASUALTIES_CONTEXT_VALUES</item>
        /// <item>EVENT_CAUSE_VALUES</item>
        /// <item>EVENT_EGEO_TYPE_VALUES</item>
        /// <item>EVENT_EGEO_WEATHER_VALUES</item>
        /// <item>EVENT_ETYPE_ACTOR_VALUES</item>
        /// <item>EVENT_ETYPE_CATEGORY_VALUES</item>
        /// <item>EVENT_ETYPE_LOCTYPE_VALUES</item>
        /// <item>EVENT_SOURCE_VALUES</item>
        /// <item>EVENT_STATUS_VALUES</item>
        /// <item>MISSION_STATUS_VALUES</item>
        /// <item>MISSION_TYPE_VALUES</item>
        /// <item>POSITION_COORDSYS_VALUES</item>
        /// <item>POSITION_ETYPE_VALUES</item>
        /// <item>POSITION_HEIGHTROLE_VALUES</item>
        /// <item>POSITION_TYPE_VALUES</item>
        /// <item>RESOURCE_CONTACT_TYPE_VALUES</item>
        /// <item>RESOURCE_RGEO_TYPE_VALUES</item>
        /// <item>RESOURCE_RTYPE_CAPABILITY_VALUES</item>
        /// <item>RESOURCE_RTYPE_CHARACTERISTICS_VALUES</item>
        /// <item>RESOURCE_RTYPE_CLASS_VALUES</item>
        /// <item>RESOURCE_STATUS_VALUES</item>
        /// <item>WIND_DIRECTION_VALUES</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Devuelve todas las tablas de valores TSO de la base de datos.")]
        public DataSet ImportAllTSOValuesTableswPerm() {
            try {
                this.Log.Debug("Solicitud de descarga de todas las tablas en función de la categoría de acceso del usuario.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.ConstructAllTSOValuesTablesSetwPerm(AuthHeader.UserName);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportAllTSOValuesTableswPerm: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve la tabla de valores TSO especificada como parámetro.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla.</param>
        /// <returns>Un DataTable con los valores de la tabla especificada.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Devuelve la tabla de valores TSO especificada.")]
        public DataTable ImportTSOValueTable(string tableName) {
            try {
                this.Log.Debug("Solicitud de descarga de la tabla: " + tableName);

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.GetTable(tableName);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportTSOValueTable: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve la tabla de valores TSO especificada como parámetro en función del nivel de acceso del usuario.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla.</param>
        /// <returns>Un DataTable con los valores de la tabla especificada en función del nivel de acceso del usuario.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Devuelve la tabla de valores TSO especificada.")]
        public DataTable ImportTSOValueTablewPerm(string tableName) {
            try {
                this.Log.Debug("Solicitud de descarga de la tabla: " + tableName + " en función del nivel de acceso del usuario.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.GetTablewPerm(tableName, AuthHeader.UserName);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportTSOValueTablewPerm: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve un DataSet con los datos de aquellas tablas de valores TSO que hayan sido actualizadas despues
        /// de la fecha ultimaActualizacion.
        /// </summary>
        /// <param name="ultimaActualizacion">Fecha de ultima actualizacion de las tablas del cliente.</param>
        /// <returns>DataSet con los datos de las tablas requeridas.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description="Actualización inteligente de todas las tablas de valores TSO en función de una fecha de actualización.")]
        public DataSet SyncAllTSOValuesTables(DateTime ultimaActualizacion) {
             try {
                this.Log.Debug("Solicitud de sincronización de las tablas de valores TSO.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.ConstructAllTSOValuesTablesSet(ultimaActualizacion);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.SyncAllTSOValuesTables: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve un DataSet con los datos de aquellas tablas de valores TSO que hayan sido actualizadas despues
        /// de la fecha ultimaActualizacion, en función del nivel de acceso del usuario.
        /// </summary>
        /// <param name="ultimaActualizacion">Fecha de ultima actualizacion de las tablas del cliente.</param>
        /// <returns>DataSet con los datos de las tablas requeridas.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Actualización inteligente de todas las tablas de valores TSO en función de una fecha de actualización y teniendo en cuenta el nivel de acceso del usuario.")]
        public DataSet SyncAllTSOValuesTableswPerm(DateTime ultimaActualizacion) {
            try {
                this.Log.Debug("Solicitud de sincronización de las tablas de valores TSO en función del nivel de acceso del usuario.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.ConstructAllTSOValuesTablesSetwPerm(AuthHeader.UserName, ultimaActualizacion);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.SyncAllTSOValuesTableswPerm: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve un DataSet con los datos de la tabla de valores TSO si ésta ha sido modificada despues
        /// de la fecha ultimaActualizacion.
        /// </summary>
        /// <param name="ultimaActualizacion">Fecha de ultima actualizacion de las tablas del cliente.</param>
        /// <param name="tableName">Nombre de la tabla a sincronizar.</param>
        /// <returns>DataSet con los datos de las tablas requeridas.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Actualización inteligente de la tabla de valores TSO especificada.")]
        public DataTable SyncTSOValueTable(DateTime ultimaActualizacion, string tableName) {
            try {
                this.Log.Debug("Solicitud de sincronización de la tabla de valores TSO " + tableName);

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.GetTable(tableName, ultimaActualizacion);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.SyncTSOValueTable: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve un DataSet con los datos de la tabla de valores TSO si ésta ha sido modificada despues
        /// de la fecha ultimaActualizacion, en función del nivel de acceso del usuario.
        /// </summary>
        /// <param name="ultimaActualizacion">Fecha de ultima actualizacion de las tablas del cliente.</param>
        /// <param name="tableName">Nombre de la tabla a sincronizar.</param>
        /// <returns>DataSet con los datos de las tablas requeridas.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Actualización inteligente de la tabla de valores TSO especificada en función del nivel de acceso del usuario.")]
        public DataTable SyncTSOValueTablewPerm(DateTime ultimaActualizacion, string tableName) {
            try {
                this.Log.Debug("Solicitud de sincronización de la tabla de valores TSO " + tableName + " en función del nivel de acceso del usuario.");

                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UpdateDAO updateDAO = new UpdateDAO(this.connection)) {
                        return updateDAO.GetTablewPerm(tableName, AuthHeader.UserName, ultimaActualizacion);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificación. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.SyncTSOValueTablewPerm: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve los roles del usuario especificado.
        /// </summary>
        /// <param name="userName">Nombre de usuario del que se obtendrán los roles.</param>
        /// <returns>Un ArrayList con los VO de los roles del usuario.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Obtiene los roles de un usuario")]
        public ArrayList GetRolesByUser(string userName) {
            try {
                this.Log.Debug("Petición de roles para el usuario " + userName);
                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UsersDAO usersDAO = new UsersDAO(this.connection)) {
                        return usersDAO.GetRoles(userName);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificacion. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e){
                this.Log.ErrorException("Excepcion en SOREWebService.GetRolesByUser: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve todos los roles definidos en la base de datos.
        /// </summary>
        /// <returns>Un ArrayList con los VO de todos los roles.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Obtiene todos los roles definidos.")]
        public ArrayList GetRoles() {
            try {
                this.Log.Debug("Petición de todos los roles definidos en la base de datos");
                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (RolesDAO rolesDAO = new RolesDAO(this.connection)) {
                        return rolesDAO.GetRoles();
                    }
                }
                else {
                    this.Log.Warn("Error de autentificacion. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepcion en SOREWebService.GetRolesByUser: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve las formas de contacto del usuario especificado.
        /// </summary>
        /// <param name="userName">Nombre de usuario.</param>
        /// <returns>ArrayList de objetos ContactVO.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Obtiene los contactos de un usuario")]
        public ArrayList GetContactsByUser(string userName) {
            try {
                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UsersDAO usersDAO = new UsersDAO(this.connection)) {
                        return usersDAO.GetContacts(userName);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificacion. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepcion en SOREWebService.GetContactsByUser: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve los tipos de contacto definidos en la base de datos.
        /// </summary>
        /// <returns>Un ArrayList de strings con los tipos de contacto.</returns>
        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Devuelve todos los tipos de contacto definidos en la base de datos")]
        public object[] GetContactTypes() {
            try {
                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (ContactDAO contactDAO = new ContactDAO(this.connection)) {
                        return contactDAO.GetTypes().ToArray();
                    }
                }
                else {
                    this.Log.Warn("Error de autentificacion. Usuario: " + AuthHeader.UserName);
                    return null;
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepcion en SOREWebService.GetContactTypes: " + e.Message, e);
                throw e;
            }
        }

        [SoapHeader("AuthHeader")]
        [WebMethod(Description = "Introduce un nuevo usuario en la base de datos")]
        public bool NewUser(UserVO userData) {
            try {
                if (this.Login(AuthHeader.UserName, AuthHeader.Key)) {
                    using (UsersDAO usersDAO = new UsersDAO(this.connection)) {
                        return usersDAO.InsertUser(userData);
                    }
                }
                else {
                    this.Log.Warn("Error de autentificacion. Usuario: " + AuthHeader.UserName);
                    return false; 
                }
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepcion en SOREWebService.NewUser: " + e.Message, e);
                throw e;
            }
        }

        #region Métodos de apoyo

        private bool Login(string userName, string key) {
            using (WebAuthenticationDAO webAuthDAO = new WebAuthenticationDAO(connection)) {
                try {
                    this.connection.Open();
                    return webAuthDAO.CheckKey(userName, key, 0);
                }
                catch (InvalidOperationException e) {
                    Log.ErrorException("InvalidOperationException en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
                catch (SqlException e) {
                    Log.ErrorException("SqlException en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
                catch (ArgumentNullException e) {
                    Log.ErrorException("ArgumentNullException en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
                catch (IndexOutOfRangeException e) {
                    Log.ErrorException("IndexOutOfRangeException en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
                catch (ArgumentOutOfRangeException e) {
                    Log.ErrorException("ArgumentOutOfRangeException en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
                catch (Exception e) {
                    Log.ErrorException("Exception en SOREWebService.Login: " + e.Message, e);
                    return false;
                }
            }
        }

        #endregion

    }

    /// <remarks>
    /// Definición de la cabecera de autenticación utilizada en las
    /// llamadas a las funciones del servicio web definido en
    /// <see cref="SOREWebService">SOREWebService</see>.
    /// </remarks>
    public class SOREWebServiceAutheticationHeader : SoapHeader {
        public string UserName;
        public string Key;
    }
}
