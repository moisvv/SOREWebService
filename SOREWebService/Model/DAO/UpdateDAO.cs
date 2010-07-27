using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CommonServer.Model.DAO;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using NLog;
using System.Collections;

namespace SOREWebService.Model.DAO {

    /// <summary>
    /// DAO para gestionar las actualizaciones de la base de datos y la sincronización de la misma con dispositivos externos.
    /// </summary>
    public class UpdateDAO : AbstractDAO {

        private Logger Log;

        /// <summary>
        /// Nombres de las tablas de los valores TSO de la base de datos
        /// </summary>
        public readonly string[] TSOValuesTablesName = new string[] {
            "CONTEXT_EXTINFO_TYPE_VALUES",
            "CONTEXT_LEVEL_VALUES",
            "CONTEXT_LINK_ROLE_VALUES",
            "CONTEXT_MODE_VALUES",
            "CONTEXT_MSGTYPE_VALUES",
            "CONTEXT_SECLASS_VALUES",
            "CONTEXT_URGENCY_VALUES",
            "EVENT_CASUALTIES_CONTEXT_VALUES",
            "EVENT_CAUSE_VALUES",
            "EVENT_EGEO_STATUS_VALUES",
            "EVENT_EGEO_TYPE_VALUES",
            "EVENT_EGEO_WEATHER_VALUES",
            "EVENT_ETYPE_ACTOR_VALUES",
            "EVENT_ETYPE_CATEGORY_VALUES",
            "EVENT_ETYPE_LOCTYPE_VALUES",
            "EVENT_SOURCE_VALUES",
            "EVENT_STATUS_VALUES",
            "MISSION_STATUS_VALUES",
            "MISSION_TYPE_VALUES",
            "POSITION_COORDSYS_VALUES",
            "POSITION_ETYPE_VALUES",
            "POSITION_HEIGHTROLE_VALUES",
            "POSITION_TYPE_VALUES",
            "RESOURCE_CONTACT_TYPE_VALUES",
            "RESOURCE_RGEO_TYPE_VALUES",
            "RESOURCE_RTYPE_CAPABILITY_VALUES",
            "RESOURCE_RTYPE_CHARACTERISTICS_VALUES",
            "RESOURCE_RTYPE_CLASS_VALUES",
            "RESOURCE_STATUS_VALUES",
            "RESOURCE_UM_VALUES",
            "WIND_DIRECTION_VALUES"
        };

        /// <summary>
		/// Construye una instancia del DAO de actualización.
		/// </summary>
		public UpdateDAO(SqlConnection con) : base(con) {
            this.Log = LogManager.GetLogger("SOREWebServiceLog");
        }

        /// <summary>
        /// Construye un DataSet con todas las tablas de los valores TSO de la base de datos.
        /// </summary>
        /// <returns>Una instancia DataSet con todas las tablas de los valores TSO.</returns>
        public DataSet ConstructAllTSOValuesTablesSet() {
            //Inicializamos el resultado
            DataSet result = new DataSet("TSOValues");
            //Recorremos la lista de nombres de tablas TSO Values
            for (int i = 0; i < this.TSOValuesTablesName.Length; i++) {
                //Obtenemos el nombre de la tabla
                string tableName = this.TSOValuesTablesName[i];
                //Obtenemos los valores de la tabla
                DataTable dataTable = GetTable(tableName);
                //Añadimos la tabla al DataSet
                if (dataTable != null) {
                    result.Tables.Add(dataTable);
                }
            }

            //Resultado
            return result;
        }

        /// <summary>
        /// Construye un DataSet con todas aquellas tablas de los valores TSO modificados despues de una fecha.
        /// </summary>
        /// <param name="ultimaActualizacion">Fecha de la ultima actualización.</param>
        /// <returns>Una instancia DataSet con todas las tablas de los valores TSO.</returns>
        public DataSet ConstructAllTSOValuesTablesSet(DateTime ultimaActualizacion) {
            //Inicializamos el resultado
            DataSet result = new DataSet("TSOValues");
            //Recorremos la lista de nombres de tablas TSO Values
            for (int i = 0; i < this.TSOValuesTablesName.Length; i++) {
                //Obtenemos el nombre de la tabla
                string tableName = this.TSOValuesTablesName[i];
                //Obtenemos los valores de la tabla
                DataTable dataTable = GetTable(tableName, ultimaActualizacion);
                //Añadimos la tabla al DataSet
                if (dataTable != null) {
                    result.Tables.Add(dataTable);
                }
            }

            //Resultado
            return result;
        }

