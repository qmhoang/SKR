using System;
using System.Collections.Generic;
using DEngine.Actor;
using DEngine.Core;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Gameplay.Talent.Components {
	public enum TargetType {
		None,
		Directional,
		Positional,
	}

	public class ActiveTalentArgTemplate {
		/// <summary>
		/// These represent our function that will generate a list of options for us to choose
		/// Arg0 (Talent) - The talent of calling this action
		/// Arg1 (Person) - The person that used this talent
		/// Arg2 (Person) - The target that is at the end of this talent
		/// returns (dynamic[]) - This is a IEnumerable list of options
		/// </summary>
		public delegate IEnumerable<dynamic> GenerateArgFunction(ActiveTalentComponent talent, Actor self, Point target);

		public GenerateArgFunction ArgFunction { get; set; }

		/// <summary>
		/// A string transformation function that will "describe" the arg,
		/// <b>If null, ToString will be called on the dynamic object</b>
		/// Arg0 (Talent) - The talent of calling this action
		/// Arg1 (Person) - The person that used this talent
		/// Arg2 (Person) - The target that is at the end of this talent
		/// Arg3 (dynamic) - The arg (which is returned by ArgFunction) that needs to be named
		/// returns (string) - the description of that arg
		/// </summary>
		public delegate string ArgDesciptionFunction(ActiveTalentComponent talent, Actor self, Point target, dynamic arg);

		public ArgDesciptionFunction ArgDesciptor { get; set; }

		/// <summary>
		/// If an arg is optional, it (the talent) will still continue if there is no options possible
		/// </summary>
		public bool Required { get; set; }

		public string PromptDescription { get; set; }

		// todo prompt failure strings

		public ActiveTalentArgTemplate() {
			Required = true;
		}
	}

	public class ActiveTalentArg {
		public ActiveTalentArgTemplate.GenerateArgFunction ArgFunction { get; private set; }
		public ActiveTalentArgTemplate.ArgDesciptionFunction ArgDesciptor { get; private set; }

		public bool Required { get; private set; }
		public string PromptDescription { get; private set; }

		public ActiveTalentArg(ActiveTalentArgTemplate template) {
			ArgFunction = template.ArgFunction;
			ArgDesciptor = template.ArgDesciptor;
			Required = template.Required;
			PromptDescription = template.PromptDescription;
		}
	}

	public class ActiveTalentTemplate : TalentComponentTemplate {
		/// <summary>
		/// This is our dynamic action that allows a talent to specify 4 different options when invoking it
		/// dynamic just means its a object file so it can be anything. You do not need to use all arg specified.
		/// Arg0 (Talent) - The talent of calling this action
		/// Arg1 (Person) - The person that used this talent
		/// Arg2 (Person) - The target that is at the end of this talent
		/// Arg3-n (dynamic) - These represent dynamic options that can be anything
		/// return - This is the return signature for our function, it represents if the action was completed/aborted, etc
		/// </summary>
		public delegate ActionResult ActionFunction(ActiveTalentComponent talent, Actor self, Point target, params dynamic[] args);

		public ActionFunction ActionOnTargetFunction { get; set; }

		public List<ActiveTalentArgTemplate> Args { get; set; }

		public TargetType RequiresTarget { get; set; }

		public int InitialRank { get; set; }
		public int MaxRank { get; set; }
		public Func<ActiveTalentComponent, Actor, int> Range { get; set; }
		public Func<ActiveTalentComponent, Actor, int> Radius { get; set; }

		public override TalentComponent Construct() {
			return new ActiveTalentComponent(this);
		}
	}

	public class ActiveTalentComponent : TalentComponent {
		private int rank;

		/// <summary>
		/// rank represents this talent's skill, unmodified by anything
		/// </summary>
		public int Rank {
			get { return rank; }
			set {
				if (rank < MaxRank && rank > 0)
					rank = value;
			}
		}

//		/// <summary>
//		/// RealRank will call the function supplied in the template to calculate this talent's modified rank, it can be anything
//		/// from a simple adding another attribute to it or something much more complicated
//		/// </summary>
//		public int RealRank { get { return calculateRealRank(this, Owner); }

		public int MaxRank { get; private set; }

		private readonly Func<ActiveTalentComponent, Actor, int> rangeFunc;
		private readonly Func<ActiveTalentComponent, Actor, int> radiusFunc;

		public int Range {
			get { return rangeFunc == null ? 0 : rangeFunc(this, Talent.Owner); }
		}

		public int Radius {
			get { return radiusFunc == null ? 0 : radiusFunc(this, Talent.Owner); }
		}

		private readonly ActiveTalentTemplate.ActionFunction actionOnTargetFunc;

		#region GenerateArgFunctions

		private readonly List<ActiveTalentArg> args;

		/// <summary>
		/// Does the argindex (arg<bold>0</bold>, arg<bold>1</bold>, arg<bold>2</bold>) exist within the talent
		/// </summary>
		public bool ContainsArg(int argNumber) {
			return argNumber < args.Count;
		}

		public int NumberOfArgs {
			get { return args.Count; }
		}

		public IEnumerable<ActiveTalentArg> Args {
			get { return args; }
		}

		#endregion

		public ActionResult InvokeAction(Point target, params dynamic[] args) {
			Talent.Owner.Talents.OnTalentUsed(new TalentUsedEvent(Talent.Owner, Talent));
			return actionOnTargetFunc(this, Talent.Owner, target, args);
		}

		public TargetType RequiresTarget { get; private set; }

		public ActiveTalentComponent(ActiveTalentTemplate template) {
			Rank = template.InitialRank;
			MaxRank = template.MaxRank;
			rangeFunc = template.Range;
			radiusFunc = template.Radius;
			RequiresTarget = template.RequiresTarget;

			actionOnTargetFunc = template.ActionOnTargetFunction;

			if (template.Args != null) {
				args = new List<ActiveTalentArg>();
				foreach (var argTemplate in template.Args)
					args.Add(new ActiveTalentArg(argTemplate));
			} else
				args = new List<ActiveTalentArg>();
		}
	}
}