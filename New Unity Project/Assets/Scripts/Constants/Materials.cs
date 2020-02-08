using UnityEngine;

public enum Material
{
    Hydrogen,
    Helium,
    Lithium,
    Boron
}

public static class MaterialExtensions
{
    public static Color MaterialColor(this Material m)
    {
        switch (m)
        {
            case Material.Hydrogen:
                return Color.white;
            case Material.Helium:
                return Color.blue;
            case Material.Lithium:
                return Color.green;
            case Material.Boron:
                return Color.red;
            default:
                return Color.magenta;
        }
    }
}