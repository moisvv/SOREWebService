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
using System.Collections;
using SOREWebService.Model.VO;

namespace SOREWebService.Model.DAO {
    public class UsersDAO : AbstractDAO {

        public UsersDAO(SqlConnection con) : base(con) { }

        private string insertUserCmd = "INSERT INTO USERS (USER_NAME, PASSWORD, NAME) VALUES (@USER_NAME, @PASSWORD, @NAME)";
        private string insertContactCmd = "INSERT INTO CONTACT_USER (ID_USER, TIPO, DETAIL) VALUES (@ID_USER, @TIPO, @DETAIL)";
        private string insertRoleCmd = "INSERT INTO USERS_ROLES (ID_USER, ID_ROLE) VALUES (@ID_USER, @ID_ROLE)";

        public ArrayList GetRoles(string userName){
            ArrayList resultado = new ArrayList();
            int id_user = -1;
            this.cmd.CommandText = "SELECT ID_USER FROM USERS WHERE USER_NAME = " + userName;
            using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                if (reader.Read()) {
                    if (!reader.IsDBNull(0)) {
                        id_user = reader.GetInt32(0);
                    }
                }
            }
            if (id_user != -1) {
                this.cmd.CommandText = "SELECT ROLES.ID_ROLE, NAME, DESCRIPTION FROM ROLES INNER JOIN USERS_ROLES ON USERS_ROLES.ID_USER = " + id_user + " AND USERS_ROLES.ID_ROLE = ROLES.ID_ROLE";
                using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                    if (reader.Read()) {
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
            }
            return resultado;
        }


        public ArrayList GetContacts(string userName) {
            ArrayList resultado = new ArrayList();
            this.cmd.CommandText = "SELECT CONTACT_USER.ID_USER, TIPO, DETAIL FROM CONTACT_USER INNER JOIN USERS ON USERS.USER_NAME = " + userName + " AND USERS.ID_CONTACT = CONTACT_USER.ID_CONTACT";
            using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                while (reader.Read()) {
                    int ID = -1;
                    Int16 tipo = -1;
                    string detail = "";

                    if (!reader.IsDBNull(0)) {
                        ID = reader.GetInt32(0);
                    }
                    if (!reader.IsDBNull(1)) {
                        tipo = reader.GetInt16(1);
                    }
                    if (!reader.IsDBNull(2)) {
                        detail = reader.GetString(2);
                    }
                    ContactVO contactVO = new ContactVO();
                    contactVO.ID = ID;
                    contactVO.Type = tipo;
                    contactVO.Detail = detail;
                    resultado.Add(contactVO);
                }
            }
            return resultado;
        }

        public bool InsertUser(UserVO user) {

            //Introducimos usuario en la tabla USERS
            this.cmd.Parameters.Clear();
            this.cmd.CommandText = this.insertUserCmd;
            this.cmd.Parameters.AddWithValue("@USER_NAME", user.UserName);
            this.cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            this.cmd.Parameters.AddWithValue("@NAME", user.Name);
            int rows = this.cmd.ExecuteNonQuery();
            if (rows != 1) {
                return false;
            }

            //Obtenemos el ID con el que se ha guardado este usuario
            int id_user = -1;
            this.cmd.Parameters.Clear();
            this.cmd.CommandText = "SELECT ID_USER FROM USERS WHERE USER_NAME = " + user.UserName;
            using (SqlDataReader reader = this.cmd.ExecuteReader()) {
                if (reader.Read()) {
                    if (!reader.IsDBNull(0)) {
                        id_user = reader.GetInt32(0);
                    }
                }
            }
            if (id_user == -1) {
                return false;
            }

            //Introducimos los contactos del usuario
            this.cmd.CommandText = this.insertContactCmd;
            foreach (ContactVO contactVO in user.Contacts) {
                this.cmd.Parameters.Clear();
                this.cmd.Parameters.AddWithValue("@ID_USER", id_user);
                this.cmd.Parameters.AddWithValue("@TIPO", contactVO.Type);
                this.cmd.Parameters.AddWithValue("@DETAIL", contactVO.Detail);
                rows = this.cmd.ExecuteNonQuery();
                if (rows != 1) {
                    return false;
                }
            }

            //Introducimos los roles del usuario
            this.cmd.CommandText = this.insertRoleCmd;
            foreach (RoleVO roleVO in user.Roles) {
                this.cmd.Parameters.Clear();
                this.cmd.Parameters.AddWithValue("@ID_USER", id_user);
                this.cmd.Parameters.AddWithValue("@ID_ROLE", roleVO.ID);
                rows = this.cmd.ExecuteNonQuery();
                if (rows != 1) {
                    return false;
                }
            }

            //Si todo ha ido bien, devolvemos true
            return true;
        }
    }
}
