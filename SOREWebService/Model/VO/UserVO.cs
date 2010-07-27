using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace SOREWebService.Model.VO {

    /// <summary>
    /// Virtual Object para datos de usuario
    /// </summary>
    public class UserVO {

        private int id_user;
        private string userName;
        private string password;
        private string name;
        private ArrayList roles;
        private ArrayList contacts;

        public string UserName {
            get {
                return this.userName;
            }
            set {
                this.userName = value;
            }
        }

        public string Password {
            get {
                return this.password;
            }
            set {
                this.password = value;
            }
        }

        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
            }
        }

        public ArrayList Roles {
            get {
                return this.roles;
            }
            set {
                this.roles = value;
            }
        }

        public ArrayList Contacts {
            get {
                return this.contacts;
            }
            set {
                this.contacts = value;
            }
        }

        public int ID {
            get {
                return this.id_user;
            }
            set {
                this.id_user = value;
            }
        }

    }
}
