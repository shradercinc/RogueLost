using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
