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


    // Start is called before the first frame update
    void Start()
    {
        _obstacles = new GameObject[_farCorner.x + 1, _farCorner.y + 1];
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Makes Walls
    public void GenerateWalls()
    {
        // bottom row
        for (int i = 0; i <= _farCorner.x; i++)
        {
            var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
            pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, 0, 0));
        }

        // left column
        for (int i = 1; i < _farCorner.y; i++)
        {
            var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
            pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(0, i, 0));
        }

        // top row
        for (int i = 0; i <= _farCorner.x; i++)
        {
            var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
            pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, _farCorner.y, 0));
        }

        // right row
        for (int i = 1; i <= _farCorner.y; i++)
        {
            var pillar = Instantiate(obstaclePrefabs[0], obstacleHolder);
            pillar.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(_farCorner.x, i, 0));
        }
    }
}
