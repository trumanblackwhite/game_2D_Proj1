using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private BattleSystem battleSystem;

    public override void Install(GameServices s)
    {
        battleSystem.Construct(s);
        battleSystem.Begin();
    }
}