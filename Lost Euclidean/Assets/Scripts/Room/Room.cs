using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class representing each individual room in the game.
 */
public class Room : MonoBehaviour
{
    /*
     * Random Notes:
     * Rooms are currently built to fit a 16:9 aspect ratio screen.
     * Rooms including walls are 18 tiles by 11 tiles, the room floor then is 16 tiles by 9 tiles.
     */

    [SerializeField] private GameObject[] obstaclePrefabs;


    [Header("references to children, ignore")]
    [SerializeField] private Grid grid;
    [SerializeField] private Transform obstacleHolder;

    private GameObject[,] _obstacles;

    private Vector2Int _farCorner = new Vector2Int(17, 10);
    private Vector2Int _roomCenter = new Vector2Int(9, 5);
    // near corner is the origin at (0, 0)

    public Coords roomCoords;
    public RoomManager.RoomState state = RoomManager.RoomState.normal;

    //track room exits
    public (bool north, bool south, bool east, bool west) nsewExits;
    public (Room north, Room south, Room east, Room west) nsewRooms;

    public GameObject northLight, southLight, westLight, eastLight;
    private static readonly int Target = Shader.PropertyToID("_Target");
    private static readonly int colorProperty = Shader.PropertyToID("_Color");

    // Start is called before the first frame update
    void Start()
    {
        _obstacles = new GameObject[_farCorner.x + 1, _farCorner.y + 1];
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if()
    // }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger");
        if (other.tag == "Player")
        {
            // Debug.Log(roomCoords.x + ", " + roomCoords.y);
            RoomManager.instance.SetCurrentRoom(roomCoords);

            if (GameManager.instance.isTeleporting)
            {
                RoomManager.instance.GetPreviousRoom().GenerateExitLinks();
            }
        }
    }

