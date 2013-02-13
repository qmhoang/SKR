using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Components.Actions;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class ContainerComponent : Component, IPositionChanged {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Entity> itemContainer;

		public event ComponentEventHandler<EventArgs<Entity>> ItemRemoved;
		public event ComponentEventHandler<EventArgs<Entity>> ItemAdded;

		public void OnItemAdded(EventArgs<Entity> e) {
			ComponentEventHandler<EventArgs<Entity>> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Entity> e) {
			ComponentEventHandler<EventArgs<Entity>> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}

		public ContainerComponent() {
			itemContainer = new List<Entity>();			
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(itemContainer != null);			
		}

		public bool Contains(Entity item) {
			return itemContainer.Contains(item);
		}
		
		public int Count {
			get { return itemContainer.Count; }
		}

		/// <summary>
		/// Total number of items (stacked items aren't counted as 1 item)
		/// </summary>
		public int TotalCount {
			get {
				return itemContainer.Sum(i => i.Has<Item>() ? i.Get<Item>().Amount : 1);
			}
		}
		
		public IEnumerable<Entity> Items {
			get { return itemContainer; }
		}

		public bool Exist(Predicate<Entity> match) {
			Contract.Requires<ArgumentNullException>(match != null, "match");
			return itemContainer.Exists(match);
		}

		/// <summary>
		/// Get item that matches preditcate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public Entity GetItem(Predicate<Entity> match) {
			Contract.Requires<ArgumentNullException>(match != null, "match");
			return itemContainer.Find(match);
		}

		/// <summary>
		/// Add item into inventory
		/// </summary>
		public bool Add(Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);

			if (Contains(item))
				return false;

			Logger.DebugFormat("{0} is adding {1} to his inventory.", OwnerUId, Identifier.GetNameOrId(item));

			if (item.Has<Item>() &&
				item.Get<Item>().StackType == StackType.Hard &&
				Exist(e => e.Get<ReferenceId>() == item.Get<ReferenceId>())) {

				Logger.DebugFormat("{0} is stackable and exist in container, merging.", Identifier.GetNameOrId(item));

				var existing = GetItem(e => e.Get<ReferenceId>() == item.Get<ReferenceId>());
				existing.Get<Item>().Amount += item.Get<Item>().Amount;
				item.IsActive = false;				

				return true;
			} else {
				itemContainer.Add(item);
				OnItemAdded(new EventArgs<Entity>(item));

				// make the item we just added invisible
				if (item.Has<VisibleComponent>())
					item.Get<VisibleComponent>().VisibilityIndex = -1;

				// just in case, move the item to the entity's location
				if (item.Has<Location>())
					item.Get<Location>().Point = Entity.Get<Location>().Point;

				return true;
			}
		}
		
		public bool Remove(Entity item) {
			if (item == null)
				return false;
			if (!itemContainer.Contains(item))
				return false;

			Logger.DebugFormat("{0} is removing {1} to his inventory.", OwnerUId, Identifier.GetNameOrId(item));

			OnItemRemoved(new EventArgs<Entity>(item));
			itemContainer.Remove(item);

			if (item.Has<VisibleComponent>()) {
				item.Get<VisibleComponent>().Reset();
			}

			return true;
		}

		public override Component Copy() {
			var container = new ContainerComponent();

			foreach (var entity in itemContainer) {
				container.itemContainer.Add(entity.Copy());
			}
			
			if (ItemAdded != null)
				container.ItemAdded = (ComponentEventHandler<EventArgs<Entity>>)ItemAdded.Clone();
			if (ItemRemoved != null)
				container.ItemRemoved = (ComponentEventHandler<EventArgs<Entity>>)ItemRemoved.Clone();

			
			return container;
		}

		public void Move(Point prev, Point curr) {
			foreach (var item in Items) {
				if (item.Has<Location>())
					item.Get<Location>().Point = curr;
			}
		}
	}
}