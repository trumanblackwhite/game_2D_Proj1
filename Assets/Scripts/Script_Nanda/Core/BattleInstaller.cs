using UnityEngine;
using System.Collections;

public class BattleInstaller : SceneInstaller
{
    public override void Install(GameServices s)
    {
        StartCoroutine(AutoWinAfterDelay(s));
    }

    private IEnumerator AutoWinAfterDelay(GameServices s)
    {
        yield return new WaitForSeconds(2f);
        s.Battle.Finish(BattleResult.Win);
    }
}