using System;
using System.Windows.Forms;
using System.Data;
using Microsoft.Data.SqlClient;


namespace PersonelIzinTakip
{
    public partial class LoginForm : Form
    {
        private readonly string connStr =
            @"Server=.\SQLEXPRESS;Database=PersonelDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        TextBox txtKullanici;
        TextBox txtSifre;
        Button btnGiris;

        public LoginForm()
        {
            this.Text = "Yönetici Girişi";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 300;
            this.Height = 200;

            Label lblK = new Label();
            lblK.Text = "Kullanıcı Adı";
            lblK.Left = 20;
            lblK.Top = 20;
            this.Controls.Add(lblK);

            txtKullanici = new TextBox();
            txtKullanici.Left = 120;
            txtKullanici.Top = 20;
            this.Controls.Add(txtKullanici);

            Label lblS = new Label();
            lblS.Text = "Şifre";
            lblS.Left = 20;
            lblS.Top = 60;
            this.Controls.Add(lblS);

            txtSifre = new TextBox();
            txtSifre.Left = 120;
            txtSifre.Top = 60;
            txtSifre.PasswordChar = '*';
            this.Controls.Add(txtSifre);

            btnGiris = new Button();
            btnGiris.Text = "Giriş Yap";
            btnGiris.Left = 120;
            btnGiris.Top = 100;
            btnGiris.Click += BtnGiris_Click;
            this.Controls.Add(btnGiris);
        }

        void BtnGiris_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Adminler WHERE KullaniciAdi=@k AND Sifre=@s",
                    conn);

                cmd.Parameters.AddWithValue("@k", txtKullanici.Text);
                cmd.Parameters.AddWithValue("@s", txtSifre.Text);

                conn.Open();
                int sonuc = (int)cmd.ExecuteScalar();

                if (sonuc > 0)
                {
                    MainForm main = new MainForm();
                    main.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalı kullanıcı adı veya şifre");
                }
            }
        }
    }
}
