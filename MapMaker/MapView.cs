using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkrGame.Universe.Locations;

namespace MapMaker {
	public sealed partial class MapView : UserControl {
		private CityMap level;
		public CityMap Level {
			get { return level; }
			set {
				level = value;
				Invalidate();
			}
		}

		public MapView() {
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			Font = new Font("Consolas", 8.0f);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if (Level != null) {
				for (int i = 0; i < Level.Data.GetLength(0); i++) {
					for (int j = 0; j < Level.Data.GetLength(1); j++) {
						PrintTile(i, j, level.Data[i, j], e.Graphics);
					}
				}
			}


		}

		private void PrintTile(int x, int y, CityTile tile, Graphics g) {
			switch (tile) {
				case CityTile.None:
					break;
				case CityTile.Ground:
					Draw(' ', x, y, Brushes.White, Brushes.Brown, g);
					break;
				case CityTile.Water:
					Draw(' ', x, y, Brushes.White, Brushes.Blue, g);
					break;
				case CityTile.Road:
					Draw(' ', x, y, Brushes.White, Brushes.SlateGray, g);
					break;
				case CityTile.Commercial:
					Draw(' ', x, y, Brushes.White, Brushes.DodgerBlue, g);
					break;
				case CityTile.Residential:
					Draw(' ', x, y, Brushes.White, Brushes.Green, g);
					break;
				case CityTile.Industrial:
					Draw(' ', x, y, Brushes.White, Brushes.Red, g);
					break;
				default:
					throw new ArgumentOutOfRangeException("tile");
			}
		}

		private void Draw(char c, int x, int y, Brush fg, Brush bg, Graphics g) {
			g.DrawRectangle(line, x * TileLength, y * TileLength, TileLength, TileLength);
			g.FillRectangle(bg, x * TileLength + 1, y * TileLength + 1, TileLength - 1, TileLength - 1);
			g.DrawString(c.ToString(), font, fg, x * TileLength, y * TileLength);
		}

		private static Pen line = new Pen(Color.FromArgb(32, 0, 0, 0));
		private static Font font = new Font("Consolas", 8.0f);
		private static int TileLength = 10;
	}
}
