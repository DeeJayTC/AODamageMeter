using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Agent : Profession
    {
        protected internal Agent() { }

        public override string Name => "Agent";
        public override Color Color => Color.FromArgb(84, 117, 165);

		public override string[] ClassNanoList => new string[] { "feline grace", "enhanced senses", "take the shot", "ruse of taren", "concentration", "sureshot","false profession", "assume profession", "mimic profession"  };

	}
}
