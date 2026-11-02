using UnityEngine;

public class PartnerFollower : MonoBehaviour
{
    [SerializeField] private MonoBehaviour player;
    private PlayerParty party;
    private SpriteRenderer body;

    void Awake()
    {
        body = GetComponent<SpriteRenderer>();
    }

    public void Construct(PlayerParty party, MonoBehaviour player)
    {
        this.party = party;
        this.player = player;
    }

    void LateUpdate()
    {
        if (player == null) return;
        
        MonsterInstance m = party != null ? party.Partner : null;
        body.enabled = m != null;
        if (m == null) return;

        // Ikut di samping player
        transform.position = player.transform.position + new Vector3(0.8f, 0, 0);
        
        // Ganti sprite sesuai data monster
        int frame = Mathf.FloorToInt(Time.time / 0.25f) % 2;
        if (m.Data.Frames != null && m.Data.Frames.Length > 0)
        {
            body.sprite = m.Data.Frames[frame % m.Data.Frames.Length];
        }
    }
}