        /// <summary>
        /// Construye un DataSet con todas las tablas de los valores TSO de la base de datos en función de la categoría de acceso del usuario.
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <returns>Una instancia DataSet con todas las tablas de los valores TSO.</returns>
        public DataSet ConstructAllTSOValuesTablesSetwPerm(string userName) {
            //Inicializamos el resultado
            DataSet result = new DataSet("TSOValues");
            //Recorremos la lista de nombres de tablas TSO Values
            for (int i = 0; i < this.TSOValuesTablesName.Length; i++) {
                //Obtenemos el nombre de la tabla
                string tableName = this.TSOValuesTablesName[i];
                //Obtenemos los valores de la tabla
                DataTable dataTable = GetTablewPerm(tableName, userName);
                //Añadimos la tabla al DataSet
                if (dataTable != null) {
                    result.Tables.Add(dataTable);
                }
            }

            //Resultado
            return result;
        }

        /// <summary>
        /// Construye un DataSet con aquellas tablas de los valores TSO de la base de datos que hayan sido modificadas a partir de una fecha y en función de la categoría de acceso del usuario.
        /// </summary>
        /// <param name="userName">Nombre de usuario.</param>
        /// <param name="ultimaActualizacion">Fecha de la última actualización.</param>
        /// <returns>Una instancia DataSet con todas las tablas de los valores TSO.</returns>
        public DataSet ConstructAllTSOValuesTablesSetwPerm(string userName, DateTime ultimaActualizacion) {
            //Inicializamos el resultado
            DataSet result = new DataSet("TSOValues");
            //Recorremos la lista de nombres de tablas TSO Values
            for (int i = 0; i < this.TSOValuesTablesName.Length; i++) {
                //Obtenemos el nombre de la tabla
                string tableName = this.TSOValuesTablesName[i];
                //Obtenemos los valores de la tabla
                DataTable dataTable = GetTablewPerm(tableName, userName, ultimaActualizacion);
                //Añadimos la tabla al DataSet
                if (dataTable != null) {
                    result.Tables.Add(dataTable);
                }
            }

            //Resultado
            return result;
        }

        /// <summary>
        /// Devuelve un DataTable que contiene los valores de la tabla cuyo nombre se pasa como parámetro.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla</param>
        /// <returns>Un DataTable con los valores de la tabla especificada.</returns>
        public DataTable GetTable(string tableName) {
            cmd.CommandText = "SELECT * FROM " + tableName;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable(tableName);
            adapter.Fill(result);
            return result;
        }

        /// <summary>
        /// Devuelve un DataTable que contiene los valores de aquella tabla cuyo nombre se pasa como parámetro y que ha sido modificada a partir de una fecha.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla.</param>
        /// <param name="ultimaActualizacion">Fecha de la última actualización.</param>
        /// <returns>Un DataTable con los valores de la tabla especificada.</returns>
        public DataTable GetTable(string tableName, DateTime ultimaActualizacion) {
            cmd.CommandText = "SELECT UPDATE_DATETIME FROM TSO_VALUES_TABLES_UPDATES WHERE TABLE_NAME = '" + tableName + "'";
            using (SqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.Read()) {
                    DateTime fechaTabla = ultimaActualizacion;
                    if (!reader.IsDBNull(0)) {
                        fechaTabla = reader.GetDateTime(0);
                    }
                    if (fechaTabla <= ultimaActualizacion) {
                        return null;
                    }
                }
            }
            cmd.CommandText = "SELECT * FROM " + tableName;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable(tableName);
            adapter.Fill(result);
            return result;
        }

        /// <summary>
        /// Devuelve un DataTable que contiene los valores de la tabla cuyo nombre se pasa como parámetro en función de la categoría de acceso del usuario.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla</param>
        /// <param name="userName">Nombre de usuario</param>
        /// <returns>Un DataTable con los valores de la tabla especificada.</returns>
        public DataTable GetTablewPerm(string tableName, string userName) {
            try {
                DataTable resultado = new DataTable(tableName);
                resultado.Columns.Add("ID");
                resultado.Columns.Add("VALUE");
                resultado.Columns.Add("NAME");
                resultado.Columns.Add("DEFINITION");
                resultado.Columns.Add("PERMISSION");

                //Obtenemos el Rol del usuario
                cmd.CommandText = "SELECT ID_ROLE FROM USERS WHERE USER_NAME = @user_name";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@user_name", userName);
                sbyte id_role = -1;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        id_role = (sbyte)reader.GetByte(0);
                    }
                }
                //Obtenemos el nivel de acceso del usuario
                cmd.CommandText = "SELECT * FROM " + tableName;
                short ID;
                string VALUE;
                string NAME;
                string DEFINITION;
                byte[] PERMISSION = new byte[50];
                bool permissionNull = false;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        int i = 0;
                        ID = reader.GetInt16(i++);
                        if (!reader.IsDBNull(i)) {
                            VALUE = reader.GetString(i++);
                        }
                        else {
                            VALUE = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            NAME = reader.GetString(i++);
                        }
                        else {
                            NAME = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            DEFINITION = reader.GetString(i++);
                        }
                        else {
                            DEFINITION = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            reader.GetBytes(i++, 0, PERMISSION, 0, 50);
                        }
                        else {
                            permissionNull = true;
                        }

