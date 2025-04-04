using System;

#if UNITY_EDITOR
using System.Reflection;
#endif

using NaughtyAttributes;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/TacticData")]
    public class TacticData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color BackgroundAlbedo { get; private set; } = new Color(0, 0, 0, 0.5f);
        [field: ShowAssetPreview][field: SerializeField] public Sprite Sprite { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _description;
        [field: SerializeField][field: SerializeReference] public ITacticEffect Effect { get; private set; }
        public string Description => _description;
        private string _tooltip;
        public string Tooltip => _tooltip ??= $"<u>{Name}</u>\n{_description}";
        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _tooltip = $"<u>{Name}</u>\n{_description}";
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            _tooltip = null;
        }

        [SerializeField] private string _namespace = "CaptainCoder.Dungeoneering.Encounter";
        [SerializeField] private string _addEffectClassName;
        [Button]
        public void SetEffectType()
        {
            Type t = Type.GetType($"{_namespace}.{_addEffectClassName}");
            ConstructorInfo constructor = t.GetConstructors()[0];
            Effect = (ITacticEffect)constructor.Invoke(default);
        }
#endif
    }

}