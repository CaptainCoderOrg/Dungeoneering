using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EnemyFigurePanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        [SerializeField] private ILivingEntityRenderer[] _entityRenderers;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private CanvasRebuilder _rebuilder;

        void Awake()
        {
            Hide();
            FindAllRenderers();
        }

        [Button]
        private void FindAllRenderers()
        {
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
        }
        [Button]
        public void Render() => Render(_figureData);
        public void Render(FigureData data)
        {
            _figureData = data;
            foreach (IFigureRenderer renderer in _figureRenderers)
            {
                renderer.Render(data);
            }
            foreach (ILivingEntityRenderer renderer in _entityRenderers)
            {
                renderer.Render(data.EntityData);
            }
            StartCoroutine(Show());
        }

        public void Toggle(FigureData data)
        {
            if (_figureData != data) { Render(data); }
            else if (_canvasGroup.alpha == 0) { Render(data); }
            else { Hide(); }
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = true;
        }

        public IEnumerator Show()
        {
            yield return null;
            _rebuilder.ForceRebuild();
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = false;
        }
    }
}