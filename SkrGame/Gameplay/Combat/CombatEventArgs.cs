using System;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Gameplay.Combat {
	public enum CombatEventResult {
		Hit,
		Miss,
		Dodge,
		Parry,
		Blocked,
	}

	public class CombatEventArgs : EventArgs {
		public Entity Attacker { get; private set; }
		public Entity Defender { get; private set; }
		public DefendComponent.AttackablePart BodyPart { get; private set; }

		public CombatEventResult Result { get; private set; }

		public int Damage { get; private set; }
		public int DamageResisted { get; private set; }
		public int DamageTaken { get; private set; }

		public CombatEventArgs(Entity attacker, Entity defender, DefendComponent.AttackablePart bp, CombatEventResult result = CombatEventResult.Miss, int damage = 0, int damageResisted = 0, int damageTaken = 0) {
			Attacker = attacker;
			Defender = defender;
			BodyPart = bp;
			Result = result;
			Damage = damage;
			DamageResisted = damageResisted;
			DamageTaken = damageTaken;
		}

		public override string ToString() {
			return String.Format("Attacker: {4}, Defender: {5}, Result: {0} - damage: {1}, resisted: {2}, taken: {3}", Result.ToString(), Damage, DamageResisted, DamageTaken, 
				Identifier.GetNameOrId(Attacker),
				Identifier.GetNameOrId(Defender));
		}
	}
}