    /// <summary>
    /// Generates walls surrounding the room.
    /// </summary>
    /// <param name="north">if a door should be placed north.</param>
    /// <param name="south">if a door should be placed south.</param>
    /// <param name="east">if a door should be placed east.</param>
    /// <param name="west">if a door should be placed west.</param>
    public void GenerateWalls(bool north = false, bool south = false, bool east = false, bool west = false)
    {
        // East wall, facing left is the default rotation on the walls.
        /*
         * 90 is South
         * 180 is West
         * 270 is North
         */

        // bottom row
        for (int i = 0; i <= _farCorner.x; i++)
        {
            if (!south || (i != 8 && i != 9))
            {
                var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
                pillar.transform.rotation = Quaternion.Euler(0, 90, 0);
                pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, 0, 0));
            }
            else if (i == 8)
            {
                var gate = Instantiate(obstaclePrefabs[3], obstacleHolder);
                gate.transform.rotation = Quaternion.Euler(0, 90, 0);
                gate.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, 0, 0))
                                               + new Vector3(grid.cellSize.x * 0.5f, 0, 0);
            }
        }

        // left column
        for (int i = 1; i < _farCorner.y; i++)
        {
            if (!west || (i != 4 && i != 5))
            {
                var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
                pillar.transform.rotation = Quaternion.Euler(0, 180, 0);
                pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(0, i, 0));
            }
            else if (i == 4)
            {
                var gate = Instantiate(obstaclePrefabs[3], obstacleHolder);
                gate.transform.rotation = Quaternion.Euler(0, 180, 0);
                gate.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(0, i, 0))
                                               + new Vector3(0, 0, grid.cellSize.x * 0.5f);
            }
        }

        // top row
        for (int i = 0; i <= _farCorner.x; i++)
        {
            if (!north || (i != 8 && i != 9))
            {
                var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
                pillar.transform.rotation = Quaternion.Euler(0, 270, 0);
                pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, _farCorner.y, 0));
            }
            else if (i == 8)
            {
                var gate = Instantiate(obstaclePrefabs[3], obstacleHolder);
                gate.transform.rotation = Quaternion.Euler(0, 270, 0);
                gate.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, _farCorner.y, 0))
                                               + new Vector3(grid.cellSize.x * 0.5f, 0, 0);
            }
        }

        // right row
        for (int i = 1; i <= _farCorner.y; i++)
        {
            if (!east || (i != 4 && i != 5))
            {
                var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
                pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(_farCorner.x, i, 0));
            }
            else if (i == 4)
            {
                var gate = Instantiate(obstaclePrefabs[3], obstacleHolder);
                gate.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(_farCorner.x, i, 0))
                                               + new Vector3(0, 0, grid.cellSize.x * 0.5f);
            }
        }

        nsewExits = (north, south, east, west);
    }

    //make obstacles in rooms
    public void GenerateObstacles(RoomManager.RoomState state)
    {
        this.state = state;
        switch (state)
        {
            case RoomManager.RoomState.blue:
                var blue = Instantiate(obstaclePrefabs[1], gameObject.transform);
                blue.GetComponent<Pillar>().state = RoomManager.RoomState.blue;
                blue.transform.GetChild(1).GetComponent<Light>().color = GameManager.instance.colorsPillar[0];
                blue.transform.GetChild(2).GetComponent<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[0]);
                blue.transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[0]);
                break;
            case RoomManager.RoomState.purple:
                var purple = Instantiate(obstaclePrefabs[1], gameObject.transform);
                purple.GetComponent<Pillar>().state = RoomManager.RoomState.purple;
                purple.transform.GetChild(1).GetComponent<Light>().color = GameManager.instance.colorsPillar[2];
                purple.transform.GetChild(2).GetComponent<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[2]);
                purple.transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[2]);
                break;
            case RoomManager.RoomState.yellow:
                var yellow = Instantiate(obstaclePrefabs[1], gameObject.transform);
                yellow.GetComponent<Pillar>().state = RoomManager.RoomState.yellow;
                yellow.transform.GetChild(1).GetComponent<Light>().color = GameManager.instance.colorsPillar[1];
                yellow.transform.GetChild(2).GetComponent<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[1]);
                yellow.transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[1]);
                break;
            case RoomManager.RoomState.green:
                var green = Instantiate(obstaclePrefabs[1], gameObject.transform);
                green.GetComponent<Pillar>().state = RoomManager.RoomState.green;
                green.transform.GetChild(1).GetComponent<Light>().color = GameManager.instance.colorsPillar[3];
                green.transform.GetChild(2).GetComponent<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[3]);
                green.transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[3]);
                break;
            case RoomManager.RoomState.exit:
                break;
            case RoomManager.RoomState.start:
                break;
            default:
                Vector3 roomPos1 = gameObject.transform.position;
                var ez = Random.Range(-3, 4);
                var ex = Random.Range(-6, 7);
                var enemy = Instantiate(obstaclePrefabs[4], roomPos1 + grid.GetCellCenterLocal(new Vector3Int(ex, ez, 0)), Quaternion.identity);
                enemy.GetComponent<Enemy>().hx = ex;
                enemy.GetComponent<Enemy>().hz = ez;
                enemy.GetComponent<Enemy>().location = roomCoords;
                // exit.transform.GetChild(1).GetComponent<Light>().color = Color.white;
                break;

        }
    }

    private void LightHelper(RoomManager.RoomState state, GameObject light)
    {
        bool interestingRoom = false;
        if (state == RoomManager.RoomState.blue && !GameManager.instance.bluePillar)
        {
            // Debug.Log(state);
            light.GetComponent<Light>().color = GameManager.instance.colorsPillar[0];
            light.GetComponentInChildren<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[0]);
            interestingRoom = true;
        }
        if (state == RoomManager.RoomState.yellow && !GameManager.instance.yellowPillar)
        {
            // Debug.Log(state);
            light.GetComponent<Light>().color = GameManager.instance.colorsPillar[1];
            light.GetComponentInChildren<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[1]);
            interestingRoom = true;
        }
        if (state == RoomManager.RoomState.purple && !GameManager.instance.purplePillar)
        {
            // Debug.Log(state);
            light.GetComponent<Light>().color = GameManager.instance.colorsPillar[2];
            light.GetComponentInChildren<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[2]);
            interestingRoom = true;
        }
        if (state == RoomManager.RoomState.green && !GameManager.instance.greenPillar)
        {
            // Debug.Log(state);
            light.GetComponent<Light>().color = GameManager.instance.colorsPillar[3];
            light.GetComponentInChildren<SpriteRenderer>().material.SetColor(colorProperty, GameManager.instance.colorsPillar[3]);
            interestingRoom = true;
        }

        if (interestingRoom)
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }

    //decide room exit links + add light indicators
    public void GenerateExitLinks()
    {
        if (nsewExits.south)
        {
            nsewRooms.north = RoomManager.instance.GetRandomNorthRoom();
            LightHelper(nsewRooms.north.state, southLight);

        }
        if (nsewExits.north)
        {
            nsewRooms.south = RoomManager.instance.GetRandomSouthRoom();
            LightHelper(nsewRooms.south.state, northLight);
        }
        if (nsewExits.west)
        {
            nsewRooms.east = RoomManager.instance.GetRandomEastRoom();
            LightHelper(nsewRooms.east.state, westLight);
        }
        if (nsewExits.east)
        {
            nsewRooms.west = RoomManager.instance.GetRandomWestRoom();
            LightHelper(nsewRooms.west.state, eastLight);
        }
    }

    public void TurnOffAllLights()
    {
        southLight.SetActive(false);
        northLight.SetActive(false);
        eastLight.SetActive(false);
        westLight.SetActive(false);
    }

}
