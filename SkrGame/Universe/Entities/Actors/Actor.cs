using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Location;
using log4net;


namespace SkrGame.Universe.Entities.Actors {
	public enum Condition {
		Encrumbrance, // 0 - none, 1 - light, 2 - medium, etc
	}

	public class ActorCondition {
		private Dictionary<Condition, int> myStatus;
		private Actor actor;

		public ActorCondition(Actor actor) {
			this.actor = actor;
			myStatus = new Dictionary<Condition, int>();
		}

		/// <summary>
		/// Gets status condition of person, return int value of condition, -1 if player doesn't have condition
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public int GetConditionStatus(Condition condition) {
			return myStatus.ContainsKey(condition) ? myStatus[condition] : -1;
		}

		public void SetConditionStatus(Condition condition, int status) {
			if (myStatus.ContainsKey(condition))
				myStatus[condition] = status;
			else
				myStatus.Add(condition, status);
		}
	}

	public class TagChangedEvent : EventArgs {
		public string TagId { get; private set; }
		public int ValueChanged { get; private set; }
		public int NewValue { get; private set; }

		public TagChangedEvent(string tagId, int valueChanged, int newValue) {
			TagId = tagId;
			ValueChanged = valueChanged;
			NewValue = newValue;
		}
	}



	public abstract class Actor : Entity {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public string FullId {
			get { return string.Format("RefId: {0}, Uid: {1}", RefId, Uid); }
		}

		public ActorBody Body { get; private set; }
		public ActorTalents Talents { get; private set; }

		public event EventHandler<EventArgs> HealthChanged;

		protected void OnHealthChange() {
			var handler = HealthChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private readonly Dictionary<BodySlot, Item> equippedItems;
		private readonly ActorCondition conditionStatuses;
		private readonly Dictionary<string, int> tags;

		public event EventHandler<TagChangedEvent> TagChanged;

		public void OnTagChanged(TagChangedEvent e) {
			EventHandler<TagChangedEvent> handler = TagChanged;
			if (handler != null)
				handler(this, e);
		}

		public int GetTag(string id) {
			return tags.ContainsKey(id) ? tags[id] : 0;
		}

		public void SetTag(string id, int value) {
			OnTagChanged(new TagChangedEvent(id, value - GetTag(id), value));
			tags[id] = value;
		}

		public void IncrementTag(string id, int increment = 1) {
			if (tags.ContainsKey(id)) {
				tags[id] += increment;
				OnTagChanged(new TagChangedEvent(id, increment, GetTag(id)));
			} else
				SetTag(id, increment);
		}

		public override int ActionPoints {
			get { return base.ActionPoints; }
			set {
				RecalculateFov = true;
				base.ActionPoints = value;
			}
		}

		public Level Level { get; private set; }

		public World World {
			get { return World.Instance; }
		}

		private readonly ItemContainer inventory;
		private int additionalWeight;

		public int Dodge {
			get { return Talents.GetTalent("attrb_agility").As<AttributeComponent>().Rank + Talents.GetTalent("attrb_cunning").As<AttributeComponent>().Rank; }
		}

		public int Lift {
			get {
				return
						(int)
						(Talents.GetTalent("attrb_strength").As<AttributeComponent>().Rank * Talents.GetTalent("attrb_strength").As<AttributeComponent>().Rank * 18 * Math.Pow(World.STANDARD_DEVIATION, -2.0) +
						 additionalWeight);
			}
		}

		public override bool Dead {
			get { return Body.Health < 0; }
		}

		public override void OnDeath() {
			Level.RemoveActor(this);
			World.RemoveEntity(this);

			World.AddMessage(String.Format("{0} has died.", Name));
		}

		protected Actor(string name, string refId, string asset, Level level) {
			Level = level;
			RefId = refId;

			Name = name;

			inventory = new ItemContainer();
			tags = new Dictionary<string, int>();


			Asset = asset;

			Body = new ActorBody(this);
			Talents = new ActorTalents(this);

			conditionStatuses = new ActorCondition(this);
			equippedItems = new Dictionary<BodySlot, Item>();

			RecalculateFov = true;
		}

		private void CheckEncumbrance() {
			var weight = inventory.Weight + equippedItems.Values.Sum(i => i.Weight);
			if (weight <= Lift)
				SetConditionStatus(Condition.Encrumbrance, 0);
			else if (weight > Lift)
				SetConditionStatus(Condition.Encrumbrance, 1);
			else if (weight > 2 * Lift)
				SetConditionStatus(Condition.Encrumbrance, 2);
			else if (weight > 4 * Lift)
				SetConditionStatus(Condition.Encrumbrance, 3);
			else if (weight > 6 * Lift)
				SetConditionStatus(Condition.Encrumbrance, 4);
		}

		public void CalculateFov() {
			Level.CalculateFOV(Position, SightRadius);
			RecalculateFov = false;
		}

		public override void Update() {
			//            CheckEncumbrance();
		}

		protected bool RecalculateFov;

		public override bool HasLineOfSight(Point position) {
			if (RecalculateFov)
				CalculateFov();
			return Level.IsVisible(position);
		}

		public override bool CanSpot(Entity actor) {
			throw new NotImplementedException();
			return HasLineOfSight(actor.Position);
		}

		#region Move

		public override ActionResult Move(int dx, int dy) {
			return Move(new Point(dx, dy));
		}

		public override ActionResult Move(Point p) {
			Point nPos = p + Position;

			if (!Level.IsWalkable(nPos)) {
				World.AddMessage("There is something in the way");
				return ActionResult.Aborted;
			}

			if (!Level.IsInBoundsOrBorder(nPos))
				return ActionResult.Aborted;

			if (Level.DoesActorExistAtLocation(nPos))
				Talents.MeleeAttack().As<ActiveTalentComponent>().InvokeAction(nPos);
			else {
				Position = nPos;

				foreach (var feature in Level.Features)
					feature.Near(this);
			}

			ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);

			RecalculateFov = true;
			return ActionResult.Success;
		}

