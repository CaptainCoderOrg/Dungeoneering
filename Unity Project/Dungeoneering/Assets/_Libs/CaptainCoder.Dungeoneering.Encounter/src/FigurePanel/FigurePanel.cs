using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class FigurePanel : MonoBehaviour
    {
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        [AssertIsSet][SerializeField] private CanvasGroup _canvasGroup;
        [AssertIsSet][SerializeField] private CanvasRebuilder _rebuilder;

        void Awake()
        {
            _canvasGroup.alpha = 0;
            FindAllRenderers();
        }

        [Button]
        private void FindAllRenderers()
        {
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
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
            StartCoroutine(Show());
        }

        public IEnumerator Show()
        {
            yield return null;
            _rebuilder.ForceRebuild();
            _canvasGroup.alpha = 1;
        }
    }
}