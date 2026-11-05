using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private GameServices services;
    private BattleSession session;
    private MonsterInstance partner;
    private MonsterInstance enemy;

    private readonly AttackAction attack = new AttackAction();
    private readonly SkillAction skill = new SkillAction();
    private readonly ItemAction item = new ItemAction();
    private readonly EscapeAction escape = new EscapeAction();

    private int pendingChoice = -1;
    private bool over;
    private BattleResult result;

    public bool Busy { get; private set; }

    public event System.Action<string> Said;          // P5-5: BattleView mendengarkan
    public event System.Action<int> ChoiceRequested;  // P5-5: presenter membuka menu

    public void Construct(GameServices s)
    {
        services = s;
        session = s.Battle;
        partner = s.Party.Partner;
        enemy = session.Enemy;
    }

    public void Choose(int index) => pendingChoice = index;
    public void Begin() => StartCoroutine(Routine());

    private void Say(string line)
    {
        Debug.Log($"[Battle] {line}");
        Said?.Invoke(line);
    }

    private void EndBattle(BattleResult r)
    {
        if (over) return;
        over = true;
        result = r;
        session.Finish(r);
    }

    IEnumerator Routine()
    {
        Busy = true;
        Say($"{enemy.Data.DisplayName} liar muncul! (Lv {enemy.Level})");
        yield return new WaitForSeconds(0.8f);

        while (!over)
        {
            if (partner.Speed >= enemy.Speed)
            {
                yield return PlayerTurn();
                if (!over) yield return EnemyTurn();
            }
            else
            {
                yield return EnemyTurn();
                if (!over) yield return PlayerTurn();
            }
        }

        yield return Ending();
        Busy = false;
    }

    IEnumerator PlayerTurn()
    {
        Say("Pilih aksi: 1=Serang 2=Jurus 3=Ramuan 4=Kabur");
        ChoiceRequested?.Invoke(0);
        pendingChoice = -1;
        while (pendingChoice < 0) yield return null;   // menunggu pilihan (debug / presenter)

        switch (pendingChoice)
        {
            case 0: attack.Execute(session, partner, enemy); break;
            case 1: skill.Execute(session, partner, enemy); break;
            case 2: item.Execute(session, partner, enemy); break;
            case 3: escape.Execute(session, partner, enemy); break;
        }

        if (session.Escaped) EndBattle(BattleResult.Escape);
        else Report();

        yield return new WaitForSeconds(0.6f);
    }

    IEnumerator EnemyTurn()
    {
        if (enemy.CurrentHP <= 0) yield break;
        attack.Execute(session, enemy, partner);   // AI sementara: selalu serangan biasa
        Report();
        yield return new WaitForSeconds(0.6f);
    }

    private void Report()
    {
        Say($"{partner.Data.DisplayName} {partner.CurrentHP}/{partner.MaxHP} HP | {enemy.Data.DisplayName} {enemy.CurrentHP}/{enemy.MaxHP} HP");
        if (enemy.CurrentHP <= 0) EndBattle(BattleResult.Win);
        else if (partner.CurrentHP <= 0) EndBattle(BattleResult.Lose);
    }

    IEnumerator Ending()
    {
        if (result == BattleResult.Win)
        {
            int exp = enemy.Level * 10;
            Say($"{enemy.Data.DisplayName} kalah! +{exp} EXP");
            partner.GainExp(exp);
        }
        else if (result == BattleResult.Escape) Say("Kamu berhasil kabur!");
        else Say($"{partner.Data.DisplayName} pingsan... kamu terbangun di desa.");

        yield return new WaitForSeconds(1f);
    }
}