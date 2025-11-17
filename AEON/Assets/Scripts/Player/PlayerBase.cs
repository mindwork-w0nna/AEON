using UnityEngine;
using AEON.Core;

namespace AEON.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class PlayerBase : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected WorldType playerWorld;

        protected float currentHealth;
        protected Rigidbody2D rb;
        protected Vector2 moveInput;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }

        protected virtual void Update()
        {
            HandleInput();
        }

        protected virtual void FixedUpdate()
        {
            HandleMovement();
        }

        protected abstract void HandleInput();

        protected virtual void HandleMovement()
        {
            rb.velocity = moveInput * moveSpeed;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died!");
        }

        public float GetHealth() => currentHealth;
        public WorldType GetPlayerWorld() => playerWorld;
    }
}
