using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class SelectedDungeonWalls : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject NorthWall { get; private set; } = default!;
        [field: SerializeField]
        public GameObject EastWall { get; private set; } = default!;
        [field: SerializeField]
        public GameObject SouthWall { get; private set; } = default!;
        [field: SerializeField]
        public GameObject WestWall { get; private set; } = default!;
        public Facing Facing
        {
            set
            {
                GameObject wall = value switch
                {
                    Facing.North => NorthWall,
                    Facing.East => EastWall,
                    Facing.South => SouthWall,
                    Facing.West => WestWall,
                    _ => throw new System.Exception($"Unknown Facing: {value}"),
                };
                wall.SetActive(true);
            }
        }

        void Awake()
        {
            NorthWall.SetActive(false);
            EastWall.SetActive(false);
            SouthWall.SetActive(false);
            WestWall.SetActive(false);
        }
    }
}