using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    static public Main S;
    static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemySpawnPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]{WeaponType.blaster,
                                                            WeaponType.blaster,
                                                            WeaponType.spread,
                                                            WeaponType.shield};

    public bool _______________________________;

    public WeaponType[] activeWeaponTypes;
    public float enemySpawnRate;

    private void Awake()
    {
        S = this;
        //set utils camBounds
        Utils.SetCameraBounds(this.GetComponent<Camera>());
        enemySpawnRate = 1f / enemySpawnPerSecond;
        Invoke("SpawnEnemy", enemySpawnRate);

        W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
        foreach( WeaponDefinition def in weaponDefinitions)
        {
            W_DEFS[def.type] = def;
        }
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //need to check if key exists
        if (W_DEFS.ContainsKey(wt))
        {
            return W_DEFS[wt];
        }
        //this returns a def for WeaponType.none
        return new WeaponDefinition();
    }

    private void Start()
    {
        activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
        for(int i=0; i<weaponDefinitions.Length; i++)
        {
            activeWeaponTypes[i] = weaponDefinitions[i].type;
        }
    }

    public void SpawnEnemy()
    {
        //Pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;
        //Position the enemy above the screen with random x pos
        Vector3 pos = Vector3.zero;
        float xMin = Utils.camBounds.min.x + enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - enemySpawnPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = Utils.camBounds.max.y + enemySpawnPadding;
        go.transform.position = pos;
        //call SpawnEnemy again in a couple seconds
        Invoke("SpawnEnemy", enemySpawnRate);
    }

    public void DelayedRestart(float delay)
    {
        //Invoke restart method
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        //reload scene 0
        SceneManager.LoadScene("_Scene_0");
    }

    public void ShipDestroyed(Enemy e)
    {
        if(Random.value <= e.powerUpDropChance)
        {
            //choose a powerUp to drop
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            //spawn powerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            //set it to the pos of destroyed ship
            pu.transform.position = e.transform.position;
        }
    }
}
