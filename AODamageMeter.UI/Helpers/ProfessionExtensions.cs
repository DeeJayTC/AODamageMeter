using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace AODamageMeter.UI.Helpers
{
    public static class ProfessionExtensions
    {
        private static readonly Dictionary<Profession, string> _professionIconPaths = Profession.All
            .ToDictionary(p => p, p => $"/Icons/{p.GetType().Name}.png");

        private static readonly Dictionary<string, Color> _professionColors = Profession.All
            .ToDictionary(p => p.Name, p => Color.FromRgb(p.Color.R, p.Color.G, p.Color.B));

        public static string GetIconPath(this Profession profession)
		{
			if(profession.GetType() == typeof(Professions.Unknown))
			{
				return $"/Icons/Unknown.png";
			}
			return profession == null ? $"/Icons/NPC.png" : $"/Icons/{profession}.png";
		}
   

        public static Color GetColor(this Profession profession)
		{
			if (profession.GetType() == typeof(Professions.Unknown))
			{
				return Colors.White;
			}
			return _professionColors[profession.Name ?? Profession.Unknown.Name];
		}
    }
}
