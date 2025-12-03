// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Lightweight generic singleton base for manager-type MonoBehaviours.
/// Ensures a single instance is accessible via the static Instance property and persists across scenes.
/// </summary>
/// <typeparam name="T">Concrete MonoBehaviour type deriving from this class.</typeparam>
public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance; // Backing field for the singleton instance

    /// <summary>
    /// Global access to the singleton instance. If none exists in the scene, this will return null.
    /// Consumers should perform null checks during domain reload or early startup.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(); // Attempt to locate an existing instance in the scene
                // Note: We intentionally do not create a new GameObject here to allow explicit bootstrapping by scenes
            }
            return _instance;
        }
    }

    /// <summary>
    /// Awake ensures a single instance is kept and marks it DontDestroyOnLoad so it persists across scenes.
    /// If a duplicate is detected it is destroyed to maintain single-instance guarantee.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T; // Assign instance

            // Ensure the manager is at root level so DontDestroyOnLoad works reliably
            transform.SetParent(null);

            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else if (_instance != this)
        {
            // Destroy duplicate instance created in another scene to enforce singleton behaviour
            Destroy(gameObject);
        }
    }
}