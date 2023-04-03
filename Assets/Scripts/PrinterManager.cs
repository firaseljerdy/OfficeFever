using UnityEngine;
using System;

public class PrinterManager : MonoBehaviour
{
    public static PrinterManager Instance { get; private set; }

    public event Action OnPapersCollected;

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
    }

    public void PapersCollected()
    {
        OnPapersCollected?.Invoke();
    }
}
