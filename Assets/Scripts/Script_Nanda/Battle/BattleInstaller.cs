using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private BattlePresenter presenter;
    [SerializeField] private BattleView view;

    public override void Install(GameServices s)
    {
        battleSystem.Construct(s);
        presenter.Construct(s, battleSystem, view);
        battleSystem.Begin();
    }
}