using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKR.Universe.Factories {
    public class GuidFactory {
        private static long id = 0;
        public static long Generate() {
            return id++;
        }
    }
}
