using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterInitializer : MonoBehaviour
    {
        private EncounterController _controller;
        private EncounterController Controller => _controller = (_controller == null ? GetComponentInParent<EncounterController>() : _controller);
        private EncounterState State => Controller.State;
        [AssertIsSet][SerializeField] private EncounterFigureController _enemyFigurePrefab;
        [AssertIsSet][SerializeField] private Transform _enemyFigureParent;
        [AssertIsSet][field: SerializeField] private EnemyFigurePanel _enemyFigurePanel;
        [AssertIsSet][field: SerializeField] private EnemyFigurePanel[] _heroFigurePanels;

        public void Init(EncounterData encounterData)
        {
            State.Figures.Clear();
            foreach (EnemyFigure f in encounterData.EnemyFigures)
            {
                if (State.Figures.ContainsKey(f.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {f.Position}");
                }
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.Figure = FigureData.CopyEnemyAndCreate(f.EnemyEntityTemplate, f.Position);

                State.Figures[f.Position] = controller;
                controller.OnSelected.AddListener(_enemyFigurePanel.Render);
            }

            for (int ix = 0; ix < encounterData.HeroFigures.Count; ix++)
            {
                HeroFigure h = encounterData.HeroFigures[ix];
                if (State.Figures.ContainsKey(h.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {h.Position}");
                }
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.Figure = FigureData.Create(h.HeroEntity, h.Position);

                State.Figures[h.Position] = controller;
                controller.OnSelected.AddListener(_heroFigurePanels[ix].Render);
            }
        }
    }
}