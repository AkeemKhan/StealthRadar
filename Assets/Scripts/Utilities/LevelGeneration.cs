﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class LevelGeneration : MonoBehaviour
    {
        public GameObject RoomObject;
        public GameObject FilledSpace;

        public int CountFromZeroY;
        public int CountFromZeroX;

        Vector2 WorldSize = new Vector2(4, 4);
        Room[,] Rooms;
        List<Vector2> TakenPositions = new List<Vector2>();
        int GridSizeX, GridSizeY = 20, NumberOfRooms = 20;
        
        
        void Start()
        {
            if (NumberOfRooms >= (WorldSize.x * 2) * (WorldSize.y * 2))
            {
                NumberOfRooms = Mathf.RoundToInt((WorldSize.x * 2) * (WorldSize.y * 2));
            }

            GridSizeX = Mathf.RoundToInt(WorldSize.x);
            GridSizeY = Mathf.RoundToInt(WorldSize.y);

            CreateRooms();                        
            DrawMap(); //instantiates objects to make up a map
        }

        public void CreateRooms()
        {
            Rooms = new Room[GridSizeX * 2, GridSizeY * 2];

            //for (int i = 0; i < GridSizeX * 2; i++)
            //{
            //    for(int j = 0; j < GridSizeY * 2; j++)
            //    {
            //        Rooms[i, j] = new Room(new Vector2(GridSizeX * i * 2, GridSizeY * j * 2), 2);
            //    }
            //}

            Rooms[GridSizeX, GridSizeY] = new Room(Vector2.zero, 1);
            TakenPositions.Insert(0, Vector2.zero);
            Vector2 checkPos = Vector2.zero;

            float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
            for (int i = 0; i < NumberOfRooms - 1; i++)
            {
                float randomPerc = ((float)i) / (((float)NumberOfRooms - 1));
                randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
                //grab new position
                checkPos = NewPosition();
                //test new position
                if (NumberOfNeighbors(checkPos, TakenPositions) > 1 && UnityEngine.Random.value > randomCompare)
                {
                    int iterations = 0;
                    do
                    {
                        checkPos = SelectiveNewPosition();
                        iterations++;
                    } while (NumberOfNeighbors(checkPos, TakenPositions) > 1 && iterations < 100);
                    if (iterations >= 50)
                        print("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, TakenPositions));
                }
                //finalize position
                Rooms[(int)checkPos.x + GridSizeX, (int)checkPos.y + GridSizeY] = new Room(checkPos, 0);
                TakenPositions.Insert(0, checkPos);
            }
        }

        public Vector2 NewPosition()
        {
            int x = 0, y = 0;
            Vector2 checkingPos = Vector2.zero;
            do
            {
                int index = Mathf.RoundToInt(UnityEngine.Random.value * (TakenPositions.Count - 1)); // pick a random room
                x = (int)TakenPositions[index].x;//capture its x, y position
                y = (int)TakenPositions[index].y;
                bool UpDown = (UnityEngine.Random.value < 0.5f);//randomly pick wether to look on hor or vert axis
                bool positive = (UnityEngine.Random.value < 0.5f);//pick whether to be positive or negative on that axis
                if (UpDown)
                { //find the position bnased on the above bools
                    if (positive)
                    {
                        y += 1;
                    }
                    else
                    {
                        y -= 1;
                    }
                }
                else
                {
                    if (positive)
                    {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                    }
                }
                checkingPos = new Vector2(x, y);
            } while (TakenPositions.Contains(checkingPos) || x >= GridSizeX || x < -GridSizeX || y >= GridSizeY || y < -GridSizeY); //make sure the position is valid
            return checkingPos;
        }

        public int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions)
        {
            int ret = 0; // start at zero, add 1 for each side there is already a room
            if (usedPositions.Contains(checkingPos + Vector2.right))
            { //using Vector.[direction] as short hands, for simplicity
                ret++;
            }
            if (usedPositions.Contains(checkingPos + Vector2.left))
            {
                ret++;
            }
            if (usedPositions.Contains(checkingPos + Vector2.up))
            {
                ret++;
            }
            if (usedPositions.Contains(checkingPos + Vector2.down))
            {
                ret++;
            }
            return ret;
        }

        public Vector2 SelectiveNewPosition()
        { // method differs from the above in the two commented ways
            int index = 0, inc = 0;
            int x = 0, y = 0;
            Vector2 checkingPos = Vector2.zero;
            do
            {
                inc = 0;
                do
                {
                    //instead of getting a room to find an adject empty space, we start with one that only 
                    //as one neighbor. This will make it more likely that it returns a room that branches out
                    index = Mathf.RoundToInt(UnityEngine.Random.value * (TakenPositions.Count - 1));
                    inc++;
                } while (NumberOfNeighbors(TakenPositions[index], TakenPositions) > 1 && inc < 100);
                x = (int)TakenPositions[index].x;
                y = (int)TakenPositions[index].y;
                bool UpDown = (UnityEngine.Random.value < 0.5f);
                bool positive = (UnityEngine.Random.value < 0.5f);
                if (UpDown)
                {
                    if (positive)
                    {
                        y += 1;
                    }
                    else
                    {
                        y -= 1;
                    }
                }
                else
                {
                    if (positive)
                    {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                    }
                }
                checkingPos = new Vector2(x, y);
            } while (TakenPositions.Contains(checkingPos) || x >= GridSizeX || x < -GridSizeX || y >= GridSizeY || y < -GridSizeY);
            if (inc >= 100)
            { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
                print("Error: could not find position with only one neighbor");
            }
            return checkingPos;
        }

        public void DrawMap()
        {
            for(int i = 0; i < GridSizeX * 2; i++)
            {
                for(int j = 0; j < GridSizeY * 2; j++)
                {
                    if (Rooms[i,j] == null)
                    {
                        Instantiate(FilledSpace, new Vector3(20 * (i - GridSizeX), 10 * (j - GridSizeY)), Quaternion.identity);
                    }
                    else
                    {
                        Vector2 drawPos = Rooms[i, j].GridPosition;
                       
                        Debug.Log(drawPos);
                        drawPos.x *= 20;
                        drawPos.y *= 10;

                        // Instantiate the Room
                        Instantiate(RoomObject, new Vector3(drawPos.x, drawPos.y), Quaternion.identity);
                    }
                }
            }
        }
    }
}
