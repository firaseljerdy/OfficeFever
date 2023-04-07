/*
 * The PaperPool script is a Singleton object pooling system for paper GameObjects.
 * It optimizes performance by reusing inactive paper objects instead of instantiating new ones.
 * The script maintains a Queue of GameObjects representing the pool of paper objects. The GetPaper method dequeues
 * and activates a paper object from the pool or instantiates a new one if the pool is empty.
 * The ReturnPaper method deactivates the paper object and enqueues it back into the pool.
 */


using System.Collections.Generic;
using UnityEngine;

public class PaperPool : MonoBehaviour
{
    public static PaperPool Instance { get; private set; }

    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private int poolSize = 100;
    private GameObject poolParent;

    private Queue<GameObject> pooledPapers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        pooledPapers = new Queue<GameObject>();
        poolParent = new GameObject("Paper Pool");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject paper = Instantiate(paperPrefab);
            paper.SetActive(false);
            paper.transform.SetParent(poolParent.transform);
            pooledPapers.Enqueue(paper);
        }
    }

    public GameObject GetPaper()
    {
        if (pooledPapers.Count > 0)
        {
            GameObject paper = pooledPapers.Dequeue();
            paper.SetActive(true);
            //Debug.Log("Paper retrieved from pool");
            return paper;
        }
        else
        {
            GameObject paper = Instantiate(paperPrefab);
            Debug.Log("New paper instantiated");
            return paper;
        }
    }

    public void ReturnPaper(GameObject paper)
    {
        Debug.Log("Returning paper to the pool: " + paper.name);
        paper.SetActive(false);
        pooledPapers.Enqueue(paper);
    }
}
