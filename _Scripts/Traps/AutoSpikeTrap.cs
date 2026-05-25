/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls an automated spike trap that periodically cycles between active and hidden states.
/// Routes all damage through the central combat authority.
/// </summary>
public class AutoSpikeTrap : MonoBehaviour
{
    [Header("Timing Settings")]
    [Tooltip("Delay in seconds before the trap starts its cycle, useful for desyncing multiple traps.")]
    [SerializeField] private float startOffset = 0f;

    [Tooltip("Duration in seconds the spikes remain in the active/dangerous position.")]
    [SerializeField] private float activeDuration = 2.0f;

    [Tooltip("Duration in seconds the spikes remain hidden/safe.")]
    [SerializeField] private float inactiveDuration = 2.0f;

    [Header("Damage Settings")]
    [Tooltip("Amount of damage dealt per second while the player stands on the active spikes.")]
    [SerializeField] private float damageAmount = 25f;

    [Header("Visuals & Motion")]
    [Tooltip("The transform containing the spike meshes that will be moved up and down.")]
    [SerializeField] private Transform spikesVisual;

    [Tooltip("Height difference on the Y axis between hidden and active states.")]
    [SerializeField] private float moveDistance = 1.0f;

    [Tooltip("Speed at which the spikes extend or retract.")]
    [SerializeField] private float moveSpeed = 5.0f;

    private bool isDangerous = false;
    private Vector3 hiddenPosition;
    private Vector3 activePosition;

    private void Awake()
    {
        if (spikesVisual != null)
        {
            hiddenPosition = spikesVisual.localPosition;
            activePosition = hiddenPosition + new Vector3(0, moveDistance, 0);
        }
    }

    private void Start()
    {
        StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        yield return new WaitForSeconds(startOffset);

        while (true)
        {
            yield return StartCoroutine(MoveSpikes(activePosition));
            isDangerous = true;
            yield return new WaitForSeconds(activeDuration);

            yield return StartCoroutine(MoveSpikes(hiddenPosition));
            isDangerous = false;
            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private IEnumerator MoveSpikes(Vector3 target)
    {
        if (spikesVisual == null) yield break;

        while (Vector3.Distance(spikesVisual.localPosition, target) > 0.01f)
        {
            spikesVisual.localPosition = Vector3.MoveTowards(spikesVisual.localPosition, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        spikesVisual.localPosition = target;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDangerous && other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (CombatManager.Instance != null)
                {
                    float dps = damageAmount * Time.deltaTime * 5f;
                    CombatManager.Instance.ProcessPlayerHit(damageable, dps, Vector3.zero, transform.position);
                }
            }
        }
    }
}