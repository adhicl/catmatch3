using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //[RequireComponent(typeof(SpriteRenderer))]
    public class DiagonalTextureScroll : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        public Vector2 scrollSpeed;

        private Vector2 offset = Vector2.zero;

        void Start()
        {
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            _spriteRenderer.material = new Material(_spriteRenderer.material);
        }

        void Update()
        {
            Debug.Log("Scroll");
            // Apply the offset to the material's main texture
            offset += scrollSpeed * Time.deltaTime;
            _spriteRenderer.material.SetTextureOffset("_MainTex", offset);
        }
    }
    
}