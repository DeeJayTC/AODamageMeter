﻿using System.Drawing;

namespace AODamageMeter.Professions
{
    public class Shade : Profession
    {
        protected internal Shade() { }

        public override string Name => "Shade";
        public override Color Color => Color.FromArgb(61, 153, 165);

		public override string[] ClassNanoList => throw new System.NotImplementedException();
	}
}
