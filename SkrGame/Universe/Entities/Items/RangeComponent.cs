using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;

namespace SkrGame.Universe.Entities.Items {
	// todo ammo cases

	public class RangeComponent : Component {
		public class Template {
			public int Accuracy { get; set; }
			public Rand Damage { get; set; }
			public double Penetration { get; set; }
			public Action<Entity, Entity> OnHit { get; set; }
			public int Range { get; set; }

			public int APToAttack { get; set; }

			public double AttackSpeed {
				get { return World.ActionPointsToSpeed(APToAttack); }
				set { APToAttack = World.SpeedToActionPoints(value); }
			}

			public int RoF {
				get { return (int) Math.Round(1 / World.ActionPointsToSeconds(APToAttack)); }
				set { APToAttack = World.SecondsToActionPoints(1.0 / value); }
			}

			public int APToReload { get; set; }

			public double ReloadSpeed {
				get { return World.ActionPointsToSpeed(APToReload); }
				set { APToReload = World.SpeedToActionPoints(value); }
			}

			public bool SwapClips { get; set; } // if true, a new clip replaces the old; if false, additional cartridges are added like as in a shotgun

			public int Recoil { get; set; }
			public int Reliability { get; set; }
			public string AmmoCaliber { get; set; }
			public int Shots { get; set; }
			public bool OneInTheChamber { get; set; } // does gun have a chamber that allows a bullet inside

			public string ActionDescription { get; set; }
			public string ActionDescriptionPlural { get; set; }

			public string Skill { get; set; }
			public int Strength { get; set; }

			public int APToReady { get; set; }

			public double ReadySpeed {
				get { return World.ActionPointsToSpeed(APToReady); }
				set { APToReady = World.SpeedToActionPoints(value); }
			}

			//			public bool UnreadyAfterAttack { get; set; }

			public DamageType DamageType { get; set; }

			// not used yet
			//			public int ShotsPerBurst { get; set; }		// number of bullets fired per burst
			//			public int BurstPenalty { get; set; }		// penalty for each shot of the burst
			//			public int BurstAP { get; set; }			// AP cost for a burst
		}

		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		public int Accuracy { get; protected set; }

		public Action<Entity, Entity> OnHit { get; private set; }

		public DamageType DamageType { get; protected set; }
		public double Penetration { get; protected set; }
		public int APToAttack { get; protected set; }

		public string Skill { get; private set; }

		public Rand Damage { get; protected set; }
		public int APToReady { get; protected set; }
		public int Strength { get; protected set; }

		public int Range { get; protected set; }

		public int APToReload { get; protected set; }
		public bool SwapClips { get; private set; } // for revolvers and shotguns

		public int Recoil { get; protected set; }
		public int Reliability { get; protected set; }
		public string AmmoCaliber { get; protected set; }
		public int Shots { get; set; }
		public int ShotsRemaining { get; set; }
		public bool OneInTheChamber { get; private set; }

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(Recoil >= 0);
			Contract.Invariant(OneInTheChamber ? Shots + 1 >= ShotsRemaining : Shots >= ShotsRemaining);
			Contract.Invariant(Shots > 0);
			Contract.Invariant(ShotsRemaining >= 0);
			Contract.Invariant(Accuracy >= 0);
			Contract.Invariant(Penetration > 0.0f);
			Contract.Invariant(Damage != null);
			Contract.Invariant(Strength > 0);
			Contract.Invariant(Range > 0);
			Contract.Invariant(APToAttack > 0);
			Contract.Invariant(APToReady > 0);
			Contract.Invariant(APToReload > 0);
			Contract.Invariant(!String.IsNullOrEmpty(AmmoCaliber));
			Contract.Invariant(!String.IsNullOrEmpty(Skill));
			Contract.Invariant(DamageType != null);
			Contract.Invariant(!String.IsNullOrEmpty(ActionDescription));
			Contract.Invariant(!String.IsNullOrEmpty(ActionDescriptionPlural));
		}

		private RangeComponent(string actionDescription, string actionDescriptionPlural, int accuracy, Action<Entity, Entity> onHit, DamageType damageType, double penetration, int apToAttack, string skill,
		                       Rand damage, int apToReady, int strength, int range, int apToReload, bool swapClips, int recoil, int reliability, string ammoType, int shots, int shotsRemaining,
		                       bool oneInTheChamber) {
			ActionDescription = actionDescription;
			ActionDescriptionPlural = actionDescriptionPlural;
			Accuracy = accuracy;
			OnHit = onHit;
			DamageType = damageType;
			Penetration = penetration;
			APToAttack = apToAttack;
			Skill = skill;
			Damage = damage;
			APToReady = apToReady;
			Strength = strength;
			Range = range;
			APToReload = apToReload;
			SwapClips = swapClips;
			Recoil = recoil;
			Reliability = reliability;
			AmmoCaliber = ammoType;
			Shots = shots;
			ShotsRemaining = shotsRemaining;
			OneInTheChamber = oneInTheChamber;
		}

		public RangeComponent(Template template) :
				this(template.ActionDescription,
				     template.ActionDescriptionPlural,
				     template.Accuracy,
				     template.OnHit,
				     template.DamageType,
				     template.Penetration,
				     template.APToAttack,
				     template.Skill,
				     template.Damage,
				     template.APToReady,
				     template.Strength,
				     template.Range,
				     template.APToReload,
				     template.SwapClips,
				     template.Recoil,
				     template.Reliability,
				     template.AmmoCaliber,
				     template.Shots,
				     template.OneInTheChamber ? template.Shots + 1 : template.Shots,
				     template.OneInTheChamber) {}

		public override Component Copy() {
			return new RangeComponent(ActionDescription, ActionDescriptionPlural, Accuracy, OnHit, DamageType,
			                          Penetration, APToAttack, Skill, Damage, APToReady, Strength, Range, APToReload, SwapClips, Recoil,
			                          Reliability, AmmoCaliber, Shots, ShotsRemaining, OneInTheChamber);

		}
	}
}