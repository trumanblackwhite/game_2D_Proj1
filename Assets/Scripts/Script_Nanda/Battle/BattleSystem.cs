using System;
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
    public MonsterInstance Partner => partner;
    public MonsterInstance Enemy => enemy;
    public bool IsBoss => session != null && session.IsBoss;
    public BattleSession Session => session;

    // ✅ EVENT LENGKAP (Wajib ada agar Presenter bisa memicu animasi)
    public event Action<string> Said;
    public event Action<int> ChoiceRequested;
    public event Action<MonsterInstance> Fainted;
    public event Action Victory;
    public event Action<MonsterData, MonsterData> Evolving;
    public event Action<MonsterInstance> Evolved;
    public event Action<MonsterData> LevelUp;

    public void Construct(GameServices s)
    {
        services = s;
        session = s.Battle;
        partner = s.Party.Partner;
        enemy = session.Enemy;
    }

    public void Choose(int index) => pendingChoice = index;
    public void Begin() => StartCoroutine(Routine());

    // ✅ PERBAIKAN 1: Say jadi IEnumerator agar bisa menunggu (P5 Slide 22)
    public IEnumerator Say(string text, float hold = 0.8f)
    {
        Said?.Invoke(text);
        float wait = hold + text.Length * 0.018f; // 18ms per huruf
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            yield return null;
        }
    }

    private void EndBattle(BattleResult r)
    {
        if (over) return;
        over = true;
        result = r;
        // session.Finish(r) TIDAK dipanggil di sini, tapi di akhir Ending()
    }

    IEnumerator Routine()
    {
        partner.Heal(partner.MaxHP);
        enemy.Heal(enemy.MaxHP);

        Busy = true;
        yield return Say($"{enemy.Data.DisplayName} liar muncul! (Lv {enemy.Level})", 0.9f);

        while (!over)
        {
            bool playerFirst = partner.Speed >= enemy.Speed ||
                              (partner.Speed == enemy.Speed && UnityEngine.Random.value < 0.5f);

            if (playerFirst)
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
        yield return Say("Pilih aksi: 1=Serang 2=Jurus 3=Ramuan 4=Kabur", 0.6f);
        ChoiceRequested?.Invoke(0);
        pendingChoice = -1;
        while (pendingChoice < 0) yield return null;

        BattleAction action = null;
        switch (pendingChoice)
        {
            case 0: action = attack; break;
            case 1: action = skill; break;
            case 2: action = item; break;
            case 3: action = escape; break;
        }

        if (action != null)
        {
            // ✅ PERBAIKAN 2: yield return agar animasi serangan jalan
            yield return action.Execute(this, partner, enemy);
        }

        yield return new WaitForSeconds(0.6f);

        if (session != null && session.Escaped)
        {
            EndBattle(BattleResult.Escape);
        }
        else
        {
            CheckBattleEnd();
        }
    }

    IEnumerator EnemyTurn()
    {
        if (enemy.CurrentHP <= 0) yield break;

        yield return attack.Execute(this, enemy, partner);
        yield return new WaitForSeconds(0.6f);

        CheckBattleEnd();
    }

    private void CheckBattleEnd()
    {
        if (enemy.CurrentHP <= 0)
        {
            Fainted?.Invoke(enemy);
            EndBattle(BattleResult.Win);
        }
        else if (partner.CurrentHP <= 0)
        {
            Fainted?.Invoke(partner);
            EndBattle(BattleResult.Lose);
        }
    }

    IEnumerator Ending()
    {
        if (result == BattleResult.Win)
        {
            // Tunggu animasi pingsan musuh selesai dulu
            yield return new WaitForSeconds(0.6f);

            Victory?.Invoke();
            yield return Say($"{enemy.Data.DisplayName} kalah!", 0.8f);

            int exp = (10 + enemy.Level * 8) * (IsBoss ? 2 : 1);
            yield return Say($"{partner.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

            // Loop ini yang menangani Level Up dan Evolusi (misal Lv 5) secara berurutan
            foreach (GrowthEvent growth in partner.GainExp(exp))
            {
                if (growth.Type == GrowthType.LevelUp)
                {
                    LevelUp?.Invoke(growth.From);
                    yield return Say(growth.Message, 0.9f);
                    continue;
                }

                // Proses Evolusi
                yield return Say($"Apa? {growth.From.DisplayName} mulai bersinar...", 0.9f);
                Evolving?.Invoke(growth.From, growth.To);
                yield return new WaitForSeconds(2.4f);    
                Evolved?.Invoke(partner);                 
                yield return Say(growth.Message, 1.2f);
            }

            int healed = partner.Heal(Mathf.RoundToInt(partner.MaxHP * 0.25f));
            if (healed > 0)
            {
                yield return Say($"{partner.Data.DisplayName} beristirahat sejenak dan pulih {healed} HP.", 0.8f);
            }
        }
        else if (result == BattleResult.Escape)
        {
            yield return Say("Kamu berhasil kabur!", 0.8f);
        }
        else
        {
            yield return Say($"{partner.Data.DisplayName} pingsan... kamu terbangun di desa.", 1.2f);
        }

        // ✅ PERBAIKAN 4: Tunggu 1.5 detik agar pemain bisa melihat hasil sebelum scene close
        yield return new WaitForSeconds(1.5f);

        // Baru panggil Finish di paling akhir
        if (session != null)
        {
            session.Finish(result);
        }
    }
}