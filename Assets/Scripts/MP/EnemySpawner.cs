using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemySpawner : MonoBehaviourPun
{
    enum Type
    {
        Standard,
        Major,
        Roomba
    };
    [SerializeField]
    Type type = new Type();
    private string enemyPrefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    public bool isEnabled = true;
    private List<GameObject> curEnemies = new List<GameObject>();

    [Header("Options")]
    public bool limitedSpawn;
    public bool debug;

    private void Start()
    {
        switch (type)
        {
            case Type.Major:
                enemyPrefabPath = "Major_Enemy";
                break;
            case Type.Roomba:
                enemyPrefabPath = "WheelEnemy";
                break;
            case Type.Standard:
                enemyPrefabPath = "Enemy_Standard";
                break;
            default:
                enemyPrefabPath = "Enemy_Standard";
                break;
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient||!isEnabled)
            return;
        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                switch (enemyPrefabPath)
                {
                    case "Major_Enemy":
                        enemyPrefabPath = "WheelEnemy";
                        Debug.Log("Changed to ROOMBA");
                        break;
                    case "WheelEnemy":
                        enemyPrefabPath = "Enemy_Standard";
                        Debug.Log("Changed to Standard");
                        break;
                    case "Enemy_Standard":
                        enemyPrefabPath = "Major_Enemy";
                        Debug.Log("Changed to Major");
                        break;
                    default:
                        enemyPrefabPath = "Enemy_Standard";
                        Debug.Log("Your debug failed. Changed to Standard.");
                        break;
                }
            }
        }
    }

    void TrySpawn()
    {
        // remove any dead enemies from the curEnemies list
        for (int x = 0; x < curEnemies.Count; ++x)
        {
            if (!curEnemies[x])
                curEnemies.RemoveAt(x);
        }
        // if we have maxed out our enemies, return

        if (curEnemies.Count >= maxEnemies)
        {
            return;
        }
        // otherwise, spawn an enemy
        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);
        curEnemies.Add(enemy);
        // Then check if we checked off the limited spawn and reached the spawn limit, then destroy ourselves if true.
        if (curEnemies.Count >= maxEnemies && limitedSpawn)
            Destroy(this.gameObject);
    }
}
