using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailX
{
    public partial class Main: Form
    {
        private string[] emailList;
        Thread th;
        private bool mouseDown;
        private Point lastLocation;
        public Main()
        {
            InitializeComponent();
            txtBrowse.KeyPress += txtBrowse_KeyPress;
            Label.CheckForIllegalCrossThreadCalls = false;
            Button.CheckForIllegalCrossThreadCalls = false;
            TextBox.CheckForIllegalCrossThreadCalls = false;
            ListBox.CheckForIllegalCrossThreadCalls = false;
            ListView.CheckForIllegalCrossThreadCalls = false;
            DataGridView.CheckForIllegalCrossThreadCalls = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Disable TabStop for all controls
            foreach (Control control in this.Controls)
            {
                control.TabStop = false;
            }

            try
            {
                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string hostFilePath = Path.Combine(documentsPath, "Host.txt");

                // Create the file if it doesn't exist
                if (!File.Exists(hostFilePath))
                {
                    File.WriteAllText(hostFilePath, "smtp.titan.email:587"); // Default content
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void txtBrowse_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a Text File";
                openFileDialog.Filter = "Text Files|*.txt"; // Only allow text files

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Display the selected file path in the label
                    txtBrowse.Text = openFileDialog.FileName;

                    // Read all lines and store them in an array
                    emailList = File.ReadAllLines(openFileDialog.FileName);
                }
                lblLog.Text = "Total Mail: " + emailList.Length.ToString();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (emailList == null)
                return;

            MailSend ms = new MailSend();
            ms.txtEmail = txtEmail;
            ms.txtPass = txtPass;
            ms.txtSubject = txtSubject;
            ms.txtMessage = txtMessage;
            ms.btnSend = btnSend;
            ms.lblLog = lblLog;
            ms.txtDelay = txtDelay;
            ms.txtBrowse = txtBrowse;
            ms.emailList = emailList.ToList();
            th = new Thread(() => ms.Send()) { IsBackground = true };
            th.Start();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Mouse Move
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //Mouse Up
            mouseDown = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            //Mouse Down
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtEmail_Enter(object sender, EventArgs e)
        {
            if (txtEmail.Text == "Enter your email")
            {
                txtEmail.Text = "";
                txtEmail.ForeColor = Color.Black; // Normal text color
            }
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.Text = "Enter your email";
                txtEmail.ForeColor = Color.Gray; // Placeholder color
            }
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "Enter password")
            {
                txtPass.Text = "";
                txtPass.ForeColor = Color.Black; // Normal text color
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                txtPass.Text = "Enter password";
                txtPass.ForeColor = Color.Gray; // Placeholder color
            }
        }

        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is not a digit or backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8) // 8 is the ASCII code for backspace
            {
                e.Handled = true; // Prevent the input
            }
        }
    }
}
