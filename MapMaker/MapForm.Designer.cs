namespace MapMaker {
	partial class MapForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.button1 = new System.Windows.Forms.Button();
			this.mapView1 = new MapMaker.MapView();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(611, 562);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Make";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// mapView1
			// 
			this.mapView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapView1.Font = new System.Drawing.Font("Consolas", 8F);
			this.mapView1.Level = null;
			this.mapView1.Location = new System.Drawing.Point(0, 0);
			this.mapView1.Name = "mapView1";
			this.mapView1.Size = new System.Drawing.Size(698, 597);
			this.mapView1.TabIndex = 0;
			// 
			// MapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(698, 597);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.mapView1);
			this.Name = "MapForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private MapView mapView1;
		private System.Windows.Forms.Button button1;
	}
}

