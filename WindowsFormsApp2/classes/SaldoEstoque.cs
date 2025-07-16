using System;

namespace WindowsFormsApp2.classes
{
    public class SaldoEstoque
    {
        private int _idProduto;
        private DateTime _date;
        private int quantidade;
        private decimal custo_medio;
        private decimal valor_total;

        public SaldoEstoque(int _idProduto, DateTime _date, int quantidade, decimal custo_medio, decimal valor_total)
        {
            this._idProduto = _idProduto;
            this._date = _date;
            this.quantidade = quantidade;
            this.custo_medio = custo_medio;
            this.valor_total = valor_total;
        }

        public int _IdProduto { get { return _idProduto; } }
        public DateTime _Date { get { return _date; } set { _date = value; } }
        public int _Quantidade { get { return quantidade; } set { quantidade = value; } }
        public decimal _CustoMedio { get { return custo_medio; } set { custo_medio = value; } }
        public decimal _ValorTotal { get { return valor_total; } set { valor_total = value; } }
    }
}
