using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RoomGeneration : MonoBehaviour
{
    private int[,] roomGrid; //for room IDs
    private Dictionary<int, RoomData> roomInfo;
    private int totalRooms = 9;

    private void Start()
    {
        roomGrid = new int[3, 3]; //9 rooms
        roomInfo = new Dictionary<int, RoomData>();
        GenerateMap();
    }

    private void GenerateRoomData()
    {
        for (int i = 0; i < 9; i++)
        {
            RoomData data = new RoomData(i);
            //add enemies
            //add obstacles
            roomInfo.Add(i, data);
        }
    }

    //fills "roomGrid" 2D array with room IDs
    private void GenerateMap()
    {
        //randomizes array of numbers {1,2,3,4} => {3,2,4,1}
        int[] arr = Enumerable.Range(0, totalRooms).ToArray();
        System.Random random = new System.Random();
        arr = arr.OrderBy(x => random.Next()).ToArray();

        //puts ids into "roomGrid"
        int count = 0;
        for (int i = 0; i < roomGrid.GetLength(0); i++)
        {
            for (int j = 0; j < roomGrid.GetLength(1); j++)
            {
                roomGrid[i, j] = arr[count];
                count++;
            }
        }
    }
}
