using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Universe.Entities.Items {
	public sealed class Food : Component {
		public TimeSpan EatTimeLength { get; private set; }
		public int Nutrition { get; private set; }
		public int Water { get; private set; }

		public Food(TimeSpan eatTimeLength, int nutrition, int water) {
			EatTimeLength = eatTimeLength;
			Nutrition = nutrition;
			Water = water;
		}

		public override Component Copy() {
			return new Food(EatTimeLength, Nutrition, Water);
		}
	}

	public sealed class Item : Component {
		public class Template {
			public int Weight { get; set; }
			public int Size { get; set; }
			public int Value { get; set; }
			public int Hardness { get; set; }
			public StackType StackType { get; set; }
		}

		public int Weight { get; private set; }
		public int Size { get; private set; }
		public int Value { get; private set; }

		public int Hardness { get; set; }

		private int amount;
		public int Amount {
			get {
				return StackType != StackType.Hard ? 1 : amount;
			}
			set {
				Contract.Requires<ArgumentException>(StackType == StackType.Hard);
				Contract.Requires<ArgumentException>(value > 0);
				amount = value;				
			}
		}

		public StackType StackType { get; private set; }

		public Item(int weight, int size, int value, int hardness, StackType stackType = StackType.None) {
			amount = 1;
			Weight = weight;
			Size = size;
			Value = value;
			Hardness = hardness;
			StackType = stackType;
		}

		public Item(Template template)
			: this(template.Weight, template.Size, template.Value, template.Hardness, template.StackType) { }

		public override Component Copy() {
			var copy = new Item(Weight, Size, Value, Hardness, StackType);
			if (copy.StackType == StackType.Hard)
				copy.Amount = Amount;
			return copy;
		}
		
		public static Entity Split(Entity e, int amount) {
			Contract.Requires<ArgumentNullException>(e != null, "e");
			Contract.Requires<ArgumentException>(e.Has<Item>());
			Contract.Requires<ArgumentException>(e.Get<Item>().StackType == StackType.Hard);
			Contract.Requires<ArgumentException>(amount > 0 && amount < e.Get<Item>().Amount);

			e.Get<Item>().Amount -= amount;

			var copy = e.Copy();
			copy.Get<Item>().Amount = amount;
			return copy;
		}

		[ContractInvariantMethod]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant() {
			Contract.Invariant(StackType == StackType.Hard ? amount > 0 : amount == 1);
			Contract.Invariant(Size >= 0);
		}
	}

	public enum StackType {
		None,
		Hard
	}

}