using System;

namespace WindowsFormsApp2.classes
{
    public class MovimentacoesEstoque
    {
        private int idProduto;
        private DateTime data;
        private int qtde;
        private decimal vUnitario;
        private decimal vTotal;
        private string historico;

        public MovimentacoesEstoque(int idProduto, DateTime data, int qtde, decimal vUnitario, decimal vTotal, string historico)
        {
            this.idProduto = idProduto;
            this.data = data;
            this.qtde = qtde;
            this.vUnitario = vUnitario;
            this.vTotal = vTotal;
            this.historico = historico;
        }

        public int IdProduto { get { return idProduto; } }
        public DateTime Data { get { return data; } set { data = value; } }
        public int Qtde { get { return qtde; } set { qtde = value; } }
        public decimal VUnitario { get { return vUnitario; } set { vUnitario = value; } }
        public decimal VTotal { get { return vTotal; } set { vTotal = value; } }
        public string Historico { get { return historico; } set { historico = value; } }
    }
}
