using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/EncounterSettingsData")]
    public class EncounterSettingsData : ObservableSO
    {
        public float TargetZoom = 5;
        public float TargetRotation = 0;
        public int TargetPitch = 1;
        [SerializeField] private float _movementSpeed = 0.1f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = value;
                WaitForMovement = new WaitForSeconds(_movementSpeed);
            }
        }
        public WaitForSeconds WaitForMovement { get; private set; }

        void OnValidate()
        {
            WaitForMovement = new WaitForSeconds(_movementSpeed);
        }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            WaitForMovement = new WaitForSeconds(MovementSpeed);
        }
    }
}