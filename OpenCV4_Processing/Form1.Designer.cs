﻿
namespace OpenCV4_Processing
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.pBox_Test = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_Test)).BeginInit();
            this.SuspendLayout();
            // 
            // pBox_Test
            // 
            this.pBox_Test.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBox_Test.Location = new System.Drawing.Point(0, 0);
            this.pBox_Test.Margin = new System.Windows.Forms.Padding(2);
            this.pBox_Test.Name = "pBox_Test";
            this.pBox_Test.Size = new System.Drawing.Size(1021, 881);
            this.pBox_Test.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBox_Test.TabIndex = 0;
            this.pBox_Test.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 881);
            this.Controls.Add(this.pBox_Test);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pBox_Test)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pBox_Test;
    }
}

