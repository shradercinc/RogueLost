using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Stores unique info for each room.
*/
public class RoomData
{
    public int id;
    public Enemy[] enemies;
    public Obstacles[] obstacles;
    public Color color;

    public RoomData(int id)
    {
        this.id = id;
    }
}
