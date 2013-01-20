using System;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Features {
	public class Opening : Component {
		public enum OpeningStatus {
			Opened,
			Closed
		}
		public int APCost { get; set; }
		private OpeningStatus status;
		public OpeningStatus Status {
			get { return status; }
			set {
				status = value;
				OnUsed(new EventArgs<OpeningStatus>(status));
			}
		}

		public string OpenedAsset { get; private set; }
		public string ClosedAsset { get; private set; }

		public string OpenedDescription { get; private set; }
		public string ClosedDescription { get; private set; }

		public bool WalkableOpened { get; private set; }

		public event ComponentEventHandler<EventArgs<OpeningStatus>> Used;

		public const int DEFAULT_DOOR_USE_APCOST = World.DEFAULT_SPEED / World.TURN_LENGTH_IN_SECONDS;

		public void OnUsed(EventArgs<OpeningStatus> e) {
			ComponentEventHandler<EventArgs<OpeningStatus>> handler = Used;
			if (handler != null)
				handler(this, e);
		}

		public Opening(string openedAsset, string closedAsset, string openDescription, string closedDescription, bool walkableWhenOpened = true, int apCost = DEFAULT_DOOR_USE_APCOST, OpeningStatus status = OpeningStatus.Closed) {
			this.status = status;
			APCost = apCost;
			OpenedAsset = openedAsset;
			ClosedAsset = closedAsset;
			WalkableOpened = walkableWhenOpened;
			OpenedDescription = openDescription;
			ClosedDescription = closedDescription;
		}		

		public override Component Copy() {
			var opening = new Opening(OpenedAsset, ClosedAsset, OpenedDescription, ClosedDescription, WalkableOpened, APCost, Status);
			if (Used != null)
				opening.Used = (ComponentEventHandler<EventArgs<OpeningStatus>>) Used.Clone();
			return opening;
		}

		public static ActionResult Action(Entity user, Entity entry) {
			Contract.Requires<ArgumentException>(entry.Has<Opening>());
			var d = entry.Get<Opening>();
			
			if (d.Status == OpeningStatus.Closed) {
				if (entry.Has<Blocker>())
					entry.Get<Blocker>().Transparent = true;
					entry.Get<Blocker>().Walkable = d.WalkableOpened;
				if (entry.Has<Sprite>())
					entry.Get<Sprite>().Asset = d.OpenedAsset;

				d.Status = OpeningStatus.Opened;
				user.Get<ActionPoint>().ActionPoints -= d.APCost;

				World.Instance.AddMessage(String.Format("{0} {1}.", Identifier.GetNameOrId(user), d.OpenedDescription));
				return ActionResult.Success;
			} else if (d.Status == OpeningStatus.Opened) {
				if (entry.Get<Location>().Level.IsWalkable(entry.Get<Location>().Position)) {
					if (entry.Has<Blocker>())
						entry.Get<Blocker>().Transparent = entry.Get<Blocker>().Walkable = false;
					if (entry.Has<Sprite>())
						entry.Get<Sprite>().Asset = d.ClosedAsset;

					d.Status = OpeningStatus.Closed;
					user.Get<ActionPoint>().ActionPoints -= d.APCost;

					World.Instance.AddMessage(String.Format("{0} {1}.", Identifier.GetNameOrId(user), d.ClosedDescription));
					return ActionResult.Success;
				} else {
					user.Get<ActionPoint>().ActionPoints -= d.APCost;
					World.Instance.AddMessage(String.Format("{0} tries to {1}, but can't.", Identifier.GetNameOrId(user), d.ClosedDescription));
					return ActionResult.Failed;					
				}				
			}
			return ActionResult.Aborted;
		}
	}
}