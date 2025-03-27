using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackInfoRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _nameLabel;
        [AssertIsSet][SerializeField] private Transform _iconParent;
        [AssertIsSet][SerializeField] private Image _iconPrefab;

        public void Render(AttackData data)
        {
            _nameLabel.text = data.Name;
            _iconParent.DestroyAllChildren();
            Image typeIcon = Instantiate(_iconPrefab, _iconParent);
            typeIcon.sprite = data.AttackType.Sprite;
            Image attackDie = Instantiate(_iconPrefab, _iconParent);
            attackDie.sprite = data.AttackType.AttackDie.Sprite;
            attackDie.color = data.AttackType.AttackDie.Albedo;
            foreach (DieData die in data.PowerDice)
            {
                Image dieIcon = Instantiate(_iconPrefab, _iconParent);
                dieIcon.sprite = die.Sprite;
                dieIcon.color = die.Albedo;
            }
        }
    }
}
