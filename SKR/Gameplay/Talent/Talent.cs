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
//        public int Range { get; set; }
//        public int Radius { get; set; }        

        /// <summary>
        /// RealRank will call the this function to calculate this talent's modified rank, it can be anything
        /// from a simple adding another attribute to it or something much more complicated
        /// <b>If left null, it will default to simply return the raw rank</b>
        /// </summary>        
        public delegate int CalculateRealRankFunction(Talent talent, Actor self);
        
        public CalculateRealRankFunction CalculateRealRank { get; set; }


        /// <summary>
        /// This is our dynamic action that allows a talent to specify 4 different options when invoking it
        /// dynamic just means its a object file so it can be anything.  You do not need to use all arg specified.
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// Arg3-7 (dynamic)    - These represent dynamic options that can be anything
        /// return              - This is the return signature for our function, it represents if the action was completed/aborted, etc
        /// </summary>
        public delegate ActionResult ActionFunction(Talent talent, Actor self, Actor target, dynamic arg0, dynamic arg1, dynamic arg2, dynamic arg3);

       
        public ActionFunction ActionOnTargetFunction { get; set; }        

        /// <summary>
        /// These represent our functions that will generate a list of options for us to choose
        /// <b>The args must be used in order, you can't set Args0 and Args3, leaving Args1 and Args2 null</b>        
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// returns (dynamic[]) - This is a IEnumerable list of options
        /// </summary>
        public delegate IEnumerable<dynamic> GenerateArgsListFunction(Talent talent, Actor self, Actor target);

        public GenerateArgsListFunction Args0 { get; set; }
        public GenerateArgsListFunction Args1 { get; set; }
        public GenerateArgsListFunction Args2 { get; set; }
        public GenerateArgsListFunction Args3 { get; set; }

        /// <summary>
        /// These are string transformation functions that will convert the display strings for our arguments, 
        /// <b>If null, ToString will be called on the dynamic object</b>
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// Arg3 (dynamic)      - The arg that needs to be named
        /// returns (string)    - This is a IEnumerable list of options
        /// </summary>
        public delegate string ArgDesciptorFunction(Talent talent, Actor self, Actor target, dynamic arg);

        public ArgDesciptorFunction Arg0Desciptor { get; set; }
        public ArgDesciptorFunction Arg1Desciptor { get; set; }
        public ArgDesciptorFunction Arg2Desciptor { get; set; }
        public ArgDesciptorFunction Arg3Desciptor { get; set; }

        public bool RequiresTarget { get; set; }

        public delegate bool TalentPreUseCheck(Talent talent, Actor self, Actor target);

        public TalentPreUseCheck OnPreUse { get; set; }

        public static TalentTemplate CreateAttribute(Skill attrb) {
            return new TalentTemplate()
                       {
                               Skill = attrb,
                               Name = attrb.ToString(),
                               InitialRank = 0,
                               MaxRank = 20
                       };
        }
    }

    /// <summary>
    /// This is our talent class, its very complicated because it has to allow so many options for talents
    /// </summary>
    public class Talent {
        public const int MaxOptions = 4;

        #region Basic Stats
        public Actor Owner { get; set; }
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
//        public int Range { get; private set; }
//        public int Radius { get; private set; }
        
        #endregion

        private readonly TalentTemplate.CalculateRealRankFunction calculateRealRank;

        private readonly TalentTemplate.ActionFunction actionOnTargetFunc;        

        #region GenerateArgFunctions
        private readonly TalentTemplate.GenerateArgsListFunction arg0Func;
        private readonly TalentTemplate.GenerateArgsListFunction arg1Func;
        private readonly TalentTemplate.GenerateArgsListFunction arg2Func;
        private readonly TalentTemplate.GenerateArgsListFunction arg3Func;

        /// <summary>
        /// Does the talent need arguments?
        /// </summary>
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

        public int NumberOfArgs {
            get {
                if (arg0Func == null)
                    return 0;
                if (arg1Func == null)
                    return 1;
                if (arg2Func == null)
                    return 2;
                if (arg3Func == null)
                    return 3;

                return 0;
            }
            
        }

        /// <summary>
        /// Get the list of arguments for the action
        /// </summary>
        public IEnumerable<dynamic> GetArg0Parameters(Actor target) { return arg0Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg1Parameters(Actor target) { return arg1Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg2Parameters(Actor target) { return arg2Func(this, Owner, target); }
        public IEnumerable<dynamic> GetArg3Parameters(Actor target) { return arg3Func(this, Owner, target); } 

        public IEnumerable<dynamic> GetArgParameters(Actor target, int argNumber) {
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
        #endregion

        private readonly TalentTemplate.ArgDesciptorFunction arg0Desciptor;
        private readonly TalentTemplate.ArgDesciptorFunction arg1Desciptor;
        private readonly TalentTemplate.ArgDesciptorFunction arg2Desciptor;
        private readonly TalentTemplate.ArgDesciptorFunction arg3Desciptor;

        public string DescribeArg0(Actor target, dynamic arg) { return arg0Desciptor(this, Owner, target, arg); }
        public string DescribeArg1(Actor target, dynamic arg) { return arg1Desciptor(this, Owner, target, arg); }
        public string DescribeArg2(Actor target, dynamic arg) { return arg2Desciptor(this, Owner, target, arg); }
        public string DescribeArg3(Actor target, dynamic arg) { return arg3Desciptor(this, Owner, target, arg); }

        public string TransformArgToString(Actor target, dynamic arg, int argNumber) {
            switch (argNumber) {
                case 0:
                    return DescribeArg0(target, arg);
                case 1:
                    return DescribeArg1(target, arg);
                case 2:
                    return DescribeArg2(target, arg);
                case 3:
                    return DescribeArg3(target, arg);
            }
            return null;
        }        


        public ActionResult InvokeAction(Actor target, dynamic arg0 = null, dynamic arg1 = null, dynamic arg2 = null, dynamic arg3 = null) {
            return actionOnTargetFunc(this, Owner, target, arg0, arg1, arg2, arg3);
        }

        public bool RequiresTarget { get; private set; }

        private TalentTemplate.TalentPreUseCheck onPreUse;

        public bool PreUseCheck(Actor target) {
            return onPreUse(this, Owner, target);
        }

        public Talent(TalentTemplate template) {
            Skill = template.Skill;
            Name = template.Name;
            RawRank = template.InitialRank;
            MaxRank = template.MaxRank;
//            Range = template.Range;
//            Radius = template.Radius;            
            RequiresTarget = template.RequiresTarget;

            actionOnTargetFunc = template.ActionOnTargetFunction;
            arg0Func = template.Args0;
            arg1Func = template.Args1;
            arg2Func = template.Args2;
            arg3Func = template.Args3;            
            
            arg0Desciptor = template.Arg0Desciptor;
            arg1Desciptor = template.Arg1Desciptor;
            arg2Desciptor = template.Arg2Desciptor;
            arg3Desciptor = template.Arg3Desciptor;

            if (arg0Desciptor == null)
                arg0Desciptor = (t, self, target, arg) => arg.ToString();
            if (arg1Desciptor == null)
                arg1Desciptor = (t, self, target, arg) => arg.ToString();
            if (arg2Desciptor == null)
                arg2Desciptor = (t, self, target, arg) => arg.ToString();
            if (arg3Desciptor == null)
                arg3Desciptor = (t, self, target, arg) => arg.ToString();

            calculateRealRank = template.CalculateRealRank;

            onPreUse = template.OnPreUse;

            if (template.CalculateRealRank == null)
                calculateRealRank = (t, self) => t.RawRank;

        }
    }
}
