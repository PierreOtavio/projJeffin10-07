using MySql.Data.MySqlClient;
using System.Collections.Generic;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

namespace WindowsFormsApp2.services
{
    public class ProdutoService : IProduto
    {
        private readonly ConnectClass _connectClass;

        public ProdutoService()
        {
            _connectClass = new ConnectClass();
        }

        public void Adicionar(Produto produto)
        {
            var dados = new Dictionary<string, object>
        {
            { "descricao", produto.Descricao },
            { "codigo", produto.Codigo }
        };
            _connectClass.InserirGenerico("produtos", dados);
        }

        public Produto ObterPorCodigo(string codigo)
        {
            Produto produto = null;
            using (var conexao = _connectClass.ConnectTo())
            {
                string sql = "SELECT * FROM produtos WHERE codigo = @codigo LIMIT 1";
                using (var comando = new MySqlCommand(sql, conexao))
                {
                    comando.Parameters.AddWithValue("@codigo", codigo);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            produto = new Produto(
                                reader.GetInt32("id_produto"),
                                reader.GetString("descricao"),
                                reader.GetString("codigo")
                            );
                        }
                    }
                }
            }
            return produto;
        }
    }

}
