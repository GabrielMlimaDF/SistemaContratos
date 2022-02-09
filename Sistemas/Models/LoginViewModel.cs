using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistemas.Models
{
    public class LoginViewModel
    {
        public string EmailUsuario{ get; set; }
        public string SenhaUsuario { get; set; }
        public bool LembrarMe{ get; set; }

    }
}