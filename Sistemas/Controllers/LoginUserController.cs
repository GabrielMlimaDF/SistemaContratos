using Sistemas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Sistemas.Controllers
{
    public class LoginUserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var verify = UsuarioModel.ValidarUser(login.EmailUsuario, login.SenhaUsuario);

            if (verify)
            {
                FormsAuthentication.SetAuthCookie(login.EmailUsuario, login.LembrarMe);
                if (Url.IsLocalUrl(returnUrl))
                {

                    return Redirect(returnUrl);

                }
                RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["msg"] = "Usuário ou senha inválidos.";

            }

            return View();

        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");

        }
    }
}