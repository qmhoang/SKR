using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Utility;

namespace SKR.Universe.Factories {
    public abstract class UniqueIdFactory : Factory<UniqueId> {
        
    }
    public sealed class SimpleSourceUniqueIdFactory : UniqueIdFactory {
        private static long id = 0;
        public override UniqueId Construct() {
            return new UniqueId
                       {
                               Id = id++
                       };
        }
    }
}
