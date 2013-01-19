using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Talent.Components;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Gameplay.Talent {
	public class TalentTemplate {
		public string RefId { get; set; }
		public string Name { get; set; }

		public Func<Talent, Entity, bool> OnPreUse { get; set; }

		/// <summary>
		/// Call when an actor first learns the talent (NOT when rank is increased)
		/// </summary>
		public Action<Talent, Entity> OnLearn { get; set; }

		/// <summary>
		/// Called when an actor loses the talent (usually when rank is 0) (NOT when rank is decreased)
		/// </summary>
		public Action<Talent, Entity> OnUnlearn { get; set; }

		public IEnumerable<TalentComponentTemplate> Components { get; set; }
	}


	/// <summary>
	/// This is our talent class, its very complicated because it has to allow so many options for talents
	/// </summary>
	public class Talent {
		#region Basic Stats

		public Entity Owner { get; set; }
		public string RefId { get; private set; }
		public string Name { get; private set; }

		#endregion

		public bool Is(Type action) {
			return components.ContainsKey(action);
		}

		public bool Is<T>() where T : TalentComponent {
			return components.ContainsKey(typeof(T));
		}

		/// <summary>
		/// Get Item's Component
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T As<T>() where T : TalentComponent {
			if (!Is<T>())
				throw new ArgumentException("This talent has no component for this", "action");
			return (T)components.Values.First(c => c is T);
		}

		private Dictionary<Type, TalentComponent> components;

		private Func<Talent, Entity, bool> onPreUse;

		public bool PreUseCheck() {
			return onPreUse == null || onPreUse(this, Owner);
		}

		private Action<Talent, Entity> onLearn;
		private Action<Talent, Entity> onUnlearn;

		public void OnLearn() {
			if (onLearn != null)
				onLearn(this, Owner);
		}

		public void OnUnlearn() {
			if (onUnlearn != null)
				onUnlearn(this, Owner);
		}

		public Talent(TalentTemplate template) {
			RefId = template.RefId;
			Name = template.Name;

			components = new Dictionary<Type, TalentComponent>();

			if (template.Components != null)
				foreach (var componentTemplate in template.Components) {
					var comp = componentTemplate.Construct();

					comp.Talent = this;
					components.Add(comp.GetType(), comp);
				}

			onPreUse = template.OnPreUse;

			onLearn = template.OnLearn;
			onUnlearn = template.OnUnlearn;
		}
	}
}
