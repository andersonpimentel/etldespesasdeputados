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

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                foreach (var dimensao in dimensoes)
                {
                    LimparTabela(conn, "dim_" + dimensao);

                    Console.WriteLine($"Inserindo registros na tabela dim_{dimensao}");
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = $"INSERT INTO dim_{dimensao} VALUES (@cod, @desc)";
                        cmd.Parameters.Add("cod", NpgsqlDbType.Integer);
                        cmd.Parameters.Add("desc", NpgsqlDbType.Varchar);

                        foreach (var valores in dimensoesPorNome[dimensao])
                        {
                            cmd.Parameters["cod"].Value = valores.Value;
                            cmd.Parameters["desc"].Value = valores.Key;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                LimparTabela(conn, "fato_despesas");

                Console.WriteLine($"Inserindo registros na tabela fato_despesas");
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO fato_despesas (cod_tipo, cod_deputado, cod_partido, cod_uf, dt_despesa, vl_despesa) " +
                                      "VALUES (@cod_tipo, @cod_deputado, @cod_partido, @cod_uf, @dt_despesa, @vl_despesa)";
                    cmd.Parameters.Add("cod_tipo", NpgsqlDbType.Integer);
                    cmd.Parameters.Add("cod_deputado", NpgsqlDbType.Integer);
                    cmd.Parameters.Add("cod_partido", NpgsqlDbType.Integer);
                    cmd.Parameters.Add("cod_uf", NpgsqlDbType.Integer);
                    cmd.Parameters.Add("dt_despesa", NpgsqlDbType.Date);
                    cmd.Parameters.Add("vl_despesa", NpgsqlDbType.Money);

                    var i = 0;
                    foreach (var registro in registros)
                    {
                        Console.WriteLine($"Inserindo registro {++i} na tabela fato_despesas");
                        cmd.Parameters["cod_tipo"].Value = tiposUnicos[registro.desc_tipo];
                        cmd.Parameters["cod_deputado"].Value = deputadosUnicos[registro.nome_deputado];
                        cmd.Parameters["cod_partido"].Value = partidosUnicos[registro.sg_partido];
                        cmd.Parameters["cod_uf"].Value = ufsUnicas[registro.sg_uf];
                        cmd.Parameters["dt_despesa"].Value = registro.dt_despesa ?? (object)DBNull.Value;
                        cmd.Parameters["vl_despesa"].Value = registro.vl_despesa ?? (object)DBNull.Value;
                        cmd.ExecuteNonQuery();
                    }
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
