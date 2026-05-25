/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using TMPro;

/// <summary>
/// Component attached to instantiated text elements representing short-lived floating visual feedback.
/// Handles spatial drifting and fading mechanics gracefully over its intended lifespan.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0, 0);

    private Color startColor;
    private float timeElapsed = 0f;
    private Vector3 driftDirection;

    /// <summary>
    /// Initializes the text content and color, and sets the initial drift direction.
    /// </summary>
    /// <param name="text">The string to display.</param>
    /// <param name="color">The color of the text.</param>
    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        startColor = color;

        driftDirection = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            1f,
            Random.Range(-randomOffset.z, randomOffset.z)
        ).normalized;
    }

    private void Update()
    {
        transform.position += driftDirection * moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;
        float fadeAlpha = Mathf.Lerp(1f, 0f, timeElapsed / lifeTime);

        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);

        if (timeElapsed >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}