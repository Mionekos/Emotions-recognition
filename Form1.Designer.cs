namespace anicore
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonAddMffc = new System.Windows.Forms.Button();
            this.textBoxAddMffc = new System.Windows.Forms.TextBox();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.textBoxNameOfEmo = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.buttonAgain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::anicore.Properties.Resources.fon_muzyka_2280512;
            this.pictureBox1.Location = new System.Drawing.Point(-1, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(636, 492);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // buttonAddMffc
            // 
            this.buttonAddMffc.Location = new System.Drawing.Point(31, 454);
            this.buttonAddMffc.Name = "buttonAddMffc";
            this.buttonAddMffc.Size = new System.Drawing.Size(128, 23);
            this.buttonAddMffc.TabIndex = 1;
            this.buttonAddMffc.Text = "Добавление mffc";
            this.buttonAddMffc.UseVisualStyleBackColor = true;
            this.buttonAddMffc.Click += new System.EventHandler(this.buttonAddMffc_Click);
            // 
            // textBoxAddMffc
            // 
            this.textBoxAddMffc.Location = new System.Drawing.Point(31, 428);
            this.textBoxAddMffc.Name = "textBoxAddMffc";
            this.textBoxAddMffc.Size = new System.Drawing.Size(128, 20);
            this.textBoxAddMffc.TabIndex = 2;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(179, 454);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(120, 23);
            this.buttonDownload.TabIndex = 3;
            this.buttonDownload.Text = "Загрузка файла";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // textBoxNameOfEmo
            // 
            this.textBoxNameOfEmo.Location = new System.Drawing.Point(179, 428);
            this.textBoxNameOfEmo.Name = "textBoxNameOfEmo";
            this.textBoxNameOfEmo.Size = new System.Drawing.Size(120, 20);
            this.textBoxNameOfEmo.TabIndex = 4;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(211, 355);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(10, 14);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // buttonAgain
            // 
            this.buttonAgain.Location = new System.Drawing.Point(328, 454);
            this.buttonAgain.Name = "buttonAgain";
            this.buttonAgain.Size = new System.Drawing.Size(110, 23);
            this.buttonAgain.TabIndex = 6;
            this.buttonAgain.Text = "Пройти снова";
            this.buttonAgain.UseVisualStyleBackColor = true;
            this.buttonAgain.Click += new System.EventHandler(this.buttonAgain_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 489);
            this.Controls.Add(this.buttonAgain);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.textBoxNameOfEmo);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.textBoxAddMffc);
            this.Controls.Add(this.buttonAddMffc);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonAddMffc;
        private System.Windows.Forms.TextBox textBoxAddMffc;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.TextBox textBoxNameOfEmo;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button buttonAgain;
    }
}

