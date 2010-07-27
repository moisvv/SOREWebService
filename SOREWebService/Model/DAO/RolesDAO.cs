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
using SOREWebService.Model.VO;

namespace SOREWebService.Model.DAO {
    public class RolesDAO :AbstractDAO {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con"></param>
        public RolesDAO(SqlConnection con) : base(con) { }

        public ArrayList GetRoles() {
            ArrayList resultado = new ArrayList();
            this.cmd.CommandText = "SELECT ID_ROLE, NAME, DESCRIPTION FROM ROLES";
            using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                while (reader.Read()) {
                    Int16 id_role = -1;
                    string Name = "";
                    string Description = "";
                    if (!reader.IsDBNull(0)) {
                        id_role = reader.GetInt16(0);
                    }
                    if (!reader.IsDBNull(1)) {
                        Name = reader.GetString(1);
                    }
                    if (!reader.IsDBNull(2)) {
                        Description = reader.GetString(2);
                    }
                    RoleVO roleVO = new RoleVO();
                    roleVO.ID = id_role;
                    roleVO.Name = Name;
                    roleVO.Description = Description;
                    resultado.Add(roleVO);
                }
            }
            return resultado;
        }
    }
}
