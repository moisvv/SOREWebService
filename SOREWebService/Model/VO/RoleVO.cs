using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SOREWebService.Model.VO {
    /// <summary>
    /// Virtual Object para datos de roles.
    /// </summary>
    public class RoleVO {

        private Int16 id_role;
        private string name;
        private string description;

        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
            }
        }

        public string Description {
            get {
                return this.description;
            }
            set {
                this.description = value;
            }
        }

        public Int16 ID {
            get {
                return this.id_role;
            }
            set {
                this.id_role = value;
            }
        }
    }
}
