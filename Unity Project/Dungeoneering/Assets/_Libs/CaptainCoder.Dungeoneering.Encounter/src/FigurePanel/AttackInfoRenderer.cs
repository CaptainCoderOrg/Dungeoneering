using CaptainCoder.Unity.Assertions;

using TMPro;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class AttackInfoRenderer : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private TextMeshProUGUI _nameLabel;
        [AssertIsSet][SerializeField] private Transform _iconParent;
        [AssertIsSet][SerializeField] private AttackIconController _attackIconPrefab;
        [AssertIsSet][SerializeField] private DieIconController _dieIconPrefab;

        public void Render(AttackData data)
        {
            _nameLabel.text = data.Name;
            _iconParent.DestroyAllChildren();
            AttackIconController typeIcon = Instantiate(_attackIconPrefab, _iconParent);
            typeIcon.AttackType = data.AttackType;
            DieIconController attackDieIcon = Instantiate(_dieIconPrefab, _iconParent);
            attackDieIcon.Die = data.AttackType.AttackDie;
            foreach (DieData die in data.PowerDice)
            {
                DieIconController dieIcon = Instantiate(_dieIconPrefab, _iconParent);
                dieIcon.Die = die;
            }
        }
    }
}
