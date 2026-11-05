using UnityEngine;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private TextMenu menu;
    private bool open;

    void Start() => menu.Picked += OnPicked;

    void Update()
    {
        if (!open && MenuInput.Confirm)
        {
            open = true;
            menu.Open(new[] { "Serang", "Jurus", "Ramuan", "Kabur" },
                      new[] { true, true, false, true });   // Ramuan disabled
        }
    }

    void OnPicked(int i)
    {
        Debug.Log($"Dipilih: {i}");
        menu.Close();
        open = false;
    }
}