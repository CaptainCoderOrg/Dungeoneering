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
        [AssertIsSet][SerializeField] private HeroActionButtons _heroActionButtons;

        public event System.Action<HeroFigurePanel> OnMoved;

        public EncounterFigureController FigureController
        {
            get => _figureController;
            set
            {
                _figureController = value;
                if (_figureData != null) { _figureData.OnChanged -= HandleFigureChanged; }
                _figureData = _figureController.Figure;
                _figureData.OnChanged += HandleFigureChanged;
                UpdateRenderers();
                _heroActionButtons.UpdateButtons(_figureData);
            }
        }

        private void HandleFigureChanged(FigureDataChangedEvent _)
        {
            _heroActionButtons.UpdateButtons(_figureData);
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

        public void Move() => _encounterController.HeroTurnController.ShowMove();

        public void EndTurn() => _encounterController.HeroTurnController.EndTurn();

        public void SelectAttack() => _encounterController.HeroTurnController.StartAttack();

        /// <summary>
        /// Called when any figure is starting their turn
        /// </summary>
        /// <param name="figureController"></param>
        internal void StartTurn(EncounterFigureController figureController)
        {
            if (_figureController?.Figure == null) { return; }
            if (figureController == _figureController)
            {
                _heroActionButtons.EndTurnButton.Show();
                _heroActionButtons.EndTurnButton.Enabled = true;
                _heroActionButtons.TakeTurnButton.Hide();
            }
            else
            {
                _heroActionButtons.TakeTurnButton.Enabled = false;
                _heroActionButtons.EndTurnButton.Hide();
            }
        }

        /// <summary>
        /// Called when any figure is ending their turn
        /// </summary>
        internal void TurnEnded()
        {
            if (_figureController?.Figure == null) { return; }
            _heroActionButtons.TakeTurnButton.Show();
            _heroActionButtons.EndTurnButton.Hide();
            _heroActionButtons.TakeTurnButton.Enabled = !_figureController.Figure.HasTakenTurn;
        }
    }
}