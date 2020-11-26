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

// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

namespace Server
{

    public partial class Form1 : Form
    {

        int portNum;
        int MAX_CLIENT = 20;
        bool listening;
        string fileDirectory;
        Socket server;
        //public static ManualResetEvent allDone = new ManualResetEvent(false);
        IPAddress ipAddress;
        List<Socket> clientSocketList = new List<Socket>() ;
        List<string> usernameList = new List<string>();

        public Form1()
        {
            InitializeComponent();
            

            string host = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostEntry(host);
            logBox.AppendText("Server IP: " + ip.AddressList[1].ToString()+"\n");
            ipAddress = ip.AddressList[1];
            //clientSocketList = new List<Socket>();
            //usernameList = new List<string>();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            string port = portBox.Text;
            portBox.Enabled = false;
            try
            {
                portNum = Int32.Parse(port);
            }
            catch (FormatException)
            {
                logBox.AppendText($"ERROR not a valid number: '{portNum}' \n");
            }

            fileDirectory = fileBox.Text;
            fileBox.Enabled = false;

            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNum);
                server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(localEndPoint);
                server.Listen(MAX_CLIENT);
                listening = true;

                logBox.AppendText($"Server STARTED at port: {portNum} \n");

                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();


            }
            catch (Exception except)
            {

                logBox.AppendText($"ERROR:" + except.ToString() +"\n");
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {

                server.Close();
                //listening = false;
                logBox.AppendText($"Server STOPPED \n");
                portBox.Text = "";
                fileBox.Text = "";
                portBox.Enabled = true;
                fileBox.Enabled = true;
            }
            catch (Exception)
            {

                logBox.AppendText($"ERROR: Server cannot stop \n");
            }            
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    Socket newClient = server.Accept();
                    clientSocketList.Add(newClient);
                    logBox.AppendText("A client is connected.\n");

                    Thread receiveThread = new Thread(() => Receive(newClient)); // updated
                    receiveThread.Start();
                }
                catch (Exception e)
                {

                    logBox.AppendText("The socket stopped working.\n");
                    listening = false;
                }
            }
        }

        private void Receive(Socket thisClient) // updated
        {
            bool connected = true;

            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    thisClient.Receive(buffer);
                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    logBox.AppendText("Client: " + incomingMessage + "\n");
                }
                catch
                {
                    
                    //thisClient.Close();
                    clientSocketList.Remove(thisClient);
                    listening = false;
                    connected = false;
                }
            }
        }

    }
}
