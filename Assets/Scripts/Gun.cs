using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    public void Fire()
    {
        Instantiate(bullet , gun.position, transform.rotation);
    }
}
