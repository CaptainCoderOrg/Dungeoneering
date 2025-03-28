using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterInitializer : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterState _state;
        [AssertIsSet][SerializeField] private EncounterFigureController _enemyFigurePrefab;
        [AssertIsSet][SerializeField] private Transform _enemyFigureParent;
        [AssertIsSet][field: SerializeField] private EnemyFigurePanel _enemyFigurePanel;

        void Awake()
        {
            _controller = GetComponentInParent<EncounterController>();
            _state = _controller.State;
        }

        public void Init(EncounterData encounterData)
        {
            _state.Figures.Clear();
            foreach (EnemyFigure f in encounterData.EnemyFigures)
            {
                if (_state.Figures.ContainsKey(f.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {f.Position}");
                }
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.Figure = FigureData.CopyEnemyAndCreate(f.EnemyEntityTemplate, f.Position);

                _state.Figures[f.Position] = controller;
                controller.OnSelected.AddListener(_enemyFigurePanel.Render);
            }
        }
    }
}