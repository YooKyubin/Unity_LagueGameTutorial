using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun StartingGun;
    Gun equippedGun;

    void Start()
    {
        if (StartingGun != null)
        {
            EquipGun(StartingGun);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation).GetComponent<Gun>();
        equippedGun.transform.parent = weaponHold;
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }

    }
    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
            equippedGun.Aim(aimPoint);
    }

    public void Reload()
    {
        if (equippedGun != null)
            equippedGun.Reload();
    }

}