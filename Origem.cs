using System;
using CsvHelper.Configuration.Attributes;

namespace ETLDespesasDeputados
{
    class Origem
    {
        //public int? codlegislatura { get; set; }
        [Name("datemissao")]
        public DateTime? dt_despesa { get; set; }
        //public string idedocumento { get; set; }
        //public string idecadastro { get; set; }
        //public string indtipodocumento { get; set; }
        //public string nucarteiraparlamentar { get; set; }
        [Name("nudeputadoid")]
        public int? cod_deputado { get; set; }
        //public string nulegislatura { get; set; }
        //public string numano { get; set; }
        //public string numespecificacaosubcota { get; set; }
        //public string numlote { get; set; }
        //public string nummes { get; set; }
        //public string numparcela { get; set; }
        //public string numressarcimento { get; set; }
        //public string numsubcota { get; set; }
        [Name("sgpartido")]
        public string sg_partido { get; set; }
        [Name("sguf")]
        public string sg_uf { get; set; }
        [Name("txnomeparlamentar")]
        public string nome_deputado { get; set; }
        [Name("txtdescricao")]
        //public string txtcnpjcpf { get; set; }
        public string desc_tipo { get; set; }
        //public string txtdescricaoespecificacao { get; set; }
        //public string txtfornecedor { get; set; }
        //public string txtnumero { get; set; }
        //public string txtpassageiro { get; set; }
        //public string txttrecho { get; set; }
        [Name("vlrdocumento")]
        public decimal? vl_despesa { get; set; }
        //public string vlrglosa { get; set; }
        //public string vlrliquido { get; set; }
        //public string vlrrestituicao { get; set; }
    }
}