﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Client
{
    public partial class CLIENT : Form
    {
        int MAX_BUF = (2 << 22);
        IPAddress serverIPAddress;
        int serverPortNum;
        //bool sendFilePermit = false;
        string username;
        string filepath;
        bool connected = false;
        Socket clientSocket;
        string DOWNLOAD_DIR = "";
        bool ACK_CHECK = false;


        private static ManualResetEvent mre = new ManualResetEvent(false);
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
            string ipBoxText = ipBox.Text;
            bool inputCheck = true;
            username = usernameBox.Text;

            try
            {
                Int32.TryParse(portBox.Text, out serverPortNum);
                serverIPAddress = IPAddress.Parse(ipBoxText);
            }
            catch
            {
                ipBox.Text = "";
                portBox.Text = "";
                usernameBox.Text = "";
                inputCheck = false;
            }

            if (inputCheck && username != "" && ipBoxText != "")
            {

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
                        incomingMessage = incomingMessage.TrimEnd('\0');
                        outputBox.AppendText("Server: " + incomingMessage + "\n");
                        Thread listenThread = new Thread(listenServer);
                        listenThread.IsBackground = true;
                        listenThread.Start();
                    }
                    catch
                    {
                        outputBox.AppendText("Connection STOPPED \n");
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }

                    if (incomingMessage == "REJECT")
                    {
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }

                }
                catch (Exception except)
                {
                    outputBox.AppendText("ERROR: Connection Fault not established \n");
                    connected = false;
                    clientSocket.Close();
                    enableInputBoxes();

                }
            }
            else
            {
                outputBox.AppendText("Invalid or Empty Inputs\n");
                enableInputBoxes();
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                try
                {
                    enableInputBoxes();
                    //ipBox.Enabled = true;
                    //portBox.Enabled = true;
                    //usernameBox.Enabled = true;
                    connected = false;
                    clientSocket.Close();

                    outputBox.AppendText($"Connection STOPPED by client\n");
                }
                catch (Exception except)
                {

                    outputBox.AppendText($"ERROR: Cannot Stop \n");
                }
            }
            else
            {
                outputBox.AppendText("ERROR: not connected\n");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chooseFileButton_Click(object sender, EventArgs e)
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
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot upload without connection\n");
            }
            else
            {
                try
                {
                    connection = checkConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot upload without connection\n");
                }

                if (connection)
                {
                    uploadFileBox.Enabled = false;
                    filepath = uploadFileBox.Text;
                    if (!File.Exists(filepath))
                    {
                        outputBox.AppendText("ERROR: File Does Not Exist\n");
                        uploadFileBox.Text = "";
                        uploadFileBox.Enabled = true;
                    }
                    else
                    {
                        uploadFile(filepath);
                        uploadFileBox.Enabled = true;
                    }
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    enableInputBoxes();
                    clientSocket.Close();
                }
            }
        }

        private void enableInputBoxes()
        {
            if (ipBox.InvokeRequired || portBox.InvokeRequired || usernameBox.InvokeRequired)
            {
                ipBox.BeginInvoke(new Action(delegate { enableInputBoxes(); }));
                return;
            }
            ipBox.Enabled = true;
            portBox.Enabled = true;
            usernameBox.Enabled = true;
        }

        private void uploadFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);
            string uploadMessage = "UPLOAD" + " " + filename;
            Byte[] commandBuffer = new Byte[64];
            commandBuffer = Encoding.Default.GetBytes(uploadMessage);
            Array.Resize(ref commandBuffer, 64);
            clientSocket.Send(commandBuffer);

            //TODO: ACK
            mre.WaitOne();

            Byte[] uploadBuffer = new Byte[MAX_BUF];
            try
            {

                //clientSocket.SendFile(filepath);
                // StreamReader sr = new StreamReader("TestFile.txt")
                using (FileStream fsSource = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {

                    // Read the source file into a byte array.
                    ulong numBytesToRead = (ulong)fsSource.Length;
                    //Byte[] fileSizeBuffer = new Byte[64];

                    Byte[] fileSizeBuffer = BitConverter.GetBytes(numBytesToRead);
                    Array.Resize(ref fileSizeBuffer, 64);
                    clientSocket.Send(fileSizeBuffer);


                    ulong numBytesRead = 0;
                    int n, temp;

                    while (numBytesToRead > 0)
                    {

                        // Read may return anything from 0 to numBytesToRead.
                        n = fsSource.Read(uploadBuffer, 0, MAX_BUF);

                        temp = clientSocket.Send(uploadBuffer, n, SocketFlags.None);
                        // Break when the end of the file is reached.

                        numBytesRead += (ulong)n;
                        numBytesToRead -= (ulong)n;

                    }
                }

            }
            catch (Exception except)
            {
                outputBox.AppendText("Error: during file sending.\n");
                outputBox.AppendText("Connection STOPPED \n");
                clientSocket.Close();
                connected = false;
                enableInputBoxes();
                return;

            }
            safeLogWrite("File Sending is finished!!\n");

        }

        private void listenServer()
        {
            while (connected)
            {
                string incomingMessage = "";

                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);
                    incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.TrimEnd('\0');
                    if (incomingMessage != "")
                    {
                        
                        if (incomingMessage.Contains("ACK"))
                        {
                            ACK_CHECK = true;
                            mre.Set();

                        }
                        
                        if(incomingMessage.Contains("ERR"))
                        {
                            ACK_CHECK = false;
                            mre.Set();
                        }

                        string text = "Server: " + incomingMessage + "\n";
                        safeLogWrite(text);

                        if(incomingMessage.Contains("DOWNLOAD") && ACK_CHECK)
                        {
                            string filename = incomingMessage.Split()[2];
                            downloadFile(filename, DOWNLOAD_DIR);
                        }
                        else if(incomingMessage.Contains("GET") && ACK_CHECK)
                        {
                            //TODO
                        }

                    }
                    else
                    {
                        //outputBox.AppendText("Empty String \n");
                        throw new System.InvalidOperationException("Empty String Received");
                    }
                }
                catch
                {
                    //mre.Set();
                    //outputBox.AppendText("Connection STOPPED \n");
                    if (connected)
                    {
                        string text = "Connection Stopped by Server \n";
                        safeLogWrite(text);
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }
                }
            }
        }

        private bool checkConnection(Socket socket)
        {
            bool blockingState = socket.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                socket.Blocking = false;
                socket.Send(tmp, 0, 0);
                //Console.WriteLine("Connected!");
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    //Console.WriteLine("Still Connected, but the Send would block");
                }
                else
                {
                    //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                }
                outputBox.AppendText("ERROR: Connection Check is a failed !!\n");
            }
            finally
            {
                socket.Blocking = blockingState;
            }
            return socket.Connected;
        }

        public void safeLogWrite(string EventText)
        {
            if (outputBox.InvokeRequired)
            {
                outputBox.BeginInvoke(new Action(delegate
                {
                    safeLogWrite(EventText);
                }));
                return;
            }
            outputBox.AppendText(EventText);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void copyButton_Click(object sender, EventArgs e)
        {

        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot upload without connection\n");
            }
            else
            {
                try
                {
                    connection = checkConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot upload without connection\n");
                }

                if (connection)
                {
                    uploadFileBox.Enabled = false;
                    string downloadFilename = uploadFileBox.Text;
                    filepath = DOWNLOAD_DIR;

                    if (DOWNLOAD_DIR != "")
                    {
                        //TODO:
                        string commandMessage = "DOWNLOAD" + " " + downloadFilename;
                        Byte[] commandBuffer = new Byte[64];
                        commandBuffer = Encoding.Default.GetBytes(commandMessage);
                        Array.Resize(ref commandBuffer, 64);
                        clientSocket.Send(commandBuffer);
                    }
                    else
                    {
                        outputBox.AppendText("ERROR:Download Directory is not set \n");
                    }

                    uploadFileBox.Enabled = true;
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    enableInputBoxes();
                    clientSocket.Close();
                }
            }
        }

        private bool downloadFile(string filename, string downloadDirectory)
        {
            bool fileDownloadError = true;
            bool result = true;

            FileStream downloadFile = File.Create(Path.Combine(downloadDirectory, filename));
            Byte[] downloadFileBuffer = new Byte[MAX_BUF];

            while (true)
            {
                try
                {
                    Array.Clear(downloadFileBuffer, 0, MAX_BUF);
                    int numBytes = clientSocket.Receive(downloadFileBuffer);
                    int index = Array.FindIndex(downloadFileBuffer, checkEnd);

                    if (index > -1)
                    {
                        downloadFile.Write(downloadFileBuffer, 0, index);
                        break;
                    }
                    else
                    {
                        downloadFile.Write(downloadFileBuffer, 0, numBytes);
                    }
                }
                catch
                {
                    outputBox.AppendText("ERROR: During File Download\n");
                    downloadFile.Close();
                    fileDownloadError = false;
                    break;
                }
            }
            downloadFile.Close();

            if (fileDownloadError)
            {
                string output = $"File {filename} Downloaded\n";
                safeLogWrite(output);
                string message = filename + " Received";
                sendServerMessage(clientSocket, message);
            }
            else
            {
                outputBox.AppendText($"File {filename} Could Not Downloaded\n");
                result = false;
            }

            return result;
        }

        private bool checkEnd(Byte b)
        {
            if (b == '\0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void sendServerMessage(Socket server, string message)
        {
            try
            {
                Byte[] buffer = Encoding.Default.GetBytes(message);
                server.Send(buffer);
            }
            catch
            {
                outputBox.AppendText($"ERROR: message to client: {message} not sent\n");
            }
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            string temp = "";
            downloadFolderBox.Enabled = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                downloadFolderBox.Text = folderBrowserDialog1.SelectedPath;
                temp = downloadFolderBox.Text;
            }
            if (Directory.Exists(temp))
            {
                DOWNLOAD_DIR = temp;
                outputBox.AppendText($"DOWNLOAD DIRECTORY: {DOWNLOAD_DIR} \n");
                downloadFolderBox.Enabled = false;
            }
            else
            {
                outputBox.AppendText("ERROR: path does not exist \n");
                DOWNLOAD_DIR = "";
            }
        }
    }
}
