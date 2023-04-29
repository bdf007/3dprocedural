using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Generation : MonoBehaviour
{
    private int mapWidth = 7;
    private int mapLength = 7;
    private int roomsToGenerate = 12;

    private int roomCount;
    private bool roomsInstantiated;
    private int multiplier = 35;

    // spawn chance
    public float enemySpawnChance = 0.7f;
    public float coinSpawnChance = 0.8f;
    public float healthSpawnChance = 0.3f;

    public int maxEnemiesPerRoom = 1;
    public int maxCoinsPerRoom = 3;
    public int maxHealthPerRoom = 1;


    // store our first'room position for procedural level generation
    private Vector3 firstRoomPosition;

    // a 2D boolean array to map out the level
    private bool[,] map;
    // the room prefab to instantiate
    public GameObject roomPrefab; 

    private List<Room> roomObjects = new List<Room>();

    public Transform navMeshObject;
    private NavMeshSurface[] surfaces;

    // creating a singleton
    public static Generation instance;

    private void Awake()
    {
        //// if there is already an (original) instance of this script existing, which is not 'this' instance
        //if (instance != null && instance != this)
        //{
        //    // destroy this gameObject(clone).
        //    Destroy(gameObject);
        //    return;
        //}
        // if this is not a clone, don't destroy on load.
        instance = this;
        //DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        surfaces = navMeshObject.GetComponentsInChildren<NavMeshSurface>();
        surfaces = FindObjectsOfType<NavMeshSurface>();

        //randomize the number of rooms to generate
        roomsToGenerate = Random.Range(8, 16);
        // random seed aasigned to a random number generator
        Random.InitState(Random.Range(0, 999999));
        //Random.InitState(14);
        Generate();

    }

    public void Generate()
    {
        // create a new map of the specified size
        map = new bool[mapWidth, mapLength];
        CheckRoom(mapWidth/2, mapLength/2, 0, Vector3.zero, true);
        InstantiateRooms();
        // Find the player in the scene, and position them inside the first room 
        FindObjectOfType<Player>().transform.position = firstRoomPosition * multiplier;
        //Debug.Log("coordinate of the player is " + firstRoomPosition);
        //FindAnyObjectByType<Player>().transform.SetParent(navMeshObject, true);

        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

    void CheckRoom(int x, int z, int remaining, Vector3 generalDirection, bool firstRoom = false)
    {
        // if we have generated the specified number of rooms, stop
        if (roomCount >= roomsToGenerate)
        {
            return;
        }
        //if this outside the bonds of the actual map, stop
        if (x < 0 || x > mapWidth -1 || z < 0 || z > mapLength -1)
        {
            return;
        }

        // if this is not the first romm and there is no more room to check, stop
        if (!firstRoom && remaining <= 0)
        {
            return;
        }

        // if the given map tile is already occupied, stop the function.
        if (map[x, z])
        {
            return;
        }

        // if this is the first room, store the room position.
        if (firstRoom)
        {
            firstRoomPosition = new Vector3(x, 0, z);
        }

        // add one to roomCount and set the map tile to be true.
        roomCount++;
        map[x, z] = true;

        bool north = Random.value > (generalDirection == Vector3.forward ? 0.2f : 0.8f);
        bool south = Random.value > (generalDirection == Vector3.back ? 0.2f : 0.8f);
        bool east = Random.value > (generalDirection == Vector3.right ? 0.2f : 0.8f);
        bool west = Random.value > (generalDirection == Vector3.left ? 0.2f : 0.8f);

        int maxRemaining = roomsToGenerate / 4;

        // if north is true, make a room one tile above the current.
        if (north || firstRoom)
        {
            CheckRoom(x, z + 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector3.forward : generalDirection);
        }

        // if south is true, make a room one tile below the current.
        if (south || firstRoom)
        {
            CheckRoom(x, z - 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector3.back : generalDirection);
        }

        // if east is true, make a room one tile to the right of the current.
        if (east || firstRoom)
        {
            CheckRoom(x + 1, z, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector3.right : generalDirection);
        }

        // if west is true, make a room one tile to the left of the current.
        if (west || firstRoom)
        {
            CheckRoom(x - 1, z, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector3.left : generalDirection);
        }
    }

    void InstantiateRooms()
    {
        if(roomsInstantiated)
        {
            return;
        }
        roomsInstantiated = true;

        // loop through each element inside of our map array
        for(int x = 0; x < mapWidth; x++)
        {
            for(int z = 0; z<mapLength; z++)
            {
                if (map[x,z] == false)
                {
                    continue;
                }
                // if the map tile is true, instantiate a room at the given position.
                GameObject roomObject = Instantiate(roomPrefab, new Vector3(x, 0, z) * multiplier, Quaternion.identity);
                // get a reference to the Room script of the new room object
                Room room = roomObject.GetComponent<Room>();

                // if we're within the boundary of the map, AND if there is room above us
                if(z<mapLength-1 && map[x, z+1] == true)
                {
                    //enabe the north door and disable the north wall
                    room.northDoor.gameObject.SetActive(true);
                    room.northWall.gameObject.SetActive(false);
                }
                // if we're within the boundary of the map, AND if there is room below us
                if(z>0  && map[x, z-1] == true)
                {
                    // enable the south door and disable the south wall
                    room.southDoor.gameObject.SetActive(true);
                    room.southWall.gameObject.SetActive(false);
                }
                // if we're within the boundary of the map, AND if there is a room to the right of us
                if (x < mapWidth - 1 && map[x + 1, z] == true)
                {
                    // enable the east door and disable the east wall
                    room.eastDoor.gameObject.SetActive(true);
                    room.eastWall.gameObject.SetActive(false);
                }

                // if we're within the boundary of the map, AND if there is a room to the left of us
                if (x  > 0 && map[x - 1, z] == true)
                {
                    // enable the west door and disable the east wall
                    room.westDoor.gameObject.SetActive(true);
                    room.westWall.gameObject.SetActive(false);
                }

                // if this not the firstroom call generate
                if(firstRoomPosition != new Vector3(x, 0, z))
                {
                    room.GenerateInterior();
                }
                // add the room to the roomObjects list 
                roomObjects.Add(room);
                room.transform.SetParent(navMeshObject, false);
            }
        }

        // after Looping through every element inside the map array, call calculateKeyAndExit
        CalculateKeyAndExit();
    }

    void CalculateKeyAndExit()
    {
        float maxDist = 0;
        Room a = null;
        Room b = null;
        foreach (Room aRoom in roomObjects)
        {
            foreach (Room bRoom in roomObjects)
            {
                // compare each of the rooms to find out which pair is the furtherest away.
                float dist = Vector3.Distance(aRoom.transform.position, bRoom.transform.position);
                if (dist > maxDist)
                {
                    a = aRoom;
                    b = bRoom;
                    maxDist = dist;
                }
            }
        }
        // once room A and room B are found, spawn in the key and the exitdoor.
        a.SpawnPrefab(a.keyPrefab);
        b.SpawnPrefab(b.exitPrefab);
    }
}
