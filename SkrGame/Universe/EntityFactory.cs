using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using DEngine.Actor;
using DEngine.Components;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Gameplay.Combat;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Entities.Items.Components;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Locations;
using log4net;

namespace SkrGame.Universe {
	public class EntityFactory {
		public class Template : IEnumerable<Component> {
			private readonly Dictionary<Type, Component> components;

			public Template() {
				components = new Dictionary<Type, Component>();
			}

			public Template(IEnumerable<Component> components)
				: this(components.ToArray()) { }

			public Template(params Component[] components)
				: this() {
				if (components == null)
					return;

				Add(components);
			}

			/// <summary>
			/// Add a component to the template
			/// </summary>
			/// <param name="component"></param>
			public Template Add(Component component) {
				if (components.ContainsKey(component.GetType())) {
					components[component.GetType()] = component;
				} else {
					components.Add(component.GetType(), component);
				}
				return this;
			}

			/// <summary>
			/// Add a collection of components
			/// </summary>
			/// <param name="comps"></param>
			public Template Add(IEnumerable<Component> comps) {
				foreach (var component in comps) {
					Add(component);
				}
				return this;
			}

			/// <summary>
			/// Add a collection of components
			/// </summary>
			/// <param name="comps"></param>
			public Template Add(params Component[] comps) {
				return Add(comps as IEnumerable<Component>);

			}

			/// <summary>
			/// Get a component of a given type
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public T Get<T>() where T : Component {
				Component o;
				components.TryGetValue(typeof(T), out o);
				return (T)o;
			}

			/// <summary>
			/// Check if the template contains a given type
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public bool Has<T>() where T : Component {
				return components.ContainsKey(typeof(T));
			}

			#region IEnumerable

			public IEnumerator<Component> GetEnumerator() {
				return components.Values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			#endregion
		}

		private Dictionary<string, Template> compiledTemplates;
		private Dictionary<string, Tuple<string, Template>> inheritanceTemplates;
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public EntityFactory() {
			compiledTemplates = new Dictionary<string, Template>();
			inheritanceTemplates = new Dictionary<string, Tuple<string, Template>>();

			FeatureFactory.Init(this);
			ItemFactory.Init(this);
			Compile();
		}

		public void Compile() {
			Queue<string> queues = new Queue<string>();
			foreach (var t in inheritanceTemplates) {
				queues.Enqueue(t.Key);
			}

			while (queues.Count > 0) {
				string id = queues.Dequeue();
				var baseId = inheritanceTemplates[id].Item1;

				Logger.DebugFormat("Dequequeing {0}{1}.", id, String.IsNullOrEmpty(baseId) ? "" : string.Format(" which inherits from {0}", baseId));

				if (String.IsNullOrEmpty(baseId)) {
					Logger.DebugFormat("No inheritance, compiling {0}", id);
					compiledTemplates.Add(id, inheritanceTemplates[id].Item2);
				} else if (compiledTemplates.ContainsKey(baseId)) {
					Logger.DebugFormat("Inherited class found, compiling {0}", id);
					Template template = new Template(compiledTemplates[baseId]);
					template.Add(inheritanceTemplates[id].Item2);

					compiledTemplates.Add(id, template);
				} else {
					Logger.DebugFormat("No inherited class found, requequeing {0}", id);
					queues.Enqueue(id);
				}
			}

			inheritanceTemplates.Clear();
		}

		public IEnumerable<Component> Get(string id) {
			return compiledTemplates[id].Select(c => c.Copy());
		}

		public void Add(string refId, params Component[] comps) {
			var t = new Template(comps);
			t.Add(new ReferenceId(refId));
			Add(t);
		}

		public void Add(Template template) {
			Contract.Requires<ArgumentNullException>(template != null, "entity");			
			Contract.Requires<ArgumentException>(template.Has<ReferenceId>());
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(template.Get<ReferenceId>().RefId));

			compiledTemplates.Add(template.Get<ReferenceId>().RefId, template);
		}

		public void Inherits(string refId, string baseEntity, params Component[] comps) {
			var t = new Template(comps);
			t.Add(new ReferenceId(refId));

			Inherits(baseEntity, t);
		}

		public void Inherits(string baseEntity, Template template) {
			Contract.Requires<ArgumentNullException>(template != null, "entity");
			Contract.Requires<ArgumentException>(template.Has<ReferenceId>());
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(template.Get<ReferenceId>().RefId));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(baseEntity));

			inheritanceTemplates.Add(template.Get<ReferenceId>().RefId, new Tuple<string, Template>(baseEntity, template));
		}

		public Entity Create(string refId, EntityManager em) {
			return em.Create(Get(refId));
		}
	}
}