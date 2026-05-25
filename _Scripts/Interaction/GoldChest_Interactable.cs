/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Interaction handler dictating gold chest opening logic, spawning physical loot outputs on demand,
/// and subsequently locking down interaction capabilities avoiding repetitive exploitation.
/// </summary>
public class GoldChest_Interactable : BaseInteractable
{
    [Header("Loot Settings")]
    [Tooltip("Target scriptable object containing table loot weight distributions.")]
    [SerializeField] private LootTableSO lootTable;

    [Tooltip("Ejection position reference node evaluating item physics generation.")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Animation component actively controlling visual structure opening phases.")]
    [SerializeField] private Animator animator;

    private bool isOpen = false;

    private void Start()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    /// <summary>
    /// Checks dynamic active booleans determining correct string context returned during HUD queries.
    /// </summary>
    /// <returns>Formatted interaction prompt or empty string effectively silencing HUD displays once opened.</returns>
    public override string GetInteractionPrompt()
    {
        return isOpen ? "" : "[E] Open Chest";
    }

    /// <summary>
    /// Resolves primary interaction event requesting loot generation operations and animations successfully.
    /// </summary>
    public override void RequestInteract()
    {
        if (isOpen) return;

        OpenChest();
    }

    private void OpenChest()
    {
        isOpen = true;

        if (AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.chestOpen, transform.position);
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Open");
        }

        SpawnLoot();

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    private void SpawnLoot()
    {
        if (lootTable != null && spawnPoint != null)
        {
            GameObject prefabToSpawn = GetRandomLoot();

            if (prefabToSpawn != null)
            {
                GameObject lootObj = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

                Rigidbody rb = lootObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.up * 2f + transform.forward * 1f, ForceMode.Impulse);
                }
            }
        }
    }

    private GameObject GetRandomLoot()
    {
        if (lootTable.commonLootList.Length > 0)
        {
            int randomIndex = Random.Range(0, lootTable.commonLootList.Length);
            return lootTable.commonLootList[randomIndex];
        }
        return null;
    }

    /// <summary>
    /// Resolves hold-key interactions. Left functionally empty for chests.
    /// </summary>
    public override void RequestHoldInteract() { }

    /// <summary>
    /// Ends hold-key interaction. Left functionally empty for chests.
    /// </summary>
    public override void RequestReleaseInteract() { }
}