namespace SkrGame.Universe.Entities.Items {
    public abstract class ItemComponentTemplate {
        public string ComponentId { get; set; }
//        public ItemAction Action { get; set; }

        public string ActionDescription { get; set; }
        public string ActionDescriptionPlural { get; set; }

        public Item Item { get; set; }
    }

    /// <summary>
    /// Item component represents an item's functions (armor, melee, gun, food, etc)
    /// </summary>
    public abstract class ItemComponent {
        public string ComponentId { get; protected set; }
//        public Type Action { get; protected set; }

        public Item Item { get; set; }

        public string ActionDescription { get; protected set; }
        public string ActionDescriptionPlural { get; protected set; }

        protected ItemComponent(string componentId, string actionDescription, string actionDescriptionPlural) {
            ComponentId = componentId;
//            Action = action;
            ActionDescription = actionDescription;
            ActionDescriptionPlural = actionDescriptionPlural;
        }
    }
}