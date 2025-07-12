using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface IEstoqueService
    {
        void RegistrarEntrada(MovimentacoesEstoque entrada);
        void RegistrarSaida(MovimentacoesEstoque saida);
        //decimal CalcularCustoMedio(int idProduto);
    }
}
