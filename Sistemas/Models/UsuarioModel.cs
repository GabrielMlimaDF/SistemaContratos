using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sistemas.Models
{
    public class UsuarioModel
    {
        public static bool ValidarUser(string EmailUsuario, string SenhaUsuario)
        {
           
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sql = @"select count(*) From logins Where EmailUsuario=@EmailUsuario and SenhaUsuario=@SenhaUsuario";
                var ret = db.ExecuteScalar(sql, new { EmailUsuario = EmailUsuario, SenhaUsuario = SenhaUsuario });

                if ((int)ret !=0)
                {
                    return true;
                }
                return false;

            }

            
        }
        
        
    }
}
