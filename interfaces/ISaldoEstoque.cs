using WindowsFormsApp2.classes;

namespace WindowsFormsApp2.interfaces
{
    public interface ISaldoEstoque
    {
        void AtualizarSaldo(SaldoEstoque saldo);
        SaldoEstoque ObterSaldoAtual(int idProduto);
    }
}
