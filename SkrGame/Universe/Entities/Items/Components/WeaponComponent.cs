using System;
using DEngine.Core;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items.Components {
	public interface IWeaponComponentTemplate : IItemComponentTemplate {
		string Skill { get; set; }
		int Strength { get; set; }
		int APToReady { get; set; }
		bool UnreadyAfterAttack { get; set; }
		DamageType DamageType { get; set; }
	}

	public interface IWeaponComponent : IItemComponent {
		Rand Damage { get; }
		DamageType DamageType { get; }
		double Penetration { get; }

		Action<Actor, Actor> OnHit { get; }

		string Skill { get; }
		int APToAttack { get; }
		int APToReady { get; }
		int Strength { get; }
	}
}