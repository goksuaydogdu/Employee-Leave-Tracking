using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PersonelIzinTakip
{
    public class IzinMesaiForm : Form
    {
        private readonly string connStr =
            @"Server=.\SQLEXPRESS;Database=PersonelDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        private Label lblPersonel, lblTur, lblBas, lblBit, lblAck;
        private ComboBox cmbPersonel, cmbTur;
        private DateTimePicker dtBas, dtBit;
        private TextBox txtAck;

        private Button btnEkle, btnSil, btnListele, btnTemizle, btnGeri;
        private DataGridView dgv;

        private TextBox txtKayitId;
        private Label lblKayitId;

        public IzinMesaiForm()
        {
            this.Text = "İzin / Mesai İşlemleri (SQL)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 950;
            this.Height = 560;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblKayitId = new Label() { Text = "KayıtID:", Left = 20, Top = 20, Width = 120 };
            txtKayitId = new TextBox() { Left = 150, Top = 18, Width = 250, ReadOnly = true };

            lblPersonel = new Label() { Text = "Personel:", Left = 20, Top = 55, Width = 120 };
            cmbPersonel = new ComboBox() { Left = 150, Top = 53, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            lblTur = new Label() { Text = "İşlem Türü:", Left = 20, Top = 90, Width = 120 };
            cmbTur = new ComboBox() { Left = 150, Top = 88, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTur.Items.AddRange(new object[] { "İzin", "Mesai" });
            cmbTur.SelectedIndex = 0;

            lblBas = new Label() { Text = "Başlangıç:", Left = 20, Top = 125, Width = 120 };
            dtBas = new DateTimePicker() { Left = 150, Top = 123, Width = 250, Format = DateTimePickerFormat.Short };

            lblBit = new Label() { Text = "Bitiş:", Left = 20, Top = 160, Width = 120 };
            dtBit = new DateTimePicker() { Left = 150, Top = 158, Width = 250, Format = DateTimePickerFormat.Short };

            lblAck = new Label() { Text = "Açıklama:", Left = 20, Top = 195, Width = 120 };
            txtAck = new TextBox() { Left = 150, Top = 193, Width = 250, Height = 60, Multiline = true };

            btnEkle = new Button() { Text = "Ekle", Left = 430, Top = 18, Width = 170 };
            btnSil = new Button() { Text = "Sil (Seçili)", Left = 430, Top = 53, Width = 170 };
            btnListele = new Button() { Text = "Listele", Left = 430, Top = 88, Width = 170 };
            btnTemizle = new Button() { Text = "Temizle", Left = 430, Top = 123, Width = 170 };
            btnGeri = new Button() { Text = "Geri", Left = 430, Top = 158, Width = 170 };

            dgv = new DataGridView()
            {
                Left = 20,
                Top = 270,
                Width = 890,
                Height = 240,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnEkle.Click += BtnEkle_Click;
            btnSil.Click += BtnSil_Click;
            btnListele.Click += (s, e) => Listele();
            btnTemizle.Click += (s, e) => Temizle();
            btnGeri.Click += (s, e) => this.Close();

            dgv.CellClick += Dgv_CellClick;

            this.Controls.Add(lblKayitId);
            this.Controls.Add(txtKayitId);
            this.Controls.Add(lblPersonel);
            this.Controls.Add(cmbPersonel);
            this.Controls.Add(lblTur);
            this.Controls.Add(cmbTur);
            this.Controls.Add(lblBas);
            this.Controls.Add(dtBas);
            this.Controls.Add(lblBit);
            this.Controls.Add(dtBit);
            this.Controls.Add(lblAck);
            this.Controls.Add(txtAck);

            this.Controls.Add(btnEkle);
            this.Controls.Add(btnSil);
            this.Controls.Add(btnListele);
            this.Controls.Add(btnTemizle);
            this.Controls.Add(btnGeri);

            this.Controls.Add(dgv);

            PersonelleriDoldur();
            Listele();
        }

        private void PersonelleriDoldur()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT PersonelID, (Ad + ' ' + Soyad + ' - ' + Departman) AS AdSoyad FROM Personeller ORDER BY PersonelID DESC",
                    conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbPersonel.DataSource = dt;
                    cmbPersonel.DisplayMember = "AdSoyad";
                    cmbPersonel.ValueMember = "PersonelID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (PersonelleriDoldur):\n" + ex.Message);
            }
        }

        private void Listele()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT 
    I.KayitID,
    I.PersonelID,
    P.Ad,
    P.Soyad,
    P.Departman,
    I.IslemTuru,
    I.BaslangicTarihi,
    I.BitisTarihi,
    I.Aciklama
FROM IzinMesai I
INNER JOIN Personeller P ON P.PersonelID = I.PersonelID
ORDER BY I.KayitID DESC
", conn))
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
            if (cmbPersonel.SelectedValue == null)
            {
                MessageBox.Show("Önce personel seç.");
                return;
            }

            if (dtBit.Value.Date < dtBas.Value.Date)
            {
                MessageBox.Show("Bitiş tarihi başlangıçtan küçük olamaz.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO IzinMesai (PersonelID, IslemTuru, BaslangicTarihi, BitisTarihi, Aciklama)
VALUES (@pid, @tur, @bas, @bit, @ack)
", conn))
                {
                    cmd.Parameters.AddWithValue("@pid", (int)cmbPersonel.SelectedValue);
                    cmd.Parameters.AddWithValue("@tur", cmbTur.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@bas", dtBas.Value.Date);
                    cmd.Parameters.AddWithValue("@bit", dtBit.Value.Date);
                    cmd.Parameters.AddWithValue("@ack", (object?)txtAck.Text.Trim() ?? DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kayıt eklendi.");
                Temizle();
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Ekle):\n" + ex.Message);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKayitId.Text))
            {
                MessageBox.Show("Silmek için listeden bir kayıt seç.");
                return;
            }

            var onay = MessageBox.Show("Seçili kaydı silmek istiyor musun?", "Onay", MessageBoxButtons.YesNo);
            if (onay != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM IzinMesai WHERE KayitID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", int.Parse(txtKayitId.Text));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Kayıt silindi.");
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

            txtKayitId.Text = dgv.CurrentRow.Cells["KayitID"].Value?.ToString() ?? "";

            var pidObj = dgv.CurrentRow.Cells["PersonelID"].Value;
            if (pidObj != null && int.TryParse(pidObj.ToString(), out int pid))
            {
                cmbPersonel.SelectedValue = pid;
            }

            cmbTur.SelectedItem = dgv.CurrentRow.Cells["IslemTuru"].Value?.ToString() ?? "İzin";

            if (DateTime.TryParse(dgv.CurrentRow.Cells["BaslangicTarihi"].Value?.ToString(), out DateTime bas))
                dtBas.Value = bas;

            if (DateTime.TryParse(dgv.CurrentRow.Cells["BitisTarihi"].Value?.ToString(), out DateTime bit))
                dtBit.Value = bit;

            txtAck.Text = dgv.CurrentRow.Cells["Aciklama"].Value?.ToString() ?? "";
        }

        private void Temizle()
        {
            txtKayitId.Clear();
            txtAck.Clear();
            dtBas.Value = DateTime.Today;
            dtBit.Value = DateTime.Today;
            if (cmbTur.Items.Count > 0) cmbTur.SelectedIndex = 0;
        }
    }
}
