using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SKR.Universe.Entities.Items;

namespace SKR.Universe.Entities.Actor {
    public class ItemContainer : IEnumerable<Item> {
        protected List<Item> items;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ItemContainer() {
            items = new List<Item>();
        }

        public int Weight {
            get { return items.Sum(i => i.Weight); }
        }

        public int Count {
            get { return items.Count; }
        }


        public void AddItemsFromContainer(ItemContainer container) {
            foreach (var item in container.items)
                AddItem(item);
        }


        public bool Exist(Predicate<Item> match) {
            return items.Exists(match);
        }

        public Item GetItem(Predicate<Item> match) {
            return items.Find(match);
        }

        /// <summary>
        /// Add item to container, if item is stackable, add a single stacked item (not the entire stack)
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item) {
            if (items.Contains(item))
                throw new ArgumentException(String.Format("Item (Guid: {0}) already exist in in the container.", item.Guid));

            Log.DebugFormat("Adding item: {0}, Guid: {1}", item.Name, item.Guid);

            items.Add(item);
        }

        public void RemoveItem(Item item) {
            if (!items.Contains(item))
                throw new ArgumentException(String.Format("Item (Guid: {0}) does not exist in in the container.", item.Guid));
            Log.DebugFormat("Removing item: {1} Guid: {0}", item.Guid, item.Name);

            items.Remove(item);
        }

        public IEnumerator<Item> GetEnumerator() {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }
}