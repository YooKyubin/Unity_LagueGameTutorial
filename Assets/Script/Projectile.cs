using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10f;
    float damage = 1;

    float lifetime = 3f;
    float skinWidth = .1f; // 한 프레임에 총알도 enemy도 움직이는 바람에 발생할 수 있는 겹침 문제를 해결
    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = new Collider[1];
        int numInitialCollisions = Physics.OverlapSphereNonAlloc(transform.position, .1f, initialCollisions, collisionMask);
        if (numInitialCollisions > 0 )
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);        
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) 
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) 
        {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
