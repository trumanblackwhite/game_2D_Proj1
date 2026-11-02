using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDatabase",
                 menuName = "Pulau Monster/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private MonsterData[] allMonsters;
    public MonsterData[] AllMonsters => allMonsters;
}