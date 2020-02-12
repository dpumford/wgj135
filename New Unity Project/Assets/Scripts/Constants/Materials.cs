using System.Collections.Generic;
using UnityEngine;

public enum Material
{
    Hydrogen,
    Helium,
    Lithium,
    Boron,
    Turret,
}

public static class MaterialExtensions
{
    private static Dictionary<Material, Color> colorMap = new Dictionary<Material, Color>()
{
    { Material.Hydrogen, Color.white },
    { Material.Helium, new Color(0.3058824f, 0.3058824f, 0.7490196f) },
    { Material.Lithium, new Color(0.3098039f, 0.7490196f, 0.3803922f) },
    { Material.Boron, new Color(0.7490196f, 0, 0) },
    { Material.Turret, new Color(1f, 0.56f, 0) }
};

    public static Color MaterialColor(this Material m)
    {
        if (colorMap.ContainsKey(m))
        {
            return colorMap[m];
        }

        return Color.magenta;
    }
}