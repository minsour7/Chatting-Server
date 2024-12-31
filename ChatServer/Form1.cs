using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ChatServer
{
    // ������ txtChatMsg �ؽ�Ʈ �ڽ��� ���� �������� ��������Ʈ
    // ���� ���� ���°��� Form1Ŭ������ UI�����尡 �ƴ� �ٸ� �������� ClientHandler�� ������ �̱⿡ ClientHandler�� �����忡�� ��������Ʈ�� ȣ���Ͽ� �ؽ�Ʈ �ڽ��� ���� ��
    delegate void SetTextDelegate(string s);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener chatServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 2022);
        public static ArrayList clientSocetArray = new ArrayList();

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // ���� ������ ���� ������ ���
                if (lblMsg.Tag.ToString() == "Stop")
                {
                    chatServer.Start();
                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));
                    waitThread.Start();

                    lblMsg.Text = "Server ���� ����";
                    lblMsg.Tag = "Start";
                    btnStart.Text = "���� ����";
                }
                else
                {
                    chatServer.Stop();
                    foreach (Socket socket in Form1.clientSocetArray)
                    {
                        socket.Close();
                    }
                    clientSocetArray.Clear();

                    lblMsg.Text = "Server ���� ����";
                    lblMsg.Tag = "Stop";
                    btnStart.Text = "���� ����";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("���� ���� ���� :" + ex.Message);
            }
        }

        // ���ѷ����� Ŭ�� ������ ��ٸ�
        private void AcceptClient()
        {
            Socket socketClient = null;
            while (true)
            {
                try
                {
                    socketClient = chatServer.AcceptSocket();

                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.ClientHandler_Setup(this, socketClient, this.txtChatMsg);
                    Thread thd_ChatProcess = new Thread(new ThreadStart(clientHandler.Chat_Process));
                    thd_ChatProcess.Start();
                }
                catch (Exception ex)
                {
                    Form1.clientSocetArray.Remove(socketClient);
                    break;
                }
            }
        }

        // �ؽ�Ʈ �ڽ��� ��ȭ������ ���� �޼ҵ�
        public void SetText(string text)
        {
            // txtChatMsg.InvokeRequired �� True�� ��ȯ�ϸ� ���� �����尡 UI�����尡 �ƴ����� Invoke�Ͽ� UI�����尡 ��������Ʈ�� ������ �޼ҵ带 ����
            // False�ϰ�� UI������������ ���� ����
            if (this.txtChatMsg.InvokeRequired)
            {
                SetTextDelegate setTextDelegate = new SetTextDelegate(SetText); // ��������Ʈ ����
                this.Invoke(setTextDelegate, new object[] { text });           // ��������Ʈ�� ���� ���� ��
            }
            else
            {
                this.txtChatMsg.AppendText(text); // �ؽ�Ʈ �ڽ��� ���� ��
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            chatServer.Stop();
        }
    }

    public class ClientHandler
    {
        private TextBox txtChatMsg;
        private Socket socketClient;
        private NetworkStream netStream;
        private StreamReader strReader;
        private Form1 form1;
        public void ClientHandler_Setup(Form1 form1, Socket socketClient, TextBox txtChatMsg)
        {
            this.txtChatMsg = txtChatMsg; // ä�� �޼��� ����� ���� TextBox
            this.socketClient = socketClient; // Ŭ���̾�Ʈ ���� ����
            this.netStream = new NetworkStream(socketClient);
            Form1.clientSocetArray.Add(socketClient); // Ŭ���̾�Ʈ ���Ӽ����� List �߰�
            this.strReader = new StreamReader(netStream);
            this.form1 = form1;
        }

        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    string lstMessage = strReader.ReadLine(); // ���ڿ� ����
                    if (lstMessage != null && lstMessage != "")
                    {
                        form1.SetText(lstMessage + "\r\n");
                        byte[] bytSand_Data = Encoding.Default.GetBytes(lstMessage);
                        lock (Form1.clientSocetArray)
                        {
                            foreach (Socket socket in Form1.clientSocetArray)
                            {
                                NetworkStream stream = new NetworkStream(socket);
                                stream.Write(bytSand_Data, 0, bytSand_Data.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ä�� ���� :" + ex.ToString());
                    Form1.clientSocetArray.Remove(socketClient);
                    break;
                }
            }

        }

    }
}
