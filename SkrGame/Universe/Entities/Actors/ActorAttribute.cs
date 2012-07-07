using System;
using System.Collections.Generic;
using System.Linq;
using DEngine.Core;

namespace SKR.Universe.Entities.Actors {
//    public class ActorAttribute {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
//        /// </summary>
//        public ActorAttribute(int current) {
//            CurrentValue = current;
//        }
//
//        public ActorAttribute(ActorAttribute that) : this(that.Current) {}
//
//        protected int CurrentValue;
//
//        public virtual int Current {
//            get { return CurrentValue; }
//            set { CurrentValue = value; }
//        }
//    }
//
//    public class DerivedActorAttribute : ActorAttribute {
//        public ActorAttribute Base { get; private set; }
//
//        public override int Current {
//            get {
//                return base.Current + Base.Current;
//            }
//            set {
//                base.Current = value;
//            }
//        }
//
//        public DerivedActorAttribute(int value, ActorAttribute @base) : base(value) {
//            Base = @base;
//        }
//    }
//
//    // BUG - defaults may have circular dependency issues right now
//    public class SkillAttribute : ActorAttribute {
//        public List<Tuple<ActorAttribute, int>> Defaults { get; private set; }
//        public ActorAttribute Base { get; private set; }
//        public string Information { get; private set; }
//
//        public override int Current {
//            get {
//                int max = base.Current == 0 ? Int32.MinValue : base.Current + Base.Current;
//                var list = Defaults.Select(d => d.Item1.Current - d.Item2).Concat(new[] {max});
//                max = list.Max();
//
//                return max;
//            }
//            set {
//                base.Current = value;
//            }
//        }
//
//        public SkillAttribute(int value, ActorAttribute @base, string info, List<Tuple<ActorAttribute, int>> defaults)
//            : base(value) {
//            Defaults = defaults;
//            Information = info;
//            Base = @base;
//        }
//
//        public SkillAttribute(int value, ActorAttribute @base, string info)
//            : this(value, @base, info, new List<Tuple<ActorAttribute, int>>()) {            
//        }
//    }    
}