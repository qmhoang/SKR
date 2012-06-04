using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKR.Universe.Factories {
    public abstract class Factory<TProduct> {
        public abstract TProduct Construct();
    }
    public abstract class Factory<TKey, TProduct> {
        public abstract TProduct Construct(TKey identifier);
    }

    public abstract class Factory<TKey, TIdentifier, TProduct> {
        public abstract TProduct Construct(TKey key, TIdentifier uid);
    }
}
