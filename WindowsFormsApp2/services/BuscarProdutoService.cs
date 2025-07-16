using System;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

namespace WindowsFormsApp2.services
{
    namespace WindowsFormsApp2.services
    {
        public class BuscarProdutoService : IBuscarOuCriarProduto
        {
            private readonly ProdutoService produtoService;

            public BuscarProdutoService(ProdutoService produtoService)
            {
                this.produtoService = produtoService;
            }

            public Produto BuscarOuCriarProduto(string nome, string codigo)
            {
                Produto produto = null;
                if (!string.IsNullOrWhiteSpace(codigo))
                    produto = produtoService.ObterPorCodigo(codigo);

                if (produto == null)
                {
                    if (string.IsNullOrWhiteSpace(codigo))
                        codigo = new Random().Next(100000, 999999).ToString();

                    var novoProduto = new Produto(nome, codigo);
                    produtoService.Adicionar(novoProduto);
                    produto = produtoService.ObterPorCodigo(codigo);
                }
                return produto;
            }
        }
    }
}
