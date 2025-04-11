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
        [AssertIsSet][SerializeField] private CanvasGroupHider _deadIndicator;

        public FigureData FigureData
        {
            get => _figureData;
            set
            {
                if (_figureData != null) { _figureData.EntityData.OnChanged -= HandleEnityChanged; }
                _figureData = value;
                _figureData.EntityData.OnChanged += HandleEnityChanged;
                FindAllRenderers();
                UpdateRenderers();
            }
        }

        private void HandleEnityChanged(LivingEntityChangeEvent @event)
        {
            if (@event is EntityDeathEvent)
            {
                _deadIndicator.Show();
            }
        }

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
        public void UpdateRenderers()
        {
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

        public void Render(FigureData figureData)
        {
            FigureData = figureData;
            Show();
        }

        public void Show()
        {
            _deadIndicator.Hide();
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = false;
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