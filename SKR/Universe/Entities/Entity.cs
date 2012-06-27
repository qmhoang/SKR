using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using DEngine.Entities;

namespace SKR.Universe.Entities {
    public class GameEntity : EngineEntity {
        public GameEntity(RefId refId) : base(refId) {}
        public GameEntity(RefId refId, Point position, Dictionary<ComponentType, EntityComponent> components) : base(refId, position, components) { }
    }
}
