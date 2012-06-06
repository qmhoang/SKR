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

    public class Talent {
        public Skill Skill { get; private set; }
        public string Name { get; private set; }
        public int RawRank { get; set; }
        public int MaxRank { get; private set; }
        public int Range { get; private set; }
        public int Radius { get; private set; }        
        public ITalentAction Action { get; private set; }

        public Talent(Skill skill, string name, int maxRank, int range, int radius, ITalentAction action) {
            Skill = skill;
            Name = name;
            RawRank = 0;
            MaxRank = maxRank;
            Range = range;
            Radius = radius;
            Action = action;                            
        }
    }    

    public interface ITalentAction {
        
    }

    public class PassiveSkillAction : ITalentAction {
        public Func<Person, int> RealRank { get; private set; }

        public PassiveSkillAction(Func<Person, int> rank) {
            RealRank = rank;
        }
    }

    public class TargetItemAction: ITalentAction {
        public Func<Person, Person, Item, bool> Action { get; private set; }         
    }

    public class TargetBodyPartAction: ITalentAction {
        public Func<Person, Person, BodyPart, bool> Action { get; private set; } 
    }

    public class TargetBodyPartWithWeaponAction : ITalentAction {
        public Func<Person, Person, BodyPart, ItemComponent, bool> Action { get; private set; }

        public TargetBodyPartWithWeaponAction(Func<Person, Person, BodyPart, ItemComponent, bool> action) {
            Action = action;
        }
    }

    public class TargetPersonAction : ITalentAction {
        public Func<Person, Person, bool> Action { get; private set; }

        public TargetPersonAction(Func<Person, Person, bool> action) {
            Action = action;
        }
    }
}
