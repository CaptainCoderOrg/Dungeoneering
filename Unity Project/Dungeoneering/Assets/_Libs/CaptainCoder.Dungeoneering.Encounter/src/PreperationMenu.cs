using System.Linq;

using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class PreperationMenu : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private SelectedPreperationOption[] _selectedOptions;
        [AssertIsSet][SerializeField] private PreperationMenuOption[] _options;

        void Awake()
        {
            foreach (var option in _options)
            {
                option.OnSelectOption += Select;
            }
        }

        public void Select(PreperationData data)
        {
            SelectedPreperationOption option = _selectedOptions.FirstOrDefault(o => o.PreperationData == null);
            if (option != null)
            {
                option.PreperationData = data;
            }
            else
            {
                _selectedOptions[_selectedOptions.Length - 1].PreperationData = data;
            }
        }
    }
}