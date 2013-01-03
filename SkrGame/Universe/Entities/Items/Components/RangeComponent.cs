using System;
using DEngine.Core;
using DEngine.Entity;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items.Components {
	public class AmmoComponentTemplate {
		public string Type { get; set; }
		public string ComponentId { get; set; }

		public string ActionDescription { get; set; }
		public string ActionDescriptionPlural { get; set; }
	}

	public class AmmoComponent : EntityComponent {
		public string Type { get; private set; }

		public AmmoComponent(AmmoComponentTemplate template) {
			ActionDescription = template.ActionDescription;
			ActionDescriptionPlural = template.ActionDescriptionPlural;

			Type = template.Type;
		}
		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }
	}
	// todo ammo cases

	public class RangeComponentTemplate {
		public int Accuracy { get; set; }
		public Rand Damage { get; set; }
		public double Penetration { get; set; }
		public Action<Actor, Actor> OnHit { get; set; }
		public int Range { get; set; }
		public int WeaponSpeed {
			get { return World.ActionPointsToSpeed(APToAttack); }
			set { APToAttack = World.SpeedToActionPoints(value); }
		}
		public int RoF {
			get { return (int) Math.Round(1 / World.ActionPointsToSeconds(APToAttack)); }
			set { APToAttack = World.SecondsToActionPoints(1.0 / value); }
		}
		public int APToAttack { get; set; }
		public int ReloadSpeed {
			get { return World.ActionPointsToSpeed(APToReload); }
			set { APToReload = World.SpeedToActionPoints(value); }
		}
		public int APToReload { get; set; }
		public int Recoil { get; set; }
		public int Reliability { get; set; }
		public string AmmoType { get; set; }
		public int Shots { get; set; }

		public string ComponentId { get; set; }

		public string ActionDescription { get; set; }
		public string ActionDescriptionPlural { get; set; }

		public string Skill { get; set; }
		public int Strength { get; set; }

		public int APToReady { get; set; }
		public bool UnreadyAfterAttack { get; set; }

		public DamageType DamageType { get; set; }

		// not used yet
		public bool SwapClips { get; set; }			// if true, a new clip replaces the old; if false, additional cartridges are added like as in a shotgun
		public int ShotsPerBurst { get; set; }		// number of bullets fired per burst
		public int BurstPenalty { get; set; }		// penalty for each shot of the burst
		public int BurstAP { get; set; }			// AP cost for a burst
	}

	public class RangeComponent : EntityComponent {
		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		public int Accuracy { get; protected set; }

		public Action<Actor, Actor> OnHit { get; private set; }

		public DamageType DamageType { get; protected set; }
		public double Penetration { get; protected set; }
		public int APToAttack { get; protected set; }

		public string Skill { get; private set; }

		public Rand Damage { get; protected set; }
		public int APToReady { get; protected set; }
		public int Strength { get; protected set; }

		public int Range { get; protected set; }
		public int APToReload { get; protected set; }

		public int Recoil { get; protected set; }
		public int Reliability { get; protected set; }
		public string AmmoType { get; protected set; }
		public int Shots { get; set; }
		public int ShotsRemaining { get; set; }

		public RangeComponent(RangeComponentTemplate template) {
			ActionDescription = template.ActionDescription;
			ActionDescriptionPlural = template.ActionDescriptionPlural;

			Skill = template.Skill;

			Accuracy = template.Accuracy;
			Damage = template.Damage;
			DamageType = template.DamageType;
			Penetration = template.Penetration;
			OnHit = template.OnHit;
			APToReady = template.APToReady;
			APToAttack = template.APToAttack;
			Range = template.Range;
			APToReload = template.APToReload;
			Recoil = template.Recoil;
			Reliability = template.Reliability;
			AmmoType = template.AmmoType;
			ShotsRemaining = Shots = template.Shots;
		}
	}
}