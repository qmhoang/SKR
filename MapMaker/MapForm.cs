using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapMaker {
	public partial class MapForm : Form {
		public MapForm() {
			InitializeComponent();
			this.mapView1.Level = new CityMap(new DEngine.Core.Size(100, 100), CityTile.Ground);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
//			mapView1.Size = new Size(Size.Width - 10, Size.Height - 10);
			mapView1.Refresh();
		}
	}
}
