using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PersonelIzinTakip
{
    public class PersonelForm : Form
    {
        private readonly string connStr =
            @"Server=.\SQLEXPRESS;Database=PersonelDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        private Label lblId, lblAd, lblSoyad, lblDepartman;
        private TextBox txtId, txtAd, txtSoyad, txtDepartman;
        private Button btnEkle, btnGuncelle, btnSil, btnListele, btnTemizle, btnGeri;
        private DataGridView dgv;

        public PersonelForm()
        {
            this.Text = "Personel İşlemleri (SQL)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 900;
            this.Height = 520;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblId = new Label() { Text = "PersonelID:", Left = 20, Top = 20, Width = 120 };
            txtId = new TextBox() { Left = 150, Top = 18, Width = 220, ReadOnly = true };

            lblAd = new Label() { Text = "Ad:", Left = 20, Top = 55, Width = 120 };
            txtAd = new TextBox() { Left = 150, Top = 53, Width = 220 };

            lblSoyad = new Label() { Text = "Soyad:", Left = 20, Top = 90, Width = 120 };
            txtSoyad = new TextBox() { Left = 150, Top = 88, Width = 220 };

            lblDepartman = new Label() { Text = "Departman:", Left = 20, Top = 125, Width = 120 };
            txtDepartman = new TextBox() { Left = 150, Top = 123, Width = 220 };

            btnEkle = new Button() { Text = "Ekle", Left = 400, Top = 18, Width = 160 };
            btnGuncelle = new Button() { Text = "Güncelle (Seçili)", Left = 400, Top = 53, Width = 160 };
            btnSil = new Button() { Text = "Sil (Seçili)", Left = 400, Top = 88, Width = 160 };
            btnListele = new Button() { Text = "Listele", Left = 400, Top = 123, Width = 160 };
            btnTemizle = new Button() { Text = "Temizle", Left = 580, Top = 18, Width = 160 };
            btnGeri = new Button() { Text = "Geri", Left = 580, Top = 53, Width = 160 };

            dgv = new DataGridView()
            {
                Left = 20,
                Top = 180,
                Width = 840,
                Height = 290,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnEkle.Click += BtnEkle_Click;
            btnGuncelle.Click += BtnGuncelle_Click;
            btnSil.Click += BtnSil_Click;
            btnListele.Click += (s, e) => Listele();
            btnTemizle.Click += (s, e) => Temizle();
            btnGeri.Click += (s, e) => this.Close();

            dgv.CellClick += Dgv_CellClick;

            this.Controls.Add(lblId);
            this.Controls.Add(txtId);
            this.Controls.Add(lblAd);
            this.Controls.Add(txtAd);
            this.Controls.Add(lblSoyad);
            this.Controls.Add(txtSoyad);
            this.Controls.Add(lblDepartman);
            this.Controls.Add(txtDepartman);

            this.Controls.Add(btnEkle);
            this.Controls.Add(btnGuncelle);
            this.Controls.Add(btnSil);
            this.Controls.Add(btnListele);
            this.Controls.Add(btnTemizle);
            this.Controls.Add(btnGeri);

            this.Controls.Add(dgv);

            Listele();
        }

        private void Listele()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT PersonelID, Ad, Soyad, Departman FROM Personeller ORDER BY PersonelID DESC",
                    conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Listele):\n" + ex.Message);
            }
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSoyad.Text) || string.IsNullOrWhiteSpace(txtDepartman.Text))
            {
                MessageBox.Show("Ad, Soyad ve Departman boş olamaz.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Personeller (Ad, Soyad, Departman) VALUES (@ad, @soyad, @dep)",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                    cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                    cmd.Parameters.AddWithValue("@dep", txtDepartman.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Personel eklendi.");
                Temizle();
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Ekle):\n" + ex.Message);
            }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Güncellemek için listeden bir personel seç.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSoyad.Text) || string.IsNullOrWhiteSpace(txtDepartman.Text))
            {
                MessageBox.Show("Ad, Soyad ve Departman boş olamaz.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Personeller SET Ad=@ad, Soyad=@soyad, Departman=@dep WHERE PersonelID=@id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                    cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                    cmd.Parameters.AddWithValue("@dep", txtDepartman.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", int.Parse(txtId.Text));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Personel güncellendi.");
                Temizle();
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Güncelle):\n" + ex.Message);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Silmek için listeden bir personel seç.");
                return;
            }

            var onay = MessageBox.Show("Seçili personeli silmek istiyor musun?", "Onay", MessageBoxButtons.YesNo);
            if (onay != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Personeller WHERE PersonelID=@id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", int.Parse(txtId.Text));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Personel silindi.");
                Temizle();
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Sil):\n" + ex.Message);
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.CurrentRow == null) return;

            txtId.Text = dgv.CurrentRow.Cells["PersonelID"].Value?.ToString() ?? "";
            txtAd.Text = dgv.CurrentRow.Cells["Ad"].Value?.ToString() ?? "";
            txtSoyad.Text = dgv.CurrentRow.Cells["Soyad"].Value?.ToString() ?? "";
            txtDepartman.Text = dgv.CurrentRow.Cells["Departman"].Value?.ToString() ?? "";
        }

        private void Temizle()
        {
            txtId.Clear();
            txtAd.Clear();
            txtSoyad.Clear();
            txtDepartman.Clear();
        }
    }
}
