using System;
using System.Globalization;

namespace ETLDespesasDeputados
{
    public static class Parametros
    {
        public const string Host = "localhost";
        public const string Username = "pentaho";
        public const string Password = "pentaho";
        public const string Database = "despesasv2-db";
        public const string Arquivo = @"C:\Users\ander_000\OneDrive\Documentos\Pós Big Data\ED-MD-DW-BI\cota_parlamentar.csv.gz";
        public static readonly DateTime DataInicio = new DateTime(2009, 1, 1);
        public static readonly DateTime DataFim = new DateTime(2019, 12, 31);
        public static readonly CultureInfo CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
    }
}