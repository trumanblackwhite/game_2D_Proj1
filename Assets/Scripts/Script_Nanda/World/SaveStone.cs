// Kode_P6 — Batu Simpan: pulihkan partner, lalu simpan permainan. Ditambahkan di P6.
// Versi demo: teks sesuai demo (P7 menambah kilau partikel).
using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip saveClip;

    private GameServices services;
    private Transform player;

    // Pengecualian yang disengaja: SaveMapper butuh banyak service sekaligus.
    public void Construct(GameServices gameServices, Transform playerTransform)
    {
        services = gameServices;
        player = playerTransform;
    }

    public void Interact(PlayerInteractor who)
    {
        if (!services.Dialogue.IsRunning) services.Dialogue.Run(Use());
    }

    private IEnumerator Use()
    {
        if (services.Audio != null) services.Audio.PlaySfx(saveClip, 0f);
        if (services.Party.Partner != null) services.Party.Partner.HealFull();
        bool saved = services.Save.Save(SaveMapper.Capture(services, player.position));
        yield return services.Dialogue.Say("", saved
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan."
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.");
    }
}
