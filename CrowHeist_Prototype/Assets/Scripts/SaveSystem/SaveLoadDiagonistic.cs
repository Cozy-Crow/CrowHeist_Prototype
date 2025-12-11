using UnityEngine;

/// <summary>
/// Diagnostic helper to debug save/load issues
/// Attach this to any SaveableObject to see detailed logs
/// </summary>
public class SaveLoadDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    [SerializeField] private bool logPosition = true;
    [SerializeField] private bool logVelocity = true;
    [SerializeField] private bool logActiveState = true;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private Rigidbody rb;
    private SaveableObject saveableObj;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        saveableObj = GetComponent<SaveableObject>();
    }

    private void Start()
    {
        LogState("Start");
    }

    private void OnEnable()
    {
        LogState("OnEnable");
    }

    private void OnDisable()
    {
        LogState("OnDisable");
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // Check if position changed significantly
            if (Vector3.Distance(transform.position, lastPosition) > 0.01f && logPosition)
            {
                Debug.Log($"[{gameObject.name}] Position changed: {lastPosition} ? {transform.position}");
            }

            // Check if velocity changed significantly
            if (Vector3.Distance(rb.velocity, lastVelocity) > 0.01f && logVelocity)
            {
                Debug.Log($"[{gameObject.name}] Velocity changed: {lastVelocity} ? {rb.velocity}");
            }

            lastPosition = transform.position;
            lastVelocity = rb.velocity;
        }
    }

    private void LogState(string context)
    {
        string log = $"[{gameObject.name}] {context}:";

        if (logActiveState)
        {
            log += $"\n  Active: {gameObject.activeSelf}";
        }

        if (logPosition)
        {
            log += $"\n  Position: {transform.position}";
        }

        if (rb != null && logVelocity)
        {
            log += $"\n  Velocity: {rb.velocity}";
            log += $"\n  Angular Velocity: {rb.angularVelocity}";
            log += $"\n  Is Kinematic: {rb.isKinematic}";
        }

        if (saveableObj != null)
        {
            log += $"\n  Unique ID: {saveableObj.UniqueId}";
            log += $"\n  Object Type: {saveableObj.ObjectType}";
        }

        Debug.Log(log);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        if (rb != null && rb.velocity.magnitude > 0.01f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        }
    }

    // Context menu for manual testing
    [ContextMenu("Log Current State")]
    private void LogCurrentState()
    {
        LogState("Manual Log");
    }

    [ContextMenu("Reset Velocity")]
    private void ResetVelocity()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log($"[{gameObject.name}] Velocity reset to zero");
        }
    }

    [ContextMenu("Teleport to Origin")]
    private void TeleportToOrigin()
    {
        transform.position = Vector3.zero;
        Debug.Log($"[{gameObject.name}] Teleported to origin");
    }
}
