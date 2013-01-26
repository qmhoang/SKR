using System;
using System.Collections.Generic;
using System.Reflection;
using DEngine.Core;
using SkrGame.Gameplay.Talent;
using SkrGame.Gameplay.Talent.Components;
using log4net;

namespace SkrGame.Universe.Entities.Actors {
	public class TalentUsedEvent : EventArgs {
		public Actor User { get; private set; }
		public Talent Talent { get; private set; }

		public TalentUsedEvent(Actor user, Talent talent) {
			User = user;
			Talent = talent;
		}
	}

	public class ActorTalents {
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Dictionary<string, Talent> talents;
		private Actor owner;

		#region Talents

		public bool KnowTalent(string skillRefId) {
			return talents.ContainsKey(skillRefId);
		}

		public bool CanLearnTalent(string talent) {
			throw new NotImplementedException();
		}

		public void LearnTalent(string skillRefId) {
			if (KnowTalent(skillRefId))
				Logger.WarnFormat("{0} already knows {1}.", owner.Name, skillRefId);
			else {
				// add talent
				var t = World.Instance.GetTalent(skillRefId);
				t.Owner = this.owner;
				talents.Add(skillRefId, t);
				t.OnLearn();

				OnTalentLearned(new EventArgs<Talent>(t));
				Logger.DebugFormat("{0} has learned {1}.", owner.Name, skillRefId);
			}
		}

		public void UnlearnTalent(string skillRefId) {
			if (!KnowTalent(skillRefId))
				Logger.WarnFormat("{0} doesn't know {1} to unlearn.", owner.Name, skillRefId);
			else {
				var t = GetTalent(skillRefId);
				t.OnUnlearn();

				talents.Remove(skillRefId);

				OnTalentUnlearned(new EventArgs<Talent>(t));
				Logger.DebugFormat("{0} has unlearned {1}.", owner.Name, skillRefId);
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

		public event EventHandler<TalentUsedEvent> TalentUsed;

		public void OnTalentUsed(TalentUsedEvent e) {
			EventHandler<TalentUsedEvent> handler = TalentUsed;
			if (handler != null)
				handler(this, e);
		}

		#endregion

		#region Basic Actions

		public Talent MeleeAttack() {
			return GetTalent("action_attack");
		}

		public Talent RangeAttack() {
			return GetTalent("action_range");
		}

		public Talent ReloadWeapon() {
			return GetTalent("action_reload");
		}

		public Talent Activate() {
			return GetTalent("action_activate");
		}

		#endregion

		public ActorTalents(Actor owner) {
			this.owner = owner;
			talents = new Dictionary<string, Talent>();

			LearnTalent("action_attack");
			LearnTalent("action_range");
			LearnTalent("action_reload");
			LearnTalent("action_activate");

			LearnTalent("attrb_strength");
			LearnTalent("attrb_agility");
			LearnTalent("attrb_constitution");
			LearnTalent("attrb_intellect");
			LearnTalent("attrb_cunning");
			LearnTalent("attrb_resolve");
			LearnTalent("attrb_presence");
			LearnTalent("attrb_grace");
			LearnTalent("attrb_composure");

			GetTalent("attrb_strength").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_agility").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_constitution").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_intellect").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_cunning").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_resolve").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_presence").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_grace").As<AttributeComponent>().Rank = World.MEAN;
			GetTalent("attrb_composure").As<AttributeComponent>().Rank = World.MEAN;

			LearnTalent("skill_sword");
			LearnTalent("skill_knife");

			LearnTalent("skill_pistol");
			LearnTalent("skill_bow");

			LearnTalent("skill_unarmed");
		}
	}
}