using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Core;
using DEngine.Level;

namespace MapMaker {
	public enum CityTile {
		None,
		Ground,
		Water,
		Road,
		Commercial,
		Residential,
		Industrial
	}
	public class CityMap : Map2D {
		public CityTile[,] Data { get; set; }

		public CityMap(Size size) : base(size) {
			Data = new CityTile[size.Width, size.Height];
		}

		public CityMap(Size size, CityTile fill)
			: base(size) {
			Data = new CityTile[size.Width, size.Height];

			for (int i = 0; i < Data.GetLength(0); i++) {
				for (int j = 0; j < Data.GetLength(1); j++) {
					Data[i, j] = fill;
				}
			}
		}


	}
}
