using System;
using System.Collections.Generic;
using System.Reflection;
using DEngine.Actor;
using DEngine.Core;
using SKR.Universe.Entities.Items;
using SKR.Universe.Location;
using log4net;

namespace SKR.Universe.Entities.Actor {
    public enum Attribute {
        Strength,
        Agility,
        Constitution,
        Intellect,
        Cunning,
        Resolve,
        Presence,
        Grace,
        Composure,
        None
    }

    public enum Skill {
        Brawling,
        Knife
    }


    public class ActorCharacteristics {
        private Dictionary<Attribute, ActorAttribute> Attributes;
        private Dictionary<BodyPartType, BodyPart> BodyParts;
        private Dictionary<Skill, SkillAttribute> Skills;

        public int Health { get; set; }
        public int MaxHealth { get; protected set; }

        public MeleeComponent Punch;
        public MeleeComponent Kick;

        public ActorCharacteristics() {
            Attributes = new Dictionary<Attribute, ActorAttribute>
                             {
                                     {Attribute.Strength, new ActorAttribute(10)},
                                     {Attribute.Agility, new ActorAttribute(10)},
                                     {Attribute.Constitution, new ActorAttribute(10)},
                                     {Attribute.Intellect, new ActorAttribute(10)},
                                     {Attribute.Cunning, new ActorAttribute(10)},
                                     {Attribute.Resolve, new ActorAttribute(10)},
                                     {Attribute.Presence, new ActorAttribute(10)},
                                     {Attribute.Grace, new ActorAttribute(10)},
                                     {Attribute.Composure, new ActorAttribute(10)},
                             };

            BodyParts = new Dictionary<BodyPartType, BodyPart>
                            {
                                    {BodyPartType.Body, new BodyPart("Torso", BodyPartType.Body, Health, 0)},
                                    {BodyPartType.Head, new BodyPart("Head", BodyPartType.Head, Health, -5)},
                                    {BodyPartType.RightArm, new BodyPart("Right Arm", BodyPartType.RightArm, Health / 2, -2)},
                                    {BodyPartType.LeftArm, new BodyPart("Left Arm", BodyPartType.LeftArm, Health / 2, -2)},
                                    {BodyPartType.RightHand, new BodyPart("Right Hand", BodyPartType.RightHand, Health / 3, -4)},
                                    {BodyPartType.LeftHand, new BodyPart("Left Hand", BodyPartType.LeftHand, Health / 3, -4)},
                                    {BodyPartType.Leg, new BodyPart("Leg", BodyPartType.Leg, Health / 2, -2)},
                                    {BodyPartType.Feet, new BodyPart("Feet", BodyPartType.Feet, Health / 3, -4)}
                            };

            Skills = new Dictionary<Skill, SkillAttribute>
                         {
                                 {Skill.Brawling, new SkillAttribute(0, Attributes[Attribute.Agility], new List<Pair<ActorAttribute, int>>
                                                                                                         {
                                                                                                                 new Pair<ActorAttribute, int>(Attributes[Attribute.Agility], 2)
                                                                                                         })},
                                 {Skill.Knife, new SkillAttribute(0, Attributes[Attribute.Agility])}
                         };

            Punch = new MeleeComponent(Skill.Brawling, 0, -1, DamageType.Crush, 1, 100, 1, 0, 0, "punch");
            Kick = new MeleeComponent(Skill.Brawling, -2, 1, DamageType.Crush, 1, 100, 1, 0, -100, "kick");
        }


        public int GetAttribute(Attribute attrb) {
            return Attributes[attrb].Current;
        }

        public int GetSkill(Skill skill) {
            return Skills[skill].Current;
        }

        public BodyPart GetBodyPart(BodyPartType bp) {
            return BodyParts[bp];
        }

        public IEnumerable<BodyPart> BodyPartsList { get { return BodyParts.Values; } } 
    }

    public abstract class Person : AbstractActor {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActorCharacteristics Characteristics { get; private set; }

        public Level Level { get; private set; }
        public World World { get { return World.Instance; } }

        private string myName;

        public override string Name {
            get { return myName; }
        }

        public override bool Dead {
            get { return Characteristics.Health <= 0; }
        }

        protected Person(string name, Level level) {
            myName = name;
            Level = level;
            Characteristics = new ActorCharacteristics();
        }

        public void CalculateFov() {
            Level.Fov.computeFov(Position.X, Position.Y, SightRadius);
        }

        public override ActionResult Move(int dx, int dy) {
            return Move(new Point(dx, dy));
        }

        public override ActionResult Move(Point p) {
            Point nPos = p + Position;

            if (!Level.IsWalkable(nPos))
                return ActionResult.Aborted;

            if (Level.DoesActorExistAtLocation(nPos)) {
                Person m = Level.GetActorAtLocation(nPos);

//todo melee combat
            } else {
                Position = nPos;
            }

            ActionPoints -= Universe.World.DefaultTurnSpeed;

            return ActionResult.Success;
        }

        public override ActionResult Wait() {
            ActionPoints -= Universe.World.DefaultTurnSpeed;
            return ActionResult.Success;
        }

        public void Hurt(BodyPartType bp, int amount) {
            Characteristics.GetBodyPart(bp).Health -= amount;
            Characteristics.Health -= amount;
            OnHealthChange(new EventArgs<int>(-amount));
        }

        public void Heal(int amount) {
            amount = Math.Min(amount, Characteristics.MaxHealth - Characteristics.Health);
            Characteristics.Health -= amount;
            OnHealthChange(new EventArgs<int>(amount));
        }

        public event EventHandler<EventArgs<int>> HealthChanged;

        protected void OnHealthChange(EventArgs<int> e) {
            var handler = HealthChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
