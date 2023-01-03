using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Stores unique info for each room.
*/
public class RoomData
{
    public int id;
    public RoomManager.RoomState state;
    public Enemy[] enemies;
    public Obstacles[] obstacles;

    public RoomData(int id)
    {
        this.id = id;
    }
}
