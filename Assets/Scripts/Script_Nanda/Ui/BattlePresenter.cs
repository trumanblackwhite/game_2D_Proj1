using UnityEngine;

public class BattlePresenter : MonoBehaviour
{
    private BattleSystem battle;
    private BattleView view;

    public void Construct(GameServices s, BattleSystem b, BattleView v)
    {
        battle = b;
        view = v;
        v.Construct(s.Battle, s.Party.Partner, s.Battle.Enemy);
        
        b.Said += v.Say; 
        
        b.ChoiceRequested += _ => v.ShowMenu();
        v.Chosen += i => { v.HideMenu(); b.Choose(i); };
    }
}