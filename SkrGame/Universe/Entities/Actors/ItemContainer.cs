using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Entities.Actors {
    public class ItemContainer : IEnumerable<Item> {
        protected List<Item> Items;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ItemContainer() {
            Items = new List<Item>();
        }

        public int Weight {
            get { return Items.Sum(i => i.Weight); }
        }

        public int Count {
            get { return Items.Count; }
        }

        public void AddItemsFromContainer(ItemContainer container) {
            foreach (var item in container.Items)
                AddItem(item);
        }


        public bool Exist(Predicate<Item> match) {
            return Items.Exists(match);
        }

        public Item GetItem(Predicate<Item> match) {
            return Items.Find(match);
        }

        /// <summary>
        /// Add item to container, if item is stackable, add a single stacked item (not the entire stack)
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item) {
            if (Items.Contains(item))
                throw new ArgumentException(String.Format("Item (Guid: {0}) already exist in in the container.", item.UniqueId));

            Log.DebugFormat("Adding item: {0}, Guid: {1}", item.Name, item.UniqueId);

            Items.Add(item);
        }

        public void RemoveItem(Item item) {
            if (!Items.Contains(item))
                throw new ArgumentException(String.Format("Item (Guid: {0}) does not exist in in the container.", item.UniqueId));
            Log.DebugFormat("Removing item: {1} Guid: {0}", item.UniqueId, item.Name);

            Items.Remove(item);
        }

        public IEnumerator<Item> GetEnumerator() {            
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }
}