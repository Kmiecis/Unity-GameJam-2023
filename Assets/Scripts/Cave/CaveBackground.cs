using System;
using Common;
using Common.Coroutines;
using Common.Extensions;
using UnityEngine;

namespace Game
{
    public class CaveBackground : MonoBehaviour
    {
        private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
        private readonly int DISSOLVE_FLIP_ID = Shader.PropertyToID("_DissolveFlip");
        
        [SerializeField]
        protected MeshRenderer _renderer;
        [SerializeField]
        protected int _renderQueue = 2450;

        private Material _material;
        
        public void SetSharedMaterial(Material material)
        {
            UObject.Destroy(ref _material);

            _renderer.sharedMaterial = _material = new Material(material);
            _material.renderQueue = _renderQueue;
        }

        public bool HasSharedMaterial()
        {
            return _material != null;
        }

        public void Disintegrate()
        {
            _material.SetFloat(DISSOLVE_FLIP_ID, 1.0f);
            _material.SetFloat(DISSOLVE_ID, 0.0f);
            _material.CoFloat(1.0f, DISSOLVE_ID, 1.0f, Easings.SmoothStep)
                .Then(Destroy)
                .Start(this);
        }

        public void Integrate()
        {
            _material.SetFloat(DISSOLVE_FLIP_ID, 0.0f);
            _material.SetFloat(DISSOLVE_ID, 1.0f);
            _material.CoFloat(0.0f, DISSOLVE_ID, 1.0f, Easings.SmoothStep)
                .Start(this);
        }
        
        public CaveBackground Copy()
        {
            var copy = Instantiate(this, transform.parent);
            var currentIndex = transform.GetSiblingIndex();
            copy.transform.SetSiblingIndex(currentIndex + 1);
            return copy;
        }

        private void Destroy()
        {
            gameObject.Destroy();
        }

        private void OnDestroy()
        {
            _material?.Destroy();
        }
    }
}