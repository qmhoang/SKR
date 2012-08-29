using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Gameplay.Talent;
using SkrGame.Universe.Entities.Actors;
using SkrGame.Universe.Entities.Actors.NPC;
using SkrGame.Universe.Entities.Actors.NPC.AI;
using SkrGame.Universe.Entities.Actors.PC;
using SkrGame.Universe.Entities.Items;
using SkrGame.Universe.Factories;

namespace SkrGame.Universe {
    public class World {
        public const int DefaultTurnSpeed = 100;
        public const int TurnLengthInSeconds = 1;
        
        public const int Mean = 0;
        public const int StandardDeviation = 3;

        public static int SecondsToActionPoints(double seconds) {
            return TurnLengthInSeconds * (int) Math.Round(DefaultTurnSpeed / seconds);
        }

        public static double SpeedToSeconds(int speed) {
            return (DefaultTurnSpeed * TurnLengthInSeconds) / (double) speed;
        }

        public static int SecondsToSpeed(double seconds) {
            return (int) ((DefaultTurnSpeed * TurnLengthInSeconds) / seconds);
        }

        public List<string> MessageBuffer { get; private set; }

        public event EventHandler<EventArgs<string>> MessageAdded;

        public void OnMessageAdded(EventArgs<string> e) {
            EventHandler<EventArgs<string>> handler = MessageAdded;
            if (handler != null)
                handler(this, e);
        }

        private readonly List<IEntity> entities;
        private readonly List<IEntity> toAdds;
        private readonly List<IEntity> toRemove;

        public Calendar Calendar { get; private set; }

        public Player Player { get; set; }

        private readonly ItemFactory ItemFactory;        
        private readonly FeatureFactory FeatureFactory;
        private readonly TalentFactory TalentFactory;
        private readonly MapFactory MapFactory;

        public static World Instance { get; private set; }

        private World() {
            Calendar = new Calendar();

            entities = new List<IEntity> { Calendar };
            toAdds = new List<IEntity>();
            toRemove = new List<IEntity>();
            MessageBuffer = new List<string>();
            
            TalentFactory = new SourceTalentFactory();
            ItemFactory = new SourceItemFactory();
            FeatureFactory = new SourceFeatureFactory();
            MapFactory = new SourceMapFactory(FeatureFactory);

            Rng.Seed(0);
        }

        private void Temp() {
            var level = MapFactory.Construct("TestMotel");
            level.World = this;
            Npc npc1 = new Npc(level) { Position = new Point(3, 4) };            
            npc1.Intelligence = new SimpleIntelligence(npc1);
//            level.AddActor(npc1);
//            AddUpdateableObject(npc1);

            level.GenerateFov();

            Player = new Player(level) { Position = new Point(0, 0) };            

            Player.AddItem(CreateItem("largeknife"));

            var i = CreateItem("brassknuckles");
            Player.AddItem(i);
            Player.Equip(BodyPartType.LeftHand, i);

            Player.AddItem(CreateItem("glock22"));
            Player.AddItem(CreateItem(".40S&W"));
        }

        public Talent GetTalent(Skill skill) {
            return TalentFactory.Construct(skill);
        }

        public void InsertMessage(string message) {
            MessageBuffer.Add(message);
            OnMessageAdded(new EventArgs<string>(message));
        }

        public static World Create() {            
            Instance = new World();     
            Instance.Temp();
            return Instance;
        }

        public Item CreateItem(string key) {
            return ItemFactory.Construct(key);
        }

        public void AddUpdateableObject(IEntity i) {
            toAdds.Add(i);
        }

        public void RemoveUpdateableOjects(IEntity i) {
            toRemove.Add(i);
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
                foreach (IEntity entity in entities) {
                    entity.ActionPoints += entity.Speed;
                    if (entity.Updateable && !entity.Dead)
                        entity.Update();
                }
                Player.Update();
                Player.ActionPoints += Player.Speed;
            }

            entities.RemoveAll(actor => actor.Dead);
        }
    }
}
