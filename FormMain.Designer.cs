namespace heytea_diy_gui
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panMain = new System.Windows.Forms.Panel();
            this.boxImage = new System.Windows.Forms.PictureBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.barThreshold = new System.Windows.Forms.TrackBar();
            this.panButton = new System.Windows.Forms.Panel();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnReadPicture = new System.Windows.Forms.Button();
            this.panMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boxImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barThreshold)).BeginInit();
            this.panButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // panMain
            // 
            this.panMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMain.Controls.Add(this.boxImage);
            this.panMain.Controls.Add(this.lblThreshold);
            this.panMain.Controls.Add(this.barThreshold);
            this.panMain.Controls.Add(this.panButton);
            this.panMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panMain.Location = new System.Drawing.Point(0, 0);
            this.panMain.Name = "panMain";
            this.panMain.Size = new System.Drawing.Size(1418, 754);
            this.panMain.TabIndex = 0;
            // 
            // boxImage
            // 
            this.boxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boxImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxImage.Location = new System.Drawing.Point(0, 76);
            this.boxImage.Name = "boxImage";
            this.boxImage.Size = new System.Drawing.Size(1416, 570);
            this.boxImage.TabIndex = 3;
            this.boxImage.TabStop = false;
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblThreshold.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblThreshold.Location = new System.Drawing.Point(0, 646);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(906, 37);
            this.lblThreshold.TabIndex = 2;
            this.lblThreshold.Text = "Step.2调整图片(鼠标滚轮缩放拖动平移)↑和灰度阈值(拉一拉看看效果)↓";
            // 
            // barThreshold
            // 
            this.barThreshold.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barThreshold.Location = new System.Drawing.Point(0, 683);
            this.barThreshold.Maximum = 256;
            this.barThreshold.Name = "barThreshold";
            this.barThreshold.Size = new System.Drawing.Size(1416, 69);
            this.barThreshold.TabIndex = 1;
            this.barThreshold.Value = 127;
            this.barThreshold.ValueChanged += new System.EventHandler(this.BarThreshold_ValueChanged);
            // 
            // panButton
            // 
            this.panButton.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panButton.Controls.Add(this.btnUpload);
            this.panButton.Controls.Add(this.btnReadPicture);
            this.panButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.panButton.Location = new System.Drawing.Point(0, 0);
            this.panButton.Name = "panButton";
            this.panButton.Size = new System.Drawing.Size(1416, 76);
            this.panButton.TabIndex = 0;
            // 
            // btnUpload
            // 
            this.btnUpload.Enabled = false;
            this.btnUpload.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUpload.Location = new System.Drawing.Point(315, 9);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(300, 50);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Step3.上传图片";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.BtnUpload_Click);
            // 
            // btnReadPicture
            // 
            this.btnReadPicture.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReadPicture.Location = new System.Drawing.Point(9, 9);
            this.btnReadPicture.Name = "btnReadPicture";
            this.btnReadPicture.Size = new System.Drawing.Size(300, 50);
            this.btnReadPicture.TabIndex = 0;
            this.btnReadPicture.Text = "Step1.导入图片";
            this.btnReadPicture.UseVisualStyleBackColor = true;
            this.btnReadPicture.Click += new System.EventHandler(this.BtnReadPicture_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1418, 754);
            this.Controls.Add(this.panMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "茶姬杯贴diy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panMain.ResumeLayout(false);
            this.panMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boxImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barThreshold)).EndInit();
            this.panButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panMain;
        private System.Windows.Forms.Panel panButton;
        private System.Windows.Forms.Button btnReadPicture;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.TrackBar barThreshold;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.PictureBox boxImage;
    }
}

