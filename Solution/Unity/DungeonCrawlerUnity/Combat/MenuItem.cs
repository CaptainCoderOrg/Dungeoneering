using TMPro;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

[Serializable]
public class MenuItemMapping<T>
{
    public T Item = default!;
    public MenuItem MenuItem = default!;
    public string HelpMenuText = default!;
}

public class MenuItem : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI Text { get; private set; } = default!;
    public Color SelectedColor = Color.yellow;
    public Color NormalColor = Color.black;
    public bool IsSelected
    {
        set => Text.color = value ? SelectedColor : NormalColor;
    }
}