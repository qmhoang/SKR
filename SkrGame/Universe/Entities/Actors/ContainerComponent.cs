using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class ContainerComponent : Component, ICollection<Entity> {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Entity> itemContainer;

		public event EventHandler<EventArgs<Entity>> ItemRemoved;
		public event EventHandler<EventArgs<Entity>> ItemAdded;


		public void OnItemAdded(EventArgs<Entity> e) {
			EventHandler<EventArgs<Entity>> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Entity> e) {
			EventHandler<EventArgs<Entity>> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}

		internal ContainerComponent() {
			itemContainer = new List<Entity>();
		}

		public IEnumerator<Entity> GetEnumerator() {
			return itemContainer.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		
		void ICollection<Entity>.Clear() {
			foreach (var entity in itemContainer) {
				Remove(entity);
			}
			itemContainer.Clear();
		}

		public bool Contains(Entity item) {
			return itemContainer.Contains(item);
		}

		public void CopyTo(Entity[] array, int arrayIndex) {
			itemContainer.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return itemContainer.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		
		public IEnumerable<Entity> Items {
			get { return itemContainer; }
		}

		public bool Exist(Predicate<Entity> match) {
			return itemContainer.Exists(match);
		}

		/// <summary>
		/// Get item that matches preditcate
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public Entity GetItem(Predicate<Entity> match) {
			return itemContainer.Find(match);
		}

		void ICollection<Entity>.Add(Entity item) {
			Add(item);
		}

		/// <summary>
		/// Add item into inventory
		/// </summary>
		public bool Add(Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);

			if (Contains(item))
				return false;

			Logger.DebugFormat("{0} is adding {1} to his inventory.", OwnerUId, item.Id);						

			itemContainer.Add(item);
			OnItemAdded(new EventArgs<Entity>(item));

			return true;
		}

		public bool Remove(Entity item) {
			Contract.Requires<ArgumentNullException>(item != null);

			if (!itemContainer.Contains(item))
				return false;

			Logger.DebugFormat("{0} is removing {1} to his inventory.", OwnerUId, item.Id);

			OnItemRemoved(new EventArgs<Entity>(item));
			itemContainer.Remove(item);
			
			return true;
		}
		
		public override Component Copy() {
			var container = new ContainerComponent();

			foreach (var entity in itemContainer) {
				container.itemContainer.Add(entity.Copy());
			}
			
			if (ItemAdded != null)
				container.ItemAdded = (EventHandler<EventArgs<Entity>>)ItemAdded.Clone();
			if (ItemRemoved != null)
				container.ItemRemoved = (EventHandler<EventArgs<Entity>>)ItemRemoved.Clone();

			
			return container;
		}
	}
}