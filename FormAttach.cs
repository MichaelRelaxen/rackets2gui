using racketslistener2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rackets2listener
{
    public partial class FormAttach : Form
    {
        public FormAttach()
        {
            InitializeComponent();

            if (File.Exists(Environment.CurrentDirectory + @"\config.txt"))
            {
                textBoxIPAddress.Text = Func.GetConfigData("ip");
            }
            else
            {
                var config = File.Create("config.txt");
                config.Close();
            }
        }


        private void buttonAttach_Click(object sender, EventArgs e)
        {
            this.Hide();

            Func.SetConfigData("ip", Convert.ToString(textBoxIPAddress.Text));

            // Func.PrepareRatchetron(textBoxIPAddress.Text);
            var api = new Ratchetron(textBoxIPAddress.Text);
            api.Connect();


            var mainForm = new FormMain(api);
            mainForm.ShowDialog();
        }
    }
}
