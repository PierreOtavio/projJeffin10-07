using MySql.Data.MySqlClient;
using System.Collections.Generic;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

namespace WindowsFormsApp2.services
{
    internal class SaldoService : ISaldoEstoque
    {
        private readonly IPersistencia _persistencia;

        public SaldoService(IPersistencia _persistencia)
        {
            this._persistencia = _persistencia;
        }

        public void AtualizarSaldo(SaldoEstoque saldo)
        {
            var data = new Dictionary<string, object>
            {
                {"id_produto", saldo._IdProduto },
                {"data", saldo._Date },
                {"quantidade", saldo._Quantidade },
                {"custo_medio", saldo._CustoMedio },
                {"valor_total", saldo._ValorTotal }
            };

            _persistencia.InserirGenerico("saldos_estoque", data);
        }

        public SaldoEstoque ObterSaldoAtual(int idProduto)
        {
            using (var conexao = _persistencia.ConnectTo())
            {
                string sql = "SELECT * FROM saldos_estoque WHERE id_produto = @id_produto ORDER BY data DESC LIMIT 1";
                using (var comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@id_produto", idProduto);
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SaldoEstoque(
                                reader.GetInt32("id_produto"),
                                reader.GetDateTime("data"),
                                reader.GetInt32("quantidade"),
                                reader.GetDecimal("custo_medio"),
                                reader.GetDecimal("valor_total")
                            );
                        }
                    }
                }
            }
            return null;
        }
    }
}
