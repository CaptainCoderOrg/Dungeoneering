using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Dungeoneering.Unity.Editor;

using UnityEngine;
using UnityEngine.UI;

public class TextureInfoPanelTestData : MonoBehaviour
{
    public DungeonCrawlerData DungeonCrawlerData;
    public TextureInfoPanel Panel;
    public Button DeleteButton;

    void Awake()
    {
        Panel.Texture = DungeonCrawlerData.GetTexture("white-tile.png");
    }
}