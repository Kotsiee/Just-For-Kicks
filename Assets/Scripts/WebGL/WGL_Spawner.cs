using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGL_Spawner : MonoBehaviour
{
    [SerializeField] private WGL_GameInfo gameInfo;
    public IEnumerator spawnItems(float waitTime, int noItems)
    {
        if (WGL_GameInfo.inPlay)
            for (int i = 0; i <= noItems; i++)
                Instantiate(gameInfo.ballPrefab, new Vector3(Random.Range(-1.5f, 1.5f), -WGL_GameInfo.camHeight / 2, 0), Quaternion.identity, this.transform);

        yield return new WaitForSeconds(waitTime);
        StartCoroutine(spawnItems(Random.Range(1f, 5f), Random.Range(1, 3)));
    }

    public void stopSpawning()
    {
        StopAllCoroutines();
    }

    public void destroyAllItems()
    {
        foreach (Transform child in this.transform)
            Destroy(child.gameObject);
    }
}
