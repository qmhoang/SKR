using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;

namespace SkrGame.Universe.Entities.Items.Components {
	public class ArmorComponent : Component {
		public class Template {
			public List<LocationProtected> Defenses { get; set; }

			public int DonTime { get; set; }

			public string ComponentId { get; set; }
		}

		public class LocationProtected {

			public string BodyPart { get; private set; }
			/// <summary>
			/// How much area the armor covers that body part.
			/// </summary>		
			public int Coverage { get; private set; }
			public Dictionary<DamageType, int> Resistances { get; private set; }

			public LocationProtected(string bodyPart, int coverage, Dictionary<DamageType, int> resistances) {
				BodyPart = bodyPart;
				Coverage = coverage;
				Resistances = resistances;
			}
		}
		public Dictionary<string, LocationProtected> Defenses { get; private set; }
		
		public int DonTime { get; protected set; }

		private ArmorComponent() {
			Defenses = new Dictionary<string, LocationProtected>();
		}

		internal ArmorComponent(Template template) {
			Defenses = new Dictionary<string, LocationProtected>();

			foreach (var locationProtected in template.Defenses) {
				var resistances = new Dictionary<DamageType, int>(locationProtected.Resistances);

				// check for missing resistances and throws errors
				foreach (var value in Combat.DamageTypes.Values) {
					if (!resistances.ContainsKey(value)) {
						throw new ArgumentException("Resistances is missing a damage type");
					}
				}

				Defenses.Add(locationProtected.BodyPart, new LocationProtected(locationProtected.BodyPart, locationProtected.Coverage, resistances));
			}

			DonTime = template.DonTime;
		}

		public override Component Copy() {
			var armor = new ArmorComponent
			            {
			            		DonTime = DonTime
			            };
			foreach (var defense in Defenses) {
				armor.Defenses.Add(defense.Key, new LocationProtected(defense.Value.BodyPart, defense.Value.Coverage, new Dictionary<DamageType, int>(defense.Value.Resistances)));
			}			
			return new ArmorComponent();
		}
	}
}
