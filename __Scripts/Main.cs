using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    static public Main S;

    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemySpawnPadding = 1.5f;

    public bool _______________________________;

    public float enemySpawnRate;

    private void Awake()
    {
        S = this;
        //set utils camBounds
        Utils.SetCameraBounds(this.GetComponent<Camera>());
        enemySpawnRate = 1f / enemySpawnPerSecond;
        Invoke("SpawnEnemy", enemySpawnRate);
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
}
