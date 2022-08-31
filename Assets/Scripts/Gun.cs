using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform gun;

    public void Fire()
    {
        Instantiate(bullet , gun.position, transform.rotation);
    }
}
