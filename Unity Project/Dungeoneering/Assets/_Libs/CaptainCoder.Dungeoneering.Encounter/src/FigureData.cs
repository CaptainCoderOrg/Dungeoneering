using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    public class FigureData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AnimationData[] Animations { get; private set; }
    }
}