namespace ChatServer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtChatMsg = new TextBox();
            btnStart = new Button();
            lblMsg = new Label();
            SuspendLayout();
            // 
            // txtChatMsg
            // 
            txtChatMsg.Location = new Point(12, 28);
            txtChatMsg.Multiline = true;
            txtChatMsg.Name = "txtChatMsg";
            txtChatMsg.ScrollBars = ScrollBars.Vertical;
            txtChatMsg.Size = new Size(385, 346);
            txtChatMsg.TabIndex = 0;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(242, 386);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(155, 32);
            btnStart.TabIndex = 1;
            btnStart.Tag = "";
            btnStart.Text = "서버 시작";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Location = new Point(22, 394);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(96, 15);
            lblMsg.TabIndex = 2;
            lblMsg.Tag = "Stop";
            lblMsg.Text = "Server 중지 상태";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 450);
            Controls.Add(lblMsg);
            Controls.Add(btnStart);
            Controls.Add(txtChatMsg);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtChatMsg;
        private Button btnStart;
        private Label lblMsg;
    }
}
