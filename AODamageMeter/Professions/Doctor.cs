using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Doctor : Profession
    {
        protected internal Doctor() { }

        public override string Name => "Doctor";
        public override Color Color => Color.FromArgb(198, 40, 40);

		public override string[] ClassNanoList => throw new System.NotImplementedException();
	}
}
