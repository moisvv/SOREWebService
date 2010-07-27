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
    public class ContactVO {
        private int id_contact;
        private Int16 type;
        private string detail;

        public int ID {
            get {
                return this.id_contact;
            }
            set {
                this.id_contact = value;
            }
        }

        public Int16 Type {
            get {
                return this.type;
            }
            set {
                this.type = value;
            }
        }

        public string Detail {
            get {
                return this.detail;
            }
            set {
                this.detail = value;
            }
        }
    }
}
