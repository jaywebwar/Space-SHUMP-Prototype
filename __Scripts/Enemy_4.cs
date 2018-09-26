using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    public GameObject go;
    public Material mat;
}

public class Enemy_4 : Enemy {
    //Moves randomly about the map until shot down
    public Vector3[] points;
    public float timeStart;
    public float duration = 4;//duration of movement

    public Part[] parts;

	// Use this for initialization
	void Start () {
        points = new Vector3[2];
        points[0] = pos;
        points[1] = pos;

        InitMovement();

        //Cache Go & Mat for each part
        Transform t;
        foreach(Part p in parts)
        {
            t = transform.Find(p.name);
            if(t != null)
            {
                p.go = t.gameObject;
                p.mat = p.go.GetComponent<Renderer>().material;
            }
        }
	}

    void InitMovement()
    {
        //Pick new point to move to on screen
        Vector3 p1 = Vector3.zero;
        float esp = Main.S.enemySpawnPadding;
        Bounds cBounds = Utils.camBounds;
        p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
        p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

        //shift points
        points[0] = points[1];
        points[1] = p1;

        //reset time
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if(u > 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);

        pos = (1 - u) * points[0] + u * points[1];
    }

    //since it is a unity function, override isn't necessary
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //prevent damage off screen
                bounds.center = transform.position + boundsCenterOffset;
                if(bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                {
                    Destroy(other);
                }

                //Hurt this enemy
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                if(prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            //then don't damage the part
                            Destroy(other);//other is projectile
                            return;
                        }
                    }
                }
                //It's not protected, so damage
                prtHit.health -= Main.W_DEFS[p.type].damageOnHit;
                //Show damage on the part
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);//remove part
                }
                //check to see if whole ship is destroyed
                bool allDestroyed = true; //assume it is
                foreach(Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);//destroy projectile
                break;
        }
    }

    //Finds parts within using name or go
    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if(prt.name == n)
            {
                return prt;
            }
        }
        return null;
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return prt;
            }
        }
        return null;
    }

    bool Destroyed(GameObject go)
    {
        return Destroyed(FindPart(go));
    }

    bool Destroyed(string n)
    {
        return Destroyed(FindPart(n));
    }

    bool Destroyed(Part prt)
    {
        if(prt == null)
        {
            return true;
        }
        return (prt.health <= 0);
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        remainingDamageFrames = showDamageForFrames;
    }
}
