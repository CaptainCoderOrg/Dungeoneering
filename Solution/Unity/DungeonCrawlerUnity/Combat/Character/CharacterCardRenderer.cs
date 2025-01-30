using CaptainCoder.Dungeoneering.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class CharacterCardRenderer : MonoBehaviour
{
    public int CharacterIx;
    public GameObject Selected = default!;
    public GameObject Finished = default!;
    public GameObject Dead = default!;
    public GameObject Cabinet = default!;
    public CharacterCardIconDatabase Icons = default!;

    public Image StateIcon = default!;
    public TextMeshProUGUI Name = default!;
    public TwoValueRenderer Health = default!;
    public TwoValueRenderer Energy = default!;
    public ValueRenderer Armor = default!;
    public ValueRenderer Speed = default!;
    public ValueRenderer MovementPoints = default!;
    public ValueRenderer AttackPoints = default!;
    public bool IsSelected { set => Selected.SetActive(value); }
    public bool IsFinished { set => Finished.SetActive(value); }

    public PlayerCharacter? Character
    {
        set
        {
            if (value is null)
            {
                RenderNoCharacter();
            }
            else
            {
                Render(value);
            }
        }
    }

    public void Render(PlayerCharacter character)
    {
        Cabinet.SetActive(character.Card == Characters.NoBody);
        Name.text = character.Card.Name;
        Health.Value = character.Health();
        Health.BaseValue = character.Card.BaseHealth;

        Energy.Value = character.Energy();
        Energy.BaseValue = character.Card.BaseEnergy;

        Armor.Value = character.Card.BaseArmor;
        Speed.Value = character.Card.BaseSpeed;
        MovementPoints.Value = character.MovementPoints;
        AttackPoints.Value = character.AttackPoints;

        StateIcon.sprite = character.State switch
        {
            CharacterState.Normal => null,
            CharacterState.Guard => Icons.GuardState,
            CharacterState.Rest => Icons.RestState,
            var state => throw new NotImplementedException($"Unknown state: {state}"),
        };
        StateIcon.gameObject.SetActive(StateIcon.sprite is not null);
        Dead.SetActive(character.IsDead());
    }

    public void RenderNoCharacter()
    {
        Debug.Assert(false, "RenderNoCharacter() is not implemented");
    }

    public void Start()
    {
        if (CharacterIx == 0) { CrawlingModeController.Shared.Party.OnTopLeftChange += Render; }
        else if (CharacterIx == 1) { CrawlingModeController.Shared.Party.OnTopRightChange += Render; }
        else if (CharacterIx == 2) { CrawlingModeController.Shared.Party.OnBottomLeftChange += Render; }
        else if (CharacterIx == 3) { CrawlingModeController.Shared.Party.OnBottomRightChange += Render; }
        else { throw new Exception($"Invalid Character IX"); }
        Render(CrawlingModeController.Shared.Party[CharacterIx]);
    }
}

[CreateAssetMenu(fileName = "CharacterCardIconDatabase", menuName = "Data/Character Card Icons")]
public class CharacterCardIconDatabase : ScriptableObject
{
    public Sprite GuardState = default!;
    public Sprite RestState = default!;
}


public class ValueRenderer : MonoBehaviour
{
    public TextMeshProUGUI Text = default!;
    public int Value
    {
        set => Text.text = value.ToString();
    }
}

public class TwoValueRenderer : MonoBehaviour
{
    public TextMeshProUGUI Text = default!;
    private int _value = 10;
    private int _baseValue = 10;
    public int Value
    {
        set
        {
            _value = value;
            Text.text = $"{_value}/{_baseValue}";
        }
    }
    public int BaseValue
    {
        set
        {
            _baseValue = value;
            Text.text = $"{_value}/{_baseValue}";
        }
    }
}