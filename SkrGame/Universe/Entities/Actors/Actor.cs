using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Locations;
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

	public class Actor : EntityComponent{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly ActorCondition conditionStatuses;
		private readonly Dictionary<string, Talent> talents;
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
			} else {
				SetTag(id, increment);
			}
		}
		
		public Level Level { get; private set; }

		public World World {
			get { return World.Instance; }
		}

		public int Lift {
			get {
				return
						(int)
						(GetTalent("attrb_strength").As<AttributeComponent>().Rank * GetTalent("attrb_strength").As<AttributeComponent>().Rank * 18 * Math.Pow(World.STANDARD_DEVIATION, -2.0));
			}
		}



		public string Name { get; set; }
		
		public Actor(string name, Level level) {
			Level = level;

			Name = name;

			talents = new Dictionary<string, Talent>();
			tags = new Dictionary<string, int>();

//			LearnTalent("action_attack");
//			LearnTalent("action_range");
//			LearnTalent("action_reload");
//			LearnTalent("action_activate");
//
//			LearnTalent("attrb_strength");
//			LearnTalent("attrb_agility");
//			LearnTalent("attrb_constitution");
//			LearnTalent("attrb_intellect");
//			LearnTalent("attrb_cunning");
//			LearnTalent("attrb_resolve");
//			LearnTalent("attrb_presence");
//			LearnTalent("attrb_grace");
//			LearnTalent("attrb_composure");
//
//			GetTalent("attrb_strength").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_agility").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_constitution").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_intellect").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_cunning").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_resolve").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_presence").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_grace").As<AttributeComponent>().Rank = World.MEAN;
//			GetTalent("attrb_composure").As<AttributeComponent>().Rank = World.MEAN;
//
//			LearnTalent("skill_sword");
//			LearnTalent("skill_knife");
//
//			LearnTalent("skill_pistol");
//			LearnTalent("skill_bow");
//
//			LearnTalent("skill_unarmed");			

			conditionStatuses = new ActorCondition(this);
		}


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


		#region Talents
		public bool KnowTalent(string skillRefId) {
			return talents.ContainsKey(skillRefId);
		}

		public bool CanLearnTalent(string talent) {
			throw new NotImplementedException();
		}

		public void LearnTalent(string skillRefId) {
			if (KnowTalent(skillRefId))
				Logger.WarnFormat("{0} already knows {1}.", Name, skillRefId);
			else {
				// add talent
//				var t = World.GetTalent(skillRefId);
//				t.Owner = this;
//				talents.Add(skillRefId, t);
//				t.OnLearn();
//
//				OnTalentLearned(new EventArgs<Talent>(t));
//				Logger.DebugFormat("{0} has learned {1}.", Name, skillRefId);
			}
		}

		public void UnlearnTalent(string skillRefId) {
			if (!KnowTalent(skillRefId)) {
				Logger.WarnFormat("{0} doesn't know {1} to unlearn.", Name, skillRefId);
			} else {
				var t = GetTalent(skillRefId);
				t.OnUnlearn();

				talents.Remove(skillRefId);

				OnTalentUnlearned(new EventArgs<Talent>(t));
				Logger.DebugFormat("{0} has unlearned {1}.", Name, skillRefId);
			}
		}

		public Talent GetTalent(string skillRefId) {
			return talents[skillRefId];
		}

		public event EventHandler<EventArgs<Talent>> TalentLearned;

		public void OnTalentLearned(EventArgs<Talent> e) {
			EventHandler<EventArgs<Talent>> handler = TalentLearned;
			if (handler != null)
				handler(this, e);
		}

		public event EventHandler<EventArgs<Talent>> TalentUnlearned;

		public void OnTalentUnlearned(EventArgs<Talent> e) {
			EventHandler<EventArgs<Talent>> handler = TalentUnlearned;
			if (handler != null)
				handler(this, e);
		}

		#endregion
	}
}