using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class Room
    {
        public Vector2 GridPosition;
        public int Type;
        public bool Top, Bot, Left, Right;
        public bool IsUsed = false;
        public Room(Vector2 pos, int type)
        {
            GridPosition = pos;
            Type = type;
            IsUsed = true;
        }

        public Room() { }
    }
}
