using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ChatServer
{
    // 서버의 txtChatMsg 텍스트 박스에 글을 쓰기위한 델리게이트
    // 실제 글을 쓰는것은 Form1클래스의 UI쓰레드가 아닌 다른 스레드인 ClientHandler의 스레드 이기에 ClientHandler의 스레드에서 델리게이트를 호출하여 텍스트 박스에 글을 씀
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
                // 현재 서버가 종료 상태인 경우
                if (lblMsg.Tag.ToString() == "Stop")
                {
                    chatServer.Start();
                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));
                    waitThread.Start();

                    lblMsg.Text = "Server 시작 상태";
                    lblMsg.Tag = "Start";
                    btnStart.Text = "서버 종료";
                }
                else
                {
                    chatServer.Stop();
                    foreach (Socket socket in Form1.clientSocetArray)
                    {
                        socket.Close();
                    }
                    clientSocetArray.Clear();

                    lblMsg.Text = "Server 종료 상태";
                    lblMsg.Tag = "Stop";
                    btnStart.Text = "서버 시작";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("서버 시작 오류 :" + ex.Message);
            }
        }

        // 무한루프중 클라 접속을 기다림
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

        // 텍스트 박스에 대화내용을 쓰는 메소드
        public void SetText(string text)
        {
            // txtChatMsg.InvokeRequired 가 True를 반환하면 현재 스레드가 UI스레드가 아님으로 Invoke하여 UI스레드가 델리게이트에 설정된 메소드를 실행
            // False일경우 UI스레드임으로 문제 없음
            if (this.txtChatMsg.InvokeRequired)
            {
                SetTextDelegate setTextDelegate = new SetTextDelegate(SetText); // 델리게이트 선언
                this.Invoke(setTextDelegate, new object[] { text });           // 델리게이트를 통해 글을 씀
            }
            else
            {
                this.txtChatMsg.AppendText(text); // 텍스트 박스에 글을 씀
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
            this.txtChatMsg = txtChatMsg; // 채팅 메세지 출력을 위한 TextBox
            this.socketClient = socketClient; // 클라이언트 접속 소켓
            this.netStream = new NetworkStream(socketClient);
            Form1.clientSocetArray.Add(socketClient); // 클라이언트 접속소켓을 List 추가
            this.strReader = new StreamReader(netStream);
            this.form1 = form1;
        }

        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    string lstMessage = strReader.ReadLine(); // 문자열 받음
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
                    MessageBox.Show("채팅 오류 :" + ex.ToString());
                    Form1.clientSocetArray.Remove(socketClient);
                    break;
                }
            }

        }

    }
}
