using System;
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

        private void ConnectButton_Click(object sender, EventArgs e)
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
                        Thread listenThread = new Thread(ListenServer);
                        listenThread.IsBackground = true;
                        listenThread.Start();
                    }
                    catch
                    {
                        outputBox.AppendText("Connection STOPPED \n");
                        clientSocket.Close();
                        connected = false;
                        EnableInputBoxes();
                    }

                    if (incomingMessage == "REJECT")
                    {
                        clientSocket.Close();
                        connected = false;
                        EnableInputBoxes();
                    }

                }
                catch (Exception except)
                {
                    outputBox.AppendText("ERROR: Connection Fault not established \n");
                    connected = false;
                    clientSocket.Close();
                    EnableInputBoxes();

                }
            }
            else
            {
                outputBox.AppendText("Invalid or Empty Inputs\n");
                EnableInputBoxes();
            }

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                try
                {
                    EnableInputBoxes();
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

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            fileBox.Enabled = true;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();
            fileBox.Text = openFileDialog1.FileName;           
        }

        private void UploadButton_Click(object sender, EventArgs e)
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
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot upload without connection\n");
                }

                if (connection)
                {
                    fileBox.Enabled = false;
                    filepath = fileBox.Text;
                    if (!File.Exists(filepath))
                    {
                        outputBox.AppendText("ERROR: File Does Not Exist\n");
                        fileBox.Text = "";
                        fileBox.Enabled = true;
                    }
                    else
                    {
                        UploadFile(filepath);
                        fileBox.Enabled = true;
                    }
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
            }
        }

        private void EnableInputBoxes()
        {
            if (ipBox.InvokeRequired || portBox.InvokeRequired || usernameBox.InvokeRequired)
            {
                ipBox.BeginInvoke(new Action(delegate { EnableInputBoxes(); }));
                return;
            }
            ipBox.Enabled = true;
            portBox.Enabled = true;
            usernameBox.Enabled = true;
        }

        private void UploadFile(string filepath)
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
                EnableInputBoxes();
                return;

            }
            SafeLogWrite("File Sending is finished!!\n");

        }

        private void ListenServer()
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
                        SafeLogWrite(text);

                        if(incomingMessage.Contains("DOWNLOAD") && ACK_CHECK)
                        {
                            string filename = incomingMessage.Split()[2];
                            connected = DownloadFile(filename, DOWNLOAD_DIR);
                        }
                        else if(incomingMessage.Contains("GETFILE") && ACK_CHECK)
                        {
                            connected = GetFileList();
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
                        SafeLogWrite(text);
                        clientSocket.Close();
                        connected = false;
                        EnableInputBoxes();
                    }
                }
            }
        }

        private bool CheckConnection(Socket socket)
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

        public void SafeLogWrite(string EventText)
        {
            if (outputBox.InvokeRequired)
            {
                outputBox.BeginInvoke(new Action(delegate
                {
                    SafeLogWrite(EventText);
                }));
                return;
            }
            outputBox.AppendText(EventText);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot copy without connection initialized.\n");
            }
            else
            {
                try
                {
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot copy without connection.\n");
                }

                if (connection && (!IsFileBoxEmpty()))
                {
                    string copyFilename = fileBox.Text;

                    string commandMessage = "COPY" + " " + copyFilename;
                    Byte[] commandBuffer = new Byte[64];
                    commandBuffer = Encoding.Default.GetBytes(commandMessage);
                    Array.Resize(ref commandBuffer, 64); //TODO: is it necessary?
                    clientSocket.Send(commandBuffer);
                    
                }
                else if(IsFileBoxEmpty())
                {
                    outputBox.AppendText("ERROR: File Box is Empty\n");
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
                fileBox.Enabled = true;
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot download without connection initialized.\n");
            }
            else
            {
                try
                {
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot download without connection.\n");
                }

                if (connection && (!IsFileBoxEmpty()))
                {
                    fileBox.Enabled = false;
                    string downloadFilename = fileBox.Text;
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
                    else if(IsFileBoxEmpty())
                    {
                        outputBox.AppendText("ERROR:File Name is Empty\n");
                    }
                    else
                    {
                        outputBox.AppendText("ERROR: Download Directory is not set \n");
                    }
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
                fileBox.Enabled = true;
            }
        }

        private bool CheckEnd(Byte b)
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

        private void SendServerMessage(Socket server, string message)
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

        private void BrowseFolderButton_Click(object sender, EventArgs e)
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

        private void GetFileButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {                
                outputBox.AppendText("Cannot get files without connection initialized.\n");
            }
            else
            {
                try
                {
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot get files without connection.\n");
                }

                if (connection)
                {
                    if (myfileRadioButton.Checked || publicFileRadioButton.Checked)
                    {
                        string commandMessage = "";
                        if (myfileRadioButton.Checked)
                        {
                            commandMessage = "GETFILE ME";

                        }
                        else if(publicFileRadioButton.Checked)
                        {
                            commandMessage = "GETFILE PUBLIC";
                        }
                       
                        Byte[] commandBuffer = new Byte[64];
                        commandBuffer = Encoding.Default.GetBytes(commandMessage);
                        Array.Resize(ref commandBuffer, 64); //TODO: is it necesarry ?
                        clientSocket.Send(commandBuffer);
                    }
                    else
                    {
                        outputBox.AppendText("ERROR: File Option is not selected\n");
                    }
                }
                else
                {
                    outputBox.AppendText("ERROR: Not Connected\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
                fileBox.Enabled = true;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot download without connection initialized.\n");
            }
            else
            {
                try
                {
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot delete without connection.\n");
                }

                if (connection && (!IsFileBoxEmpty()))
                {
                    string deleteFilename = fileBox.Text;                  

                    string commandMessage = "DELETE" + " " + deleteFilename;
                    Byte[] commandBuffer = new Byte[64];
                    commandBuffer = Encoding.Default.GetBytes(commandMessage);
                    Array.Resize(ref commandBuffer, 64); //TODO: is it necesarry
                    clientSocket.Send(commandBuffer);
                }
                else if(IsFileBoxEmpty())
                {
                    outputBox.AppendText("ERROR: File Name is Empty\n");
                }
                else
                {
                    outputBox.AppendText("ERROR: Not Connected\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
                fileBox.Enabled = true;
            }
        }

        private void ChangeAccessButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            if (clientSocket == null)
            {
                outputBox.AppendText("Cannot change access without connection initialized.\n");
            }
            else
            {
                try
                {
                    connection = CheckConnection(clientSocket);
                }
                catch (Exception except)
                {
                    outputBox.AppendText("Cannot change access without connection.\n");
                }

                if (connection && (!IsFileBoxEmpty()))
                {

                    string changeAccessFilename = fileBox.Text;

                    string commandMessage = "CH_ACCESS" + " " + changeAccessFilename;
                    Byte[] commandBuffer = new Byte[64];
                    commandBuffer = Encoding.Default.GetBytes(commandMessage);
                    Array.Resize(ref commandBuffer, 64); //TODO: is it necessary?
                    clientSocket.Send(commandBuffer);
                }
                else if (IsFileBoxEmpty())
                {
                    outputBox.AppendText("ERROR: File Box is Empty\n");
                }
                else
                {
                    outputBox.AppendText("ERROR: NOT CONNECTED\n");
                    EnableInputBoxes();
                    clientSocket.Close();
                }
                fileBox.Enabled = true;
            }
        }

        private bool DownloadFile(string filename, string downloadDirectory)
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
                    int index = Array.FindIndex(downloadFileBuffer, CheckEnd);

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
                    SafeLogWrite("ERROR: During File Download\n");
                    downloadFile.Close();
                    fileDownloadError = false;
                    break;
                }
            }
            downloadFile.Close();

            if (fileDownloadError)
            {
                string output = $"Client: File {filename} Downloaded\n";
                SafeLogWrite(output);
                string message = filename + " Received";
                SendServerMessage(clientSocket, message);
            }
            else
            {
                outputBox.AppendText($"File {filename} Could Not Downloaded\n");
                result = false;
            }

            return result;
        }

        private bool GetFileList()
        {
            List<String> fileList = new List<String>();
            bool getFileError = false;
            bool result = true;
            int numBytes;
            int fileCount = 0;
            string getFileMessage;

            try
            {
                Byte[] fileCountBuffer = new byte[64];
                numBytes = clientSocket.Receive(fileCountBuffer);
                getFileMessage = Encoding.Default.GetString(fileCountBuffer);
                fileCount = Int32.Parse(getFileMessage.TrimEnd('\0'));
            }
            catch
            {
                outputBox.AppendText("ERROR: During Get File\n");
                getFileError = true;
            }

            Byte[] getFileBuffer = new byte[128];
            bool fileNumCheck = true;
            int temp = 0;
            while (fileNumCheck && (fileCount > 0))
            {
                try
                {
                    Array.Clear(getFileBuffer, 0, 128);
                    numBytes = clientSocket.Receive(getFileBuffer,0,128, SocketFlags.None);
                    //int index = Array.FindIndex(getFileBuffer, checkEnd);
                    getFileMessage = Encoding.Default.GetString(getFileBuffer);
                    getFileMessage = getFileMessage.TrimEnd('\0');
                    string[] fileArray = getFileMessage.Split('\n');
                    fileList.AddRange(fileArray);
                    temp += 1;
                    if (temp == fileCount)
                    {
                        break;
                    }
                }
                catch
                {
                    outputBox.AppendText("ERROR: During Get File\n");
                    getFileError = true;
                    break;
                }
            }

            if (!getFileError)
            {
                string output = "Client: Received File List\n";
                SafeLogWrite(output);
                
                output = "\nFILE LIST:\n-------\n";
                SafeLogWrite(output);
                if (fileCount > 0)
                {
                    int count = 0;
                    fileList.RemoveAll(CheckEmptyString);
                    foreach (string element in fileList)
                    {
                        count++;
                        string fileLine = $"File-{count}: {element}\n";
                        SafeLogWrite(fileLine);
                    }
                }
                else
                {
                    output = "File List is Empty\n";
                    SafeLogWrite(output);
                }
                output = "-------\n\n";
                SafeLogWrite(output);
                //safeLogWrite(fileList);
                string message = "File List Received";
                SendServerMessage(clientSocket, message);
            }
            else
            {
                string output = "Could Not Received File List";
                SafeLogWrite(output);
                result = false;
            }

            return result;
        }

        private static bool CheckEmptyString(String s)
        {
            return (s == "");
        }

        private bool IsFileBoxEmpty()
        {
            fileBox.Enabled = false;
            return (fileBox.Text == "");
        }
    }
}
