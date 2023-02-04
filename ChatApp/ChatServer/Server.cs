using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ChatServer
{
    public partial class Server : Form
    {
        SimpleTcpServer server;

        public Server()
        {
            InitializeComponent();
        }


        private void Server_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;

            server = new SimpleTcpServer(txtIP.Text);

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnSend.Enabled = false;
            txtIP.Enabled = true;
        }


        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker) delegate
            {
                txtInfo.Text += $"<{DateTime.Now}-{e.IpPort}>: {Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}" +
                $"{Environment.NewLine}{Environment.NewLine}";
            });
        }


        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"<{DateTime.Now}-{e.IpPort}>: Disconnected " +
                $"{Environment.NewLine}{Environment.NewLine}";
                lstClientIP.Items.Remove(e.IpPort);
            });
        }


        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"<{DateTime.Now}-{e.IpPort}>: Connected" +
                $"{Environment.NewLine}{Environment.NewLine}";

                lstClientIP.Items.Add(e.IpPort);
            });
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtInfo.Text += $"<{DateTime.Now}> Chat server started" +
                $"{Environment.NewLine}{Environment.NewLine}";

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnSend.Enabled = true;
            txtIP.Enabled = false;
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            txtInfo.Text += $"<{DateTime.Now}> Chat server stopped" +
                $"{Environment.NewLine}{Environment.NewLine}";

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnSend.Enabled = false;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server.IsListening && !string.IsNullOrEmpty(txtInfo.Text) && lstClientIP.SelectedItem != null)
            {
                server.Send(lstClientIP.SelectedItem.ToString(), txtMessage.Text);
                txtInfo.Text += $"<{DateTime.Now}-Server>: {txtMessage.Text}" +
                    $"{Environment.NewLine}{Environment.NewLine}";
                txtMessage.Text = string.Empty;
            }
        }
    }
}
