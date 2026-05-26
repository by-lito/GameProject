using UnityEngine;

public class CameraFollow3D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    [SerializeField] private float followSpeed = 5f;

    private void Start()
    {
        TryFindPlayerTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            TryFindPlayerTarget();

            if (target == null)
            {
                return;
            }
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }

    private void TryFindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("[CameraFollow3D] No se ha encontrado ningún GameObject con tag Player en la escena.", this);
        }
    }
}