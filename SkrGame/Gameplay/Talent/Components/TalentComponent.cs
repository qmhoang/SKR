namespace SkrGame.Gameplay.Talent.Components {
	public abstract class TalentComponent {
		public Talent Talent { get; set; }
	}
	
	public abstract class TalentComponentTemplate {
		public abstract TalentComponent Construct();
	}
}