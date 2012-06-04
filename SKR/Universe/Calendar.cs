using DEngine.Actor;

namespace SKR.Universe {
    public class Calendar : IEntity {
        public int Second { get; private set; }
        public int Minute { get; private set; }
        public int Hour { get; private set; }
        public int DayOfTheYear { get; private set; }
        public int Month { get { return DayOfTheYear / 28 + 1; } }
        public int Week { get { return DayOfTheMonth / 7 + 1; } }
        public int DayOfTheMonth { get { return DayOfTheYear % 28; } }
        public int DayOfTheWeek { get { return DayOfTheYear % 7; } }

        public int ActionPoints { get; set; }

        public int Speed {
            get { return World.DefaultTurnSpeed; }
        }

        public void Update() {            
            
            Second += World.TurnLengthInSeconds;
            if (Second >= 60)
                Minute++;
            if (Minute >= 60)
                Hour++;
            if (Hour >= 24)
                DayOfTheYear++;

            Second %= 60;
            Minute %= 60;
            Hour %= 24;
            DayOfTheYear %= 364;
        }

        public bool Dead {
            get { return false; }
        }

        public void OnDeath() {

        }

        public bool Updateable {
            get { return ActionPoints > 0; }
        }
    }
}
