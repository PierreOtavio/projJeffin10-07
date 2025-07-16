using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

namespace WindowsFormsApp2.services
{
    public class CalculoCustoMedio : ICalculoSaldo
    {
        public SaldoEstoque Calcular(IEnumerable<MovimentacoesEstoque> movimentacoes, int idProduto)
        {
            var entradas = movimentacoes.Where(m => m.VUnitario > 0);
            var saidas = movimentacoes.Where(m => m.VUnitario == 0);

            int saldoQuantidade = entradas.Sum(m => m.Qtde) - saidas.Sum(m => m.Qtde);
            int totalQuantidadeEntradas = entradas.Sum(m => m.Qtde);
            decimal totalValorEntradas = entradas.Sum(m => m.VTotal);
            decimal custoMedio = totalQuantidadeEntradas > 0 ? totalValorEntradas / totalQuantidadeEntradas : 0;
            decimal valorTotal = saldoQuantidade * custoMedio;

            return new SaldoEstoque(idProduto, DateTime.Now, saldoQuantidade, custoMedio, valorTotal);
        }
    }
}
