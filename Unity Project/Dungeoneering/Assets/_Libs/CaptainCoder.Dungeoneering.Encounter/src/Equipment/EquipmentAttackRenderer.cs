
using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EquipmentAttackRenderer : MonoBehaviour, IEquipmentRenderer
    {
        [AssertIsSet][SerializeField] private AttackInfoRenderer _attackInfoRenderer;
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        public void Render(EquipmentData data)
        {
            if (data is HeldEquipmentData held && held.Attack != null)
            {
                _attackInfoRenderer.Render(held.Attack);
                _layoutElement.ignoreLayout = false;
                _canvasGroup.alpha = 1;
            }
            else
            {
                _layoutElement.ignoreLayout = true;
                _canvasGroup.alpha = 0;
            }
        }
    }
}