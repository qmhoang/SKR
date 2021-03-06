using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SKR.UI.Prompts;
using SkrGame.Universe;
using libtcod;

namespace SKR.UI.Menus.Debug {
	public class DebugLookWindow : LookWindow {
		private TreeView _v;
		private World _world;
		public DebugLookWindow(Point origin, World world, MapPanel panel, PromptWindowTemplate template) : base(origin, null, panel, template) {
			this._world = world;
		}

		protected override void OnSettingUp() {
			base.OnSettingUp();
		}

		protected override string Text {
			get {
				return String.Format("Use [789456123] to look around.");
			}
		}

		protected override void MovePosition(Direction direction) {
			base.MovePosition(direction);
			if (_v != null && ContainsControl(_v))
				RemoveControl(_v);
			var entitiesAt = _world.CurrentLevel.GetEntitiesAt(SelectedPosition).ToList();
			if (entitiesAt.Count > 0) {
				var nodes = new List<TreeNode>();

				foreach (var entity in entitiesAt) {
					var n = new TreeNode(entity.Id.ToString());

					foreach (var component in entity.Components) {
						n.AddChild(new TreeNode(component.GetType().ToString()));
					}

					nodes.Add(n);
				}

				_v = new TreeView(new TreeViewTemplate
				{
					FrameTitle = false,
					TopLeftPos = Point.One,
					AutoSizeOverride = new Size(Size.Width - 3, Size.Height - 3),
					Items = nodes
				});
				
				AddControl(_v);
			}			
		}

		protected override void Redraw() {
			base.Redraw();

			Canvas.Console.setBackgroundFlag(TCODBackgroundFlag.Alpha);
			Canvas.PrintString(0, 0, "* ", Pigments[PigmentType.Window].ReplaceForeground(ColorPresets.Blue));
			Canvas.Console.printRect(2, 0, Size.Width - 4, Size.Height - 2, Text);
		}
	}
}
