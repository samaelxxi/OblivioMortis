using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColliderObserver : MonoBehaviour
{
    public event System.Action<Collider> OnTriggerEnterEvent;
    public event System.Action<Bullet> OnBulletTriggerEnterEvent;

    void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    public void UseBullet(Bullet bullet)
    {  // TODO ehhhhhhhhhhhhhhhh
        OnBulletTriggerEnterEvent?.Invoke(bullet);
    }
}
