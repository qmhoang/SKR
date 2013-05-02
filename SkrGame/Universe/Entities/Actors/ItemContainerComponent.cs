using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Actions;
using SkrGame.Systems;
using SkrGame.Universe.Entities.Items;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public sealed class ItemContainerComponent : Component {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Entity> _itemContainer;

		public event ComponentEventHandler<ItemContainerComponent, EventArgs<Entity>> ItemRemoved;
		public event ComponentEventHandler<ItemContainerComponent, EventArgs<Entity>> ItemAdded;

		public void OnItemAdded(EventArgs<Entity> e) {
			var handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Entity> e) {
			var handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}

		public ItemContainerComponent() {
			_itemContainer = new List<Entity>();	
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(_itemContainer != null);			
		}

		[Pure]
		public bool Contains(Entity item) {
			return _itemContainer.Contains(item);
		}
		
		public int Count {
			get { return _itemContainer.Count; }
		}

		/// <summary>
		/// Total number of items (stacked items aren't counted as 1 item)
		/// </summary>
		public int TotalCount {
			get {
				return _itemContainer.Sum(i => i.Has<Item>() ? i.Get<Item>().Amount : 1);
			}
		}
		
		public IEnumerable<Entity> Items {
			get { return _itemContainer; }
		}

		public bool Exist(Predicate<Entity> match) {
			Contract.Requires<ArgumentNullException>(match != null, "match");
			return _itemContainer.Exists(match);
		}

		/// <summary>
		/// Get item that matches preditcate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public Entity GetItem(Predicate<Entity> match) {
			Contract.Requires<ArgumentNullException>(match != null, "match");
			return _itemContainer.Find(match);
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
				_itemContainer.Add(item);
				OnItemAdded(new EventArgs<Entity>(item));

				return true;
			}
		}
		
		public bool Remove(Entity item) {
			if (item == null)
				return false;
			if (!_itemContainer.Contains(item))
				return false;

			Logger.DebugFormat("{0} is removing {1} to his inventory.", OwnerUId, Identifier.GetNameOrId(item));

			OnItemRemoved(new EventArgs<Entity>(item));
			_itemContainer.Remove(item);

			return true;
		}

		public override Component Copy() {
			var container = new ItemContainerComponent();

			foreach (var entity in _itemContainer) {
				container._itemContainer.Add(entity.Copy());
			}

			if (ItemAdded != null)
				container.ItemAdded = (ComponentEventHandler<ItemContainerComponent, EventArgs<Entity>>) ItemAdded.Clone();
			if (ItemRemoved != null)
				container.ItemRemoved = (ComponentEventHandler<ItemContainerComponent, EventArgs<Entity>>) ItemRemoved.Clone();

			
			return container;
		}
	}
}