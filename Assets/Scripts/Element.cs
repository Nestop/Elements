using UnityEngine;

namespace GameLogic
{
    public class Element
    {
        public int id;
        public Transform transform;
        public SpriteRenderer spriteRenderer;
        public Animator animator;
        public bool Destroy;

        public Element(int id, Transform transform, SpriteRenderer spriteRenderer)
        {
            this.id = id;
            this.transform = transform;
            this.spriteRenderer = spriteRenderer;
            Destroy = false;
            animator = transform.gameObject.GetComponent<Animator>();
        }
    }
}