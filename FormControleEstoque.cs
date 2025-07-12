using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.services;

public class FormControleEstoque : Form
{
    private DateTimePicker dtpData;
    private TextBox txtHistorico, txtQuantidade, txtCustoUnitario, txtCodigoBusca;
    private Button btnEntrada, btnSaida, btnBuscar;
    private Label lblProdutoEncontrado;
    private DataGridView dgvMovimentacoes;

    private ProdutoService produtoService;
    private MovimentacaoService movimentacaoService;
    private SaldoService saldoService;
    private EstoqueService estoqueService;

    private Produto produtoAtual;

    public FormControleEstoque()
    {
        this.Text = "Controle Físico-Financeiro de Estoque";
        this.Size = new Size(1100, 550);

        dtpData = new DateTimePicker() { Location = new Point(20, 20), Width = 120 };
        txtHistorico = new TextBox() { Location = new Point(160, 20), Width = 140, Text = "" };
        txtQuantidade = new TextBox() { Location = new Point(320, 20), Width = 80, Text = "" };
        txtCustoUnitario = new TextBox() { Location = new Point(420, 20), Width = 100, Text = "" };

        btnEntrada = new Button() { Text = "Registrar Entrada", Location = new Point(540, 18), Width = 140 };
        btnSaida = new Button() { Text = "Registrar Saída", Location = new Point(700, 18), Width = 140 };

        txtCodigoBusca = new TextBox() { Location = new Point(20, 60), Width = 120, Text = "" };
        btnBuscar = new Button() { Text = "Buscar Produto", Location = new Point(150, 58), Width = 120 };
        lblProdutoEncontrado = new Label() { Location = new Point(280, 60), Width = 400, Height = 25 };

        dgvMovimentacoes = new DataGridView()
        {
            Location = new Point(20, 100),
            Size = new Size(1040, 400),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        Controls.Add(dtpData);
        Controls.Add(txtHistorico);
        Controls.Add(txtQuantidade);
        Controls.Add(txtCustoUnitario);
        Controls.Add(btnEntrada);
        Controls.Add(btnSaida);
        Controls.Add(txtCodigoBusca);
        Controls.Add(btnBuscar);
        Controls.Add(lblProdutoEncontrado);
        Controls.Add(dgvMovimentacoes);

        produtoService = new ProdutoService();
        movimentacaoService = new MovimentacaoService();
        saldoService = new SaldoService(new ConnectClass());
        estoqueService = new EstoqueService(movimentacaoService, saldoService, produtoService);

        btnEntrada.Click += BtnEntrada_Click;
        btnSaida.Click += BtnSaida_Click;
        btnBuscar.Click += BtnBuscar_Click;
    }

    private void BtnBuscar_Click(object sender, EventArgs e)
    {
        produtoAtual = produtoService.ObterPorCodigo(txtCodigoBusca.Text.Trim());
        if (produtoAtual != null)
        {
            lblProdutoEncontrado.Text = $"Produto: {produtoAtual.Descricao} (Código: {produtoAtual.Codigo})";
            AtualizarGrid();
        }
        else
        {
            lblProdutoEncontrado.Text = "Produto não encontrado.";
            dgvMovimentacoes.DataSource = null;
        }
    }

    private void BuscarOuCriarProduto(string descricao)
    {
        // Tenta buscar produto pelo código já digitado (se houver)
        string codigo = txtCodigoBusca.Text.Trim();

        if (!string.IsNullOrWhiteSpace(codigo))
            produtoAtual = produtoService.ObterPorCodigo(codigo);

        // Se não encontrou, gera código aleatório e cria produto
        if (produtoAtual == null)
        {
            do
            {
                codigo = new Random().Next(100000, 999999).ToString();
                produtoAtual = produtoService.ObterPorCodigo(codigo);
            } while (produtoAtual != null);

            produtoAtual = new Produto(descricao, codigo);
            produtoService.Adicionar(produtoAtual);
            produtoAtual = produtoService.ObterPorCodigo(codigo);
        }
    }

    private void BtnEntrada_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtHistorico.Text) ||
            string.IsNullOrWhiteSpace(txtQuantidade.Text) ||
            string.IsNullOrWhiteSpace(txtCustoUnitario.Text))
        {
            MessageBox.Show("Preencha todos os campos para entrada.");
            return;
        }
        if (!int.TryParse(txtQuantidade.Text, out int qtd) || qtd <= 0 ||
            !decimal.TryParse(txtCustoUnitario.Text, out decimal custo) || custo <= 0)
        {
            MessageBox.Show("Quantidade ou custo inválidos.");
            return;
        }

        BuscarOuCriarProduto(txtHistorico.Text.Trim());
        int idProduto = produtoAtual.IdProduto;

        var entrada = new MovimentacoesEstoque(
            idProduto,
            dtpData.Value,
            qtd,
            custo,
            qtd * custo,
            txtHistorico.Text
        );
        estoqueService.RegistrarEntrada(entrada);

        MessageBox.Show("Entrada registrada com sucesso!");
        AtualizarGrid();
        LimparCampos();
    }

    private void BtnSaida_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtHistorico.Text) ||
            string.IsNullOrWhiteSpace(txtQuantidade.Text))
        {
            MessageBox.Show("Preencha todos os campos para saída.");
            return;
        }
        if (!int.TryParse(txtQuantidade.Text, out int qtd) || qtd <= 0)
        {
            MessageBox.Show("Quantidade inválida.");
            return;
        }

        BuscarOuCriarProduto(txtHistorico.Text.Trim());
        int idProduto = produtoAtual.IdProduto;

        var saida = new MovimentacoesEstoque(
            idProduto,
            dtpData.Value,
            qtd,
            0,
            0,
            txtHistorico.Text
        );
        estoqueService.RegistrarSaida(saida);

        MessageBox.Show("Saída registrada com sucesso!");
        AtualizarGrid();
        LimparCampos();
    }

    private void AtualizarGrid()
    {
        if (produtoAtual == null) return;

        var movimentacoes = movimentacaoService.ListarPorProduto(produtoAtual.IdProduto).ToList();
        int saldoQuantidade = 0;
        decimal saldoValor = 0;
        decimal custoMedio = 0;

        var listaExibicao = movimentacoes.Select(mov =>
        {
            bool isEntrada = mov.VUnitario > 0; // Entrada se valor unitário > 0
            if (isEntrada)
            {
                saldoQuantidade += mov.Qtde;
                saldoValor += mov.Qtde * mov.VUnitario;
                custoMedio = saldoQuantidade > 0 ? saldoValor / saldoQuantidade : 0;
            }
            else
            {
                saldoQuantidade -= mov.Qtde;
                saldoValor -= mov.Qtde * custoMedio;
                // custoMedio permanece igual
            }


            return new
            {
                Data = mov.Data.ToString("dd/MMM"),
                Código = produtoAtual.Codigo,
                Histórico = mov.Historico,
                QuantidadeEntrada = isEntrada ? mov.Qtde.ToString() : "-",
                CustoUnitario = isEntrada ? mov.VUnitario.ToString("C") : "-",
                TotalEntrada = isEntrada ? (mov.Qtde * mov.VUnitario).ToString("C") : "-",
                QuantidadeSaida = !isEntrada ? mov.Qtde.ToString() : "-",
                CustoMedioSaida = !isEntrada ? custoMedio.ToString("C") : "-",
                TotalSaida = !isEntrada ? (mov.Qtde * custoMedio).ToString("C") : "-",
                SaldoQuantidade = saldoQuantidade,
                CustoMedioSaldo = custoMedio.ToString("C"),
                TotalSaldo = (saldoQuantidade * custoMedio).ToString("C")
            };
        }).ToList();

        dgvMovimentacoes.DataSource = listaExibicao;
    }

    private void LimparCampos()
    {
        txtHistorico.Text = "";
        txtQuantidade.Text = "";
        txtCustoUnitario.Text = "";
    }
}
