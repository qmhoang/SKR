﻿using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actor.PC;
using SKR.Universe.Location;

namespace SKR.Universe {
    public class World {
        public static readonly int DefaultTurnSpeed = 100;
        public static readonly int TurnLengthInSeconds = 1;

        private static World instance;

        public List<string> MessageBuffer { get; private set; }

        public event EventHandler<EventArgs<string>> MessageAdded;

        public void OnMessageAdded(EventArgs<string> e) {
            EventHandler<EventArgs<string>> handler = MessageAdded;
            if (handler != null)
                handler(this, e);
        }

        private readonly List<IEntity> updateables;
        private readonly List<IEntity> toAdds;
        private readonly List<IEntity> toRemove;

        public Calendar Calendar { get; private set; }

        public Player Player { get; set; }

        public static World Instance {
            get {
                return instance;
            }
        }

        private World(Player player) {
            Calendar = new Calendar();

            updateables = new List<IEntity> { Calendar };
            toAdds = new List<IEntity>();
            toRemove = new List<IEntity>();
            MessageBuffer = new List<string>();
            Player = player;
        }

        public void InsertMessage(string message) {
            MessageBuffer.Add(message);
            OnMessageAdded(new EventArgs<string>(message));
        }

        public static void Create() {
            var level = new Level(new Size(80, 60));
            for (int x = 1; x < 10; x++) {
                for (int y = 1; y < 10; y++) {
                    level.SetTile(x, y, Tile.Grass);
                }
            }
            level.GenerateFov();
            instance = new World(new Player(level)) {Player = {Position = new Point(2, 2)}};
        }

        public void AddUpdateableObject(IEntity i) {
            toAdds.Add(i);
        }

        public void RemoveUpdateableOjects(IEntity i) {
            toRemove.Add(i);
        }

        public void Update() {
            foreach (var updateable in toRemove) {
                updateables.Remove(updateable);
            }

            updateables.AddRange(toAdds);

            toAdds.Clear();
            toRemove.Clear();

            // update everything while the player cannot act
            // we iterate through every updateable adding their speed to their AP.  If its >0 they can act
            while (Player.ActionPoints <= 0) {
                foreach (IEntity a in updateables) {
                    a.ActionPoints += a.Speed;
                    if (a.ActionPoints > 0 && !a.Dead)
                        a.Update();
                }
                Player.Update();
                Player.ActionPoints += Player.Speed;
            }

            updateables.RemoveAll(actor => actor.Dead);
        }    
    }
}
