/*
 * The PaperPrinter script handles the printing of paper objects at different print locations, pooling, and reusing paper objects for optimization.
 * Variables:
 * printLocations: An array of Transforms representing the printing locations.
 * paperPrefab: The GameObject representing the paper prefab.
 * printInterval: The time interval between consecutive paper printings.
 * lerpSpeed: The speed of Lerp used to move the paper to the target location.
 * positionVariation: A Vector2 representing the random position variation on the x and y-axis.
 * rotationVariation: A Vector2 representing the random rotation variation on the x and z-axis.
 * printerExitPoint: A Transform representing the exit point from where the papers are printed.
 */

using System.Collections;
using UnityEngine;

public class PaperPrinter : MonoBehaviour
{
    public Transform[] printLocations;
    public GameObject paperPrefab;
    public float printInterval = 2.0f;
    public float lerpSpeed = 1.0f;
    public Vector2 positionVariation = new Vector2(0.01f, 0.01f);
    public Vector2 rotationVariation = new Vector2(2f, 2f);

    public Transform printerExitPoint;

    private int currentStackIndex = 1;
    private int[] currentStackHeight;
    private int maxStackHeight = 50;

    private void Start()
    {
        currentStackHeight = new int[printLocations.Length];
        StartCoroutine(PrintPapers());
        PrinterManager.Instance.OnPapersCollected += ResetStackHeight;
    }

    private IEnumerator PrintPapers()
    {
        while (true)
        {
            GameObject paper = PaperPool.Instance.GetPaper();
            paper.transform.position = transform.position;
            paper.transform.rotation = Quaternion.identity;

            StartCoroutine(LerpPaperToLocation(paper, printLocations[currentStackIndex]));

            currentStackHeight[currentStackIndex]++;
            if (currentStackHeight[currentStackIndex] >= maxStackHeight)
            {
                currentStackHeight[currentStackIndex] = 0;
            }
            currentStackIndex = (currentStackIndex + 1) % printLocations.Length;

            yield return new WaitForSeconds(printInterval);
        }
    }

    private IEnumerator LerpPaperToLocation(GameObject paper, Transform target)
    {
        paper.transform.SetParent(target);
        paper.transform.localScale = paperPrefab.transform.localScale;

        // Disable collider during the Lerp animation
        Collider paperCollider = paper.GetComponent<Collider>();
        if (paperCollider != null)
        {
            paperCollider.enabled = false;
        }

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        int stackHeightIndex = currentStackHeight[currentStackIndex];
        targetPosition += new Vector3(Random.Range(-positionVariation.x, positionVariation.x), stackHeightIndex * 0.01f, Random.Range(-positionVariation.y, positionVariation.y));
        targetRotation *= Quaternion.Euler(Random.Range(-rotationVariation.x, rotationVariation.x), 0, Random.Range(-rotationVariation.y, rotationVariation.y));

        float startTime = Time.time;
        Vector3 startPosition = printerExitPoint.position;
        Quaternion startRotation = paper.transform.rotation;

        while (Time.time - startTime < lerpSpeed)
        {
            float t = (Time.time - startTime) / lerpSpeed;
            paper.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            paper.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        // Enable collider after the Lerp animation is completed
        if (paperCollider != null)
        {
            paperCollider.enabled = true;
        }
    }
    public void ResetStackHeight()
    {
        currentStackHeight[currentStackIndex] = 0;
    }

    private void OnDestroy()
    {
        if (PrinterManager.Instance != null)
        {
            PrinterManager.Instance.OnPapersCollected -= ResetStackHeight;
        }
    }
}
