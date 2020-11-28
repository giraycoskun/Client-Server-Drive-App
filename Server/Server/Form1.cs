using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
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
            Control.CheckForIllegalCrossThreadCalls = false;
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
            bool input_check = true;
            string port = portBox.Text;
            portBox.Enabled = false;
            try
            {
                portNum = Int32.Parse(port);
            }
            catch (FormatException)
            {
                logBox.AppendText($"ERROR not a valid number: '{portNum}' \n");
                input_check = false;
            }

            if(fileDirectory== "")
            {
                input_check = false;
            }
            if (input_check)
            {
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

                    logBox.AppendText($"ERROR:" + except.ToString() + "\n");
                }
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {

                server.Close();
                listening = false;
                logBox.AppendText($"Server STOPPED \n");
                portBox.Text = "";
                fileBox.Text = "";
                portBox.Enabled = true;
                fileBox.Enabled = true;
            }
            catch (Exception)
            {

                logBox.AppendText($"ERROR: Server cannot stop properly \n");
                listening = false;
                logBox.AppendText($"Server STOPPED \n");
                portBox.Text = "";
                fileBox.Text = "";
                portBox.Enabled = true;
                fileBox.Enabled = true;
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

                    logBox.AppendText("The server stopped working.\n");
                    listening = false;
                }
            }
        }

        private void Receive(Socket thisClient) // updated
        {
            string username = "";
            try
            {
                Byte[] buffer = new Byte[64];
                thisClient.Receive(buffer);
                username = Encoding.Default.GetString(buffer);
                username = username.Substring(0, username.IndexOf("\0"));
                logBox.AppendText("Client tries to connect: " + username + "\n");
                //incomingmessage = "giray"
                //string username = incomingMessage.Split(' ').ToList()[1];
            }
            catch
            {
                thisClient.Close();
                clientSocketList.Remove(thisClient);
                listening = false;
                return;
            }
            bool result = checkUsername(username);
            if (result)
            {
                usernameList.Add(username);
                logBox.AppendText("Client is Accepted: HI " + username + "\n");
                //logBox.AppendText(usernameList.ToString()+"\n");
                handleClient(thisClient, username);
            }
            else
            {
                rejectClient(thisClient, username);
                thisClient.Shutdown(SocketShutdown.Both);
                thisClient.Close();
                logBox.AppendText("Client is Rejected: " + username + "\n");
                clientSocketList.Remove(thisClient);
            }
        }
        private bool checkUsername(string username)
        {
            bool result = true;
            
            if (usernameList.Contains(username))
            {
                result = false;
            }
            return result;
        }

        private void handleClient(Socket client, string username)
        {
            try
            {
                string hello_message = "Hi from server";
                Byte[] buffer = Encoding.Default.GetBytes(hello_message);
                client.Send(buffer);
            }
            catch
            {
                logBox.AppendText($"ERROR: hi message from server to client {username} could not sent!!");
            }

            bool connected = client.Connected;
            while (connected)
            {
                connected = client.Connected;
                //FILE UPLOAD
                //Thread.Sleep(5000);
                //logBox.AppendText("5 seconds past");
            }
            logBox.AppendText($"User: {username} disconnected");
            usernameList.Remove(username);
        }

        private void rejectClient(Socket client, string username)
        {
            try
            {
                string hello_message = "REJECT";
                Byte[] buffer = Encoding.Default.GetBytes(hello_message);
                client.Send(buffer);
            }
            catch
            {
                logBox.AppendText($"ERROR: REJECT message from server to client {username} could not sent!!");
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string temp = "";
            fileBox.Enabled = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                fileBox.Text = folderBrowserDialog1.SelectedPath;
                temp = fileBox.Text;
            }
            if(Directory.Exists(temp))
            {
                fileDirectory = temp;
                logBox.AppendText($"DOWNLOAD DIRECTORY: {fileDirectory} \n");
                fileBox.Enabled = false;
            }
            else
            {
                logBox.AppendText("ERROR: path does not exist \n");
                fileDirectory = "";
            }
        }

        private void fileBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
