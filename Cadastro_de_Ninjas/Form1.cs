using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cadastro_de_Ninjas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigurarCombos();
        }

        private void ConfigurarCombos()
        {
            cbxGenero.Items.Clear();
            cbxGenero.Items.Add("M");
            cbxGenero.Items.Add("F");

            cbxTipoSanguineo.Items.Clear();
            cbxTipoSanguineo.Items.Add("Katon (Fogo)");
            cbxTipoSanguineo.Items.Add("Suiton (Água)");
            cbxTipoSanguineo.Items.Add("Doton (Terra)");
            cbxTipoSanguineo.Items.Add("Futon (Vento)");
            cbxTipoSanguineo.Items.Add("Raiton (Raio)");
            cbxTipoSanguineo.Items.Add("Kekkei Genkai");
        }

        private double CalculaPoder(Ninja ninja)
        {
            if (ninja.Habilidades.Altura == 0) return 0;
            return (ninja.Habilidades.Peso + (ninja.Habilidades.Altura * 100)) / 2;
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                Ninja ninja = new Ninja();

                ninja.Nome = txtNome.Text;
                ninja.VilaOrigem = txtNacionalidade.Text;
                ninja.RankNinja = txtModalidade.Text;

                if (cbxGenero.SelectedItem != null)
                    ninja.Genero = cbxGenero.SelectedItem.ToString();

                ninja.DataNascimento = dtpNascimento.Value.Date;
                ninja.Idade = DateTime.Now.Year - ninja.DataNascimento.Year;

                double.TryParse(txtAltura.Text.Replace("_", "").Trim(), out double alturaTratada);
                ninja.Habilidades.Altura = alturaTratada;

                double.TryParse(txtPeso.Text.Replace("_", "").Trim(), out double pesoTratado);
                ninja.Habilidades.Peso = pesoTratado;

                if (cbxTipoSanguineo.SelectedItem != null)
                    ninja.Habilidades.ElementoChakra = cbxTipoSanguineo.SelectedItem.ToString();

                ninja.Habilidades.JutsusPrincipais = txtAlergia.Text;
                ninja.Habilidades.NivelDePoder = CalculaPoder(ninja);

                bool sucesso = BancoDeDados.InserirNinja(ninja);

                if (sucesso)
                {
                    MessageBox.Show("Ninja salvo no MySQL com Sucesso! Dattebayo!");
                    LimparCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar: " + ex.Message);
            }
        }

        private void LimparCampos()
        {
            txtAlergia.Text = string.Empty;
            txtAltura.Text = string.Empty;
            txtModalidade.Text = string.Empty;
            txtNacionalidade.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtPeso.Text = string.Empty;
            cbxGenero.SelectedIndex = -1;
            cbxTipoSanguineo.SelectedIndex = -1;
            dtpNascimento.Value = DateTime.Now;
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            dgvAtletas.DataSource = BancoDeDados.BuscaNinjas();
        }

        private void txtNome_TextChanged(object sender, EventArgs e)
        {

        }
    }
}