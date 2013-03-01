using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Actions.Features;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Useables;
using SkrGame.Universe.Locations;

namespace SkrGame.Universe.Entities.Features {
	public sealed class Opening : Component, IUseable {
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

		public const int DefaultDoorUseAPCost = World.OneSecondInAP;

		public void OnUsed(EventArgs<OpeningStatus> e) {
			Contract.Requires<ArgumentNullException>(e != null, "e");
			ComponentEventHandler<EventArgs<OpeningStatus>> handler = Used;
			if (handler != null)
				handler(this, e);
		}

		public Opening(string openedAsset, string closedAsset, string openDescription, string closedDescription, bool walkableWhenOpened = true, int apCost = DefaultDoorUseAPCost, OpeningStatus status = OpeningStatus.Closed) {
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(closedAsset));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(openedAsset));

			this.status = status;
			APCost = apCost;
			OpenedAsset = openedAsset;
			ClosedAsset = closedAsset;
			WalkableOpened = walkableWhenOpened;
			OpenedDescription = openDescription;
			ClosedDescription = closedDescription;			
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(!String.IsNullOrEmpty(OpenedAsset));
			Contract.Invariant(!String.IsNullOrEmpty(ClosedAsset));
		}

		public override Component Copy() {
			var opening = new Opening(OpenedAsset, ClosedAsset, OpenedDescription, ClosedDescription, WalkableOpened, APCost, Status);
			if (Used != null)
				opening.Used = (ComponentEventHandler<EventArgs<OpeningStatus>>) Used.Clone();
			return opening;
		}

		private static readonly UseAction Open =
				new UseAction("Open door", (user, featureEntity, action) =>
				                           	{
				                           		if (featureEntity.Has<Opening>())
				                           			user.Get<ActorComponent>().Enqueue(new OpenDoorAction(user, featureEntity));
				                           		return ActionResult.Aborted;
				                           	});

		private static readonly UseAction Close =
				new UseAction("Close door", (user, featureEntity, action) =>
				                            	{
				                            		if (featureEntity.Has<Opening>())
				                            			user.Get<ActorComponent>().Enqueue(new CloseDoorAction(user, featureEntity));
				                            		return ActionResult.Aborted;
				                            	});

		public IEnumerable<UseAction> Uses {
			get {
				if (Status == OpeningStatus.Closed) {
					yield return Open;
				} else {
					yield return Close;
				}
			}
		}
	}
}