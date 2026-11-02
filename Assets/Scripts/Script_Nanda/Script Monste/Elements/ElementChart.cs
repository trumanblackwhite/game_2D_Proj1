using UnityEngine;

public static class ElementChart
{
    public static float Multiplier(Element attacker, Element defender)
    {
        // Pyro > Sprout, Aqua > Pyro, Sprout > Aqua, Volt > Aqua
        if (attacker == Element.Pyro   && defender == Element.Sprout) return 1.5f;
        if (attacker == Element.Aqua   && defender == Element.Pyro)  return 1.5f;
        if (attacker == Element.Sprout && defender == Element.Aqua)  return 1.5f;
        if (attacker == Element.Volt   && defender == Element.Aqua)  return 1.5f;

        // Kebalikannya
        if (defender == Element.Pyro   && attacker == Element.Sprout) return 0.75f;
        if (defender == Element.Aqua   && attacker == Element.Pyro)  return 0.75f;
        if (defender == Element.Sprout && attacker == Element.Aqua)  return 0.75f;
        if (defender == Element.Aqua   && attacker == Element.Volt)  return 0.75f;

        return 1f;
    }
}