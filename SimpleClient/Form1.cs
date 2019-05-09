using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace SimpleClient
{
    public partial class SimpleClient : Form
    {
        Client client = new Client();

        private bool running = true;

        public SimpleClient()
        {
            InitializeComponent();
            timer1.Interval = 500;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtboxIp.Text) || string.IsNullOrWhiteSpace(txtBoxPort.Text))
                {
                    MessageBox.Show("Please provide valid IP address and port number!");
                }
                else
                {
                    client.Connect(txtboxIp.Text, txtBoxPort.Text);
                    txtboxIp.Enabled = false;
                    txtBoxPort.Enabled = false;
                    lblId.Text = client.Id;
                    btnConnect.Enabled = false;
                    timer1.Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Invalid IP address!");
            }
        }

        private void lstBoxChat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Message m = new Message();
            m.MessageType = Message.Type.User;
            m.From = Int32.Parse(lblId.Text);
            m.To = Int32.Parse(txtRecipementId.Text);
            m.Content = txtBoxMsg.Text;
            client.messages.Enqueue(string.Format("From: {0} To: {1} \n {2}", m.From, m.To, m.Content));
            client.Send(m);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (client.messages.Count > 0)
            {
                listViewChatWindow.Items.Add(new ListViewItem().Text = string.Format(client.messages.Dequeue()));
                //listViewChatWindow.Items.Add(client.messages.Dequeue());
            }
        }
    }
}
