using CaptainCoder.Dungeoneering.Player.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public abstract class AbstractMenuController<TEnum> : MonoBehaviour
{
    [SerializeField]
    public MenuItemMapping<TEnum>[] MenuItems = [];
    [SerializeField]
    public GameObject Menu = default!;

    private int _selectedIx = 0;

    public virtual void OnEnable()
    {
        Menu.SetActive(true);
    }

    public virtual void OnDisable()
    {
        Menu.SetActive(false);
    }

    protected abstract void SelectOption(TEnum action);
    public void Update() => PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleMenuAction);

    private void HandleMenuAction(MenuControl control)
    {
        Action action = control switch
        {
            MenuControl.Down or MenuControl.Right => () => Next(1),
            MenuControl.Up or MenuControl.Left => () => Next(-1),
            MenuControl.Select => () => SelectOption(MenuItems[_selectedIx].Item),
            _ => () => { }
            ,
        };
        action.Invoke();
    }

    private void Next(int delta)
    {
        _selectedIx += delta;
        if (_selectedIx < 0) { _selectedIx = MenuItems.Length - 1; }
        else if (_selectedIx >= MenuItems.Length) { _selectedIx = 0; }
        Select(_selectedIx);
    }

    protected virtual void OnSelectionChange(TEnum selected) { }

    public void Select(int ix)
    {
        foreach (var item in MenuItems)
        {
            item.MenuItem.IsSelected = false;
        }
        _selectedIx = ix;
        MenuItems[ix].MenuItem.IsSelected = true;
        if (MenuItems[ix].HelpMenuText.Trim() != string.Empty)
        {
            CombatHelpPanel.Shared.Text = MenuItems[ix].HelpMenuText;
        }
        OnSelectionChange(MenuItems[ix].Item);
    }
}