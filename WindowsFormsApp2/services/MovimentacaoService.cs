using MySql.Data.MySqlClient;
using System.Collections.Generic;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

namespace WindowsFormsApp2.services
{
    internal class MovimentacaoService : IMovimentacaoEstoque
    {
        //private readonly ConnectClass _connectClass;
        private readonly IPersistencia _persistencia;

        public MovimentacaoService(IPersistencia persistencia)
        {
            this._persistencia = persistencia;
        }

        public void Adicionar(MovimentacoesEstoque movimentacoes)
        {
            var dados = new Dictionary<string, object>
            {
                {"id_produto", movimentacoes.IdProduto },
                {"data", movimentacoes.Data },
                {"quantidade", movimentacoes.Qtde },
                {"valor_unitario", movimentacoes.VUnitario },
                {"valor_total", movimentacoes.VTotal },
                {"historico", movimentacoes.Historico },
            };

            _persistencia.InserirGenerico("movimentacoes_estoque", dados);
        }

        public IEnumerable<MovimentacoesEstoque> ListarPorProduto(int idProduto)
        {
            var lista = new List<MovimentacoesEstoque>();
            using (var conexao = _persistencia.ConnectTo())
            {
                string sql = "SELECT * FROM movimentacoes_estoque WHERE id_produto = @idProduto ORDER BY data";
                using (var comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@idProduto", idProduto);
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var mov = new MovimentacoesEstoque(
                                reader.GetInt32("id_produto"),
                                reader.GetDateTime("data"),
                                reader.GetInt32("quantidade"),
                                reader.GetDecimal("valor_unitario"),
                                reader.GetDecimal("valor_total"),
                                reader.GetString("historico")
                            );
                            lista.Add(mov);
                        }
                    }
                }
            }
            return lista;
        }
    }
}
