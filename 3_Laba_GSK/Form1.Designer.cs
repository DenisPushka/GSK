
namespace _3_Laba_GSK
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.comboBoxColor = new System.Windows.Forms.ComboBox();
            this.comboBoxFill = new System.Windows.Forms.ComboBox();
            this.comboBoxSelectFigure = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.comboBoxVertCount = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBoxSelectTmo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBoxMain)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(1020, 83);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(97, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Вывод границ";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.ComboBox_HaveBorder);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Location = new System.Drawing.Point(1029, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button_Clear);
            // 
            // pictureBox1
            // 
            this.pictureBoxMain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBoxMain.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxMain.Location = new System.Drawing.Point(23, 12);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(1395, 616);
            this.pictureBoxMain.TabIndex = 2;
            this.pictureBoxMain.TabStop = false;
            this.pictureBoxMain.Click += new System.EventHandler(this.Form1_Load);
            this.pictureBoxMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureMouseDown);
            this.pictureBoxMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMouseMove);
            // 
            // comboBox1
            // 
            this.comboBoxColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxColor.FormattingEnabled = true;
            this.comboBoxColor.Items.AddRange(new object[] {"Черный", "Красный", "Зеленый", "Синий"});
            this.comboBoxColor.Location = new System.Drawing.Point(320, 79);
            this.comboBoxColor.Name = "comboBoxColor";
            this.comboBoxColor.Size = new System.Drawing.Size(150, 21);
            this.comboBoxColor.TabIndex = 3;
            this.comboBoxColor.Text = "Цвета";
            this.comboBoxColor.SelectedIndexChanged += new System.EventHandler(this.ComboBox_GetColor);
            // 
            // comboBox2
            // 
            this.comboBoxFill.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxFill.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.comboBoxFill.ForeColor = System.Drawing.SystemColors.InfoText;
            this.comboBoxFill.FormattingEnabled = true;
            this.comboBoxFill.Items.AddRange(new object[] {"Закрасить внутри", "Закрасить снаружи"});
            this.comboBoxFill.Location = new System.Drawing.Point(274, 50);
            this.comboBoxFill.Margin = new System.Windows.Forms.Padding(5);
            this.comboBoxFill.Name = "comboBoxFill";
            this.comboBoxFill.Size = new System.Drawing.Size(264, 21);
            this.comboBoxFill.TabIndex = 4;
            this.comboBoxFill.Text = "Тип заливки";
            this.comboBoxFill.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectPainting);
            // 
            // comboBox3
            // 
            this.comboBoxSelectFigure.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxSelectFigure.FormattingEnabled = true;
            this.comboBoxSelectFigure.Items.AddRange(new object[] {"Треугольник", "Флаг", "Квадрат", "Уголок1", "Уголок2", "Звезда"});
            this.comboBoxSelectFigure.Location = new System.Drawing.Point(829, 50);
            this.comboBoxSelectFigure.Name = "comboBoxSelectFigure";
            this.comboBoxSelectFigure.Size = new System.Drawing.Size(150, 21);
            this.comboBoxSelectFigure.TabIndex = 5;
            this.comboBoxSelectFigure.Text = "Фигуры";
            this.comboBoxSelectFigure.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectFigure);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.comboBoxOperation);
            this.panel1.Controls.Add(this.comboBoxVertCount);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.comboBoxSelectTmo);
            this.panel1.Controls.Add(this.comboBoxSelectFigure);
            this.panel1.Controls.Add(this.comboBoxFill);
            this.panel1.Controls.Add(this.comboBoxColor);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Location = new System.Drawing.Point(23, 634);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1395, 177);
            this.panel1.TabIndex = 6;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(603, 106);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(157, 17);
            this.checkBox2.TabIndex = 13;
            this.checkBox2.Text = "Применить Кривую безье";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBoxBeziers);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(346, 18);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 11;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // comboBox6
            // 
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Items.AddRange(new object[] {"Закрашивание", "Перемещение", "Вращение", "Вращение относительно заданного центра", "Масштабирование", "Масштабирование по ОУ", "Масштабирование относит. заданного центра", "Отражение OX", "Отражение OY", "Отражение нач. кординат"});
            this.comboBoxOperation.Location = new System.Drawing.Point(603, 18);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(175, 21);
            this.comboBoxOperation.TabIndex = 9;
            this.comboBoxOperation.Text = "Операции с многоугольником";
            this.comboBoxOperation.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectOperation);
            // 
            // comboBox5
            // 
            this.comboBoxVertCount.FormattingEnabled = true;
            this.comboBoxVertCount.Items.AddRange(new object[] {"5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"});
            this.comboBoxVertCount.Location = new System.Drawing.Point(846, 81);
            this.comboBoxVertCount.Name = "comboBoxVertCount";
            this.comboBoxVertCount.Size = new System.Drawing.Size(121, 21);
            this.comboBoxVertCount.TabIndex = 8;
            this.comboBoxVertCount.Text = "Кол-во n углов";
            this.comboBoxVertCount.SelectedIndexChanged += new System.EventHandler(this.ComboBox_VertexCount);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.Location = new System.Drawing.Point(615, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(147, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Применить ТМО";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button_RunTMO);
            // 
            // comboBox4
            // 
            this.comboBoxSelectTmo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxSelectTmo.FormattingEnabled = true;
            this.comboBoxSelectTmo.Items.AddRange(new object[] {"Объединение", "Пересечение", "Симметрическая разность", "Разность А\\В", "Разность В\\А"});
            this.comboBoxSelectTmo.Location = new System.Drawing.Point(603, 50);
            this.comboBoxSelectTmo.Name = "comboBoxSelectTmo";
            this.comboBoxSelectTmo.Size = new System.Drawing.Size(175, 21);
            this.comboBoxSelectTmo.TabIndex = 6;
            this.comboBoxSelectTmo.Text = "ТМО";
            this.comboBoxSelectTmo.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectTMO);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1441, 814);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxMain);
            this.MinimumSize = new System.Drawing.Size(876, 589);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Лабараторная 3";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Click += new System.EventHandler(this.Form1_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureMouseDown);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBoxMain)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private System.Windows.Forms.ComboBox comboBoxColor;
        private System.Windows.Forms.ComboBox comboBoxFill;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBoxSelectTmo;
        private System.Windows.Forms.ComboBox comboBoxSelectFigure;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBoxVertCount;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}

