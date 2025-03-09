using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;
using CaptainCoder.Unity.UI;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class ConfirmTextureReplacementPrompt : ConfirmPromptPanel
    {
        [field: SerializeField]
        public RawImage OriginalImage { get; private set; }
        [field: SerializeField]
        public RawImage NewImage { get; private set; }

        void Awake()
        {
            Assertion.NotNull(this, (OriginalImage, "Original Image was not set"), (NewImage, "New Image was not set"));
        }

        public void PromptReplacement(TextureReference original, Texture2D newTexture, System.Action onConfirm, System.Action onCancel = null)
        {
            NewImage.texture = newTexture;
            OriginalImage.texture = original.Material.Unselected.mainTexture;
            base.Prompt("Are you sure you want to replace this texture?", onConfirm, onCancel);
        }

    }
}