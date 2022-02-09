using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistemas.Models
{
    public class CadEmpresa
    {
        public int IdEmpresa { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string Cnae { get; set; }
        public string IEstadual { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public string DescCnaePrincipal { get; set; }
        public string Email { get; set; }
        public DateTime Abertura { get; set; }
        public string NaturezaJur { get; set; }
        public string Porte { get; set; }
        public string Fantasia { get; set; }
        public AtvSecundaria AtvSecundaria { get; set; }

    }
    public class AtvSecundaria
    {
        public string Atvtext { get; set; }
        public string Atvcode { get; set; }
    }
}