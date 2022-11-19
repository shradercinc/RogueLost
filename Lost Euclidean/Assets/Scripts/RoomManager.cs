using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using System;

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

public class RoomManager : MonoBehaviour
{
    private int[,] roomGrid;
    private Dictionary<int, RoomData> roomInfo;
    private int roomNo = 14;

    private Coords currCoords;

    private int maxGridX = 6;
    private int maxGridY = 6;

    public GameObject roomPrefab;

    private void Start()
    {
        currCoords = new Coords(0, 0);
        roomGrid = new int[maxGridX, maxGridY];
        GenerateRoomLayout();

        roomInfo = new Dictionary<int, RoomData>();
        GenerateRoomData();

        GenerateRooms();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     currRoomCoords.y++;
        // }
        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     currRoomCoords.x--;
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     currRoomCoords.y--;
        // }
        // else if (Input.GetKeyDown(KeyCode.D))
        // {
        //     currRoomCoords.x++;
        // }
    }

    //using Prim's Algorithm
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

    private void GenerateRoomData()
    {
        for (int i = 0; i < roomNo + 1; i++)
        {
            RoomData data = new RoomData(i);
            data.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //add enemies
            //add obstacles
            roomInfo.Add(i, data);
        }
    }

    private void GenerateRooms()
    {
        // int x = 0;
        // int z = 0;
        for (int i = 0; i < roomGrid.GetLength(0); i++)
        {
            for (int j = 0; j < roomGrid.GetLength(1); j++)
            {
                Debug.Log(roomGrid[i, j]);
                if (roomGrid[i, j] == 1)
                    Instantiate(roomPrefab, new Vector3(i * 18, 0, j * 11), Quaternion.identity);
            }
        }
    }
}
