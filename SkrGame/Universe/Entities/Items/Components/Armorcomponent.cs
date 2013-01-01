using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkrGame.Gameplay.Combat;

namespace SkrGame.Universe.Entities.Items.Components {
	public class ArmorComponentTemplate : IItemComponentTemplate {
		public Dictionary<DamageType, int> Resistances { get; set; }

		/// <summary>
		/// This is the how much armor is divided by when an attack hits an area that the armor doesn't cover.  
		/// If the armor has a DR if 6 and a non coverage divisor of 1.5 and the wearer hits a chink in the armor, 
		/// the armor has a DR of 4 for the purpose of that attack. 
		/// </summary>
		public double NonCoverageDivisor { get; set; }

		public int Defense { get; set; }

		/// <summary>
		/// How much area the armor covers that body part.
		/// </summary>
		public int Coverage { get; set; }
		public int DonTime { get; set; }
		// todo can race wear armor

		public string ComponentId { get; set; }
		public string ActionDescription { get; set; }
		public string ActionDescriptionPlural { get; set; }

		public IItemComponent Construct(Item item) {
			return new ArmorComponent(item, this);
		}
	}

	public class ArmorComponent : IItemComponent {
		public string ComponentId { get; private set; }

		public Item Item { get; set; }

		public string ActionDescription { get; private set; }
		public string ActionDescriptionPlural { get; private set; }

		public Dictionary<DamageType, int> Resistances { get; set; }

		public double NonCoverageDivisor { get; set; }

		public int Defense { get; protected set; }

		public int Coverage { get; protected set; }
		public int DonTime { get; protected set; }

		public ArmorComponent(Item item, ArmorComponentTemplate template) {
			Item = item;

			ComponentId = template.ComponentId;
			ActionDescription = template.ActionDescription;
			ActionDescriptionPlural = template.ActionDescriptionPlural;

			Resistances = new Dictionary<DamageType, int>(template.Resistances);

			// check for missing resistances and throws errors
			foreach (var value in Combat.DamageTypes.Values) {
				if (!Resistances.ContainsKey(value)) {
					throw new ArgumentException("Resistances is missing a damage type");
				}
			}

			Defense = template.Defense;
			NonCoverageDivisor = template.NonCoverageDivisor;

			Coverage = template.Coverage;
			DonTime = template.DonTime;
		}
	}
}
