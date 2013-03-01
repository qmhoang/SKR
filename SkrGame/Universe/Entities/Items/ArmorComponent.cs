using System;
using System.Collections.Generic;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;

namespace SkrGame.Universe.Entities.Items {
	public sealed class ArmorComponent : Component {
		public class Template {
			public List<Part> Defenses { get; set; }

			public int DonTime { get; set; } // todo move to equipable?
		}

		public class Part {
			public string BodyPart { get; private set; }
			/// <summary>
			/// How much area the armor covers that body part.
			/// </summary>		
			public int Coverage { get; private set; }
			public StaticDictionary<DamageType, int> Resistances { get; private set; }

			public Part(string bodyPart, int coverage, StaticDictionary<DamageType, int> resistances) {
				BodyPart = bodyPart;
				Coverage = coverage;
				Resistances = resistances;
			}

			public Part(string bodyPart, int coverage, Dictionary<DamageType, int> resistances) {
				BodyPart = bodyPart;
				Coverage = coverage;
				Resistances = new StaticDictionary<DamageType, int>(resistances);
			}
		}
		public StaticDictionary<string, Part> Defenses { get; private set; }

		public int DonTime { get; private set; }

		private ArmorComponent() {}

		internal ArmorComponent(Template template) {
			var d = new Dictionary<string, Part>();

			foreach (var locationProtected in template.Defenses) {
				var resistances = new StaticDictionary<DamageType, int>(locationProtected.Resistances);

				// check for missing resistances and throws errors
				foreach (var value in Combat.DamageTypes.Values) {
					if (!resistances.ContainsKey(value)) {
						throw new ArgumentException("Resistances is missing a damage type");
					}
				}

				d.Add(locationProtected.BodyPart, new Part(locationProtected.BodyPart, locationProtected.Coverage, resistances));
			}

			Defenses = new StaticDictionary<string, Part>(d);

			DonTime = template.DonTime;
		}

		public override Component Copy() {
			return new ArmorComponent
			            {
			            		DonTime = DonTime,
			            		Defenses = new StaticDictionary<string, Part>(Defenses)
			            };			
		}
	}
}
