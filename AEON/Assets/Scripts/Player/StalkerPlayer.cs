using UnityEngine;
using AEON.Core;
using AEON.World;

namespace AEON.Player
{
    public class StalkerPlayer : PlayerBase
    {
        [Header("Stalker Settings")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private LayerMask environmentLayer;

        private float lastAttackTime;

        protected override void Awake()
        {
            base.Awake();
            playerWorld = WorldType.Light;
        }

        protected override void HandleInput()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        private void Attack()
        {
            lastAttackTime = Time.time;

            Vector2 attackDirection = moveInput.magnitude > 0.1f ? moveInput : Vector2.down;
            Vector2 attackPosition = (Vector2)transform.position + attackDirection * attackRange;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(attackPosition, attackRange * 0.5f, Vector2.zero, 0f, environmentLayer);

            foreach (var hit in hits)
            {
                if (HitRegistry.Instance != null)
                {
                    HitRegistry.Instance.RegisterHit(hit.point, attackDamage);
                }
            }

            if (HitRegistry.Instance != null && hits.Length == 0)
            {
                HitRegistry.Instance.RegisterHit(attackPosition, attackDamage);
            }

            Debug.Log($"Stalker attacked at {attackPosition}");
        }

        private void Interact()
        {
            Vector2 interactPosition = (Vector2)transform.position + moveInput * 1.5f;
            GridCell cell = GridManager.Instance?.GetCellAtWorldPosition(interactPosition);

            if (cell != null)
            {
                Debug.Log($"Stalker interacting with cell at {cell.position}");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 attackDirection = moveInput.magnitude > 0.1f ? moveInput : Vector2.down;
            Vector2 attackPosition = (Vector2)transform.position + attackDirection * attackRange;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition, attackRange * 0.5f);
        }
    }
}
