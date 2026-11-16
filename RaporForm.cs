using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PersonelIzinTakip
{
    public class RaporForm : Form
    {
        private readonly string connStr =
            @"Server=.\SQLEXPRESS;Database=PersonelDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        private Button btnDepartmanRaporu;
        private Button btnPersonelToplamGun;
        private Button btnTumKayitlar;
        private Button btnGeri;

        private DataGridView dgv;

        public RaporForm()
        {
            this.Text = "Raporlama";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 950;
            this.Height = 520;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            btnDepartmanRaporu = new Button()
            {
                Text = "Departmana Göre İzin/Mesai Sayısı",
                Left = 20,
                Top = 20,
                Width = 280,
                Height = 35
            };
            btnDepartmanRaporu.Click += (s, e) => DepartmanRaporu();

            btnPersonelToplamGun = new Button()
            {
                Text = "Personel Bazlı Toplam İzin Günü",
                Left = 320,
                Top = 20,
                Width = 250,
                Height = 35
            };
            btnPersonelToplamGun.Click += (s, e) => PersonelToplamGunRaporu();

            btnTumKayitlar = new Button()
            {
                Text = "Tüm İzin/Mesai Kayıtları (JOIN)",
                Left = 590,
                Top = 20,
                Width = 230,
                Height = 35
            };
            btnTumKayitlar.Click += (s, e) => TumKayitlar();

            btnGeri = new Button()
            {
                Text = "Geri",
                Left = 830,
                Top = 20,
                Width = 80,
                Height = 35
            };
            btnGeri.Click += (s, e) => this.Close();

            dgv = new DataGridView()
            {
                Left = 20,
                Top = 70,
                Width = 890,
                Height = 400,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(btnDepartmanRaporu);
            this.Controls.Add(btnPersonelToplamGun);
            this.Controls.Add(btnTumKayitlar);
            this.Controls.Add(btnGeri);
            this.Controls.Add(dgv);

            TumKayitlar(); // açılınca direkt listele
        }

        private void TumKayitlar()
        {
            string sql = @"
SELECT 
    I.KayitID,
    P.Ad,
    P.Soyad,
    P.Departman,
    I.IslemTuru,
    I.BaslangicTarihi,
    I.BitisTarihi,
    I.Aciklama
FROM IzinMesai I
INNER JOIN Personeller P ON P.PersonelID = I.PersonelID
ORDER BY I.KayitID DESC";

            GridDoldur(sql);
        }

        private void DepartmanRaporu()
        {
            string sql = @"
SELECT 
    P.Departman,
    I.IslemTuru,
    COUNT(*) AS KayitSayisi
FROM IzinMesai I
INNER JOIN Personeller P ON P.PersonelID = I.PersonelID
GROUP BY P.Departman, I.IslemTuru
ORDER BY P.Departman, I.IslemTuru";

            GridDoldur(sql);
        }

        private void PersonelToplamGunRaporu()
        {
            // Sadece "İzin" kayıtlarını topluyoruz
            // Gün hesabı: DATEDIFF(day, Baslangic, Bitis) + 1
            string sql = @"
SELECT
    P.PersonelID,
    P.Ad,
    P.Soyad,
    P.Departman,
    SUM(DATEDIFF(day, I.BaslangicTarihi, I.BitisTarihi) + 1) AS ToplamIzinGunu
FROM IzinMesai I
INNER JOIN Personeller P ON P.PersonelID = I.PersonelID
WHERE I.IslemTuru = 'İzin'
GROUP BY P.PersonelID, P.Ad, P.Soyad, P.Departman
ORDER BY ToplamIzinGunu DESC";

            GridDoldur(sql);
        }

        private void GridDoldur(string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL HATASI (Rapor):\n" + ex.Message);
            }
        }
    }
}
