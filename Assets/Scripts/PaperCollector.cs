using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperCollector : MonoBehaviour
{
    public GameObject[] printers;
    public Transform handTarget;
    public float collectionDistance = 1.5f;
    public float collectionSpeed = 0.5f;
    public float rotationSpeed = 5.0f;
    public PaperSetter paperSetter;

    private List<Transform> printerTargets;
    private bool[] coroutineStarted;
    public bool IsCollecting { get; private set; }
    private int paperCount;
    private bool[] inRange;

    private void Start()
    {
        printerTargets = new List<Transform>();
        foreach (GameObject printer in printers)
        {
            PaperPrinter printerScript = printer.GetComponent<PaperPrinter>();
            if (printerScript != null)
            {
                foreach (Transform location in printerScript.printLocations)
                {
                    printerTargets.Add(location);
                }
            }
        }

        coroutineStarted = new bool[printerTargets.Count];
        inRange = new bool[printerTargets.Count];
    }

    private void Update()
    {
        for (int i = 0; i < printerTargets.Count; i++)
        {
            Transform deskTarget = printerTargets[i];
            float distanceToDesk = HorizontalDistance(transform.position, deskTarget.position);
            Debug.Log("Distance to desk " + i + ": " + distanceToDesk);
            if (distanceToDesk <= collectionDistance)
            {
                Debug.Log("In range of desk " + i);
                inRange[i] = true;
                if (!coroutineStarted[i])
                {
                    Debug.Log("Coroutine started for desk: " + i);
                    coroutineStarted[i] = true;
                    StartCoroutine(CollectPapers(deskTarget, i));
                }
            }
            else
            {
                inRange[i] = false;
            }
        }

        if (paperSetter.CheckDistance(transform.position) && handTarget.childCount > 0)
        {
            paperSetter.SetPapers(handTarget);
        }

        for (int i = 0; i < handTarget.childCount; i++)
        {
            Transform child = handTarget.GetChild(i);
            if (child.gameObject.CompareTag("Paper"))
            {
                child.localPosition = new Vector3(0, i * 0.01f, 0);
            }
        }
    }

    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        Vector2 aXZ = new Vector2(a.x, a.z);
        Vector2 bXZ = new Vector2(b.x, b.z);
        return Vector2.Distance(aXZ, bXZ);
    }

    private IEnumerator CollectPapers(Transform deskTarget, int deskIndex)
    {
        List<Transform> allPapers = new List<Transform>();

        for (int j = 0; j < deskTarget.childCount; j++)
        {
            Transform child = deskTarget.GetChild(j);
            if (child.gameObject.CompareTag("Paper"))
            {
                allPapers.Add(child);
                child.SetParent(null);
                Debug.Log("Paper added to allPapers: " + child.name);
            }
        }
        Debug.Log("All papers count: " + allPapers.Count);
        bool[] paperArrived = new bool[allPapers.Count];
        int arrivedCount = 0;

        while (arrivedCount < allPapers.Count)
        {
            if (inRange[deskIndex])
            {
                for (int i = 0; i < allPapers.Count; i++)
                {
                    if (!paperArrived[i])
                    {
                        // Calculate the height of the existing stack of papers in the handTarget
                        float stackHeight = handTarget.childCount * 0.01f;
                        Vector3 targetPosition = handTarget.position + new Vector3(0, stackHeight, 0);


                        allPapers[i].position = Vector3.MoveTowards(allPapers[i].position, targetPosition, collectionSpeed * Time.deltaTime);
                        allPapers[i].rotation = Quaternion.RotateTowards(allPapers[i].rotation, handTarget.rotation, rotationSpeed * Time.deltaTime);

                        Debug.Log("Moving paper " + i + " towards hand. Current position: " + allPapers[i].position + " Target position: " + targetPosition);

                        if (Vector3.Distance(allPapers[i].position, targetPosition) < 0.01f)
                        {
                            allPapers[i].SetParent(handTarget);
                            paperArrived[i] = true;
                            arrivedCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < allPapers.Count; i++)
                {
                    if (!paperArrived[i])
                    {
                        Vector3 deskPaperPosition = deskTarget.TransformPoint(new Vector3(0, i * 0.01f, 0));
                        allPapers[i].position = Vector3.MoveTowards(allPapers[i].position, deskPaperPosition, collectionSpeed * Time.deltaTime);
                        allPapers[i].rotation = Quaternion.RotateTowards(allPapers[i].rotation, deskTarget.rotation, rotationSpeed * Time.deltaTime);
                    }
                }
            }
            yield return null;
        }
        Debug.Log("Arrived papers count: " + arrivedCount);
        paperCount += allPapers.Count;
        coroutineStarted[deskIndex] = false;
        allPapers.Clear();
    }
}