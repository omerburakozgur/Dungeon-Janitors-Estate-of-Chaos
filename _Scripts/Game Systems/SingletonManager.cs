/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Lightweight generic singleton base for manager-type MonoBehaviours.
/// Ensures a single instance is accessible via the static Instance property and persists across scenes.
/// </summary>
/// <typeparam name="T">Concrete MonoBehaviour type deriving from this class.</typeparam>
public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

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
                _instance = FindObjectOfType<T>();
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
            _instance = this as T;

            transform.SetParent(null);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}