                        //Suponemos que el usuario tiene acceso a la fila
                        bool access = true;
                        //Si el nivel de acceso leido es distinto de null
                        if (!permissionNull) {
                            if (id_role != -1) {
                                if (PERMISSION[id_role] == 0) {
                                    access = false;
                                }
                            }
                        }
                        //Si el usuario tiene acceso a la fila, añadimos los datos al resultado
                        if (access) {
                            object[] values = new object[5];
                            reader.GetValues(values);
                            resultado.Rows.Add(values);
                        }
                    }
                }
                return resultado;
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportAllTSOValuesTables: " + e.Message, e);
                throw e;
            }
        }

        /// <summary>
        /// Devuelve un DataTable que contiene los valores de la tabla cuyo nombre se pasa como parámetro si ha sido modificada a partir de una fecha y en función de la categoría de acceso del usuario.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla.</param>
        /// <param name="userName">Nombre de usuario.</param>
        /// <param name="ultimaActualizacion">Fecha de la ultima actualización.</param>
        /// <returns>Un DataTable con los valores de la tabla especificada.</returns>
        public DataTable GetTablewPerm(string tableName, string userName, DateTime ultimaActualizacion) {
            try {

                //Comprobamos que la fecha de la ultima actualizacion sea anterior a la fecha de la ultima modificación de la tabla.
                cmd.CommandText = "SELECT UPDATE_DATETIME FROM TSO_VALUES_TABLES_UPDATES WHERE TABLE_NAME = '" + tableName + "'";
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        DateTime fechaTabla = ultimaActualizacion;
                        if (!reader.IsDBNull(0)) {
                            fechaTabla = reader.GetDateTime(0);
                        }
                        if (fechaTabla <= ultimaActualizacion) {
                            return null;
                        }
                    }
                }

                DataTable resultado = new DataTable(tableName);
                resultado.Columns.Add("ID");
                resultado.Columns.Add("VALUE");
                resultado.Columns.Add("NAME");
                resultado.Columns.Add("DEFINITION");
                resultado.Columns.Add("PERMISSION");

                //Obtenemos el Rol del usuario
                cmd.CommandText = "SELECT ID_ROLE FROM USERS WHERE USER_NAME = @user_name";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@user_name", userName);
                sbyte id_role = -1;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        id_role = (sbyte)reader.GetByte(0);
                    }
                }
                //Obtenemos el nivel de acceso del usuario
                cmd.CommandText = "SELECT * FROM " + tableName;
                short ID;
                string VALUE;
                string NAME;
                string DEFINITION;
                byte[] PERMISSION = new byte[50];
                bool permissionNull = false;
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        int i = 0;
                        ID = reader.GetInt16(i++);
                        if (!reader.IsDBNull(i)) {
                            VALUE = reader.GetString(i++);
                        }
                        else {
                            VALUE = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            NAME = reader.GetString(i++);
                        }
                        else {
                            NAME = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            DEFINITION = reader.GetString(i++);
                        }
                        else {
                            DEFINITION = "";
                        }
                        if (!reader.IsDBNull(i)) {
                            reader.GetBytes(i++, 0, PERMISSION, 0, 50);
                        }
                        else {
                            permissionNull = true;
                        }

                        //Suponemos que el usuario tiene acceso a la fila
                        bool access = true;
                        //Si el nivel de acceso leido es distinto de null
                        if (!permissionNull) {
                            if (id_role != -1) {
                                if (PERMISSION[id_role] == 0) {
                                    access = false;
                                }
                            }
                        }
                        //Si el usuario tiene acceso a la fila, añadimos los datos al resultado
                        if (access) {
                            object[] values = new object[5];
                            reader.GetValues(values);
                            resultado.Rows.Add(values);
                        }
                    }
                }
                return resultado;
            }
            catch (Exception e) {
                this.Log.ErrorException("Excepción en SOREWebService.ImportAllTSOValuesTables: " + e.Message, e);
                throw e;
            }
        }

    }
}
