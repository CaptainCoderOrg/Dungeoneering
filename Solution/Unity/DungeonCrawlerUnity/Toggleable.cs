using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class Toggleable : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}