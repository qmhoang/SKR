namespace SkrGame.Universe.Entities.Items.Components {
	public interface IItemComponentTemplate {
		string ComponentId { get; set; }

		string ActionDescription { get; set; }
		string ActionDescriptionPlural { get; set; }

		IItemComponent Construct(Item item);
	}

	/// <summary>
	/// Item component represents an item's functions (armor, melee, gun, food, etc)
	/// </summary>
	public interface IItemComponent {
		string ComponentId { get; }

		Item Item { get; }

		string ActionDescription { get; }
		string ActionDescriptionPlural { get; }
	}
}