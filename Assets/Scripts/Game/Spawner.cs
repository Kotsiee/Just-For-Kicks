using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameInformation gameInfo;
    public IEnumerator spawnItems(float waitTime, int noItems)
    {
        if (GameInformation.inPlay)
            for (int i = 0; i <= noItems; i++)
                Instantiate(gameInfo.ballPrefab, new Vector3(Random.Range(-1.5f, 1.5f), -GameInformation.camHeight / 2, 0), Quaternion.identity, this.transform);

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
