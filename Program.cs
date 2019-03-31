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
        static void Main(string[] args)
        {
            var registros = Compressao.Conteudo(new FileInfo(Parametros.Arquivo));

            Console.WriteLine("Obtendo deputados únicos");
            var deputadosUnicos = ValoresUnicos(registros, r => r.nome_deputado);

            Console.WriteLine("Obtendo partidos únicos");
            var partidosUnicos = ValoresUnicos(registros, r => r.sg_partido);

            Console.WriteLine("Obtendo UFs únicos");
            var ufsUnicas = ValoresUnicos(registros, r => r.sg_uf);

            Console.WriteLine("Obtendo tipos únicos");
            var tiposUnicos = ValoresUnicos(registros, r => r.desc_tipo);

            var connString = $"Host={Parametros.Host};Username={Parametros.Username};Password={Parametros.Password};Database={Parametros.Database}";

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

                foreach (var dimensao in dimensoesPorNome.Keys)
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

                LimparTabela(conn, "dim_data");

                Console.WriteLine($"Inserindo registros na tabela dim_data");
                using (var writer = conn.BeginBinaryImport($"COPY dim_data (cod_data, dia_mes, dia_semana, nome_dia_semana, mes, nome_mes, ano, txt_data, vl_data) FROM STDIN (FORMAT BINARY)"))
                {
                    var dataAtual = Parametros.DataInicio;
                    var id = 1;

                    do
                    {
                        writer.StartRow();
                        writer.Write(id++, NpgsqlDbType.Integer);
                        writer.Write(dataAtual.Day, NpgsqlDbType.Integer);
                        writer.Write(((int)dataAtual.DayOfWeek) + 1, NpgsqlDbType.Integer);
                        writer.Write(dataAtual.ToString("dddd", Parametros.CultureInfo), NpgsqlDbType.Varchar);
                        writer.Write(dataAtual.Month, NpgsqlDbType.Integer);
                        writer.Write(dataAtual.ToString("MMMM", Parametros.CultureInfo), NpgsqlDbType.Varchar);
                        writer.Write(dataAtual.Year, NpgsqlDbType.Integer);
                        writer.Write(dataAtual.ToString("yyyy-MM-dd HH:mm:ss"), NpgsqlDbType.Varchar);
                        writer.Write(dataAtual, NpgsqlDbType.Date);

                        dataAtual = dataAtual.AddDays(1);
                    }
                    while (dataAtual <= Parametros.DataFim);

                    writer.Complete();
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
