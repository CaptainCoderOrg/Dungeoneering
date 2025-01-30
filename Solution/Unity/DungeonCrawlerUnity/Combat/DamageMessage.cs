using System.Collections;

using TMPro;

using UnityEngine;
namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class DamageMessage : MonoBehaviour
{
    public TextMeshPro Text = default!;

    public void Render(int damage, Position position)
    {
        if (damage < 0) { return; }
        gameObject.transform.localPosition = position.ToVector3Int();
        gameObject.SetActive(true);
        Text.text = damage.ToString();
        StartCoroutine(DestroyAfter(2f));
    }

    private IEnumerator DestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}