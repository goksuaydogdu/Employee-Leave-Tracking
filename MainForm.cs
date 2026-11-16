using System;
using System.Windows.Forms;

namespace PersonelIzinTakip
{
    public class MainForm : Form
    {
        private Button btnPersonel;
        private Button btnIzin;
        private Button btnRapor;
        private Button btnCikis;

        public MainForm()
        {
            // Form ayarları
            this.Text = "Ana Menü";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 320;
            this.Height = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Personel İşlemleri Butonu
            btnPersonel = new Button();
            btnPersonel.Text = "Personel İşlemleri";
            btnPersonel.Width = 220;
            btnPersonel.Height = 35;
            btnPersonel.Left = 40;
            btnPersonel.Top = 30;
            btnPersonel.Click += BtnPersonel_Click;
            this.Controls.Add(btnPersonel);

            // İzin / Mesai İşlemleri Butonu
            btnIzin = new Button();
            btnIzin.Text = "İzin / Mesai İşlemleri";
            btnIzin.Width = 220;
            btnIzin.Height = 35;
            btnIzin.Left = 40;
            btnIzin.Top = 80;
            btnIzin.Click += BtnIzin_Click;
            this.Controls.Add(btnIzin);

            // Raporlama Butonu
            btnRapor = new Button();
            btnRapor.Text = "Raporlama";
            btnRapor.Width = 220;
            btnRapor.Height = 35;
            btnRapor.Left = 40;
            btnRapor.Top = 130;
            btnRapor.Click += BtnRapor_Click;
            this.Controls.Add(btnRapor);

            // Çıkış Butonu
            btnCikis = new Button();
            btnCikis.Text = "Çıkış";
            btnCikis.Width = 220;
            btnCikis.Height = 35;
            btnCikis.Left = 40;
            btnCikis.Top = 180;
            btnCikis.Click += BtnCikis_Click;
            this.Controls.Add(btnCikis);
        }

        private void BtnPersonel_Click(object sender, EventArgs e)
        {
            PersonelForm f = new PersonelForm();
            f.ShowDialog();

        }

       private void BtnIzin_Click(object sender, EventArgs e)
{
    IzinMesaiForm f = new IzinMesaiForm();
    f.ShowDialog();
}

       private void BtnRapor_Click(object sender, EventArgs e)
{
    RaporForm f = new RaporForm();
    f.ShowDialog();
}

        private void BtnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
