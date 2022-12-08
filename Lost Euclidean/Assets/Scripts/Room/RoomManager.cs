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
    // private Dictionary<int, RoomData> roomInfo;

    //current room coords that player is in
    private Coords currCoords;
    private Coords prevCoords;
    private Coords startingCoords;

    private int roomNo = 14;
    private int maxGridX = 6;
    private int maxGridY = 6;

    //dictionary for all the room objects
    private Dictionary<(int, int), Room> rooms;

    //for minimap
    public Dictionary<(int, int), Room> foundRooms;

    //list for which rooms have which exits
    private List<Room> northDoorRooms;
    private List<Room> southDoorRooms;
    private List<Room> eastDoorRooms;
    private List<Room> westDoorRooms;

    [Header("0:start, 1:blank, 2:exit (do not change this order)")]
    public GameObject[] roomPrefabs;

    public static RoomManager instance;

    public enum RoomState { none, normal, blue, yellow, green, purple, exit, start }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        northDoorRooms = new List<Room>();
        southDoorRooms = new List<Room>();
        eastDoorRooms = new List<Room>();
        westDoorRooms = new List<Room>();

        rooms = new Dictionary<(int, int), Room>();
        foundRooms = new Dictionary<(int, int), Room>();
        currCoords = new Coords(0, 0);
        prevCoords = new Coords(0, 0);
        roomGrid = new int[maxGridX, maxGridY];
        GenerateRoomLayout();

        // roomInfo = new Dictionary<int, RoomData>();
        // GenerateRoomData();

        GenerateRooms();

        GameManager.instance.CreatePlayer();
        // Debug.Log(currCoords.x + ", " + currCoords.y);
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
        startingCoords = new Coords(x, y);
        roomGrid[x, y] = (int)RoomState.start; //7
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

        //pick random active rooms and put pillars + exit
        var tempCoords = activeCoords.ToDictionary(entry => entry.Key, entry => entry.Value);
        tempCoords.Remove(x + "," + y); //remove start room

        //blue pillar
        var blueCoords = tempCoords.ElementAt(Random.Range(0, tempCoords.Count)).Value;
        roomGrid[blueCoords.x, blueCoords.y] = (int)RoomState.blue;
        tempCoords.Remove(blueCoords.x + "," + blueCoords.y);
        //yellow pillar
        var yellowCords = tempCoords.ElementAt(Random.Range(0, tempCoords.Count)).Value;
        roomGrid[yellowCords.x, yellowCords.y] = (int)RoomState.yellow;
        tempCoords.Remove(yellowCords.x + "," + yellowCords.y);
        //green pillar
        var greenCoords = tempCoords.ElementAt(Random.Range(0, tempCoords.Count)).Value;
        roomGrid[greenCoords.x, greenCoords.y] = (int)RoomState.green;
        tempCoords.Remove(greenCoords.x + "," + greenCoords.y);
        //purple pillar
        var purpleCoords = tempCoords.ElementAt(Random.Range(0, tempCoords.Count)).Value;
        roomGrid[purpleCoords.x, purpleCoords.y] = (int)RoomState.purple;
        tempCoords.Remove(purpleCoords.x + "," + purpleCoords.y);
        //exit
        var exitCoords = tempCoords.ElementAt(Random.Range(0, tempCoords.Count)).Value;
        roomGrid[exitCoords.x, exitCoords.y] = (int)RoomState.exit;
        tempCoords.Remove(exitCoords.x + "," + exitCoords.y);
    }

    public Room GetStartingRoom()
    {
        return rooms[(startingCoords.x, startingCoords.y)];
    }

    public Room GetCurrentRoom()
    {
        return rooms[(currCoords.x, currCoords.y)];
    }

    public void SetCurrentRoom(Coords coords)
    {
        prevCoords = currCoords;
        currCoords = coords;

        //for minimap
        if (!foundRooms.ContainsKey((currCoords.x, currCoords.y)))
            foundRooms.Add((currCoords.x, currCoords.y), GetCurrentRoom());
        UIManager.instance.UpdateMinimap();
    }

    public Room GetPreviousRoom()
    {
        return rooms[(prevCoords.x, prevCoords.y)];
    }

    //instantiate rooms in scene
    private void GenerateRooms()
    {
        for (int i = 0; i < roomGrid.GetLength(0); i++)
        {
            for (int j = 0; j < roomGrid.GetLength(1); j++)
            {
                // Debug.Log(roomGrid[i, j]);
                if (roomGrid[i, j] != 0)
                {
                    Room room;

                    if (roomGrid[i, j] == (int)RoomState.start) //make starting room
                    {
                        room = Instantiate(roomPrefabs[0], new Vector3(i * 18, 0, j * 11), Quaternion.identity).GetComponent<Room>();
                        foundRooms.Add((i, j), room); //set starting room on minimap
                        // UIManager.instance.UpdateMinimap();
                    }
                    else if (roomGrid[i, j] >= (int)RoomState.blue && roomGrid[i, j] <= (int)RoomState.purple)
                    {
                        room = Instantiate(roomPrefabs[1], new Vector3(i * 18, 0, j * 11), Quaternion.identity).GetComponent<Room>();
                    }
                    else if (roomGrid[i, j] == (int)RoomState.exit)
                    {
                        room = Instantiate(roomPrefabs[2], new Vector3(i * 18, 0, j * 11), Quaternion.identity).GetComponent<Room>();
                    }
                    else
                    {
                        room = Instantiate(roomPrefabs[Random.Range(3, roomPrefabs.Length)], new Vector3(i * 18, 0, j * 11), Quaternion.identity).GetComponent<Room>();
                    }

                    room.gameObject.tag = "Room"; //TODO: idk why this isn't setting even tho it's in the prefab
                    rooms.Add((i, j), room);
                    room.roomCoords = new Coords(i, j);

                    // Checks for adjacent rooms and generate walls accordingly.
                    bool north = false;
                    bool south = false;
                    bool east = false;
                    bool west = false;
                    if (j + 1 < roomGrid.GetLength(1))
                    {
                        north = roomGrid[i, j + 1] != 0;
                    }
                    if (j - 1 >= 0)
                    {
                        south = roomGrid[i, j - 1] != 0;
                    }
                    if (i + 1 < roomGrid.GetLength(0))
                    {
                        east = roomGrid[i + 1, j] != 0;
                    }
                    if (i - 1 >= 0)
                    {
                        west = roomGrid[i - 1, j] != 0;
                    }

                    //adds to door tracking lists
                    if (north)
                    {
                        if (!northDoorRooms.Contains(room))
                            northDoorRooms.Add(room);
                    }
                    if (south)
                    {
                        if (!southDoorRooms.Contains(room))
                            southDoorRooms.Add(room);
                    }
                    if (east)
                    {
                        if (!eastDoorRooms.Contains(room))
                            eastDoorRooms.Add(room);
                    }
                    if (west)
                    {
                        if (!westDoorRooms.Contains(room))
                            westDoorRooms.Add(room);
                    }


                    room.GenerateWalls(north, south, east, west);
                    room.GenerateObstacles((RoomState)roomGrid[i, j]);
                }
            }
        }

        foreach (Room room in rooms.Values)
        {
            room.GenerateExitLinks();
        }
    }

    //get random room from direction
    public Room GetRandomNorthRoom()
    {
        return northDoorRooms[Random.Range(0, northDoorRooms.Count)];
    }

    public Room GetRandomSouthRoom()
    {
        return southDoorRooms[Random.Range(0, southDoorRooms.Count)];
    }

    public Room GetRandomEastRoom()
    {
        return eastDoorRooms[Random.Range(0, eastDoorRooms.Count)];
    }

    public Room GetRandomWestRoom()
    {
        return westDoorRooms[Random.Range(0, westDoorRooms.Count)];
    }

    public void TurnOffAllLights()
    {
        foreach (Room room in rooms.Values)
        {
            room.TurnOffAllLights();
        }
    }

    public void RegenerateLinks()
    {
        foreach (var room in rooms.Values)
        {
            room.GenerateExitLinks();
        }
    }

    public bool CheckIfHasPillar(Coords coord)
    {
        if (roomGrid[coord.x, coord.y] >= 2 && roomGrid[coord.x, coord.y] <= 5)
            return true;
        else
            return false;
    }
    public bool CheckIfHasPillar(int x, int y)
    {
        // Debug.Log(string.Format("{0},{1}:{2}", x, y, roomGrid[x, y]));
        if (roomGrid[x, y] >= 2 && roomGrid[x, y] <= 5)
            return true;
        else
            return false;
    }
}
