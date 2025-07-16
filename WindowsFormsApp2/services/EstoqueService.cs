using System;
using System.Windows.Forms;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;

public class EstoqueService : IEstoqueService
{
    private readonly IMovimentacaoEstoque _movimentacaoRepository;
    private readonly ISaldoEstoque _saldoRepository;
    private readonly IProduto _produtoRepository;
    private readonly ICalculoSaldo _calculoSaldo;

    public EstoqueService(IMovimentacaoEstoque movimentacaoRepository, ISaldoEstoque saldoRepository, IProduto produtoRepository, ICalculoSaldo calculoSaldo)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _saldoRepository = saldoRepository;
        _produtoRepository = produtoRepository;
        _calculoSaldo = calculoSaldo;
    }

    public void RegistrarEntrada(MovimentacoesEstoque entrada)
    {
        if (entrada != null)
            try
            {
                _movimentacaoRepository.Adicionar(entrada);
                AtualizarSaldo(entrada.IdProduto);
            }
            catch (Exception ex)
            {
                // Log detalhado pode ser adicionado aqui
                throw new Exception("Erro ao registrar entrada: " + ex.Message, ex);
            }
        else
            throw new ArgumentException("Dados inválidos para movimentação de entrada.");
    }


    public void RegistrarSaida(MovimentacoesEstoque saida)
    {
        if (saida == null)
            throw new ArgumentException("Dados inválidos para movimentação de saída.");

        MessageBox.Show($"Saída recebida: Produto={saida.IdProduto}, Qtde={saida.Qtde}");

        var saldoAtual = _saldoRepository.ObterSaldoAtual(saida.IdProduto);

        if (saldoAtual == null)
        {
            MessageBox.Show("Saldo não encontrado para o produto informado.");
            throw new InvalidOperationException("Saldo inexistente para este produto.");
        }
        else
        {
            MessageBox.Show($"Saldo atual: Qtde={saldoAtual._Quantidade}, Custo Médio={saldoAtual._CustoMedio}");
        }

        if (saldoAtual._Quantidade < saida.Qtde)
        {
            MessageBox.Show($"Estoque insuficiente: disponível={saldoAtual._Quantidade}, solicitado={saida.Qtde}");
            throw new InvalidOperationException("Estoque insuficiente para realizar a saída.");
        }

        try
        {
            _movimentacaoRepository.Adicionar(saida);
            AtualizarSaldo(saida.IdProduto);
            MessageBox.Show("Saída registrada e saldo atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao registrar saída: " + ex.Message);
            throw new Exception("Erro ao registrar saída: " + ex.Message, ex);
        }
    }



    private void AtualizarSaldo(int idProduto)
    {
        var movimentacoes = _movimentacaoRepository.ListarPorProduto(idProduto);
        var saldo = _calculoSaldo.Calcular(movimentacoes, idProduto);
        _saldoRepository.AtualizarSaldo(saldo);
    }
}
