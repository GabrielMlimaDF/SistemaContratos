//função para temporizar alertas
    window.setTimeout(function () {
        $('.alert').alert('close');
        }, 7000);

//Listar atividades *Empresa cnae
 function ListarAtividades(Listaratv) {
     document.getElementById('Listaratv').value = Listaratv;

}
//Permitir apenas numeros
document.getElementById("searchcnpj").onkeypress = function (e) {
    var chr = String.fromCharCode(e.which);
    if ("1234567890".indexOf(chr) < 0)
        return false;
};

