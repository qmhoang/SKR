using System;
using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
//	public class HouseItemFeature : Component {
//		public int APCostPerAction { get; set; }
//		
//		public override Component Copy() {
//			throw new NotImplementedException();
//		}
//	}
	public enum LockStatus {
		Locked,
		Opened,
		Destroyed,

	}
	public class LockFeature : Component {
		public int Quality { get; set; }
		public LockStatus Status { get; set; }

		public override Component Copy() {
			throw new NotImplementedException();
		}
	}
}