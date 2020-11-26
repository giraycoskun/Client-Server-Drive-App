using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class CLIENT : Form
    {
        IPAddress serverIPAddress;
        int serverPortNum;
        string username;
        Socket clientSocket;
        
        public CLIENT()
        {
            InitializeComponent();
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void CLIENT_Load(object sender, EventArgs e)
        {

        }

        private void ipBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            ipBox.Enabled = false;
            portBox.Enabled = false;
            usernameBox.Enabled = false;

            string ipBoxText= ipBox.Text;
        
            username = usernameBox.Text;

            try
            {
                Int32.TryParse(portBox.Text, out serverPortNum);
            }
            catch
            {
                outputBox.AppendText("ERROR: port number is not valid \n");
            }


            serverIPAddress = IPAddress.Parse(ipBoxText); 

            try
            {
                clientSocket.Connect(serverIPAddress, serverPortNum);
                outputBox.AppendText($"CONNECTED to the server with ip: {ipBoxText} and port: {serverPortNum} !\n");
                string helloMessage = "HI " + username;
                if (username != "" && helloMessage.Length <= 64)
                {
                    Byte[] buffer = new Byte[64];
                    buffer = Encoding.Default.GetBytes(helloMessage);
                    clientSocket.Send(buffer);
                }
                
            }
            catch (Exception except)
            {
                outputBox.AppendText("ERROR: cannot CONNECT " +except.ToString() + "\n");
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket.Close();
            }
            catch (Exception except)
            {

                outputBox.AppendText($"ERROR: Cannot Stop {except.ToString()}\n");
            }
   
            
        }
    }
}
