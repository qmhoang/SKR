using DEngine.Entities;

namespace SkrGame.Universe.Entities.Features {
	public enum LockStatus {
		Locked,
		Opened,
		Destroyed,
	}

	public class LockedFeature : Component {
		public int Quality { get; set; }
		public LockStatus Status { get; set; }

		public LockedFeature(int quality, LockStatus status = LockStatus.Locked) {
			Quality = quality;
			Status = status;
		}

		public override Component Copy() {
			return new LockedFeature(Quality, Status);
		}
	}
}