using CaptainCoder.DungeonCrawler;
using CaptainCoder.DungeonCrawler.Combat;
using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class WeaponDialogueScreen : MonoBehaviour
{
    public static WeaponDialogueScreen Shared { get; private set; } = default!;
    public WeaponDialogueScreen() { Shared = this; }
    public WeaponInfoRenderer NewWeapon = default!;
    public WeaponInfoRenderer CharacterWeapon = default!;
    public TextMeshProUGUI CharacterName = default!;
    private int _selectedIx = 0;
    private Party Party => CrawlingModeController.Shared.Party;
    private Weapon _newWeapon = default!;

    public void OnEnable()
    {
        CrawlingModeController.Shared.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        foreach (var card in Overlay.Shared.Cards) { card.IsSelected = false; }
        CrawlingModeController.Shared.gameObject.SetActive(true);
    }

    public void Update() => PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleAction);
    private void HandleAction(MenuControl control)
    {
        Action toInvoke = control switch
        {
            MenuControl.Right or MenuControl.Down => () => Next(1),
            MenuControl.Left or MenuControl.Up => () => Next(-1),
            MenuControl.Select => DoSelect,
            MenuControl.Cancel => Discard,
            _ => () => Debug.Log($"Ignored input: {control}"),
        };
        toInvoke.Invoke();
    }

    private void Next(int delta)
    {
        _selectedIx += delta;
        if (_selectedIx < 0)
        {
            _selectedIx = 4;
        }
        if (_selectedIx == 4)
        {
            Select(_selectedIx);
            return;
        }
        if (_selectedIx > 4) { _selectedIx = 0; }
        if (Party[_selectedIx].Card == Characters.NoBody)
        {
            Next(delta);
            return;
        }
        Select(_selectedIx);
    }

    private void Discard()
    {
        MessageRenderer.Shared.AddMessage($"The {_newWeapon.Name} was discarded.");
        gameObject.SetActive(false);
        CrawlingModeController.Shared.Initialize();
    }

    public void DoSelect()
    {
        if (_selectedIx == 4)
        {
            var handler = PlayerInputHandler.Shared;
            MessageRenderer.Shared.AddMessage($"Press {string.Join(" and ", [handler.GetKeys(MenuControl.Right), handler.GetKeys(MenuControl.Left)])} to select a character or press {handler.GetKeys(MenuControl.Cancel)} to discard the weapon.");
            return;
        }
        else
        {
            PlayerCharacter character = Party[_selectedIx];
            Party.UpdateCharacter(character with { Weapon = _newWeapon });
            MessageRenderer.Shared.AddMessage($"{Party[_selectedIx].Card.Name} equipped the {Party[_selectedIx].Weapon.Name}.");
        }

        gameObject.SetActive(false);
        CrawlingModeController.Shared.Initialize();
    }

    public void Initialize(Weapon weapon)
    {
        MessageRenderer.Shared.AddMessage($"Who will equip the {weapon.Name}?");
        gameObject.SetActive(true);
        _selectedIx = 4;
        _newWeapon = weapon;
        NewWeapon.Render(weapon);
        Select(4);
    }

    public void Select(int ix)
    {
        foreach (var card in Overlay.Shared.Cards) { card.IsSelected = false; }
        if (ix == 4)
        {
            var handler = PlayerInputHandler.Shared;
            CharacterName.text = $"Press {string.Join(" and ", [handler.GetKeys(MenuControl.Right), handler.GetKeys(MenuControl.Left)])} to select a character or press {handler.GetKeys(MenuControl.Cancel)} to discard the weapon.";
            CharacterWeapon.gameObject.SetActive(false);
        }
        else
        {
            Overlay.Shared.Cards[ix].IsSelected = true;
            CharacterName.text = CrawlingModeController.Shared.Party[ix].Card.Name;
            CharacterWeapon.Render(CrawlingModeController.Shared.Party[ix].Weapon);
            CharacterWeapon.gameObject.SetActive(true);
        }
    }
}