using System.Collections.Generic;
using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface IMovimentacaoEstoque
    {
        void Adicionar(MovimentacoesEstoque movimentacao);
        IEnumerable<MovimentacoesEstoque> ListarPorProduto(int idProduto);
    }
}
