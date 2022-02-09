using Dapper;
using Newtonsoft.Json;
using Sistemas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using X.PagedList;

namespace Sistemas.Controllers
{


    public class CadastrosController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();


        void connectionString()
        {
            con.ConnectionString = @"data source=TERMINAL-09\SQLEXPRESS; database=Sis_Contratos; integrated security = SSPI;";

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(Conta users)
        {

            if (users.IdUsuario == 0)
            {
                connectionString();
                con.Open();
                com.Connection = con;
                SqlCommand cmd = new SqlCommand("insert into logins(NomeUsuario,EmailUsuario,SenhaUsuario,CpfUsuario) values(@NomeUsuario,@EmailUsuario,@SenhaUsuario,@CpfUsuario)", con);
                cmd.Parameters.AddWithValue("@NomeUsuario", users.NomeUsuario);
                cmd.Parameters.AddWithValue("@EmailUsuario", users.EmailUsuario);
                cmd.Parameters.AddWithValue("@SenhaUsuario", users.SenhaUsuario);
                cmd.Parameters.AddWithValue("@CpfUsuario", users.CpfUsuario);



                int resultado = cmd.ExecuteNonQuery();
                if (resultado == 1)
                {

                    TempData["msg"] = "Usuário cadastrado com sucesso!";

                    return RedirectToAction("ListaUsuarios", "Cadastros");


                }
                con.Close();
                return View();
            }
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {


                db.Open();
                string sqlverificar = @"Select count(*) From logins Where IdUsuario=@IdUsuario";
                var retverificar = db.ExecuteScalar(sqlverificar, new { IdUsuario = users.IdUsuario });
                if ((int)retverificar == 0)
                {
                    TempData["msg2"] = "Não foi possível encontrar usuário. Edição não permitida!";
                    return RedirectToAction("ListaUsuarios");

                }
                string updateQuery = @"UPDATE [dbo].[logins] SET NomeUsuario = @NomeUsuario, EmailUsuario = @EmailUsuario, CpfUsuario = @CpfUsuario WHERE IdUsuario = @IdUsuario";
                object isActive = null;
                var result = db.Execute(updateQuery, new
                {
                    isActive,
                    users.IdUsuario,
                    users.NomeUsuario,
                    users.EmailUsuario,
                    users.CpfUsuario
                });


            }
            TempData["msg"] = "Usuário alterado com sucesso!";
            return RedirectToAction("ListaUsuarios");


        }
        [Authorize]
        public ActionResult Usuario()
        {
            var userviewmodel = new EdtUserViewModel();
            return View(userviewmodel);
        }
        [Authorize]
        [HttpGet]
        public ActionResult ListaUsuarios(int? pagina)
        {

            List<Conta> LisUser = new List<Conta>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sql = "Select* From logins ORDER BY IdUsuario DESC";
                LisUser = db.Query<Conta>(sql).ToList();
                int paginaTamanho = 15;
                int paginaNumero = (pagina ?? 1);
                return View(LisUser.ToPagedList(paginaNumero, paginaTamanho));

            }

        }
        [Authorize]
        public ActionResult SearchUser(int? pagina, string searcheng)
        {
            List<Conta> LisUser = new List<Conta>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sql = "Select * From logins ORDER BY IdUsuario DESC";
                LisUser = db.Query<Conta>(sql).ToList();
                int paginaTamanho = 15;
                int paginaNumero = (pagina ?? 1);

                var personsQry = LisUser.Where(x => x.EmailUsuario.ToLower().Contains(searcheng.ToLower())
                || x.CpfUsuario.Contains(searcheng)
                || x.NomeUsuario.ToLower().Contains(searcheng.ToLower()));


                return View("ListaUsuarios", personsQry.ToPagedList(paginaNumero, paginaTamanho));




            }



        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(int id)
        {

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sqlverificar = @"Select count(*) From logins Where IdUsuario=@IdUsuario";
                var retverificar = db.ExecuteScalar(sqlverificar, new { IdUsuario = id });
                if ((int)retverificar == 0)
                {
                    TempData["msg2"] = "Não foi possível encontrar usuário!";
                    return RedirectToAction("ListaUsuarios");

                }

                string sql = @"DELETE From logins Where IdUsuario=@IdUsuario";
                db.ExecuteScalar(sql, new { IdUsuario = id });
                string sqlfeedback = @"Select count(*) From logins Where IdUsuario=@IdUsuario";
                var ret = db.ExecuteScalar(sqlfeedback, new { IdUsuario = id });

                if ((int)ret == 0)
                {
                    TempData["msg"] = "Usuário excluido com sucesso!";
                    return RedirectToAction("ListaUsuarios");

                }
                else
                {
                    TempData["msg2"] = "Não foi possível excluir usuário!";
                    return RedirectToAction("ListaUsuarios");

                }


            }




        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario(int id, string nomeuser, string cpfuser, string emailuser)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sqlverificar = @"Select count(*) From logins Where IdUsuario=@IdUsuario";
                var retverificar = db.ExecuteScalar(sqlverificar, new { IdUsuario = id });
                if ((int)retverificar == 0)
                {
                    TempData["msg2"] = "Não foi possível encontrar usuário!";
                    return RedirectToAction("Usuario");

                }

