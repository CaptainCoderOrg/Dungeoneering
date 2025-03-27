using System.Linq;

using NaughtyAttributes;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class FigurePanel : MonoBehaviour
    {
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        public FigureData FigureData
        {
            get => _figureData;
            set
            {
                _figureData = value;
                Render(_figureData);
            }
        }

        void Awake()
        {
            FindAllRenderers();
        }

        void OnEnable() => Render();

        [Button]
        private void FindAllRenderers()
        {
            _figureRenderers = GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            Debug.Log(_figureRenderers.Length);
        }
        [Button]
        public void Render() => Render(_figureData);
        public void Render(FigureData data)
        {
            foreach (var renderer in _figureRenderers)
            {
                renderer.Render(data);
            }
            if (Application.isPlaying)
            {
                Canvas.ForceUpdateCanvases();
            }
        }

    }
}