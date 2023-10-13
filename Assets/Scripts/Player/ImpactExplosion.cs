using UnityEngine;

public class ImpactExplosion : MonoBehaviour
{
    public float lifeTime = 1f;
    
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}