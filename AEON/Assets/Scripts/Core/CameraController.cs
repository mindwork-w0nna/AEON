using UnityEngine;
using AEON.Player;

namespace AEON.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform targetPlayer;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private bool followPlayer = true;

        private void LateUpdate()
        {
            if (!followPlayer || targetPlayer == null)
            {
                if (targetPlayer == null)
                {
                    StalkerPlayer stalker = FindObjectOfType<StalkerPlayer>();
                    if (stalker != null)
                    {
                        targetPlayer = stalker.transform;
                    }
                }
                return;
            }

            Vector3 desiredPosition = targetPlayer.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }

        public void SetTarget(Transform newTarget)
        {
            targetPlayer = newTarget;
        }
    }
}
