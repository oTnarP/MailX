using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MailX
{

    class MailSend
    {
        public TextBox txtEmail { get; set; }
        public TextBox txtPass { get; set; }
        public TextBox txtSubject { get; set; }
        public RichTextBox txtMessage { get; set; }
        public TextBox lblLog { get; set; }
        public TextBox txtDelay { get; set; }
        public TextBox txtBrowse { get; set; }
        public Button btnSend { get; set; }
        public List<string> emailList { get; set; }
        private string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        
        public void Send()
        {
            // Get the user input (e.g., "5" for 5 seconds)
            int seconds = int.Parse(txtDelay.Text);
            // Convert seconds to milliseconds
            int milliseconds = seconds * 1000;

            // Read Host and Port from Host.txt
            string hostFilePath = Path.Combine(documentsPath, "Host.txt");
            if (!File.Exists(hostFilePath))
                throw new Exception("Required configuration files not found!");
            // Read the first line from Host.txt (format: smtp.titan.email:587)
            string hostLine = File.ReadLines(hostFilePath).FirstOrDefault();
            string[] hostParts = hostLine?.Split(':');
            if (hostParts == null || hostParts.Length < 2)
                throw new Exception("Invalid Host.txt format!");

            string smtpHost = hostParts[0]; // smtp.titan.email
            int smtpPort = int.Parse(hostParts[1]); // 587

            btnSend.Text = "Sending Message...";
            try
            {
                // Retrieve sender's email and password
                string senderEmail = txtEmail.Text; 
                string senderPassword = txtPass.Text;  

                // Configure SMTP settings
                SmtpClient smtp = new SmtpClient(smtpHost, smtpPort);
                smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtp.EnableSsl = true;

                // Prepare email content
                string subject = txtSubject.Text;
                string body = txtMessage.Text;

                // Loop through each email in the emailList array using a for loop
                for (int i = 0; i < emailList.Count; i++)
                {
                    string recipientEmail = emailList[i].Trim(); // Trim whitespace from email address

                    // Skip empty or invalid email addresses
                    if (string.IsNullOrWhiteSpace(recipientEmail))
                    {
                        lblLog.Text += $"Skipped invalid email: {recipientEmail}\n";
                        continue;
                    }

                    // Create a new MailMessage for each recipient
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    // Send the email
                    smtp.Send(mail);
                    lblLog.Text = "";
                    // Log success for this recipient
                    lblLog.Text += $"Sent to: {recipientEmail}";
                    File.WriteAllLines(txtBrowse.Text, File.ReadAllLines(txtBrowse.Text).Where(line => line != recipientEmail).ToArray());
                    Thread.Sleep(milliseconds);
                }
                lblLog.Text = "";
                // Update UI after all emails are sent
                lblLog.Text += "Sent Successfully!";
                Thread.Sleep(5000);
                lblLog.Text = "";
                lblLog.Text += "Nothing new...";
                btnSend.Text = "Send Message";
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during sending
                lblLog.Text = "Error: " + ex.Message;
                Thread.Sleep(15000);
                lblLog.Text = "Nothing new...";
                btnSend.Text = "Send Message";
            }
        }
    }
}
