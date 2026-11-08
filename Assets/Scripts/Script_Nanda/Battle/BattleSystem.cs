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

    // Event
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

    public IEnumerator Say(string text, float hold = 0.8f)
    {
        Said?.Invoke(text);
        float wait = hold + text.Length * 0.018f;
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            yield return null;
        }
    }

    // ✅ PERBAIKAN 1: EndBattle TIDAK langsung memanggil session.Finish
    private void EndBattle(BattleResult r)
    {
        if (over) return;
        over = true;
        result = r;
        // session.Finish(r) dipanggil di akhir Ending()
    }

    IEnumerator Routine()
    {
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

        // ✅ PERBAIKAN 2: Jalankan Ending() sampai selesai SEBELUM scene berganti
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
            // ✅ PERBAIKAN 3: Invoke Fainted agar animasi pingsan jalan
            Fainted?.Invoke(enemy); 
            EndBattle(BattleResult.Win);
        }
        else if (partner.CurrentHP <= 0)
        {
            Fainted?.Invoke(partner);
            EndBattle(BattleResult.Lose);
        }
    }

    // ✅ PERBAIKAN 4: Ending() sekarang lengkap dan Finish dipanggil di paling akhir
    IEnumerator Ending()
    {
        if (result == BattleResult.Win)
        {
            // Tunggu animasi pingsan selesai (0.6 detik sesuai P5)
            yield return new WaitForSeconds(0.6f);

            Victory?.Invoke();
            yield return Say($"{enemy.Data.DisplayName} kalah!", 0.8f);

            int exp = (10 + enemy.Level * 8) * (IsBoss ? 2 : 1);
            yield return Say($"{partner.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

            foreach (GrowthEvent growth in partner.GainExp(exp))
            {
                if (growth.Type == GrowthType.LevelUp)
                {
                    LevelUp?.Invoke(growth.From);
                    yield return Say(growth.Message, 0.9f);
                    continue;
                }

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

        yield return new WaitForSeconds(1f);

        // ✅ PENTING: Baru panggil Finish di sini agar scene tidak ganti sebelum EXP muncul
        if (session != null)
        {
            session.Finish(result);
        }
    }
}