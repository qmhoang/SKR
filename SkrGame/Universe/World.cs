using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Core;
using SkrGame.Gameplay.Talent;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Factories;
using SkrGame.Universe.Location;

namespace SkrGame.Universe {
	public class World {
		/// <summary>
		/// default speed of entities, an entity with 2x speed can act twice as before an entity with normal speed
		/// </summary>
		public const int DEFAULT_SPEED = 100;					// 
		public const int TURN_LENGTH_IN_SECONDS = 1;			// how long is a turn in seconds

		public const int MEAN = 50;								// what is the mean score for an attribute
		public const int STANDARD_DEVIATION = 15;				// what is the stddev for an attribute score

		public const int TILE_LENGTH = 1;						// length of 1 square tile

		public static int SecondsToActionPoints(double seconds) {
			return (int)Math.Round((seconds * DEFAULT_SPEED) / TURN_LENGTH_IN_SECONDS);
		}

		public static double ActionPointsToSeconds(int ap) {
			return (double)(ap * TURN_LENGTH_IN_SECONDS) / DEFAULT_SPEED;
		}

		public static int SpeedToActionPoints(int speed) {
			return SecondsToActionPoints(SpeedToSeconds(speed));
		}

		public static int ActionPointsToSpeed(int ap) {
			return SecondsToSpeed(ActionPointsToSeconds(ap));
		}

		public static double SpeedToSeconds(int speed) {
			return (DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / (double)speed;
		}

		/// <summary>
		/// Convert how fast an action in seconds to its speed, where speed represents how fast an action is
		/// </summary>
		public static int SecondsToSpeed(double seconds) {
			return (int)((DEFAULT_SPEED * TURN_LENGTH_IN_SECONDS) / seconds);
		}

		public List<Message> MessageBuffer { get; private set; }

		public event EventHandler<EventArgs<Message>> MessageAdded;
		public Message CurrentMessage;

		public void OnMessageAdded(EventArgs<Message> e) {
			EventHandler<EventArgs<Message>> handler = MessageAdded;
			if (handler != null)
				handler(this, e);
		}

		private readonly List<IObject> entities;
		private readonly List<IObject> toAdds;
		private readonly List<IObject> toRemove;

		public Calendar Calendar { get; private set; }

		public Player Player { get; set; }

		private readonly ItemFactory itemFactory;        
		private readonly FeatureFactory featureFactory;
		private readonly TalentFactory talentFactory;
		private readonly MapFactory mapFactory;

		public static World Instance { get; private set; }

		public Level CurrentLevel {
			get { return Player.Level; }
		}

		private World() {
			Calendar = new Calendar();

			entities = new List<IObject> { Calendar };
			toAdds = new List<IObject>();
			toRemove = new List<IObject>();
			MessageBuffer = new List<Message>(); 
			
			talentFactory = new SourceTalentFactory();
			itemFactory = new SourceItemFactory();
			featureFactory = new SourceFeatureFactory();
			mapFactory = new SourceMapFactory(featureFactory);

			Rng.Seed(0);
		}

		private void Temp() {
			var level = mapFactory.Construct("TestHouse");
			level.GenerateFov();
			level.World = this;
			
			Player = new Player(level) { Position = new Point(0, 0) };

			Player.AddItem(CreateItem("largeknife"));

			var i = CreateItem("brassknuckles");
			Player.AddItem(i);
			Player.Equip(BodySlot.OffHand, i);

			Player.AddItem(CreateItem("glock22"));
			Player.AddItem(CreateItem(".40S&W"));
			var ammo = CreateItem(".40S&W");
			ammo.Amount = 20;
			level.AddItem(ammo, Player.Position);

			Npc npc1 = new Npc(level) { Position = new Point(3, 4) };
			npc1.Intelligence = new SimpleIntelligence(npc1);
			level.AddActor(npc1);
			AddEntity(npc1);
		}

		public Talent GetTalent(string skillRefId) {
			return talentFactory.Construct(skillRefId);
		}

		public void AddMessage(string message, MessageType priority = MessageType.Normal) {
			var currentMessage = new Message(message, priority);

			MessageBuffer.Add(currentMessage);
			OnMessageAdded(new EventArgs<Message>(currentMessage));
		}

		public static World Create() {
			Instance = new World();
			Instance.Temp();
			return Instance;
		}

		public Item CreateItem(string key) {
			return itemFactory.Construct(key);
		}

		public void AddEntity(IObject i) {
			toAdds.Add(i);
		}

		public void RemoveEntity(IObject i) {
			toRemove.Add(i);
		}

		public void AddActorToCurrentLevel(Actor actor) {
			AddEntity(actor);
			CurrentLevel.AddActor(actor);
		}

		public void RemoveActorFromCurrentLevel(Actor actor) {
			RemoveEntity(actor);
			CurrentLevel.RemoveActor(actor);
		}

		public void Update() {
			foreach (var entity in toRemove) {
				entities.Remove(entity);
			}

			entities.AddRange(toAdds);

			toAdds.Clear();
			toRemove.Clear();

			// update everything while the player cannot act
			// we iterate through every updateable adding their speed to their AP.  If its >0 they can act
			while (!Player.Updateable) {
				foreach (IObject entity in entities) {
					entity.ActionPoints += entity.Speed;
					if (entity.Dead)
						entity.OnDeath();
					else if (entity.Updateable)
						while (entity.Updateable)
							entity.Update();
				}
				Player.Update();
				Player.ActionPoints += Player.Speed;
			}
		}
	}
}
