using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Actors;
using SKR.Universe.Entities.Items;
using SKR.Universe.Factories;

namespace SKR.Gameplay.Talent {    
    public enum TargetType {
        None,
        Directional,
        Positional,
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
        public delegate ActionResult ActionFunction(Talent talent, Actor self, Point target, params dynamic[] args);

       
        public ActionFunction ActionOnTargetFunction { get; set; }        

        /// <summary>
        /// These represent our functions that will generate a list of options for us to choose
        /// <b>The args must be used in order, you can't set Args0 and Args3, leaving Args1 and Args2 null</b>        
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// returns (dynamic[]) - This is a IEnumerable list of options
        /// </summary>
        public delegate IEnumerable<dynamic> GenerateArgsListFunction(Talent talent, Actor self, Point target);

        public List<GenerateArgsListFunction> Args { get; set; }        

        /// <summary>
        /// These are string transformation functions that will convert the display strings for our arguments, 
        /// <b>If null, ToString will be called on the dynamic object</b>
        /// Arg0 (Talent)       - The talent of calling this action
        /// Arg1 (Person)       - The person that used this talent
        /// Arg2 (Person)       - The target that is at the end of this talent
        /// Arg3 (dynamic)      - The arg that needs to be named
        /// returns (string)    - This is a IEnumerable list of options
        /// </summary>
        public delegate string ArgDesciptorFunction(Talent talent, Actor self, Point target, dynamic arg);

        public IEnumerable<ArgDesciptorFunction> ArgsDesciptor { get; set; }

        public TargetType RequiresTarget { get; set; }

        public delegate bool TalentPreUseCheck(Talent talent, Actor self);

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
        public int Range { get; private set; }
        public int Radius { get; private set; }
        
        #endregion

        private readonly TalentTemplate.CalculateRealRankFunction calculateRealRank;

        private readonly TalentTemplate.ActionFunction actionOnTargetFunc;        

        #region GenerateArgFunctions
        private readonly List<TalentTemplate.GenerateArgsListFunction> argsFuncs;


        /// <summary>
        /// Does the talent need arguments?
        /// </summary>
        public bool ContainsArg(int argNumber) {
            return argNumber < argsFuncs.Count;
        }

        public int NumberOfArgs {
            get { return argsFuncs.Count; }
            
        }

        /// <summary>
        /// Get the list of arguments for the action
        /// </summary>
        public IEnumerable<dynamic> GetArgsParameters(Point target, int argIndex) {
            return argsFuncs[argIndex].Invoke(this, Owner, target);
        }
        #endregion

        private readonly List<TalentTemplate.ArgDesciptorFunction> argsDesciptor;

        public string TransformArgToString(Point target, dynamic arg, int argsIndex) {
            if (argsDesciptor[argsIndex] != null)
                return argsDesciptor[argsIndex].Invoke(this, Owner, target, arg);
            return arg.ToString();
        }        


        public ActionResult InvokeAction(Point target, params dynamic[] args) {
            return actionOnTargetFunc(this, Owner, target, args);
        }

        public TargetType RequiresTarget { get; private set; }

        private TalentTemplate.TalentPreUseCheck onPreUse;

        public bool PreUseCheck() {
            return onPreUse(this, Owner);
        }

        public Talent(TalentTemplate template) {
            Skill = template.Skill;
            Name = template.Name;
            RawRank = template.InitialRank;
            MaxRank = template.MaxRank;
            Range = template.Range;
            Radius = template.Radius;            
            RequiresTarget = template.RequiresTarget;

            actionOnTargetFunc = template.ActionOnTargetFunction;
            argsFuncs = template.Args != null ? new List<TalentTemplate.GenerateArgsListFunction>(template.Args) : null;
                        
            argsDesciptor = template.ArgsDesciptor != null ? new List<TalentTemplate.ArgDesciptorFunction>(template.ArgsDesciptor) : null;

            calculateRealRank = template.CalculateRealRank;

            onPreUse = template.OnPreUse;

            if (template.CalculateRealRank == null)
                calculateRealRank = (t, self) => t.RawRank;

        }
    }
}
