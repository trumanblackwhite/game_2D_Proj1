using UnityEngine;

public static class DamageFormula
{
    public static int Calculate(MonsterInstance attacker, MonsterInstance target, float power)
    {
        // Meleset 6%
        if (Random.value < 0.06f) return 0;

        float raw = (attacker.Attack * power) / (target.Defense * 0.5f + 10f);
        float mult = ElementChart.Multiplier(attacker.Data.Element, target.Data.Element);
        float crit = Random.value < 0.08f ? 1.5f : 1f;

        int damage = Mathf.RoundToInt(raw * mult * crit);
        return Mathf.Max(1, damage);
    }
}