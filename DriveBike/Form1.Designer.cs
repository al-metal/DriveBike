namespace DriveBike
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
            this.rtbMiniText = new System.Windows.Forms.RichTextBox();
            this.rtbFullText = new System.Windows.Forms.RichTextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbKeywords = new System.Windows.Forms.TextBox();
            this.lbMiniText = new System.Windows.Forms.Label();
            this.lblFullText = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblKeywords = new System.Windows.Forms.Label();
            this.btnSaveTemplate = new System.Windows.Forms.Button();
            this.btnActualCategory = new System.Windows.Forms.Button();
            this.btnUpdateImages = new System.Windows.Forms.Button();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.cbUpdateSEO = new System.Windows.Forms.CheckBox();
            this.cbFullText = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rtbMiniText
            // 
            this.rtbMiniText.Location = new System.Drawing.Point(12, 27);
            this.rtbMiniText.Name = "rtbMiniText";
            this.rtbMiniText.Size = new System.Drawing.Size(668, 182);
            this.rtbMiniText.TabIndex = 0;
            this.rtbMiniText.Text = "";
            // 
            // rtbFullText
            // 
            this.rtbFullText.Location = new System.Drawing.Point(12, 228);
            this.rtbFullText.Name = "rtbFullText";
            this.rtbFullText.Size = new System.Drawing.Size(668, 182);
            this.rtbFullText.TabIndex = 1;
            this.rtbFullText.Text = "";
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(12, 426);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(668, 20);
            this.tbTitle.TabIndex = 2;
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(12, 465);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(668, 20);
            this.tbDescription.TabIndex = 3;
            // 
            // tbKeywords
            // 
            this.tbKeywords.Location = new System.Drawing.Point(12, 501);
            this.tbKeywords.Name = "tbKeywords";
            this.tbKeywords.Size = new System.Drawing.Size(668, 20);
            this.tbKeywords.TabIndex = 4;
            // 
            // lbMiniText
            // 
            this.lbMiniText.AutoSize = true;
            this.lbMiniText.Location = new System.Drawing.Point(12, 9);
            this.lbMiniText.Name = "lbMiniText";
            this.lbMiniText.Size = new System.Drawing.Size(138, 13);
            this.lbMiniText.TabIndex = 5;
            this.lbMiniText.Text = "Краткое описание товара";
            // 
            // lblFullText
            // 
            this.lblFullText.AutoSize = true;
            this.lblFullText.Location = new System.Drawing.Point(12, 212);
            this.lblFullText.Name = "lblFullText";
            this.lblFullText.Size = new System.Drawing.Size(134, 13);
            this.lblFullText.TabIndex = 6;
            this.lblFullText.Text = "Полное описание товара";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 410);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(113, 13);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "Заголовок страницы";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 449);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(57, 13);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Описание";
            // 
            // lblKeywords
            // 
            this.lblKeywords.AutoSize = true;
            this.lblKeywords.Location = new System.Drawing.Point(12, 488);
            this.lblKeywords.Name = "lblKeywords";
            this.lblKeywords.Size = new System.Drawing.Size(92, 13);
            this.lblKeywords.TabIndex = 9;
            this.lblKeywords.Text = "Ключевые слова";
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.Location = new System.Drawing.Point(686, 498);
            this.btnSaveTemplate.Name = "btnSaveTemplate";
            this.btnSaveTemplate.Size = new System.Drawing.Size(187, 23);
            this.btnSaveTemplate.TabIndex = 10;
            this.btnSaveTemplate.Text = "Сохранить шаблон";
            this.btnSaveTemplate.UseVisualStyleBackColor = true;
            this.btnSaveTemplate.Click += new System.EventHandler(this.btnSaveTemplate_Click);
            // 
            // btnActualCategory
            // 
            this.btnActualCategory.Location = new System.Drawing.Point(704, 27);
            this.btnActualCategory.Name = "btnActualCategory";
            this.btnActualCategory.Size = new System.Drawing.Size(160, 26);
            this.btnActualCategory.TabIndex = 11;
            this.btnActualCategory.Text = "Актуализировать раздел";
            this.btnActualCategory.UseVisualStyleBackColor = true;
            this.btnActualCategory.Click += new System.EventHandler(this.btnActualCategory_Click);
            // 
            // btnUpdateImages
            // 
            this.btnUpdateImages.Location = new System.Drawing.Point(704, 59);
            this.btnUpdateImages.Name = "btnUpdateImages";
            this.btnUpdateImages.Size = new System.Drawing.Size(160, 26);
            this.btnUpdateImages.TabIndex = 12;
            this.btnUpdateImages.Text = "Обновить картинки";
            this.btnUpdateImages.UseVisualStyleBackColor = true;
            this.btnUpdateImages.Click += new System.EventHandler(this.btnUpdateImages_Click);
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(704, 103);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(160, 20);
            this.tbLogin.TabIndex = 13;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(704, 138);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(160, 20);
            this.tbPassword.TabIndex = 14;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // cbUpdateSEO
            // 
            this.cbUpdateSEO.AutoSize = true;
            this.cbUpdateSEO.Location = new System.Drawing.Point(704, 164);
            this.cbUpdateSEO.Name = "cbUpdateSEO";
            this.cbUpdateSEO.Size = new System.Drawing.Size(96, 17);
            this.cbUpdateSEO.TabIndex = 15;
            this.cbUpdateSEO.Text = "Обновить сео";
            this.cbUpdateSEO.UseVisualStyleBackColor = true;
            // 
            // cbFullText
            // 
            this.cbFullText.AutoSize = true;
            this.cbFullText.Location = new System.Drawing.Point(704, 187);
            this.cbFullText.Name = "cbFullText";
            this.cbFullText.Size = new System.Drawing.Size(165, 17);
            this.cbFullText.TabIndex = 16;
            this.cbFullText.Text = "Обновить полное описание";
            this.cbFullText.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 524);
            this.Controls.Add(this.cbFullText);
            this.Controls.Add(this.cbUpdateSEO);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.btnUpdateImages);
            this.Controls.Add(this.btnActualCategory);
            this.Controls.Add(this.btnSaveTemplate);
            this.Controls.Add(this.lblKeywords);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblFullText);
            this.Controls.Add(this.lbMiniText);
            this.Controls.Add(this.tbKeywords);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.rtbFullText);
            this.Controls.Add(this.rtbMiniText);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DriveBike";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMiniText;
        private System.Windows.Forms.RichTextBox rtbFullText;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.TextBox tbKeywords;
        private System.Windows.Forms.Label lbMiniText;
        private System.Windows.Forms.Label lblFullText;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblKeywords;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.Button btnActualCategory;
        private System.Windows.Forms.Button btnUpdateImages;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.CheckBox cbUpdateSEO;
        private System.Windows.Forms.CheckBox cbFullText;
    }
}

