using UnityEngine;

/// <summary>
/// Keeps the trail renderer slightly offset from the AttackOrigin
/// along its facing direction in 2D space (based on local right axis).
/// </summary>
public class TrailAttackOrigin : MonoBehaviour
{
    [SerializeField] private float offsetDistance = 2f; // outward distance from AttackOrigin

    private Transform parentOrigin;

    private void Awake()
    {
        parentOrigin = transform.parent;
        if (!parentOrigin)
        {
            Debug.LogWarning($"{nameof(TrailAttackOrigin)}: No parent found. Attach this under the AttackOrigin");
        }
    }

    private void LateUpdate()
    {
        if (!parentOrigin) return;

        // Offset along the parent's right direction (the direction the weapon faces)
        Vector3 offsetDir = parentOrigin.up;
        transform.position = parentOrigin.position + offsetDir * offsetDistance;
    }
}