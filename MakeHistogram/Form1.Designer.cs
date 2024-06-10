namespace MakeHistogram
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
            this.components = new System.ComponentModel.Container();
            this.zG_R = new ZedGraph.ZedGraphControl();
            this.zG_G = new ZedGraph.ZedGraphControl();
            this.zG_B = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zG_R
            // 
            this.zG_R.Location = new System.Drawing.Point(2, 1);
            this.zG_R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zG_R.Name = "zG_R";
            this.zG_R.ScrollGrace = 0D;
            this.zG_R.ScrollMaxX = 0D;
            this.zG_R.ScrollMaxY = 0D;
            this.zG_R.ScrollMaxY2 = 0D;
            this.zG_R.ScrollMinX = 0D;
            this.zG_R.ScrollMinY = 0D;
            this.zG_R.ScrollMinY2 = 0D;
            this.zG_R.Size = new System.Drawing.Size(368, 356);
            this.zG_R.TabIndex = 0;
            this.zG_R.UseExtendedPrintDialog = true;
            // 
            // zG_G
            // 
            this.zG_G.Location = new System.Drawing.Point(369, 1);
            this.zG_G.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zG_G.Name = "zG_G";
            this.zG_G.ScrollGrace = 0D;
            this.zG_G.ScrollMaxX = 0D;
            this.zG_G.ScrollMaxY = 0D;
            this.zG_G.ScrollMaxY2 = 0D;
            this.zG_G.ScrollMinX = 0D;
            this.zG_G.ScrollMinY = 0D;
            this.zG_G.ScrollMinY2 = 0D;
            this.zG_G.Size = new System.Drawing.Size(368, 356);
            this.zG_G.TabIndex = 1;
            this.zG_G.UseExtendedPrintDialog = true;
            // 
            // zG_B
            // 
            this.zG_B.Location = new System.Drawing.Point(736, 1);
            this.zG_B.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.zG_B.Name = "zG_B";
            this.zG_B.ScrollGrace = 0D;
            this.zG_B.ScrollMaxX = 0D;
            this.zG_B.ScrollMaxY = 0D;
            this.zG_B.ScrollMaxY2 = 0D;
            this.zG_B.ScrollMinX = 0D;
            this.zG_B.ScrollMinY = 0D;
            this.zG_B.ScrollMinY2 = 0D;
            this.zG_B.Size = new System.Drawing.Size(368, 356);
            this.zG_B.TabIndex = 2;
            this.zG_B.UseExtendedPrintDialog = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 363);
            this.Controls.Add(this.zG_B);
            this.Controls.Add(this.zG_G);
            this.Controls.Add(this.zG_R);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zG_R;
        private ZedGraph.ZedGraphControl zG_G;
        private ZedGraph.ZedGraphControl zG_B;
    }
}

