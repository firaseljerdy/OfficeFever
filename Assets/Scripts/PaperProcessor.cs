using System.Collections;
using UnityEngine;

public class PaperProcessor : MonoBehaviour
{
    public Transform destinationDesk;
    public Transform[] moneySpawnPoints;
    public GameObject moneyPrefab;
    public float processTime = 1.0f;
    public float lerpSpeed = 1.0f;
    public float moneyHeightOffset = 0.1f;

    private int currentMoneySpawnIndex = 0;

    private void Update()
    {
        if (destinationDesk.childCount > 0 && !IsInvoking(nameof(ProcessPaper)))
        {
            Invoke(nameof(ProcessPaper), processTime);
        }
    }

    private void ProcessPaper()
    {
        if (destinationDesk.childCount <= 0)
        {
            return;
        }

        // Remove the first paper from the desk
        Destroy(destinationDesk.GetChild(0).gameObject);

        // Instantiate money and set its initial position
        SpawnMoney();

        // Update the index for the next spawn point
        currentMoneySpawnIndex = (currentMoneySpawnIndex + 1) % moneySpawnPoints.Length;
    }

    private void SpawnMoney()
    {
        Transform moneySpawnPoint = moneySpawnPoints[currentMoneySpawnIndex];
        Vector3 spawnPosition = moneySpawnPoint.position;
        spawnPosition.y += moneySpawnPoint.childCount * moneyHeightOffset;

        GameObject spawnedMoney = Instantiate(moneyPrefab, spawnPosition, moneySpawnPoint.rotation);
        spawnedMoney.transform.SetParent(moneySpawnPoint);
    }


    private IEnumerator MoveMoneyToSpawnPoint(Transform moneyTransform, Transform targetSpawnPoint)
    {
        float startTime = Time.time;
        Vector3 startPosition = moneyTransform.position;
        Quaternion startRotation = moneyTransform.rotation;

        Vector3 targetPosition = targetSpawnPoint.position;
        targetPosition.y += targetSpawnPoint.childCount * moneyHeightOffset;

        while (Time.time - startTime < lerpSpeed)
        {
            if (moneyTransform == null || targetSpawnPoint == null)
            {
                yield break;
            }
            float t = (Time.time - startTime) / lerpSpeed;
            moneyTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            moneyTransform.rotation = Quaternion.Lerp(startRotation, targetSpawnPoint.rotation, t);
            yield return null;
        }

        moneyTransform.SetParent(targetSpawnPoint);
    }

}
