using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;

namespace SkrGame.Universe.Entities.Items {
	public class MeleeComponent : Component {
		public class Template {
			public int HitBonus { get; set; }
			public Rand Damage { get; set; }
			public Action<Entity> OnHit { get; set; }
			public double Penetration { get; set; }
			public double WeaponSpeed {
				get { return World.ActionPointsToSpeed(APToAttack); }
				set { APToAttack = World.SpeedToActionPoints(value); }
			}
			public int APToAttack { get; set; }
			public int Reach { get; set; }
			public int Parry { get; set; }
			public int Targetting { get; set; }

			public string ActionDescription { get; set; }
			public string ActionDescriptionPlural { get; set; }

			public string Skill { get; set; }
			public int Strength { get; set; }

			public int APToReady { get; set; }
			public bool UnreadyAfterAttack { get; set; }

			public DamageType DamageType { get; set; }
		}

		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		public int HitBonus { get; protected set; }

		public DamageType DamageType { get; protected set; }
		public double Penetration { get; protected set; }

		public Action<Entity> OnHit { get; private set; }

		public int APToAttack { get; protected set; }
		public int APToReady { get; protected set; }

		public string Skill { get; private set; }

		public Rand Damage { get; protected set; }
		public int Strength { get; protected set; }

		public int Reach { get; protected set; }
		public int Parry { get; protected set; }
		public int Targetting { get; protected set; }	// bonus for targetting

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(Penetration > 0.0f);
			Contract.Invariant(Damage != null);
			Contract.Invariant(Strength > 0);
			Contract.Invariant(Reach >= 0);
			Contract.Invariant(APToAttack > 0);
			Contract.Invariant(APToReady > 0);
			Contract.Invariant(!String.IsNullOrEmpty(Skill));
			Contract.Invariant(DamageType != null);
			Contract.Invariant(!String.IsNullOrEmpty(ActionDescription));
			Contract.Invariant(!String.IsNullOrEmpty(ActionDescriptionPlural));
		}

		private MeleeComponent( int str,
								int hit,
								Rand dmg,
								DamageType type,
								Action<Entity> onhit,
								double pen,
								int apready,
								int apAtk,
								int reach,
								int parry,
								int target,
								string skill,
								string actionDesc,
								string actionDescPlural) {
			ActionDescription = actionDesc;
			ActionDescriptionPlural = actionDescPlural;

			Skill = skill;

			Strength = str;
			HitBonus = hit;
			Damage = dmg;
			DamageType = type;
			OnHit = onhit;
			Penetration = pen;
			APToReady = apready;
			APToAttack = apAtk;
			Reach = reach;
			Parry = parry;
			Targetting = target;
		}

		public MeleeComponent(Template template) {
			ActionDescription = template.ActionDescription;
			ActionDescriptionPlural = template.ActionDescriptionPlural;

			Skill = template.Skill;

			Strength = template.Strength;
			HitBonus = template.HitBonus;
			Damage = template.Damage;
			DamageType = template.DamageType;
			OnHit = template.OnHit;
			Penetration = template.Penetration;
			APToReady = template.APToReady;
			APToAttack = template.APToAttack;
			Reach = template.Reach;
			Parry = template.Parry;
			Targetting = template.Targetting;
		}

		public override string ToString() {
			return ActionDescription;
		}

		public override Component Copy() {
			return new MeleeComponent(Strength, HitBonus, Damage, DamageType, OnHit, Penetration, APToReady, APToAttack, Reach, Parry, Targetting, Skill, ActionDescription, ActionDescriptionPlural);
		}
	}
}
