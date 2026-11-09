// Kode_P6 — Kelas induk semua installer (inheritance + polymorphism). Ditambahkan di P4.
// Aturan: installer diletakkan di GameObject ROOT scene-nya.
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInstaller : MonoBehaviour
{
    public abstract void Install(GameServices services);

    // Mencari component bertipe T di scene ini saja, termasuk yang nonaktif.
    protected IEnumerable<T> FindInScene<T>()
    {
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
            foreach (T component in root.GetComponentsInChildren<T>(true))
                yield return component;
    }
}
