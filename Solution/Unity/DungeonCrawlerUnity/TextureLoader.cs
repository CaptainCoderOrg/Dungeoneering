using CaptainCoder.Dungeoneering.DungeonMap.IO;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class TextureData : MonoBehaviour
{

    [field: SerializeField]
    public TextAsset JsonData { get; private set; } = default!;

    [field: SerializeField]
    public MeshRenderer Renderer { get; private set; } = default!;

    public void Awake()
    {
        Renderer.material = MakeMaterial();
    }

    public Material MakeMaterial()
    {
        Texture loaded = JsonExtensions.LoadModel<Texture>(JsonData.text);
        Texture2D texture = new(2, 2);
        ImageConversion.LoadImage(texture, loaded.Data);
        Material material = new(Shader.Find("Standard"));
        material.mainTexture = texture;
        return material;
    }
}