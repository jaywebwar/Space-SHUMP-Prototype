﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,
    blaster,
    spread,
    shield
}

[System.Serializable]//Unity tries to make visible in the inspector
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;//letter to show on powerUp
    public Color color = Color.white; //color of collar & powerup
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20; //speed of projectiles
}

public class Weapon : MonoBehaviour {

    static public Transform PROJECTILE_ANCHOR;

    public bool __________________________________________;

    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot;//Time last shot was fired

    private void Awake()
    {
        collar = transform.Find("Collar").gameObject;
    }

    // Use this for initialization
    void Start () {
        SetType(_type);

        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        //Find the fireDelegate of the parent
        GameObject parentGO = transform.parent.gameObject;
        if(parentGO.tag == "Hero")
        {
            Hero.S.fireDelegate += Fire;
        }
	}

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType( WeaponType wt)
    {
        _type = wt;
        if(type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collar.GetComponent<Renderer>().material.color = def.color;
        lastShot = 0;//can fire immediately after type is set
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;//go is inactive
        if (Time.time - lastShot < def.delayBetweenShots) return;//need to wait longer before shooting again
        Projectile p;
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(-.2f, .9f, 0) * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(.2f, .9f, 0) * def.velocity;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate(def.projectilePrefab) as GameObject;
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.parent = PROJECTILE_ANCHOR;
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return p;
    }
}
