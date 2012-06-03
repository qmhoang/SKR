using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Core;

namespace SKR.Universe.Entities.Actor {
    public class ActorAttribute {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ActorAttribute(int current) {
            CurrentValue = current;
        }

        public ActorAttribute(ActorAttribute that) : this(that.Current) {}

        protected int CurrentValue;

        public virtual int Current {
            get { return CurrentValue; }
            set { CurrentValue = value; }
        }
    }

    public class DerivedActorAttribute : ActorAttribute {
        public ActorAttribute Base { get; private set; }

        public override int Current {
            get {
                return base.Current + Base.Current;
            }
            set {
                base.Current = value;
            }
        }

        public DerivedActorAttribute(int value, ActorAttribute @base) : base(value) {
            Base = @base;
        }
    }

    public class SkillAttribute : ActorAttribute {
        private List<Pair<ActorAttribute, int>> Defaults;
        public ActorAttribute Base { get; private set; }

        public override int Current {
            get {
                int max = base.Current == 0 ? Int32.MinValue : base.Current + Base.Current;
                max = Defaults.Select(d => d.First.Current - d.Second).Concat(new[] {max}).Max();

                return max;
            }
            set {
                base.Current = value;
            }
        }

        public SkillAttribute(int value, List<Pair<ActorAttribute, int>> defaults, ActorAttribute @base)
            : base(value) {
            Defaults = defaults;
            Base = @base;
        }

        public SkillAttribute(int value, ActorAttribute @base)
            : this(value, new List<Pair<ActorAttribute, int>>(), @base) { }
    }
}