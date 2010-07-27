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
using System.Collections;
using System.Data.SqlClient;

namespace SOREWebService.Model.DAO {

    /// <summary>
    /// Data Access Object para la tabla CONTACT_TYPE
    /// </summary>
    public class ContactDAO : AbstractDAO {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con"></param>
        public ContactDAO(SqlConnection con) : base(con) { }

        /// <summary>
        /// Devuelve todos los tipos definidos en la base de datos
        /// </summary>
        /// <returns>Un ArrayList con los tipos definidos en la base de datos</returns>
        public ArrayList GetTypes() {
            ArrayList resultado = new ArrayList();
            this.cmd.CommandText = "SELECT NAME FROM CONTACT_TYPE";
            using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                while (reader.Read()) {
                    if (!reader.IsDBNull(0)) {
                        string nombre = reader.GetString(0);
                        resultado.Add(nombre);
                    }
                }
            }
            return resultado;
        }
    }
}