                var userviewmodel = new EdtUserViewModel();
                userviewmodel.IdUsuario = id;
                userviewmodel.NomeUsuario = nomeuser;
                userviewmodel.CpfUsuario = cpfuser;
                userviewmodel.EmailUsuario = emailuser;

                return View("Usuario", userviewmodel);
            }
        }
        [Authorize]
        public ActionResult Empresa()
        {
            return View(new Sistemas.Models.ApiRestCnpj());
        }
        public async Task<ActionResult> BuscarCnpjApiAsync(string searchcnpj)
        {
            string dados = "";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://www.receitaws.com.br/v1/cnpj/" + searchcnpj);
                var rest = await client.GetAsync("");
                dados = await rest.Content.ReadAsStringAsync();

                if (dados == "Too many requests, please try again later.")
                {
                    TempData["errocnpj"] = "É possível consultar 3 CNPJ por minuto aguarde!";
                    return View("Empresa", new Sistemas.Models.ApiRestCnpj());
                }
            }

            var serializer = JsonConvert.DeserializeObject<ApiRestCnpj>(dados);

            foreach (var item in serializer.atividade_principal)
            {
               
                serializer.descnaeprincipal = item.text;
                serializer.cnaeprincipal = item.code;

            }

            if (serializer.status == "ERROR")
            {
                TempData["errocnpj"] = "Verifique o CNPJ digitado!";
            }

            return View("Empresa", serializer);

        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarEmpresa(CadEmpresa cadEmpresa)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {


                db.Open();

                string insertQuery = @"INSERT INTO [dbo].[CadEmpresas]([Cnpj], [RazaoSocial], [Cnae], [IEstadual], [Tel1], [Tel2], [Cep], [Endereco], [Cidade], [Bairro], [Estado], [DescCnaePrincipal],[Abertura],[NaturezaJur], [Porte],[Fantasia], [Email]) VALUES (@Cnpj, @RazaoSocial, @Cnae, @IEstadual, @Tel1, @Tel2, @Cep, @Endereco, @Cidade, @Bairro, @Estado, @DescCnaePrincipal, @Abertura, @NaturezaJur, @Porte, @Fantasia, @Email)";
                var result = db.Execute(insertQuery, cadEmpresa);

                TempData["msg"] = "Empresa cadastrada com sucesso!";
                return RedirectToAction("ListarEmpresa");

            }
        }
        public ActionResult ListarEmpresa(int? pagina)
        {
            List<CadEmpresa> LisEmp = new List<CadEmpresa>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sql = "Select* From CadEmpresas ORDER BY IdEmpresa DESC";
                LisEmp = db.Query<CadEmpresa>(sql).ToList();
                int paginaTamanho = 15;
                int paginaNumero = (pagina ?? 1);
                return View(LisEmp.ToPagedList(paginaNumero, paginaTamanho));

            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEmp(int id, string nomeemp, string cnpjemp, string endereco, string cidade, string bairro, string cep, string tel1, string Tel2, string fantasia, string IEstadual, string Abertura, string Estado, string NaturezaJur, string Porte, string CnaePrincipal, string AtvPrincipal, string EmailPrincipal)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["principal"].ConnectionString))
            {
                db.Open();
                string sqlverificar = @"Select count(*) From CadEmpresas Where IdEmpresa=@IdEmpresa";
                var retverificar = db.ExecuteScalar(sqlverificar, new { IdEmpresa = id });
                if ((int)retverificar == 0)
                {
                    TempData["msg2"] = "Não foi possível encontrar empresa!";
                    return RedirectToAction("Usuario");

                }

                var userviewmodel = new ApiRestCnpj();
                userviewmodel.IdEmpresa = id;
                userviewmodel.nome = nomeemp;
                userviewmodel.cnpj = cnpjemp;
                userviewmodel.logradouro = endereco;
                userviewmodel.municipio = cidade;
                userviewmodel.bairro = bairro;
                userviewmodel.cep = cep;
                userviewmodel.telefone = tel1;
                userviewmodel.fantasia = fantasia;
                userviewmodel.Iestadual = IEstadual;
                userviewmodel.Tel2 = Tel2;
                userviewmodel.abertura = Abertura;
                userviewmodel.uf = Estado;
                userviewmodel.natureza_juridica = NaturezaJur;
                userviewmodel.porte = Porte;
                userviewmodel.cnaeprincipal = CnaePrincipal;
                userviewmodel.descnaeprincipal = AtvPrincipal;
                userviewmodel.email = EmailPrincipal;

                return View("Empresa", userviewmodel);
            }
        }


    }
}
