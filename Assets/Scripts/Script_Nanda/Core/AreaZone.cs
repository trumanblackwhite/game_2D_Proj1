// Kode_P6 — Trigger area (Desa, Hutan, Danau, Gua). Dibuat di P3, diubah di P5, P6.
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaZone : MonoBehaviour
{
    [SerializeField] private string areaName = "Desa Awal";
    [SerializeField] private AudioClip music;                  // P6

    private AudioService audioService;

    public string AreaName => areaName;
    public event Action<AreaZone> Entered;                      // P5: HUD berlangganan

    public void Construct(AudioService audio) => audioService = audio;   // P6

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Entered?.Invoke(this);
        if (audioService != null) audioService.PlayWorldMusic(music);
    }
}
