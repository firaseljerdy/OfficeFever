/*
 The PaperProcessor script handles the paper processing and money generation mechanics.
 In the Update method, the script checks if there is any paper on the destination desk and starts a coroutine (ProcessPaperRoutine) to process the paper if needed.
 The ProcessPaper method removes the first paper from the desk, returning it to the PaperPool, and then instantiates money using the MoneyPool system.

 The spawned money's rotation is set using a Quaternion.Euler method to achieve the desired rotation.
 A coroutine (MoveMoneyToSpawnPoint) is initiated to move and rotate the money object to the designated spawn point on the desk. 
 The money object's position and rotation are interpolated using Vector3.Lerp and Quaternion.Slerp, respectively.
 */


using System.Collections;
using UnityEngine;

public class PaperProcessor : MonoBehaviour
{
    public Transform destinationDesk;
    public Transform[] moneySpawnPoints;
    public float processTime = 1.0f;
    public float lerpSpeed = 1.0f;
    public float moneyHeightOffset = 0.1f;

    private int currentMoneySpawnIndex = 0;
    private Coroutine processPaperCoroutine;

    private void Update()
    {
        //Debug.Log("PaperProcessor Update called");
        if (destinationDesk.childCount > 0 && processPaperCoroutine == null)
        {
            processPaperCoroutine = StartCoroutine(ProcessPaperRoutine());
        }
    }

    private IEnumerator ProcessPaperRoutine()
    {
        while (destinationDesk.childCount > 0)
        {
            yield return new WaitForSeconds(processTime);
            ProcessPaper();
        }
        processPaperCoroutine = null;
    }

    private void ProcessPaper()
    {
        if (destinationDesk.childCount <= 0)
        {
            return;
        }

        // Remove the first paper from the desk
        Transform paper = destinationDesk.GetChild(0);
        PaperPool.Instance.ReturnPaper(paper.gameObject);

        // Get the initial position of the money object
        Vector3 moneyStartPosition = paper.position;


        // Instantiate money and move it to the spawn point
        Transform moneySpawnPoint = moneySpawnPoints[currentMoneySpawnIndex];
        GameObject spawnedMoney = MoneyPool.Instance.GetMoney();

        // Apply the desired rotation to the spawned money
        Quaternion desiredRotation = Quaternion.Euler(90, -90, 0);
        spawnedMoney.transform.localRotation = desiredRotation;

        StartCoroutine(MoveMoneyToSpawnPoint(spawnedMoney.transform, moneyStartPosition, moneySpawnPoint, desiredRotation));

        // Update the index for the next spawn point
        currentMoneySpawnIndex = (currentMoneySpawnIndex + 1) % moneySpawnPoints.Length;
    }

    private IEnumerator MoveMoneyToSpawnPoint(Transform moneyTransform, Vector3 startPosition, Transform targetSpawnPoint, Quaternion desiredRotation)
    {
        float startTime = Time.time;
        Quaternion startRotation = moneyTransform.rotation;
        Quaternion targetRotation = targetSpawnPoint.rotation;

        int targetChildIndex = targetSpawnPoint.childCount;
        moneyTransform.SetParent(targetSpawnPoint);
        Vector3 targetLocalPosition = new Vector3(0, targetChildIndex * moneyHeightOffset, 0);
        Vector3 targetPosition = targetSpawnPoint.TransformPoint(targetLocalPosition);

        while (Time.time - startTime < lerpSpeed)
        {
            if (moneyTransform == null || targetSpawnPoint == null)
            {
                yield break;
            }
            float t = (Time.time - startTime) / lerpSpeed;
            moneyTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            moneyTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        moneyTransform.localRotation = desiredRotation;
    }

}
