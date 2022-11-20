using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using System;

/*
Coords class for storing room coords, might move/change later to struct or other file.
*/
public class Coords
{
    public int x, y;
    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void SetCoords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

/*
Manages room generation and room info.
*/
public class RoomManager : MonoBehaviour
{
    //2D array for room map, 1: has room, 0: no room
    private int[,] roomGrid;
    private Dictionary<int, RoomData> roomInfo;

    //current room coords that player is in (NOT IMPLEMENTED YET)
    private Coords currCoords;

    private int roomNo = 14;
    private int maxGridX = 6;
    private int maxGridY = 6;

    //dictionary for all the room objects
    private Dictionary<(int, int), Room> rooms;

    public GameObject roomPrefab;

    public static RoomManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rooms = new Dictionary<(int, int), Room>();
        currCoords = new Coords(0, 0);
        roomGrid = new int[maxGridX, maxGridY];
        GenerateRoomLayout();

        roomInfo = new Dictionary<int, RoomData>();
        GenerateRoomData();

        GenerateRooms();

        GameManager.instance.CreatePlayer();
        // Debug.Log(currCoords.x + ", " + currCoords.y);
    }

    private void Update()
    {
        //temp for camera testing
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     if (rooms.ContainsKey((currCoords.x, currCoords.y + 1)))
        //         currCoords.y++;

        //     Debug.Log(currCoords.x + ", " + currCoords.y);
        // }
        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     if (rooms.ContainsKey((currCoords.x - 1, currCoords.y)))
        //         currCoords.x--;

        //     Debug.Log(currCoords.x + ", " + currCoords.y);
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     if (rooms.ContainsKey((currCoords.x, currCoords.y - 1)))
        //         currCoords.y--;

        //     Debug.Log(currCoords.x + ", " + currCoords.y);
        // }
        // else if (Input.GetKeyDown(KeyCode.D))
        // {
        //     if (rooms.ContainsKey((currCoords.x + 1, currCoords.y)))
        //         currCoords.x++;

        //     Debug.Log(currCoords.x + ", " + currCoords.y);
        // }
    }

    //using Prim's Algorithm to fill roomGrid 2D array
    //http://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm
    private void GenerateRoomLayout()
    {
        Dictionary<string, Coords> activeCoords = new Dictionary<string, Coords>();

        //starting cell
        int x = Random.Range(0, maxGridX);
        int y = Random.Range(0, maxGridY);
        currCoords.SetCoords(x, y);
        roomGrid[x, y] = 1;
        activeCoords.Add(x + "," + y, new Coords(x, y));

        //make "roomNo" rooms 
        for (int i = 0; i < roomNo; i++)
        {
            //find adjacent inactive cells
            Dictionary<string, Coords> adjCoords = new Dictionary<string, Coords>();
            foreach (Coords activeCoord in activeCoords.Values)
            {
                if (activeCoord.x + 1 < maxGridX)//right
                {
                    int adjX = activeCoord.x + 1;
                    int adjY = activeCoord.y;
                    if (roomGrid[adjX, adjY] == 0)
                    {
                        string id = adjX + "," + adjY;
                        if (!adjCoords.ContainsKey(id) && !activeCoords.ContainsKey(id))
                        {
                            adjCoords.Add(id, new Coords(adjX, adjY));
                        }
                    }
                }

                if (activeCoord.x - 1 >= 0)//left
                {
                    int adjX = activeCoord.x - 1;
                    int adjY = activeCoord.y;
                    if (roomGrid[adjX, adjY] == 0)
                    {
                        string id = adjX + "," + adjY;
                        if (!adjCoords.ContainsKey(id) && !activeCoords.ContainsKey(id))
                        {
                            adjCoords.Add(id, new Coords(adjX, adjY));
                        }
                    }
                }


                if (activeCoord.y + 1 < maxGridY) //up
                {
                    int adjX = activeCoord.x;
                    int adjY = activeCoord.y + 1;
                    if (roomGrid[adjX, adjY] == 0)
                    {
                        string id = adjX + "," + adjY;
                        if (!adjCoords.ContainsKey(id) && !activeCoords.ContainsKey(id))
                        {
                            adjCoords.Add(id, new Coords(adjX, adjY));
                        }
                    }
                }

                if (activeCoord.y - 1 >= 0) //down
                {
                    int adjX = activeCoord.x;
                    int adjY = activeCoord.y - 1;
                    if (roomGrid[adjX, adjY] == 0)
                    {
                        string id = adjX + "," + adjY;
                        if (!adjCoords.ContainsKey(id) && !activeCoords.ContainsKey(id))
                        {
                            adjCoords.Add(id, new Coords(adjX, adjY));
                        }
                    }
                }
            }

            //pick random adj cell and activate
            int index = Random.Range(0, adjCoords.Values.Count - 1);
            roomGrid[adjCoords.ElementAt(index).Value.x, adjCoords.ElementAt(index).Value.y] = 1;
            activeCoords.Add(adjCoords.ElementAt(index).Key, adjCoords.ElementAt(index).Value);
            adjCoords.Remove(adjCoords.ElementAt(index).Key);
        }

        // for (int i = 0; i < roomGrid.GetLength(0); i++)
        // {
        //     for (int j = 0; j < roomGrid.GetLength(1); j++)
        //     {
        //         Debug.Log(roomGrid[i, j]);
        //     }
        // }
    }

    // make room data
    private void GenerateRoomData()
    {
        for (int i = 0; i < roomNo + 1; i++)
        {
            RoomData data = new RoomData(i);
            // data.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //add enemies
            //add obstacles
            roomInfo.Add(i, data);
        }
    }

    public Room GetCurrentRoom()
    {
        return rooms[(currCoords.x, currCoords.y)];
    }

    public void SetCurrentRoom(Coords coords)
    {
        currCoords = coords;
    }

    // public Coords GetCurrentCoords()
    // {
    //     return currCoords;
    // }

    //instantiate rooms in scene
    private void GenerateRooms()
    {
        for (int i = 0; i < roomGrid.GetLength(0); i++)
        {
            for (int j = 0; j < roomGrid.GetLength(1); j++)
            {
                // Debug.Log(roomGrid[i, j]);
                if (roomGrid[i, j] == 1)
                {
                    var room = Instantiate(roomPrefab, new Vector3(i * 18, 0, j * 11), Quaternion.identity).GetComponent<Room>();
                    rooms.Add((i, j), room);
                    room.roomCoords = new Coords(i, j);

                    // Checks for adjacent rooms and generate walls accordingly.
                    bool north = false;
                    bool south = false;
                    bool east = false;
                    bool west = false;
                    if (j + 1 < roomGrid.GetLength(1))
                    {
                        north = roomGrid[i, j + 1] == 1;
                    }
                    if (j - 1 >= 0)
                    {
                        south = roomGrid[i, j - 1] == 1;
                    }
                    if (i + 1 < roomGrid.GetLength(0))
                    {
                        east = roomGrid[i + 1, j] == 1;
                    }
                    if (i - 1 >= 0)
                    {
                        west = roomGrid[i - 1, j] == 1;
                    }

                    room.GenerateWalls(north, south, east, west);
                }
            }
        }
    }
}
