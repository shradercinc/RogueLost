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

    // Start is called before the first frame update
    void Start()
    {
        _obstacles = new GameObject[_farCorner.x + 1, _farCorner.y + 1];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger");
        if (other.tag == "Player")
        {
            // Debug.Log(roomCoords.x + ", " + roomCoords.y);
            RoomManager.instance.SetCurrentRoom(roomCoords);
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
        }

        // right row
        for (int i = 1; i <= _farCorner.y; i++)
        {
            if (!east || (i != 4 && i != 5))
            {
                var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
                pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(_farCorner.x, i, 0));
            }
        }
    }
}
