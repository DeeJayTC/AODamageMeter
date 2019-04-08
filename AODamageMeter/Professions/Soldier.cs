using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Soldier : Profession
    {
        protected internal Soldier() { }

        public override string Name => "Soldier";
        public override Color Color => Color.FromArgb(155, 132, 85);

		public override string[] ClassNanoList => throw new System.NotImplementedException();
	}
}