		public override ActionResult Wait() {
			ActionPoints -= World.SpeedToActionPoints(World.DEFAULT_SPEED);
			return ActionResult.Success;
		}

		#endregion

		#region Life

		public event EventHandler<EventArgs<Condition, int>> ConditionChanged;

		public void OnConditionChanged(EventArgs<Condition, int> e) {
			EventHandler<EventArgs<Condition, int>> handler = ConditionChanged;
			if (handler != null)
				handler(this, e);
		}

		public int GetConditionStatus(Condition condition) {
			return conditionStatuses.GetConditionStatus(condition);
		}

		public void SetConditionStatus(Condition condition, int status) {
			Logger.InfoFormat("{0}'s {1} is now set at {2}", Name, condition, status);
			OnConditionChanged(new EventArgs<Condition, int>(condition, status));
			conditionStatuses.SetConditionStatus(condition, status);
		}

		public void Damage(int damage, DamageType type, BodyPart bodyPart, out int damageResistance, out int damageDealt) {
			damageDealt = damage;
			damageResistance = 0;
			if (IsItemEquipped(bodyPart.Type)) {
				var itemAtSlot = GetItemAtBodyPart(bodyPart.Type);

				if (itemAtSlot.Is(typeof (ArmorComponent))) {
					var armor = itemAtSlot.As<ArmorComponent>();
					damageResistance = armor.Resistances[type];
					damageDealt = Math.Max(damage - damageResistance, 0);
					if (Rng.Chance(armor.Coverage / 100.0))
						Logger.InfoFormat("Damage: {3} reduced to {0} because of {1} [DR: {2}]", damageDealt, itemAtSlot.Name, damageResistance, damage);
					else {
						// we hit a chink in the armor
						damageResistance /= (int) armor.NonCoverageDivisor;
						Logger.InfoFormat("Damage: {3} reduced to {0} because of {1} [DR reduced because of non-coverage: {2}]", damageDealt, itemAtSlot.Name, damageResistance, damage);
					}
				}
			}

			if (damageDealt > bodyPart.MaxHealth) {
				damageDealt = Math.Min(damage, bodyPart.MaxHealth);
				Logger.InfoFormat("Damage: {2} reduced to {0} because of {1}'s max health", damageDealt, bodyPart.Name, damage);
			}

			bodyPart.Health -= damageDealt;
			Body.Health -= damageDealt;

			Logger.InfoFormat("{0}'s {1} was hurt ({2} damage)", Name, bodyPart.Name, damageDealt);
			OnHealthChange();
		}

