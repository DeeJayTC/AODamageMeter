using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Keeper : Profession
    {
        protected internal Keeper() { }

        public override string Name => "Keeper";
        public override Color Color => Color.FromArgb(14, 140, 4);

		public override string[] ClassNanoList => throw new System.NotImplementedException();
	}
}
