using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface IBuscarOuCriarProduto
    {
        Produto BuscarOuCriarProduto(string descricao, string codigo);
    }
}
