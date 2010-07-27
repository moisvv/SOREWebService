using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using CommonServer.Model.DAO;

namespace SOREWebService.Model.DAO {

    /// <summary>
    /// Data Access Object que implementa funciones de autentificación de usuarios:
    /// 1) Cálculo de token en función de fecha actual
    /// 2) Comprobación de clave y ventana de token
    /// 3) Obtención de contraseña de usuario desde la base de datos
    /// </summary>
    public class WebAuthenticationDAO : AbstractDAO {

        private string SelectByUserCommand = "SELECT PASSWORD FROM USERS WHERE USER_NAME = @user_name";

        /// <summary>
        /// Crea una instancia de 
        /// <see cref="WebAuthenticationDAO">WebAuthenticationDAO</see>.
        /// </summary>
        public WebAuthenticationDAO(SqlConnection con) : base(con) { }

        /// <summary>
        /// Calcula un token a partir del hash MD5 de la fecha actual en formato YYYYMMDDHHmm.
        /// </summary>
        /// <returns>El token calculado.</returns>
        public string GetToken() {
            string toBeHash;
            DateTime dt = DateTime.Now;
            toBeHash = dt.ToString("yyyyMMddHHmm");
            return Hash(toBeHash);
        }

        /// <summary>
        /// Codifica en formato MD5 el <see cref="string">string</see> 
        /// que se le pasa como parámetro.
        /// </summary>
        /// <param name="toHash">Cadena a codificar</param>
        /// <returns>
        /// Cadena codificada en formato MD5.
        /// </returns>
        private string Hash(string toHash) {
            // First we need to convert the string into bytes,
            // which means using a text encoder.
            Encoder enc = System.Text.Encoding.ASCII.GetEncoder();

            // Create a buffer large enough to hold the string.
            byte[] buffer = new byte[toHash.Length];
            enc.GetBytes(toHash.ToCharArray(), 0, toHash.Length, buffer, 0, true);

            // Use one implementation of the abstract class MD5.
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] byteResult = md5.ComputeHash(buffer);

            // Prepare result;
            string result;
            result = BitConverter.ToString(byteResult);
            result = result.Replace("-", "");
            result = result.ToLower();

            return result;
        }

        /// <summary>
        /// Obtenemos la contraseña de usuario desde la base de datos a partir de su nombre de usuario.
        /// </summary>
        /// <param name="userName">Nombre de usuario.</param>
        /// <returns>Contraseña relacionada con el nombre de usuario.</returns>
        public string GetPassword(string userName) {
            this.cmd.CommandText = this.SelectByUserCommand;
            this.cmd.Parameters.Clear();
            this.cmd.Parameters.AddWithValue("@user_name", userName);
            string password = null;
            using (IDataReader reader = cmd.ExecuteReader()) {
                if (reader.Read()) {
                    password = reader.GetString(0);
                }
            }
            return password;
        }

        /// <summary>
        /// Comprueba si la clave pasada por el usuario es todavía válida en el intervalo de tiempo de un minuto.
        /// </summary>
        /// <param name="userName">Nombre de usuario.</param>
        /// <param name="key">Clave en formanto H(userName + password + H(YYYYMMDDHHmm)).</param>
        /// <param name="minutes">Desfase en minutos a tener en cuenta para el calculo de la clave.</param>
        /// <returns>True si la clave es todavía válida, o false en caso contrario.</returns>
        public bool CheckKey(string userName, string key, int minutes) {
            //Obtenemos el password del usuario
            string password = this.GetPassword(userName);
            //Obtenemos la fecha actual con el desfase en minutos pasado como parámetro
            DateTime dt = DateTime.Now - (new TimeSpan(0, minutes, 0));
            //Calculamos la clave esperada
            string expectedKey = Hash(userName + password + Hash(dt.ToString("yyyyMMddHHmm")));
            //Si la clave pasada es igual a la clave calculado, devolvemos true
            if (expectedKey == key) {
                return true;
            }
            //Sino, si el desfase en minutos es nulo, volvemos a comprobar con un desfase de un minuto
            else if (minutes == 0) {
                return CheckKey(userName, key, 1);
            }
            //Si el desfase no es nulo, devolvemos false (máximo desfase soportado: 1 minuto)
            else {
                return false;
            }
        }

    }
}

