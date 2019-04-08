using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Enforcer : Profession
    {
        protected internal Enforcer() { }

        public override string Name => "Enforcer";
        public override Color Color => Color.FromArgb(186, 70, 55);

		public override string[] ClassNanoList => new string[] { "challenger", "rage", "essence", "mongo" };
	}
}