		public void Heal(int amount) {
			amount = Math.Min(amount, Body.MaxHealth - Body.Health);
			Body.Health -= amount;
			Logger.InfoFormat("{0} was healed {1} health", Name, amount);
			OnHealthChange();
		}

		public event EventHandler<CombatEventArgs> Attacking;
		public event EventHandler<CombatEventArgs> Defending;

		public void OnAttacking(CombatEventArgs e) {
			EventHandler<CombatEventArgs> handler = Attacking;
			if (handler != null)
				handler(this, e);
		}

		public void OnDefending(CombatEventArgs e) {
			EventHandler<CombatEventArgs> handler = Defending;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region Inventory

		/// <summary>
		/// Add item into inventory, return true if item was added, false if item couldn't for whatever reason
		/// </summary>
		public bool AddItem(Item item) {
			if (item == null)
				throw new ArgumentException("item is null", "item");
			if (inventory.Weight + item.Weight > Lift * 5)
				return false;

			Logger.InfoFormat("{0} is adding {1} to his inventory.", Name, item.Name);
			inventory.AddItem(item);
			OnItemAdded(new EventArgs<Item>(item));
			return true;
		}

		public void RemoveItem(Item item) {
			if (item == null)
				throw new ArgumentException("item is null", "item");
			Logger.InfoFormat("{0} is removing {1} from his inventory.", Name, item.Name);
			OnItemRemoved(new EventArgs<Item>(item));
			inventory.RemoveItem(item);
		}

		/// <summary>
		/// Equip item into body part, any item already in the slot will be unequipped
		/// </summary>
		/// <returns>returns success if it was successful, aborted if item already equip couldn't fit in inventory</returns>
		/// todo rethink this
		public ActionResult Equip(BodySlot bpslot, Item item) {
			if (item == null)
				throw new ArgumentException("item is null", "item");
			if (!item.Slot.HasFlag(bpslot))
				throw new ArgumentException("part cannot use this item", "item");

			if (equippedItems.ContainsKey(bpslot))
				if (!AddItem(equippedItems[bpslot]))
					return ActionResult.Aborted;

			// we have enough space in your inventory for this
			equippedItems[bpslot] = item;
			RemoveItem(item);
			OnItemEquipped(new EventArgs<Item>(item));

			Logger.InfoFormat("{0} is equipping {1} to {2}.", Name, item.Name, bpslot);
			return ActionResult.Success;
		}

		// todo rethink this, abort = nothing happen, failed = couldn't fit in inventory to dropped?
		public ActionResult Unequip(BodySlot bpslot) {
			if (!equippedItems.ContainsKey(bpslot))
				return ActionResult.Aborted;
			if (AddItem(equippedItems[bpslot])) {
				OnItemUnequipped(new EventArgs<Item>(equippedItems[bpslot]));
				Logger.InfoFormat("{0} is unequipping {1} from {2}.", Name, equippedItems[bpslot].Name, bpslot);
				equippedItems.Remove(bpslot);

				return ActionResult.Success;
			}
			return ActionResult.Aborted;
		}

		public bool IsItemEquipped(BodySlot bpslot) {
			return equippedItems.ContainsKey(bpslot);
		}

		public Item GetItemAtBodyPart(BodySlot bpSlot) {
			return IsItemEquipped(bpSlot) ? equippedItems[bpSlot] : null;
		}

		public IEnumerable<Item> Items {
			get { return inventory; }
		}

		public event EventHandler<EventArgs<Item>> ItemRemoved;
		public event EventHandler<EventArgs<Item>> ItemAdded;

		public event EventHandler<EventArgs<Item>> ItemEquipped;
		public event EventHandler<EventArgs<Item>> ItemUnequipped;

		public void OnItemEquipped(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemEquipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemUnequipped(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemUnequipped;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemAdded(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemAdded;
			if (handler != null)
				handler(this, e);
		}

		public void OnItemRemoved(EventArgs<Item> e) {
			EventHandler<EventArgs<Item>> handler = ItemRemoved;
			if (handler != null)
				handler(this, e);
		}

		#endregion
	}
}