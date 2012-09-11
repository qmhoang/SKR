using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Gameplay.Talent;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Location;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
//    public enum Attribute {
//        Strength,
//        Agility,
//        Constitution,
//        Intellect,
//        Cunning,
//        Resolve,
//        Presence,
//        Grace,
//        Composure,        
//    }

    public enum Skill {
        // basic attack
//        TargetAttack,
//        RangeTargetAttack,
//        DetailedReload,
//        LoadMagazine,
        Attack,
        Range,
        Reload,
        UseFeature,        

        //category - sword
        Sword,
        Knife,
        Longsword,

        //category - brawling
        Brawling,
        Judo,

        //category - impact weapon
        ImpactWeapon,
        Axe,

        Pistol,
        Shotgun,
        Rifle,

        Bow,
        Crossbow,

        // attributes
        Strength,
        Agility,
        Constitution,
        Intellect,
        Cunning,
        Resolve,
        Presence,
        Grace,
        Composure,
    }


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



    public abstract class Actor : Entity {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActorCharacteristics Characteristics { get; private set; }
        private ActorCondition conditionStatuses;
        private Dictionary<BodyPartType, Item> equippedItems;
        private Dictionary<Skill, Talent> talents;

        public override int ActionPoints {
            get {
                return base.ActionPoints;
            }
            set {
                RecalculateFov = true;
                base.ActionPoints = value;
            }
        } 

        public Level Level { get; private set; }

        public World World {
            get { return World.Instance; }
        }               

        private ItemContainer inventory;
        private int additionalWeight;        

        public override bool Dead {
            get { return Characteristics.Health < 0; }
        }

        public int Dodge {
            get { return Characteristics.BasicDodge; }
        }

        public int Lift {
            get { return Characteristics.Lift + additionalWeight; }
        }

        protected Actor(string refId, string asset, Level level) {            
            Level = level;
            RefId = refId;

            inventory = new ItemContainer();
            equippedItems = new Dictionary<BodyPartType, Item>();
            talents = new Dictionary<Skill, Talent>();

            LearnTalent(Skill.Attack);
            LearnTalent(Skill.Range);            
            LearnTalent(Skill.Reload);
            LearnTalent(Skill.UseFeature);

            LearnTalent(Skill.Strength, World.Mean);
            LearnTalent(Skill.Agility, World.Mean);
            LearnTalent(Skill.Constitution, World.Mean);
            LearnTalent(Skill.Intellect, World.Mean);
            LearnTalent(Skill.Cunning, World.Mean);
            LearnTalent(Skill.Resolve, World.Mean);
            LearnTalent(Skill.Presence, World.Mean);
            LearnTalent(Skill.Grace, World.Mean);
            LearnTalent(Skill.Composure, World.Mean);

            LearnTalent(Skill.Sword, 0);
            LearnTalent(Skill.Knife, 0);
            
            LearnTalent(Skill.Pistol, 0);

            LearnTalent(Skill.Brawling, 0);
            Asset = asset;

            Characteristics = new ActorCharacteristics(this);
            conditionStatuses = new ActorCondition(this);

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

        public BodyPart GetBodyPart(BodyPartType bp) {
            return Characteristics.GetBodyPart(bp);
        }

        public IEnumerable<BodyPart> BodyParts {
            get { return Characteristics.BodyPartsList; }
        }

        public void CalculateFov() {
            Level.Fov.computeFov(Position.X, Position.Y, SightRadius);
            RecalculateFov = false;
        }

        public override void Update() {            
            CheckEncumbrance();
        }

        protected bool RecalculateFov;

        public override bool HasLineOfSight(Point position) {
            if (RecalculateFov)
                CalculateFov();            
            return Level.IsVisible(position);
        }

        public override bool CanSpot(DEngine.Actor.Entity actor) {
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
                World.InsertMessage("There is something in the way");
                return ActionResult.Aborted;                
            }

            if (!Level.IsInBoundsOrBorder(nPos)) {                
                return ActionResult.Aborted;
            }

            if (Level.DoesActorExistAtLocation(nPos)) {
                GetTalent(Skill.Attack).InvokeAction(nPos);
            } else {
                Position = nPos;
                
                foreach (var feature in Level.Features)
                {
                    feature.Near(this);
                }
            }

            ActionPoints -= World.DefaultTurnSpeed;

            RecalculateFov = true;
            return ActionResult.Success;
        }

        public override ActionResult Wait() {
            ActionPoints -= World.DefaultTurnSpeed;
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
            Log.InfoFormat("{0}'s {1} is now set at {2}", Name, condition, status);
            OnConditionChanged(new EventArgs<Condition, int>(condition, status));
            conditionStatuses.SetConditionStatus(condition, status);
        }

        public void Hurt(BodyPartType bp, int amount) {
            Characteristics.GetBodyPart(bp).Health -= amount;
            Characteristics.Health -= amount;
            Log.InfoFormat("{0}'s {1} was hurt ({2} damage)", Name, GetBodyPart(bp).Name, amount);
            OnHealthChange();
        }

        public void Heal(int amount) {
            amount = Math.Min(amount, Characteristics.MaxHealth - Characteristics.Health);
            Characteristics.Health -= amount;
            Log.InfoFormat("{0} was healed {1} health", Name, amount);
            OnHealthChange();
        }

        public event EventHandler<EventArgs> HealthChanged;

        protected void OnHealthChange() {
            var handler = HealthChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public int Health {
            get { return Characteristics.Health; }
            set { Characteristics.Health = value; }
        }

        public int MaxHealth {
            get { return Characteristics.MaxHealth; }
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

            Log.InfoFormat("{0} is adding {1} to his inventory.", Name, item.Name);
            inventory.AddItem(item);
            OnItemAdded(new EventArgs<Item>(item));
            return true;
        }

        public void RemoveItem(Item item) {
            if (item == null)
                throw new ArgumentException("item is null", "item");
            Log.InfoFormat("{0} is removing {1} from his inventory.", Name, item.Name);
            OnItemRemoved(new EventArgs<Item>(item));
            inventory.RemoveItem(item);
        }

        /// <summary>
        /// Equip item into body part, any item already in the slot will be unequipped
        /// </summary>        
        /// <returns>returns success if it was successful, aborted if item already equip couldn't fit in inventory</returns>
        /// todo rethink this
        public ActionResult Equip(BodyPartType bpslot, Item item) {
            var bp = GetBodyPart(bpslot);
            if (item == null)
                throw new ArgumentException("item is null", "item");
            if (!bp.CanUseItem(item))
                throw new ArgumentException("part cannot use this item", "item");

            if (equippedItems.ContainsKey(bpslot))
                if (!AddItem(equippedItems[bpslot]))
                    return ActionResult.Aborted;

            // we have enough space in your inventory for this                
            equippedItems[bpslot] = item;
            RemoveItem(item);
            OnItemEquipped(new EventArgs<Item>(item));

            Log.InfoFormat("{0} is equipping {1} to {2}.", Name, item.Name, bpslot);
            return ActionResult.Success;
        }

        // todo rethink this, abort = nothing happen, failed = couldn't fit in inventory to dropped?
        public ActionResult Unequip(BodyPartType bpslot) {            
            if (!equippedItems.ContainsKey(bpslot))
                return ActionResult.Aborted;
            if (AddItem(equippedItems[bpslot])) {
                OnItemUnequipped(new EventArgs<Item>(equippedItems[bpslot]));
                Log.InfoFormat("{0} is unequipping {1} from {2}.", Name, equippedItems[bpslot].Name, bpslot);
                equippedItems.Remove(bpslot);

                return ActionResult.Success;
            }
            return ActionResult.Aborted;
        }

        public bool IsItemEquipped(BodyPartType bpslot) {
            return equippedItems.ContainsKey(bpslot);
        }

        public Item GetItemAtBodyPart(BodyPartType bpSlot) {
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

        #region Talents
        public bool KnowTalent(Skill talent) {
            return talents.ContainsKey(talent);
        }

        public bool CanLearnTalent(Skill talent) {
            return false; // todo
        }

        public void LearnTalent(Skill skill, int rank = 1) {
            Log.InfoFormat("{0} learned {1}", Name, skill);

            if (!KnowTalent(skill)) {
                // add talent
                var t = World.GetTalent(skill);
                t.Owner = this;
                talents.Add(skill, t);
            }

            Talent talent = GetTalent(skill);
            talent.RawRank += rank;
            OnTalentLearned(new EventArgs<Talent, int>(talent, rank));
        }

        public bool UnlearnTalent(Skill talent, int rank = 1) {
            return false;
        }

        public Talent GetTalent(Skill skill) {
            return talents[skill];
        }

        public event EventHandler<EventArgs<Talent, int>> TalentLearned;

        public void OnTalentLearned(EventArgs<Talent, int> e) {
            EventHandler<EventArgs<Talent, int>> handler = TalentLearned;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        
    }
}