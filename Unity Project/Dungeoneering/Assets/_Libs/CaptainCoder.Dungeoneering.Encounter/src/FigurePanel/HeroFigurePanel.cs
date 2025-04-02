using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroFigurePanel : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        [SerializeField] private ILivingEntityRenderer[] _entityRenderers;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private CanvasRebuilder _rebuilder;

        public FigureData FigureData
        {
            get => _figureData;
            set
            {
                _figureData = value;
                UpdateRenderers();
            }
        }

        void Awake()
        {
            Hide();
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
        }

        private void UpdateRenderers()
        {
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
            foreach (IFigureRenderer renderer in _figureRenderers)
            {
                renderer.Render(_figureData);
            }
            foreach (ILivingEntityRenderer renderer in _entityRenderers)
            {
                renderer.Render(_figureData.EntityData);
            }
            StartCoroutine(RebuildAtEndOfFrame());
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = false;
        }

        public void Toggle()
        {
            if (_canvasGroup.alpha == 0) { Show(); }
            else { Hide(); }
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = true;
        }

        public IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            _rebuilder.ForceRebuild();
        }
    }
}