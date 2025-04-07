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
                controller.OnSelected.AddListener(() => _enemyFigurePanel.Render(controller.Figure));
                controller.OnDeselected.AddListener(_enemyFigurePanel.Hide);
                controller.OnClick.AddListener(_controller.Select);
            }

            int ix = 0;
            for (; ix < encounterData.HeroFigures.Count; ix++)
            {
                HeroFigure h = encounterData.HeroFigures[ix];
                if (State.Figures.ContainsKey(h.Position))
                {
                    Debug.Log($"Illegal configuration, multiple figures in {h.Position}");
                }
                EncounterFigureController controller = Instantiate(_enemyFigurePrefab, _enemyFigureParent);
                controller.name = $"{h.HeroEntity.Name}'s Figure";
                controller.Figure = FigureData.Create(h.HeroEntity, h.Position);

                State.Figures[h.Position] = controller;
                HeroFigurePanel panel = Controller.HeroPanels[ix];
                panel.FigureController = controller;
                controller.OnClick.AddListener(_controller.Select);
                controller.OnSelected.AddListener(panel.Select);
                controller.OnDeselected.AddListener(panel.Deselect);
            }
            for (; ix < Controller.HeroPanels.Length; ix++)
            {
                Controller.HeroPanels[ix].gameObject.SetActive(false);
            }
        }
    }
}