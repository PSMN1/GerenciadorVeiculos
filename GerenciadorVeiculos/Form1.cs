using System;
using System.Windows.Forms;
using GerenciadorVeiculos.Dados;
using GerenciadorVeiculos.Modelos;

namespace GerenciadorVeiculos
{
    public partial class Form1 : Form
    {
        // Guarda o ID do veículo clicado na tabela (0 significa nenhum selecionado)
        private int idSelecionado = 0;

        public Form1()
        {
            InitializeComponent();

            // Inicia com Carro selecionado, carrega os dados e prepara a tela no modo de cadastro
            cmbTipo.SelectedIndex = 0;
            CarregarGrade();
            LimparCampos();
        }

        // Busca os dados no banco e atualiza a tabela (DataGridView)
        private void CarregarGrade()
        {
            try
            {
                VeiculoDAO dao = new VeiculoDAO();
                dgvVeiculos.DataSource = dao.ListarTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar a lista de veículos: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Alterna o texto do rótulo entre Portas e Cilindradas
        private void cmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipo.SelectedItem != null && cmbTipo.SelectedItem.ToString() == "Carro")
            {
                lblEspecifico.Text = "Portas:";
            }
            else
            {
                lblEspecifico.Text = "Cilindradas:";
            }
        }

        // CREATE: clique do botão Cadastrar Veículo
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtModelo.Text == "" || txtDiaria.Text == "" || txtEspecifico.Text == "")
                {
                    MessageBox.Show("Por favor, preencha todos os campos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string modelo = txtModelo.Text;
                decimal diaria = Convert.ToDecimal(txtDiaria.Text);
                int especifico = Convert.ToInt32(txtEspecifico.Text);
                string tipo = cmbTipo.SelectedItem.ToString();

                Veiculo veiculo = null;

                if (tipo == "Carro")
                {
                    veiculo = new Carro(modelo, diaria, especifico);
                }
                else
                {
                    veiculo = new Moto(modelo, diaria, especifico);
                }

                VeiculoDAO dao = new VeiculoDAO();
                dao.Cadastrar(veiculo);

                MessageBox.Show("Veículo cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimparCampos();
                CarregarGrade();
            }
            catch (FormatException)
            {
                MessageBox.Show("Os campos de Diária e Portas/Cilindradas aceitam somente números.", "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao salvar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Quando o usuário clica em uma linha da tabela, preenche os campos para edição ou exclusão
        private void dgvVeiculos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignora cliques no cabeçalho das colunas (RowIndex menor que zero)
            if (e.RowIndex >= 0)
            {
                DataGridViewRow linha = dgvVeiculos.Rows[e.RowIndex];

                idSelecionado = Convert.ToInt32(linha.Cells["ID"].Value);
                txtModelo.Text = linha.Cells["Modelo"].Value.ToString();
                cmbTipo.SelectedItem = linha.Cells["Tipo"].Value.ToString();
                txtDiaria.Text = linha.Cells["Diária (R$)"].Value.ToString();
                txtEspecifico.Text = linha.Cells["Portas / Cilindradas"].Value.ToString();

                // Garante que a caixa de texto do modelo permanece desbloqueada para edição
                txtModelo.ReadOnly = false;
                txtModelo.Enabled = true;

                // Alterna para o modo de Edição / Exclusão
                btnSalvar.Enabled = false;
                btnEditar.Enabled = true;
                btnExcluir.Enabled = true;
            }
        }

        // UPDATE: clique do botão Salvar Alterações
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (idSelecionado == 0)
                {
                    MessageBox.Show("Clique em um veículo na tabela antes de salvar alterações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtModelo.Text == "" || txtDiaria.Text == "" || txtEspecifico.Text == "")
                {
                    MessageBox.Show("Por favor, preencha todos os campos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string modelo = txtModelo.Text;
                decimal diaria = Convert.ToDecimal(txtDiaria.Text);
                int especifico = Convert.ToInt32(txtEspecifico.Text);
                string tipo = cmbTipo.SelectedItem.ToString();

                Veiculo veiculo = null;

                if (tipo == "Carro")
                {
                    veiculo = new Carro(modelo, diaria, especifico);
                }
                else
                {
                    veiculo = new Moto(modelo, diaria, especifico);
                }

                // Define o ID que será atualizado no banco
                veiculo.Id = idSelecionado;

                VeiculoDAO dao = new VeiculoDAO();
                dao.Atualizar(veiculo);

                MessageBox.Show("Dados do veículo atualizados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimparCampos();
                CarregarGrade();
            }
            catch (FormatException)
            {
                MessageBox.Show("Os campos de Diária e Portas/Cilindradas aceitam somente números.", "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao atualizar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DELETE: clique do botão Excluir Veículo
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (idSelecionado == 0)
                {
                    MessageBox.Show("Clique em um veículo na tabela antes de excluir.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Pede confirmação ao usuário antes de apagar definitivamente
                DialogResult resposta = MessageBox.Show(
                    $"Tem certeza que deseja excluir o veículo '{txtModelo.Text}' (ID {idSelecionado})?",
                    "Confirmar Exclusão",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resposta == DialogResult.Yes)
                {
                    VeiculoDAO dao = new VeiculoDAO();
                    dao.Excluir(idSelecionado);

                    MessageBox.Show("Veículo removido com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimparCampos();
                    CarregarGrade();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao excluir: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clique do botão Novo / Limpar: cancela a seleção atual e libera a tela para um novo cadastro
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        // Limpa as caixas de texto, zera a seleção e volta para o modo de Novo Cadastro
        private void LimparCampos()
        {
            idSelecionado = 0;
            txtModelo.Clear();
            txtDiaria.Clear();
            txtEspecifico.Clear();
            cmbTipo.SelectedIndex = 0;

            // Garante que o campo de modelo está totalmente liberado para digitação
            txtModelo.ReadOnly = false;
            txtModelo.Enabled = true;

            // Alterna para o modo de Novo Cadastro
            btnSalvar.Enabled = true;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;

            txtModelo.Focus();
        }
    }
}