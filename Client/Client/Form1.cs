using System;
using System.IO;
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
        string filepath;
        bool connected = false;
        Socket clientSocket;
        
        public CLIENT()
        {
            InitializeComponent();
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
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipBox.Enabled = false;
            portBox.Enabled = false;
            usernameBox.Enabled = false;
            string ipBoxText= ipBox.Text;
        
            username = usernameBox.Text;
            //TODO: Input Check
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
                connected = true;
                outputBox.AppendText($"CONNECTED to the server with ip: {ipBoxText} and port: {serverPortNum} !\n");
                string helloMessage = username;
                if (username != "" && helloMessage.Length <= 64)
                {
                    Byte[] buffer = new Byte[64];
                    buffer = Encoding.Default.GetBytes(helloMessage);
                    clientSocket.Send(buffer);
                }
                else
                {
                    outputBox.AppendText("Username length is either empty or too long!\n");
                }

                string incomingMessage = "";

                try
                {
                    Byte[] hello_buffer = new Byte[64];
                    clientSocket.Receive(hello_buffer);
                    incomingMessage = Encoding.Default.GetString(hello_buffer);
                    outputBox.AppendText("Server: " + incomingMessage + "\n");
                }
                catch
                {
                    outputBox.AppendText("Connection STOPPED \n");
                    clientSocket.Close();
                    connected = false;
                    enableInputBoxes();
                }

                if(incomingMessage == "REJECT")
                {
                    clientSocket.Close();
                    connected = false;
                    enableInputBoxes();
                }

            }
            catch (Exception except)
            {
                outputBox.AppendText("\nERROR: Connection Fault not established \n");
                clientSocket.Close();
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                try
                {
                    clientSocket.Close();
                    connected = false;
                    outputBox.AppendText($"\nConnection STOPPED by client\n");
                    ipBox.Enabled = true;
                    portBox.Enabled = true;
                    usernameBox.Enabled = true;
                }
                catch (Exception except)
                {

                    outputBox.AppendText($"\nERROR: Cannot Stop {except.ToString()}\n");
                }
            }            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            uploadFileBox.Enabled = true;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();
            uploadFileBox.Text = openFileDialog1.FileName;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                uploadFileBox.Enabled = false;
                filepath = uploadFileBox.Text;
                if (!File.Exists(filepath))
                {
                    outputBox.AppendText("ERROR: File Does Not Exist\n");
                    uploadFileBox.Text = "";
                    uploadFileBox.Enabled = true;
                }

                uploadFile();
                uploadFileBox.Enabled = true;

            }
            else
            {
                outputBox.AppendText("ERROR: NOT CONNECTED\n");
            }
            
        }

        private void enableInputBoxes()
        {
            ipBox.Enabled = true;
            portBox.Enabled = true;
            usernameBox.Enabled = true;

        }



        private void uploadFile()
        {
            //TODO: SEND FILE
        }
    }
}
