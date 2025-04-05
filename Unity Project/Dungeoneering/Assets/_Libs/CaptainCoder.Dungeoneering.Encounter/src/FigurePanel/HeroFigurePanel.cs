using System.Collections;
using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
using UnityEngine.UI;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class HeroFigurePanel : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public RectTransform TopLeftPivot { get; private set; }
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Image _backgroundColor;
        [SerializeField] private EncounterController _encounterController;
        [SerializeField] private EncounterFigureController _figureController;
        [SerializeField] private FigureData _figureData;
        [SerializeField] private IFigureRenderer[] _figureRenderers;
        [SerializeField] private ILivingEntityRenderer[] _entityRenderers;

        public event System.Action<HeroFigurePanel> OnMoved;
        public event System.Action OnHeroUpdated;

        public EncounterFigureController FigureController
        {
            get => _figureController;
            set
            {
                _figureController = value;
                _figureData = _figureController.Figure;
                UpdateRenderers();
            }
        }

        void Awake()
        {
            _encounterController = GetComponentInParent<EncounterController>();
            Debug.Assert(_encounterController != null, "Could not find Encounter Controller", this);
            _figureRenderers ??= GetComponentsInChildren<IFigureRenderer>(true).Where(c => (Object)c != this).ToArray();
            _entityRenderers ??= GetComponentsInChildren<ILivingEntityRenderer>(true).Where(c => (Object)c != this).ToArray();
        }

        void Start()
        {
            Rebuild();
        }


        public void SelectFigure()
        {
            _encounterController.Select(_figureController);
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
        }

        public void TakeTurn()
        {
            _encounterController.SelectTactics(this);
        }

        public void Select()
        {
            _backgroundColor.color = _selectedColor;
        }
        public void Deselect()
        {
            _backgroundColor.color = _defaultColor;
        }

        public void Rebuild() => StartCoroutine(RebuildAtEndOfFrame());

        public IEnumerator RebuildAtEndOfFrame()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            OnMoved?.Invoke(this);
        }
    }
}