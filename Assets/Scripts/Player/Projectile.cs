using Common.Extensions;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    public float speed = 2;
    public int explosionRadius = 1;
    private CaveManager _caveManager;

    private Rigidbody2D _body;

    public GameObject explosionPrefab;
    
    public Sound impactSound;
    private SoundsManager _soundsManager;
    
    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _caveManager = FindObjectOfType<CaveManager>();
        _soundsManager = FindObjectOfType<SoundsManager>();
    }

    public void SetData(Vector3 direction)
    {
        _body.velocity = new Vector2(direction.x, direction.y).normalized * speed;

        var rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 180);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var position = transform.position;
        _soundsManager.PlaySound(impactSound);
        _caveManager.Damage((int) position.x, (int) position.y, explosionRadius);
        Instantiate(explosionPrefab, position, Quaternion.identity);
        gameObject.Destroy();
    }
}