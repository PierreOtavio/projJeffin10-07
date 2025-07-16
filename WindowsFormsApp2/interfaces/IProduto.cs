using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface IProduto
    {
        void Adicionar(Produto produto);
        Produto ObterPorCodigo(string codigo);
        //IEnumerable<Produto> ListarTodos();
    }
}
