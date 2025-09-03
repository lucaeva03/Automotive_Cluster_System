using UnityEngine;

public static class MathUtility
{
    // Estende la classe float con metodo Remap - semplificazione codice
    public static float Remap(this float In, Vector2 InMinMax, Vector2 OutMinMax)
    {
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    }
}
