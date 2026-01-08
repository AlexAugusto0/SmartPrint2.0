using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    public partial class FormFiltrosCarregamento : Form
    {
        private ComboBox cmbTipo;
        private ComboBox cmbGrupo;
        private ComboBox cmbFabricante;
        private ComboBox cmbFornecedor;
        private ComboBox cmbEmpresa;
        private Button btnCancelar;
        private Button btnConfirmar;
        private Label lblTipo;
        private Label lblGrupo;
        private Label lblFabricante;
        private Label lblFornecedor;
        private Label lblEmpresa;

        public string TipoSelecionado { get; private set; }
        public string GrupoSelecionado { get; private set; }
        public string FabricanteSelecionado { get; private set; }
        public string FornecedorSelecionado { get; private set; }
        public string EmpresaSelecionada { get; private set; }

        public FormFiltrosCarregamento()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CarregarDados();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.ClientSize = new System.Drawing.Size(420, 340);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "SOFTSHOP - Carregar";
            this.BackColor = Color.White;

            //// Labels
            lblTipo = new Label
            {
                Text = "Tipo:",
                Location = new Point(20, 20),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F)

            };

            lblTipo.Visible = false;

            lblGrupo = new Label
            {
                Text = "Grupo:",
                Location = new Point(20, 60),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F)
            };

            lblFabricante = new Label
            {
                Text = "Fabricante:",
                Location = new Point(20, 100),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F)
            };

            lblFornecedor = new Label
            {
                Text = "Fornecedor:",
                Location = new Point(20, 140),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F)
            };

            lblEmpresa = new Label
            {
                Text = "Empresa:",
                Location = new Point(20, 180),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F)
            };

            // ComboBoxes
            cmbTipo = new ComboBox
            {
                Location = new Point(130, 20),
                Size = new Size(270, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            cmbTipo.Visible = false;

            cmbGrupo = new ComboBox
            {
                Location = new Point(130, 60),
                Size = new Size(270, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            cmbFabricante = new ComboBox
            {
                Location = new Point(130, 100),
                Size = new Size(270, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            cmbFornecedor = new ComboBox
            {
                Location = new Point(130, 140),
                Size = new Size(270, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            cmbEmpresa = new ComboBox
            {
                Location = new Point(130, 180),
                Size = new Size(270, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            // Botões
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(80, 260),
                Size = new Size(130, 50),
                BackColor = Color.FromArgb(255, 128, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            btnConfirmar = new Button
            {
                Text = "Confirmar",
                Location = new Point(220, 260),
                Size = new Size(130, 50),
                BackColor = Color.FromArgb(255, 165, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            // Adicionar controles ao formulário
            this.Controls.Add(lblTipo);
            this.Controls.Add(lblGrupo);
            this.Controls.Add(lblFabricante);
            this.Controls.Add(lblFornecedor);
            this.Controls.Add(lblEmpresa);
            this.Controls.Add(cmbTipo);
            this.Controls.Add(cmbGrupo);
            this.Controls.Add(cmbFabricante);
            this.Controls.Add(cmbFornecedor);
            this.Controls.Add(cmbEmpresa);
            this.Controls.Add(btnCancelar);
            this.Controls.Add(btnConfirmar);

            this.ResumeLayout(false);
        }

        private void ConfigurarFormulario()
        {
            // Adicionar tipos
            cmbTipo.Items.AddRange(new string[]
            {
                "AJUSTES",
                "BALANÇOS",
                "NOTAS ENTRADA",
                "PREÇOS ALTERADOS",
                "PROMOÇÕES"
            });

            // Adicionar empresa padrão
            cmbEmpresa.Items.Add("MATRIZ");
            cmbEmpresa.SelectedIndex = 0;

            // Evento de mudança de tipo (ainda não implementado, mas deixa preparado)
            cmbTipo.SelectedIndexChanged += CmbTipo_SelectedIndexChanged;
        }

        private void CarregarDados()
        {
            try
            {
                // Carregar Grupos
                CarregarComboDistinto(cmbGrupo, "Grupo");

                // Carregar Fabricantes
                CarregarComboDistinto(cmbFabricante, "Fabricante");

                // Carregar Fornecedores
                CarregarComboDistinto(cmbFornecedor, "Fornecedor");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarComboDistinto(ComboBox combo, string campo)
        {
            try
            {
                DataTable dt = LocalDatabaseManager.ObterValoresDistintos(campo);

                combo.Items.Clear();
                combo.Items.Add(""); // Item vazio para permitir "nenhum filtro"

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string valor = row[campo]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(valor))
                        {
                            combo.Items.Add(valor);
                        }
                    }
                }

                combo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar {campo}: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Aqui será implementada a lógica de cada tipo no futuro
            // Por enquanto deixa só preparado
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            // Verificar se pelo menos um filtro foi selecionado
            if (string.IsNullOrEmpty(cmbGrupo.Text) &&
                string.IsNullOrEmpty(cmbFabricante.Text) &&
                string.IsNullOrEmpty(cmbFornecedor.Text))
            {
                MessageBox.Show("Selecione pelo menos um filtro (Grupo, Fabricante ou Fornecedor)!",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Armazenar valores selecionados
            TipoSelecionado = cmbTipo.Text;
            GrupoSelecionado = cmbGrupo.Text;
            FabricanteSelecionado = cmbFabricante.Text;
            FornecedorSelecionado = cmbFornecedor.Text;
            EmpresaSelecionada = cmbEmpresa.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}