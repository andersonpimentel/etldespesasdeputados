using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace ETLDespesasDeputados
{
    class Program
    {
        private const string host = "localhost";
        private const string username = "pentaho";
        private const string password = "pentaho";
        private const string database = "despesasv3-db";
        private const string arquivo = @"C:\Users\ander_000\OneDrive\Documentos\Pós Big Data\ED-MD-DW-BI\cota_parlamentar.csv.gz";
        private static readonly string[] dimensoes = new[] { "deputado", "partido", "uf", "tipo" };


        static void Main(string[] args)
        {
            var registros = Compressao.Conteudo(new FileInfo(arquivo));

            Console.WriteLine("Obtendo deputados únicos");
            var deputadosUnicos = ValoresUnicos(registros, r => r.nome_deputado);

            Console.WriteLine("Obtendo partidos únicos");
            var partidosUnicos = ValoresUnicos(registros, r => r.sg_partido);

            Console.WriteLine("Obtendo UFs únicos");
            var ufsUnicas = ValoresUnicos(registros, r => r.sg_uf);

            Console.WriteLine("Obtendo tipos únicos");
            var tiposUnicos = ValoresUnicos(registros, r => r.desc_tipo);

            var connString = $"Host={host};Username={username};Password={password};Database={database}";

            var dimensoesPorNome = new Dictionary<string, Dictionary<string, int>> {
                { "deputado", deputadosUnicos },
                { "uf", ufsUnicas },
                { "partido", partidosUnicos },
                { "tipo", tiposUnicos }
            };

            var camposDescricaoPorDimensao = new Dictionary<string, string>
            {
                {"deputado", "nome_deputado"},
                {"uf", "sg_uf"},
                {"partido", "sg_partido"},
                {"tipo", "desc_tipo"}
            };

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                foreach (var dimensao in dimensoes)
                {
                    var nomeTabelaDimensao = $"dim_{dimensao}";

                    LimparTabela(conn, nomeTabelaDimensao);

                    Console.WriteLine($"Inserindo registros na tabela {nomeTabelaDimensao}");
                    using (var writer = conn.BeginBinaryImport($"COPY {nomeTabelaDimensao} (cod_{dimensao}, {camposDescricaoPorDimensao[dimensao]}) FROM STDIN (FORMAT BINARY)"))
                    {
                        foreach (var valores in dimensoesPorNome[dimensao])
                        {
                            writer.StartRow();
                            writer.Write(valores.Value, NpgsqlDbType.Integer);
                            writer.Write(valores.Key, NpgsqlDbType.Varchar);
                        }
                        writer.Complete();
                    }
                }

                LimparTabela(conn, "fato_despesas");

                Console.WriteLine($"Inserindo registros na tabela fato_despesas");
                using (var writer = conn.BeginBinaryImport($"COPY fato_despesas (cod_tipo, cod_deputado, cod_partido, cod_uf, dt_despesa, vl_despesa) FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (var registro in registros)
                    {
                        writer.StartRow();
                        writer.Write(tiposUnicos[registro.desc_tipo], NpgsqlDbType.Integer);
                        writer.Write(deputadosUnicos[registro.nome_deputado]);
                        writer.Write(partidosUnicos[registro.sg_partido]);
                        writer.Write(ufsUnicas[registro.sg_uf]);
                        if (registro.dt_despesa == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            writer.Write(registro.dt_despesa.Value, NpgsqlDbType.Date);
                        }
                        if (registro.vl_despesa == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            writer.Write(registro.vl_despesa.Value, NpgsqlDbType.Money);
                        }
                    }
                    writer.Complete();
                }
            }
        }

        private static void LimparTabela(NpgsqlConnection conn, string nome)
        {
            Console.WriteLine($"Limpando tabela {nome}");
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = $"TRUNCATE TABLE {nome}";

                cmd.ExecuteNonQuery();
            }
        }

        static Dictionary<string, int> ValoresUnicos(IEnumerable<Origem> registros, Func<Origem, string> field)
        {
            int i = 0;

            return registros.Select(field)
                            .Distinct()
                            .OrderBy(r => r)
                            .ToDictionary(k => k, v => ++i);
        }
    }
}
