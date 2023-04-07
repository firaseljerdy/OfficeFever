/*
 * The PrinterManager script is a Singleton responsible for managing printer events. It has an event called OnPapersCollected,
 * which is invoked in the PapersCollected method.
 * The event can be used by other scripts to respond to changes in the paper collection state.
 */

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
