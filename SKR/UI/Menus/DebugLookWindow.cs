using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Core;
using Ogui.Core;
using Ogui.UI;
using SKR.UI.Gameplay;
using SkrGame.Universe;
using libtcod;

namespace SKR.UI.Menus {
	public class DebugLookWindow : LookWindow {
		private TreeView v;
		private World world;
		public DebugLookWindow(Point origin, World world, MapPanel panel, PromptWindowTemplate template) : base(origin, null, panel, template) {
			this.world = world;
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
			if (v != null && ContainsControl(v))
				RemoveControl(v);
			var entitiesAt = world.CurrentLevel.GetEntitiesAt(SelectedPosition).ToList();
			if (entitiesAt.Count > 0) {
				var nodes = new List<TreeNode>();

				foreach (var entity in entitiesAt) {
					var n = new TreeNode(entity.Id.ToString());

					foreach (var component in entity.Components) {
						n.AddChild(new TreeNode(component.GetType().ToString()));
					}

					nodes.Add(n);
				}

				v = new TreeView(new TreeViewTemplate
				{
					FrameTitle = false,
					TopLeftPos = Point.One,
					AutoSizeOverride = new Size(Size.Width - 3, Size.Height - 3),
					Items = nodes
				});
				
				AddControl(v);
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
