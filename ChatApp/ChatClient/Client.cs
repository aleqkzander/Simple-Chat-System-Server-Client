using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSimpleTcp;

namespace ChatClient
{
    public partial class Client : Form
    {
        SimpleTcpClient client;

        public Client()
        {
            InitializeComponent();
        }

        private void Client_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient(txtIP.Text);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            btnSend.Enabled = false;
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
            txtIP.Enabled = true;
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"<{DateTime.Now}> Disconnected" +
                $"{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Array == null) 
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnSend.Enabled = false;
                    btnDisconnect.Enabled = false;
                    btnConnect.Enabled = true;
                    txtIP.Enabled = true;
                });
                return;
            }


            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"<{DateTime.Now}-Server>: {Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}" +
                $"{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"<{DateTime.Now}> Connected" +
                $"{Environment.NewLine}{Environment.NewLine}";
            });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect();

                btnSend.Enabled = true;
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
                txtIP.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n\n" + ex.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client.IsConnected && !string.IsNullOrEmpty(txtMessage.Text))
            {
                client.Send(txtMessage.Text);

                txtInfo.Text += $"<{DateTime.Now}-Me>: {txtMessage.Text}" +
                    $"{Environment.NewLine}{Environment.NewLine}";

                txtMessage.Text = string.Empty;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            btnSend.Enabled = false;
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
        }
    }
}
