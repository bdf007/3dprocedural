using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    // reference to the ground
    [Header("Ground Objects")]
    public Transform ground;
    public Transform groundNorth;
    public Transform groundSouth;
    public Transform groundEast;
    public Transform groundWest;

    //reference to the room's door
    [Header("Door Objects")]
    public Transform northDoor;
    public Transform southDoor;
    public Transform eastDoor;
    public Transform westDoor;
    [Header("Wall Objects")]
    public Transform northWall;
    public Transform southWall;        
    public Transform eastWall;
    public Transform westWall;

    [Header("Values")]
    public int insideWidth;
    public int insideLength;

    // objects to instantiate
    [Header("Objects to Instantiate")]
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public GameObject healthPrefab;
    public GameObject keyPrefab;
    public GameObject exitPrefab;

    

    // list of positions to avoid instantiating new objects at
    private List<Vector3> usedPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // get a reference to the room's door
        northDoor = transform.Find("NorthDoor");
        southDoor = transform.Find("SouthDoor");
        eastDoor = transform.Find("EastDoor");
        westDoor = transform.Find("WestDoor");

        // get a reference to the room's walls
        northWall = transform.Find("NorthWall");
        southWall = transform.Find("SouthWall");
        eastWall = transform.Find("EastWall");
        westWall = transform.Find("WestWall");

        // get a reference to the room's ground
        ground = transform.Find("Ground");
        groundNorth = northDoor.transform.Find("Ground");
        groundSouth = southDoor.transform.Find("Ground");
        groundEast = eastDoor.transform.Find("Ground");
        groundWest = westDoor.transform.Find("Ground");


        //get the prefab objects
        enemyPrefab = Resources.Load("Prefabs/Enemy") as GameObject;
        coinPrefab = Resources.Load("Prefabs/Coin") as GameObject;
        healthPrefab = Resources.Load("Prefabs/Health") as GameObject;
        keyPrefab = Resources.Load("Prefabs/Key") as GameObject;
        exitPrefab = Resources.Load("Prefabs/Exit") as GameObject;
    }

  

    public void GenerateInterior()
    {
        // create coins, enemies, health, etc...
        // do we spanw enemies
        if (Random.value < Generation.instance.enemySpawnChance)
        {
            SpawnPrefab(enemyPrefab, 1, Generation.instance.maxEnemiesPerRoom + 1);
            //UpdateNavMesh();
        }

        // do we spawn coins
        if (Random.value < Generation.instance.coinSpawnChance)
        {
            SpawnPrefab(coinPrefab, 1, Generation.instance.maxCoinsPerRoom + 1);
        }

        // do we spawn health
        if (Random.value < Generation.instance.healthSpawnChance)
        {
            SpawnPrefab(healthPrefab, 1, Generation.instance.maxHealthPerRoom + 1);
        }
    }

    public void SpawnPrefab(GameObject prefab, int min = 0, int max = 0)
    {
        int num = 1;
        if (min != 0 || max != 0)
        {
            num = Random.Range(min, max);
        }

        // for each prefabs,
        for (int i = 0; i < num; i++)
        {
            // instantiate the prefab
            GameObject obj = Instantiate(prefab);
            // getting the nearest position to a random position isnide the room
            Vector3 pos = transform.position + new Vector3(Random.Range(-insideLength / 2, insideWidth / 2 + 1), 0, Random.Range(-insideLength / 2, insideLength / 2 + 1));

            // if the position is already in use, pick another random position
            while (usedPositions.Contains(pos))
            {
                pos = transform.position + new Vector3(Random.Range(-insideLength / 2, insideWidth / 2 + 1), 0, Random.Range(-insideLength / 2, insideLength / 2 + 1));
            }

            // place the prefab at the random position
            obj.transform.position = pos;
            // add the current position to the list of used positions
            usedPositions.Add(pos);

            // if the prefab is an enemy
            if (prefab == enemyPrefab)
            {
                // add it to the enemyManager' enemies List
                EnemyManager.instance.enemies.Add(obj.GetComponent<Enemy>());
                // update the count of enemies
                EnemyManager.instance.enemyCount++;


            }

            //obj.transform.SetParent(Generation.instance.navMeshObject, true);
        }
    }
}
