using System.Collections;
using UnityEngine;

public class PaperProcessor : MonoBehaviour
{
    public Transform destinationDesk;
    public Transform[] moneySpawnPoints;
    public GameObject moneyPrefab;
    public float processTime = 1.0f;
    public float lerpSpeed = 1.0f;

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
        GameObject money = Instantiate(moneyPrefab);
        money.transform.position = destinationDesk.position;

        // Start the coroutine to move the money to the designated spawn point
        StartCoroutine(MoveMoneyToSpawnPoint(money.transform, moneySpawnPoints[currentMoneySpawnIndex]));

        // Update the index for the next spawn point
        currentMoneySpawnIndex = (currentMoneySpawnIndex + 1) % moneySpawnPoints.Length;
    }

    private IEnumerator MoveMoneyToSpawnPoint(Transform moneyTransform, Transform targetSpawnPoint)
    {
        float startTime = Time.time;
        Vector3 startPosition = moneyTransform.position;
        Quaternion startRotation = moneyTransform.rotation;

        while (Time.time - startTime < lerpSpeed)
        {
            float t = (Time.time - startTime) / lerpSpeed;
            moneyTransform.position = Vector3.Lerp(startPosition, targetSpawnPoint.position, t);
            moneyTransform.rotation = Quaternion.Lerp(startRotation, targetSpawnPoint.rotation, t);
            yield return null;
        }
    }
}
