using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextMenu : MonoBehaviour
{
    public event Action<int> Picked;          // dunia luar mendengarkan pilihan
    public static Action Sfx;                 // P6: dicolok ke AudioService

    [SerializeField] private Text[] rows;     // baris menu (Text legacy, urut atas-bawah)
    [SerializeField] private int columns = 1; // menu battle nanti: 2 (grid 2×2)
    [SerializeField] private Color normal = UiTheme.Text;
    [SerializeField] private Color selected = UiTheme.Gold;
    [SerializeField] private Color disabled = UiTheme.Disabled;

    private string[] labels = new string[0];
    private bool[] usable = new bool[0];
    private int count;
    private bool acceptInput;

    public int Selected { get; private set; }

    void Awake()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            int index = i;                    // JEBAKAN: salin i sebelum lambda!
            Text row = rows[i];
            row.raycastTarget = true;

            Button button = row.GetComponent<Button>();
            if (button == null) button = row.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick.AddListener(() => Click(index));

            EventTrigger trigger = row.GetComponent<EventTrigger>();
            if (trigger == null) trigger = row.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener(_ => Hover(index));
            trigger.triggers.Add(entry);
        }
    }

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null) for (int i = 0; i < count; i++) usable[i] = true;
        for (int i = 0; i < rows.Length; i++)
            rows[i].gameObject.SetActive(i < count);
        Selected = Mathf.Clamp(start, 0, count - 1);
        if (!usable[Selected]) Selected = FirstUsable();
        gameObject.SetActive(true);
        acceptInput = false;                  // telan tombol pembuka menu
        Redraw();
    }

    public void Close() => gameObject.SetActive(false);

    void Update()
    {
        if (count == 0 || !gameObject.activeInHierarchy) return;
        if (!acceptInput) { acceptInput = true; return; }

        if (MenuInput.Up) Move(-1, 0);
        else if (MenuInput.Down) Move(+1, 0);
        else if (MenuInput.Left) Move(0, -1);
        else if (MenuInput.Right) Move(0, +1);

        if (MenuInput.Confirm) Pick(Selected);
    }

    private void Move(int dRow, int dCol)
    {
        int rowsUsed = (count + columns - 1) / columns;
        int row = Mathf.Clamp(Selected / columns + dRow, 0, rowsUsed - 1);
        int col = Mathf.Clamp(Selected % columns + dCol, 0, columns - 1);
        int next = row * columns + col;
        if (next >= count || next == Selected) return;
        Selected = next;
        Redraw();
        Sfx?.Invoke();
    }

    private void Pick(int index)
    {
        if (!usable[index]) { Sfx?.Invoke(); return; }   // redup = tolak + bunyi batal
        Picked?.Invoke(index);
    }

    private void Click(int index)
    {
        if (index >= count) return;
        Hover(index);
        Pick(index);
    }

    private void Hover(int index)
    {
        if (index >= count || index == Selected) return;
        Selected = index;
        Redraw();
    }

    private int FirstUsable()
    {
        for (int i = 0; i < count; i++) if (usable[i]) return i;
        return 0;
    }

    private void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            bool on = i == Selected;
            rows[i].text = (on ? "▶" : "  ") + labels[i];
            rows[i].color = !usable[i] ? disabled : on ? selected : normal;
        }
    }
}