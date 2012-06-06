using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Utility;
using SKR.Gameplay.Talent;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Actors.NPC;
using SKR.Universe.Entities.Actors.NPC.AI;
using SKR.Universe.Entities.Actors.PC;
using SKR.Universe.Entities.Items;
using SKR.Universe.Factories;
using SKR.Universe.Location;

namespace SKR.Universe {
    public class World {        
        public static readonly int DefaultTurnSpeed = 100;
        public static readonly int TurnLengthInSeconds = 1;

        public static int SecondsToActionPoints(double seconds) {
            return TurnLengthInSeconds * (int) Math.Round(DefaultTurnSpeed / seconds);
        }

        public static int SpeedToSeconds(int speed) {
            return (DefaultTurnSpeed * TurnLengthInSeconds) / speed;
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

        private Factory<string, UniqueId, Item> ItemFactory;
        private Factory<UniqueId> IdFactory;
        private Factory<TileEnum, Tile> TileFactory;
        private Factory<Skill, Talent> TalentFactory; 

        public static World Instance { get; private set; }

        private World() {
            Calendar = new Calendar();

            entities = new List<IEntity> { Calendar };
            toAdds = new List<IEntity>();
            toRemove = new List<IEntity>();
            MessageBuffer = new List<string>();

            IdFactory = new SourceUniqueIdFactory();
            TalentFactory = new SourceTalentFactory();
            ItemFactory = new SourceItemFactory();
            TileFactory = new SourceTileFactory();
        }

        private void Temp() {
            var level = new Level(IdFactory.Construct(), new Size(80, 60), TileFactory.Construct(TileEnum.Unused));
            for (int x = 1; x < 10; x++) {
                for (int y = 1; y < 10; y++) {
                    level.SetTile(x, y, TileFactory.Construct(TileEnum.Grass));
                }
            }
            Npc npc1 = new Npc("target1", level) { Position = new Point(3, 4) };
            npc1.Intelligence = new SimpleIntelligence(npc1);
            level.Actors.Add(npc1);
            AddUpdateableObject(npc1);

            level.GenerateFov();

            Player = new Player(level) { Position = new Point(2, 2) };

            Player.AddItem(CreateItem("largeknife"));

            var i = CreateItem("brassknuckles");
            Player.AddItem(i);
            Player.Equip(BodyPartType.LeftHand, i);
        }

        public Talent GetTalent(Skill skill) {
            return TalentFactory.Construct(skill);
        }

        public void InsertMessage(string message) {
            MessageBuffer.Add(message);
            OnMessageAdded(new EventArgs<string>(message));
        }

        public static void Create() {            
            Instance = new World();     
            Instance.Temp();
        }

        public Item CreateItem(string key) {
            return ItemFactory.Construct(key, IdFactory.Construct());
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
