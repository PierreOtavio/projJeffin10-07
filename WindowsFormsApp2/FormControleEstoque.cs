using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.classes;
using WindowsFormsApp2.interfaces;
using WindowsFormsApp2.services;
using WindowsFormsApp2.services.WindowsFormsApp2.services;

public class FormControleEstoque : Form
{
    private DateTimePicker dtpDataEntrada, dtpDataSaida;
    private TextBox txtNomeProdutoEntrada, txtCodigoProdutoEntrada, txtQuantidadeEntrada, txtCustoUnitarioEntrada, txtHistoricoEntrada;
    private TextBox txtNomeProdutoSaida, txtCodigoProdutoSaida, txtQuantidadeSaida, txtHistoricoSaida;
    private TextBox txtCodigoBusca;
    private Button btnRegistrarEntrada, btnRegistrarSaida, btnBuscar;
    private Label lblProdutoBuscado;
    private DataGridView dgvMovimentacoes;

    private ProdutoService produtoService;
    private MovimentacaoService movimentacaoService;
    private SaldoService saldoService;
    private EstoqueService estoqueService;
    private IBuscarOuCriarProduto buscarCriarProdutoService;
    private ICalculoSaldo calculoSaldo;

    private Produto produtoBuscado;

    public FormControleEstoque()
    {
        this.Text = "Controle Físico-Financeiro de Estoque";
        this.Size = new Size(1200, 650);

        // ----- Painel Entrada -----
        var gbEntrada = new GroupBox() { Text = "Entrada de Produto", Location = new Point(10, 10), Size = new Size(570, 120) };
        dtpDataEntrada = new DateTimePicker() { Location = new Point(10, 30), Width = 120 };
        txtNomeProdutoEntrada = new TextBox() { Location = new Point(140, 30), Width = 120, Text = "Nome" };
        txtCodigoProdutoEntrada = new TextBox() { Location = new Point(270, 30), Width = 100, Text = "Código" };
        txtQuantidadeEntrada = new TextBox() { Location = new Point(380, 30), Width = 50, Text = "Quantidade" };
        txtCustoUnitarioEntrada = new TextBox() { Location = new Point(440, 30), Width = 60, Text = "R$ Unit." };
        txtHistoricoEntrada = new TextBox() { Location = new Point(10, 65), Width = 350, Text = "Histórico/Observação" };
        btnRegistrarEntrada = new Button() { Text = "Registrar Entrada", Location = new Point(370, 65), Size = new Size(140, 30) };
        gbEntrada.Controls.AddRange(new Control[] { dtpDataEntrada, txtNomeProdutoEntrada, txtCodigoProdutoEntrada, txtQuantidadeEntrada, txtCustoUnitarioEntrada, txtHistoricoEntrada, btnRegistrarEntrada });
        this.Controls.Add(gbEntrada);

        // ----- Painel Saída -----
        var gbSaida = new GroupBox() { Text = "Saída de Produto", Location = new Point(600, 10), Size = new Size(570, 120) };
        dtpDataSaida = new DateTimePicker() { Location = new Point(10, 30), Width = 120 };
        txtNomeProdutoSaida = new TextBox() { Location = new Point(140, 30), Width = 120, Text = "Nome" };
        txtCodigoProdutoSaida = new TextBox() { Location = new Point(270, 30), Width = 100, Text = "Código" };
        txtQuantidadeSaida = new TextBox() { Location = new Point(380, 30), Width = 50, Text = "Quantidade" };
        txtHistoricoSaida = new TextBox() { Location = new Point(10, 65), Width = 350, Text = "Histórico/Observação" };
        btnRegistrarSaida = new Button() { Text = "Registrar Saída", Location = new Point(370, 65), Size = new Size(140, 30) };
        gbSaida.Controls.AddRange(new Control[] { dtpDataSaida, txtNomeProdutoSaida, txtCodigoProdutoSaida, txtQuantidadeSaida, txtHistoricoSaida, btnRegistrarSaida });
        this.Controls.Add(gbSaida);

        // ----- Painel Busca -----
        var gbBusca = new GroupBox() { Text = "Buscar Produto", Location = new Point(10, 140), Size = new Size(1160, 60) };
        txtCodigoBusca = new TextBox() { Location = new Point(20, 22), Width = 120, Text = "Código" };
        btnBuscar = new Button() { Text = "Buscar Produto", Location = new Point(155, 20), Size = new Size(110, 30) };
        lblProdutoBuscado = new Label() { Location = new Point(275, 25), Width = 850, Height = 25 };
        gbBusca.Controls.AddRange(new Control[] { txtCodigoBusca, btnBuscar, lblProdutoBuscado });
        this.Controls.Add(gbBusca);

        // ----- DataGrid -----
        dgvMovimentacoes = new DataGridView()
        {
            Location = new Point(10, 210),
            Size = new Size(1160, 390),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        this.Controls.Add(dgvMovimentacoes);

        IPersistencia persistencia = new ConnectClass();
        produtoService = new ProdutoService(persistencia);
        movimentacaoService = new MovimentacaoService(persistencia);
        saldoService = new SaldoService(persistencia);
        calculoSaldo = new CalculoCustoMedio();
        estoqueService = new EstoqueService(movimentacaoService, saldoService, produtoService, calculoSaldo);
        buscarCriarProdutoService = new BuscarProdutoService(produtoService);

        btnRegistrarEntrada.Click += BtnRegistrarEntrada_Click;
        btnRegistrarSaida.Click += BtnRegistrarSaida_Click;
        btnBuscar.Click += BtnBuscar_Click;
    }

    private void BtnBuscar_Click(object sender, EventArgs e)
    {
        var codigo = txtCodigoBusca.Text.Trim();
        produtoBuscado = produtoService.ObterPorCodigo(codigo);
        if (produtoBuscado != null)
        {
            txtNomeProdutoSaida.Text = produtoBuscado.Descricao;
            txtCodigoProdutoSaida.Text = produtoBuscado.Codigo;
            txtNomeProdutoEntrada.Text = produtoBuscado.Descricao;
            txtCodigoProdutoEntrada.Text = produtoBuscado.Codigo;
            lblProdutoBuscado.Text = $"Produto: {produtoBuscado.Descricao} (Código: {produtoBuscado.Codigo})";
            AtualizarGrid();
        }
        else
        {
            lblProdutoBuscado.Text = "Produto não encontrado.";
            dgvMovimentacoes.DataSource = null;
        }
    }

    private void BtnRegistrarEntrada_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomeProdutoEntrada.Text) ||
            string.IsNullOrWhiteSpace(txtCodigoProdutoEntrada.Text) ||
            string.IsNullOrWhiteSpace(txtQuantidadeEntrada.Text) ||
            string.IsNullOrWhiteSpace(txtCustoUnitarioEntrada.Text))
        {
            MessageBox.Show("Preencha todos os campos obrigatórios de ENTRADA.");
            return;
        }
        if (!int.TryParse(txtQuantidadeEntrada.Text, out int qtd) || qtd <= 0 ||
            !decimal.TryParse(txtCustoUnitarioEntrada.Text, out decimal custo) || custo <= 0)
        {
            MessageBox.Show("Quantidade ou custo inválidos.");
            return;
        }

        var produto = buscarCriarProdutoService.BuscarOuCriarProduto(txtNomeProdutoEntrada.Text.Trim(), txtCodigoProdutoEntrada.Text.Trim());
        if (produto == null)
        {
            MessageBox.Show("Falha ao buscar ou criar produto.");
            return;
        }
        produtoBuscado = produto;

        var entrada = new MovimentacoesEstoque(
            produto.IdProduto,
            dtpDataEntrada.Value,
            qtd,
            custo,
            qtd * custo,
            txtHistoricoEntrada.Text.Trim()
        );
        estoqueService.RegistrarEntrada(entrada);

        MessageBox.Show("Entrada registrada com sucesso!");
        LimparCamposEntrada();
        AtualizarGrid();
    }

    private void BtnRegistrarSaida_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomeProdutoSaida.Text) ||
            string.IsNullOrWhiteSpace(txtCodigoProdutoSaida.Text) ||
            string.IsNullOrWhiteSpace(txtQuantidadeSaida.Text))
        {
            MessageBox.Show("Preencha todos os campos obrigatórios de SAÍDA.");
            return;
        }
        if (!int.TryParse(txtQuantidadeSaida.Text, out int qtd) || qtd <= 0)
        {
            MessageBox.Show("Quantidade inválida.");
            return;
        }

        var produto = buscarCriarProdutoService.BuscarOuCriarProduto(txtNomeProdutoSaida.Text.Trim(), txtCodigoProdutoSaida.Text.Trim());
        if (produto == null)
        {
            MessageBox.Show("Falha ao buscar produto.");
            return;
        }
        produtoBuscado = produto;

        var saida = new MovimentacoesEstoque(
            produto.IdProduto,
            dtpDataSaida.Value,
            qtd,
            0,
            0,
            txtHistoricoSaida.Text.Trim()
        );
        try
        {
            estoqueService.RegistrarSaida(saida);
            MessageBox.Show("Saída registrada com sucesso!");
            LimparCamposSaida();
            AtualizarGrid();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void AtualizarGrid()
    {
        if (produtoBuscado == null) return;

        var movimentacoes = movimentacaoService.ListarPorProduto(produtoBuscado.IdProduto).ToList();
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
                Código = produtoBuscado.Codigo,
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

    private void LimparCamposEntrada()
    {
        txtNomeProdutoEntrada.Text = "";
        txtCodigoProdutoEntrada.Text = "";
        txtQuantidadeEntrada.Text = "";
        txtCustoUnitarioEntrada.Text = "";
        txtHistoricoEntrada.Text = "";
    }

    private void LimparCamposSaida()
    {
        txtNomeProdutoSaida.Text = "";
        txtCodigoProdutoSaida.Text = "";
        txtQuantidadeSaida.Text = "";
        txtHistoricoSaida.Text = "";
    }
}
