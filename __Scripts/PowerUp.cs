using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    public bool ____________________________________;

    public WeaponType type;
    public GameObject cube;//ref to cube child
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();

        //Set a random velocity
        Vector3 vel = Random.onUnitSphere;//random orientation with magnitude=1
        vel.z = 0;//flatten to xy plane
        vel.Normalize();//to keep mag=1
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        GetComponent<Rigidbody>().velocity = vel;

        //Rotation
        transform.rotation = Quaternion.identity;//R[0, 0, 0]
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        InvokeRepeating("CheckOffscreen", 2f, 2f);

        birthTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        //rotate cube
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //fade out powerUp over time
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if(u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        if(u > 0)
        {
            Color c = cube.GetComponent<Renderer>().material.color;
            c.a = 1f - u;
            cube.GetComponent<Renderer>().material.color = c;

            c = letter.color;
            c.a = 1f - (u * .5f);//fade not as much
            letter.color = c;
        }
	}

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cube.GetComponent<Renderer>().material.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }

    void CheckOffscreen()
    {
        if(Utils.ScreenBoundsCheck(cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero)
        {
            Destroy(this.gameObject);
        }
    }
}
