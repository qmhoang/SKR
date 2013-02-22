using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actions;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;

namespace SkrGame.Actions.Combat {
	public class PlayerPreAttack: IOptionsAction {
		public PlayerPreAttack(Entity attacker, Direction direction, bool targetPart) {
			Contract.Requires<ArgumentException>(attacker.Has<EquipmentComponent>());
			this.attacker = attacker;
			this.targetPart = targetPart;
			this.direction = direction;

			selection = Selection.Weapon;

			targets = attacker.Get<GameObject>().Level.GetEntitiesAt(attacker.Get<GameObject>().Location + direction).Where(e => e.Has<DefendComponent>()).ToList();

			weapons = new List<Entity>();
			var equipment = attacker.Get<EquipmentComponent>();

			weapons.AddRange(equipment.Slots.Where(slot => equipment.IsSlotEquipped(slot) && equipment[slot].Has<MeleeComponent>()).Select(slot => equipment[slot]));
			
			if (weapons.Count == 0 && attacker.Has<MeleeComponent>())
				weapons.Add(attacker); // natural weapon
		}

		public int APCost {
			get { return 1; }
		}

		public PromptType RequiresPrompt {
			get {
				if (selection == Selection.Fail)
					return PromptType.None;
				if (selection == Selection.Finished || (!targetPart && selection == Selection.BodyPart))
					return PromptType.None;
				return PromptType.Options;
			}
		}

		public ActionResult OnProcess() {			
			if (selection != Selection.Fail)
				attacker.Get<ActorComponent>().Enqueue(new MeleeAttackAction(attacker, defender, weapon, targetPart ? part : defender.Get<DefendComponent>().GetRandomPart(), targetPart));
			return ActionResult.SuccessNoTime;
		}

		public string Message {
			get {
				if (selection == Selection.Weapon) {
					return "What weapon?";
				} else if (selection == Selection.Target) {
					return "Attack what?";
				} else if (selection == Selection.BodyPart) {
					return "Target what?";
				} else {
					throw new Exception();
				}			
			}
		}

		public void Fail() {
			if (selection == Selection.Weapon) {
				World.Log.Fail("No weapon available.");
			} else if (selection == Selection.Target) {
				World.Log.Fail("Nothing there to attack.");
			} else if (selection == Selection.BodyPart) {
				World.Log.Fail("No parts to attack.");
			}
			selection = Selection.Fail;
		}

		private World World {
			get { return attacker.Get<GameObject>().Level.World; }
		}

		public void SetOption(string o) {
			switch (selection) {
				case Selection.Weapon:
					selection = Selection.Target;
					World.RequireNewPrompt = true;
					weapon = weapons.Find(e => Identifier.GetNameOrId(e) == o);
					break;
				case Selection.Target:
					selection = targetPart ? Selection.BodyPart : Selection.Finished;
					World.RequireNewPrompt = true;
					defender = targets.Find(e => Identifier.GetNameOrId(e) == o);
					break;
				case Selection.BodyPart:
					selection = Selection.Finished;
					World.RequireNewPrompt = true;
					part = defender.Get<DefendComponent>().BodyPartsList.First(bp => bp.Name == o);
					break;
			}
		}


		public IEnumerable<string> Options {
			get {
				switch (selection) {
					case Selection.Weapon:
						return weapons.Select(Identifier.GetNameOrId);
					case Selection.Target:
						return targets.Select(Identifier.GetNameOrId);
					case Selection.BodyPart:
						return defender.Get<DefendComponent>().BodyPartsList.Select(bp => bp.Name);
					case Selection.Finished:
					case Selection.Fail:
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private Direction direction;
		private bool targetPart;

		private enum Selection {
			Target,
			Weapon,
			BodyPart,
			Finished,
			Fail
		}

		private Selection selection;

		private List<Entity> targets;
		private List<Entity> weapons;

		private Entity attacker;
		private Entity defender;
		private Entity weapon;
		private DefendComponent.AttackablePart part;
	}

	public class MeleeAttackAction : AttackAction {
		public MeleeAttackAction(Entity attacker, Entity defender, Entity weapon, DefendComponent.AttackablePart bodyPartTargetted, bool targettingPenalty = false)
				: base(attacker, defender, weapon, bodyPartTargetted, targettingPenalty) {
			Contract.Requires<ArgumentException>(weapon.Has<MeleeComponent>(), "weapon cannot melee attack");
		}

		public override int APCost {
			get { return Weapon.Get<MeleeComponent>().APToAttack; }
		}

		public Rand GetStrengthDamage(int strength) {
			return Rand.Gaussian(strength / 3, strength / 3 - 1, Math.Min(World.STANDARD_DEVIATION / 3, strength / 3));
		}

		public override ActionResult OnProcess() {
			int hitBonus = 0;
			var melee = Weapon.Get<MeleeComponent>();

			//apply skill
			if (Attacker.Has<ActorComponent>()) {
				hitBonus += Attacker.Get<Person>().GetSkill(melee.Skill).Value;
			} else {
				hitBonus += World.MEAN;
			}

			var defenderName = Identifier.GetNameOrId(Defender);
			var attackerName = Identifier.GetNameOrId(Attacker);

			var result = Attack(attackerName, defenderName, hitBonus + melee.HitBonus - (TargettingPenalty ? BodyPartTargetted.TargettingPenalty : 0));

			if (result == CombatEventResult.Hit) {
				const int TEMP_STR_BONUS = World.MEAN;
				var damage = Math.Max(melee.Damage.Roll() + GetStrengthDamage(TEMP_STR_BONUS).Roll(), 1);
				int damageResistance, realDamage;

				Damage(damage, melee.Penetration, melee.DamageType, out damageResistance, out realDamage);

				if (World.Player == Attacker) {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description"));
				} else {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and inflict {4} wounds.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name, "todo-description"));
				}
				
				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Hit, damage,
				                                         damageResistance, realDamage));
				return ActionResult.Success;

			} else if (result == CombatEventResult.Miss) {
				if (World.Player == Attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and misses.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				}


				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted));
			} else if (result == CombatEventResult.Dodge) {
				if (World.Player == Attacker) {
					World.Log.Bad(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                            attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				} else {
					World.Log.Good(String.Format("{0} {1} {2}'s {3}.... and {2} dodges.",
					                             attackerName, melee.ActionDescriptionPlural, defenderName, BodyPartTargetted.Name));
				}


				Logger.Info(new CombatEventArgs(Attacker, Defender, Weapon, BodyPartTargetted, CombatEventResult.Dodge));
			}
			return ActionResult.Failed;
		}		
	}
}