using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Utility;

namespace SKR.Universe.Factories {
    public sealed class SourceUniqueIdFactory : Factory<UniqueId> {
        private static long id = 0;
        public override UniqueId Construct() {
            return new UniqueId
                       {
                               Id = id++
                       };
        }
    }
}
