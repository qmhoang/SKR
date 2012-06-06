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
    public enum TalentType {
        Passive,
        TargetPerson,
        TargetPersonBodyPartWithItemComponent,
    }
    public class TalentTemplate {
        public Skill Skill { get; set; }
        public string Name { get; set; }
        public int InitialRank { get; set; }        
        public int MaxRank { get; set; }
        public int Range { get; set; }
        public int Radius { get; set; }

        /// <summary>
        /// RealRank will call the this function to calculate this talent's modified rank, it can be anything
        /// from a simple adding another attribute to it or something much more complicated
        /// <b>If left null, it will default to simply return the raw rank</b>
        /// </summary>
        public Func<Talent, Person, int> CalculateRealRank { get; set; }

        /// <summary>
        /// This is our dynamic action that allows a talent to specify 4 different options when invoking it
        /// dynamic just means its a object file so it can be anything.  You do not need to use all arg specified.
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// Arg3-7 (dynamic)    - These represent dynamic options that can be anything
        /// ArgN (ActionResult) - This is the return signature for our function, it represents if the action was completed/aborted, etc
        /// </summary>
        public Func<Talent, Person, Person, dynamic, dynamic, dynamic, dynamic, ActionResult> Action { get; set; }

        /// <summary>
        /// These represent our functions that will generate a list of options for us to choose
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// ArgN (dynamic[])    - This is a IEnumerable list of options
        /// </summary>
        public Func<Talent, Person, Person, IEnumerable<dynamic>> Args0 { get; set; }
        public Func<Talent, Person, Person, IEnumerable<dynamic>> Args1 { get; set; }
        public Func<Talent, Person, Person, IEnumerable<dynamic>> Args2 { get; set; }
        public Func<Talent, Person, Person, IEnumerable<dynamic>> Args3 { get; set; }        

        public static TalentTemplate CreateAttribute(Skill attrb) {
            return new TalentTemplate()
                       {
                               Skill = attrb,
                               Name = attrb.ToString(),
                               InitialRank = 10,
                               MaxRank = 20
                       };
        }
    }

    /// <summary>
    /// This is our talent class, its very complicated because it has to allow so many options for talents
    /// </summary>
    public class Talent {
        public const int MaxOptions = 4;

        public Person Owner { get; set; }
        public Skill Skill { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// raw rank represents this talent's skill, unmodified by anything
        /// </summary>
        public int RawRank { get; set; }  
        /// <summary>
        /// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
        /// from a simple adding another attribute to it or something much more complicated
        /// </summary>
        public int RealRank { get { return calculateRealRank(this, Owner); } }
        public int MaxRank { get; private set; }
        public int Range { get; private set; }
        public int Radius { get; private set; }

        private readonly Func<Talent, Person, int> calculateRealRank;

        private readonly Func<Talent, Person, Person, dynamic, dynamic, dynamic, dynamic, ActionResult> action;

        private readonly Func<Talent, Person, Person, IEnumerable<dynamic>> arg0Func;
        private readonly Func<Talent, Person, Person, IEnumerable<dynamic>> arg1Func;
        private readonly Func<Talent, Person, Person, IEnumerable<dynamic>> arg2Func;
        private readonly Func<Talent, Person, Person, IEnumerable<dynamic>> arg3Func;

        public bool ContainsArg0 { get { return arg0Func != null; } }
        public bool ContainsArg1 { get { return arg1Func != null; } }
        public bool ContainsArg2 { get { return arg2Func != null; } }
        public bool ContainsArg3 { get { return arg3Func != null; } }
        public bool ContainsArg(int argNumber) {
            switch (argNumber) {
                case 0:
                    return ContainsArg0;
                case 1:
                    return ContainsArg1;
                case 2:
                    return ContainsArg2;
                case 3:
                    return ContainsArg3;
            }
            return false;
        }

        public IEnumerable<dynamic> GetArg0Parameters(Person target) { return arg0Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg1Parameters(Person target) { return arg1Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg2Parameters(Person target) { return arg2Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg3Parameters(Person target) { return arg3Func(this, Owner, target); } 

        public IEnumerable<dynamic> GetArgParameters(Person target, int argNumber) {
            switch (argNumber) {
                case 0:
                    return GetArg0Parameters(target);
                case 1:
                    return GetArg1Parameters(target);
                case 2:
                    return GetArg2Parameters(target);
                case 3:
                    return GetArg3Parameters(target);
            }
            return null;
        }

        public ActionResult InvokeAction(Person target, dynamic arg0, dynamic arg1, dynamic arg2, dynamic arg3) {
            return action(this, Owner, target, arg0, arg1, arg2, arg3);
        }

        public Talent(TalentTemplate template) {
            Skill = template.Skill;
            Name = template.Name;
            RawRank = template.InitialRank;
            MaxRank = template.MaxRank;
            Range = template.Range;
            Radius = template.Radius;

            action = template.Action;
            arg0Func = template.Args0;
            arg1Func = template.Args1;
            arg2Func = template.Args2;
            arg3Func = template.Args3;

            calculateRealRank = (t, self) => t.RawRank;
        }
    }
}
