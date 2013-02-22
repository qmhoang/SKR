using System;
using System.Collections.Generic;
using DEngine.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SkrGame.Universe;
using SkrGame.Universe.Entities;

namespace SKR.UI.Menus {
	public class SkrWindowTemplate : WindowTemplate {
		public World World { get; set; }
	}

	public abstract class SkrWindow : Window {
		protected World World { get; private set; }

		public PromptWindowTemplate PromptTemplate { get; protected set; }

		protected SkrWindow(SkrWindowTemplate template) : base(template) {
			World = template.World;
		}



		#region Prompts
		protected void Options<T>(string message, IEnumerable<T> entities, Func<T, string> descriptionFunc, Action<T> action, Action fail) {
			ParentApplication.Push(
					new OptionsPrompt<T>(message,										 
										 entities,
										 descriptionFunc,
										 action,
										 fail,
										 PromptTemplate));

		}

		protected void Directions(string message, Action<Point> action) {
			ParentApplication.Push(
					new DirectionalPrompt(message,
										  World.Player.Get<GameObject>().Location,
										  action,
										  PromptTemplate));
		}

		protected void Targets(string message, Action<Point> action, MapPanel panel) {
			ParentApplication.Push(
					new TargetPrompt(message,
									 World.Player.Get<GameObject>().Location,
									 action,
									 panel,
									 PromptTemplate));
		}

		protected void Number(string message, int min, int max, int initial, Action<int> action) {
			ParentApplication.Push(new CountPrompt(message, action, max, min, initial, PromptTemplate));
		}

		protected void Boolean(string message, bool defaultBool, Action<bool> action) {
			ParentApplication.Push(new BooleanPrompt(message, defaultBool, action, PromptTemplate));
		}
		#endregion
	}
}