using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Utility;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using SKR.Universe.Factories;

namespace SKR.Gameplay.Talent {

    public abstract class Talent {
        public Person Actor { get; private set; }
        public Skill Skill { get; private set; }
        public string Name { get; private set; }
        public virtual int Rank { get; set; }

        public Talent(Person actor, Skill skill, string name, int initialRank) {
            Actor = actor;
            Skill = skill;
            Name = name;
            Rank = initialRank;    
        }
    }    

    public class AttributeTalent : Talent {
        public AttributeTalent(Person actor, Skill skill, string name, int initialRank = 10) : base(actor, skill, name, initialRank) {}
    }

    public class DerivedTalent : Talent {        

        public override int Rank {
            get { return base.Rank + Derivation(Actor); }
            set { base.Rank = value; }
        }

        public Func<Person, int> Derivation { get; private set; }

        public DerivedTalent(Person actor, Skill skill, string name, int initialRank, Func<Person, int> derivation)
            : base(actor, skill, name, initialRank) {
            Derivation = derivation;
        }
    }

    public class ActiveTalent : Talent {
        public ActiveTalent(Person actor, Skill skill, string name, int initialRank) : base(actor, skill, name, initialRank) {}

        public ActiveTalent(Person actor, Skill skill, string name, int initialRank, Func<Person, Person, bool> action) : base(actor, skill, name, initialRank) {
            Action = action;            
        }

//        public virtual bool Invoke(Person target, params ) {            
//            return (bool) Action.DynamicInvoke(Actor, target);
//        }

        public Delegate Action { get; protected set; }        
    }

    public class ActiveTalent<TOption1> : ActiveTalent {
        public Func<Person, Person, List<TOption1>> Option1List { get; private set; }


        public ActiveTalent(Person actor, Skill skill, string name, int initialRank, Func<Person, Person, TOption1, bool> action, Func<Person, Person, List<TOption1>> option1List)
            : base(actor, skill, name, initialRank) {
                Action = action;
            Option1List = option1List;
        }

    }

    public class ActiveTalent<TOption1, TOption2> : ActiveTalent {
        public Func<Person, Person, List<TOption1>> Option1List { get; private set; }
        public Func<Person, Person, List<TOption2>> Option2List { get; private set; }
        public ActiveTalent(Person actor, Skill skill, string name, int initialRank, Func<Person, Person, TOption1, TOption2, bool> action, Func<Person, Person, List<TOption1>> option1List, Func<Person, Person, List<TOption2>> option2List)
            : base(actor, skill, name, initialRank) {
            Action = action;
            Option1List = option1List;
            Option2List = option2List;
        }
    }

//    public class TalentAction {
//        public MulticastDelegate Act;
//    }
//
//    public class TalentAction<T1> : TalentAction {
//        public TalentAction(Func<Person, Person, T1> action) {
//            Act = action;
//        }
//    }
//
//    public class PassiveSkillAction : TalentAction {       
//        public PassiveSkillAction(Func<Person, int> rank) {
//            Act = rank;
//        }
//    }
//
//    public class TargetItemAction: TalentAction {
//        public Func<Person, Person, Item, bool> Action { get; private set; }         
//    }
//
//    public class TargetBodyPartAction: TalentAction {
//        public Func<Person, Person, BodyPart, bool> Action { get; private set; } 
//    }

//    public class TargetBodyPartWithWeaponAction : TalentAction {
//        public Func<Person, Person, BodyPart, ItemComponent, bool> Action { get; private set; }
//
//        public TargetBodyPartWithWeaponAction(Func<Person, Person, BodyPart, ItemComponent, bool> action) {
//            Action = action;
//        }
//    }
//
//    public class TargetPersonAction : TalentAction {
//        public Func<Person, Person, bool> Action { get; private set; }
//
//        public TargetPersonAction(Func<Person, Person, bool> action) {
//            Action = action;
//        }
//    }
}
