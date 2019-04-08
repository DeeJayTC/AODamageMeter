using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Trader : Profession
    {
        protected internal Trader() { }

        public override string Name => "Trader";
        public override Color Color => Color.FromArgb(143, 72, 126);

		public override string[] ClassNanoList => throw new System.NotImplementedException();
	}
}
