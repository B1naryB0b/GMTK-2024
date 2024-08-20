using System;
using System.Collections;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
    private bool _isTakingDamage = false;
    [SerializeField] private float damageCooldown = 0.5f; // Cooldown duration in seconds
    [SerializeField] private int dropletsReleased = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!_isTakingDamage && other.gameObject.TryGetComponent(out DropletManager dropletManager))
        {
            for (int i = 0; i < dropletsReleased; i++)
            {
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                if (dropletManager.GetMass() > 0f)
                {
                    dropletManager.SubtractMass(randomDirection);
                }
            }
            
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        _isTakingDamage = true;
        yield return new WaitForSeconds(damageCooldown);
        _isTakingDamage = false;
    }
}