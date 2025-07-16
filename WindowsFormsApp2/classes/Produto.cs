namespace WindowsFormsApp2.classes
{
    public class Produto
    {
        private int idProduto; // adicione este campo
        private string descricao;
        private string codigo;

        public Produto(int idProduto, string descricao, string codigo)
        {
            this.idProduto = idProduto;
            this.descricao = descricao;
            this.codigo = codigo;
        }

        // Construtor antigo, para compatibilidade
        public Produto(string descricao, string codigo)
        {
            this.descricao = descricao;
            this.codigo = codigo;
        }

        public int IdProduto { get { return idProduto; } set { idProduto = value; } }
        public string Descricao { get { return descricao; } set { descricao = value; } }
        public string Codigo { get { return codigo; } set { codigo = value; } }
    }

}
