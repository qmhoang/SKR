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

    public class ActiveAction<T1> {
        public Func<Person, Person, T1, bool> Action { get; private set; }

        public ActiveAction(Func<Person, Person, T1, bool> action) {
            Action = action;
        }
    }

//    public interface IAction<out T1, out T2> {
//        T1 Option1 { get; }
//        T2 Option2 { get; }
//        Func<T1, T2, bool> doSometh;
//    }
    public enum OptionMethods {
        Options,        
    }

    public class ActiveAction<T1, T2> {
        public OptionMethods OptionMethod1 { get; private set; }
        public OptionMethods OptionMethod2 { get; private set; }

        public Func<Person, Person, List<T1>> Options1;
        public Func<Person, Person, List<T2>> Options2;
        public Func<Person, Person, T1, T2, bool> Action { get; private set; }

        public ActiveAction(Func<Person, Person, List<T1>> options1, Func<Person, Person, List<T2>> options2, OptionMethods optionMethod1, OptionMethods optionMethod2, Func<Person, Person, T1, T2, bool> action) {
            Options1 = options1;
            Options2 = options2;
            OptionMethod1 = optionMethod1;
            OptionMethod2 = optionMethod2;
            Action = action;
        }
        
    }

    public class ActiveAction<T1, T2, T3, T4> {
        public Func<T1> Option1;
        public Func<T2> Option2;
        public Func<T3> Option3;
        public Func<T4> Option4;
        public Func<T1, T2, T3, T4, bool> Action { get; private set; }

        public ActiveAction(Func<T1, T2, T3, T4, bool> action) {
            Action = action;
        }
    }

//    public sealed class TalentFactory : Factory<Skill, Talent> {
//        
//        public override Talent Construct(Skill identifier) {        
//
//            switch (identifier) {
//                case Skill.Attack:
//                    break;
////                    return new Talent(identifier, "Attack", 1, 1, 0,
////                                      new ActiveAction<ItemComponent>(delegate(Person actor, Person target, )
////                                                           {
////                                                               return true;
////                                                           }));
//                case Skill.TargetAttack:
//                    return new Talent(identifier, "Attack", 1, 1, 0,
//                                      new ActiveAction<ItemComponent, BodyPart>(
//                                              delegate(Person actor, Person target, ItemComponent weapon, BodyPart bodyPart)
//                                                  {
//
//                                                  }));
//                case Skill.Brawling:
//                    break;
//                case Skill.Knife:
//                    break;
//                case Skill.Axe:
//                    break;
//                case Skill.Longsword:
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("identifier");
//            }
//        }
//    } 
}
