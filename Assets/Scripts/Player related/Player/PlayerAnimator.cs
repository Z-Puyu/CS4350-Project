
using System;
using UnityEngine;

namespace Player_related.Player
{
    [Obsolete]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator: MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private bool _isAttacking;
        private bool _comboPossible;
        
        private void Awake()
        {
            this._animator = this.GetComponent<Animator>();
            this._spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // Movement Animation
            _animator.SetBool("IsWalking", movement.IsWalking);
            
            // Flip based on horizontal direction
            if (movement.CurrentDirection.x < 0)
            {
                _spriteRenderer.flipX = true;
            } else if (movement.CurrentDirection.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
        }

        public void PlayAttack()
        {
            if (!_isAttacking)
            {
                // First Attack
                _animator.SetTrigger("AttackTrigger");
                _isAttacking = true;
                _comboPossible = true;
            } else if (_comboPossible)
            {
                // Queue Combo
                _animator.SetTrigger("AttackTrigger");
                _comboPossible = false;
            }
        }

        public void AttackAnimationEnded()
        {
            _isAttacking = false;
            _comboPossible = false;
        }
    }
}