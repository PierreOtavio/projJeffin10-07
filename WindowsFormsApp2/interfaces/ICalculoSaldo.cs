using System.Collections.Generic;
using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface ICalculoSaldo
    {
        SaldoEstoque Calcular(IEnumerable<MovimentacoesEstoque> movimentacoes, int idProduto);
    }
}
