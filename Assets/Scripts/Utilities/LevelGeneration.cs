﻿using PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class LevelGeneration : MonoBehaviour
    {
        // Temporary Game Builder
        public int EnemyCount = 10;
        public int MaxEnemyCount = 20;
        public int GridSize = 10;
        public int NumberOfRooms = 10;
        public GameObject Player;
        public GameObject Enemy;

        public EnemyGeneration EnemyGeneration;

        public List<Vector2> EnemySpawnPositions;
        public List<Vector2> PlayerSpawnPositions;

        public GameObject RoomObject;
        public GameObject FilledSpace;
        public List<GameObject> Templates;

        public static Dictionary<Node, int> NodeIdMap;
        public static Dictionary<int, Node> IdNodeMap;
        public static Dictionary<Vector3, Node> VectorNodeMap;
        public static Dictionary<Node, Vector3> NodeVectorMap;       

        public static Dictionary<int, GameObject> IdNodeMapGO;

        public static Navigation Navigation;

        public int CountFromZeroY;
        public int CountFromZeroX;

        public bool LoadTheTemplates;
        public bool GenerateLevel;
        public int CustomStage;

        Vector2 WorldSize = new Vector2(4, 4);
        Room[,] Rooms;
        List<Vector2> TakenPositions = new List<Vector2>();
        int GridSizeX, GridSizeY = 0;
                
        void Start()
        {
            GridSizeX = GridSizeY = GridSize;
            if (LoadTheTemplates)
            {
                LoadTemplates();
            }

            if (GenerateLevel)
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
            BuildNavidationGraph();
        }

        public void LoadTemplates()
        {
            UnityEngine.Object[] data = Resources.LoadAll<GameObject>("Templates");
            Debug.Log("Count --- " + data.Length);

            Templates = data.Select(p => p as GameObject).ToList();
            Debug.Log("Count --- " + data.Length);
        }

        public void CreateRooms()
        {
            Rooms = new Room[GridSizeX * 2, GridSizeY * 2];

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
            int templateCount = Templates.Count;
            
            for (int i = 0; i < GridSizeX * 2; i++)
            {
                for(int j = 0; j < GridSizeY * 2; j++)
                {
                    if (Rooms[i,j] == null)
                    {
                        Instantiate(FilledSpace, new Vector3(20 * (i - GridSizeX), 10 * (j - GridSizeY)), Quaternion.identity);
                    }
                    else
                    {
                        GameObject template = Templates[UnityEngine.Random.Range(0, templateCount)];

                        Vector2 drawPos = Rooms[i, j].GridPosition;
                       
                        drawPos.x *= 20;
                        drawPos.y *= 10;

                        // Instantiate the Room
                        Instantiate(template, new Vector3(drawPos.x, drawPos.y), Quaternion.identity);
                    }
                }
            }

            Designer();
        }   
        
        private void Designer()
        {
            InitialisePlayer();
            InitialiseEnemies();
            InitialiseExits();
        }

        private static void InitialiseExits()
        {
            var PotentialExits = GameObject.FindGameObjectsWithTag(EntityConstants.EXIT_TAG);
            PotentialExits[UnityEngine.Random.Range(0, PotentialExits.Length - 1)].GetComponent<ActiveExit>().SetAsExit();
        }

        private void InitialiseEnemies()
        {
            EnemyCount = PlayerStatistics.EnemyCount;

            EnemySpawnPositions = GameObject.FindGameObjectsWithTag(EntityConstants.ENEMY_SPAWN_TAG)
                .Select(p => new Vector2(p.transform.position.x, p.transform.position.y)).ToList();            

            EnemyGeneration.Initialise(EnemySpawnPositions, EnemyCount, PlayerStatistics.DifficultyModifier);
            EnemyGeneration.GenerateEnemies();
        }

        private void InitialisePlayer()
        {
            PlayerSpawnPositions = GameObject.FindGameObjectsWithTag(EntityConstants.PLAYER_SPAWN_TAG)
                            .Select(p => new Vector2(p.transform.position.x, p.transform.position.y)).ToList();

            Vector2 playerPosition = PlayerSpawnPositions[UnityEngine.Random.Range(0, PlayerSpawnPositions.Count - 1)];
            Instantiate(Player, new Vector3(playerPosition.x, playerPosition.y), Quaternion.identity);
        }

        public void BuildNavidationGraph()
        {
            var nodePositions = GameObject.FindGameObjectsWithTag(EntityConstants.PFNODE_TAG);
            var pfNodes = new List<Node>();
            
            int id = 0;

            foreach (var nodePosition in nodePositions)
            {
                pfNodes.Add(new Node
                {
                    Id = id,
                    X = nodePosition.transform.position.x,
                    Y = nodePosition.transform.position.y,
                });
                id++;
            }

            Navigation = new Navigation();
            var edges = new List<Edge>();

            NodeIdMap = new Dictionary<Node, int>();
            IdNodeMap = new Dictionary<int, Node>();
            VectorNodeMap = new Dictionary<Vector3, Node>();
            NodeVectorMap = new Dictionary<Node, Vector3>();

            int i = 0;
            foreach (var pfNode in pfNodes)
            {
                NodeIdMap.Add(pfNode, i);
                IdNodeMap.Add(i, pfNode);

                if (!VectorNodeMap.ContainsKey(nodePositions[pfNode.Id].transform.position))
                    VectorNodeMap.Add(nodePositions[pfNode.Id].transform.position, pfNode);

                if (!NodeVectorMap.ContainsKey(pfNode))
                    NodeVectorMap.Add(pfNode, nodePositions[pfNode.Id].transform.position);
                
                i++;
                Debug.Log($"i = {i}");
            }

            int skipped = 0;
            int potential = 0;

            foreach (var pfNode in pfNodes)
            {
                foreach (var pfNode2 in pfNodes)
                {
                    if (pfNode == pfNode2)
                        continue;

                    var x1 = pfNode.X;
                    var x2 = pfNode2.X;
                    var y1 = pfNode.Y;
                    var y2 = pfNode2.Y;

                    RaycastHit2D hit = Physics2D.Linecast(nodePositions[pfNode.Id].transform.position, nodePositions[pfNode2.Id].transform.position);

                    potential++;
                    if (hit)
                    {
                        if (hit.collider.tag == EntityConstants.WALL_TAG)
                        {
                            skipped++;
                            continue;
                        }
                    }

                    var distance = Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));                    
                    edges.Add(new Edge(NodeIdMap[pfNode], NodeIdMap[pfNode2], distance*distance));                    
                }
            }

            Debug.Log($"skipped: {skipped}");
            Debug.Log($"potential: {potential}");
            Navigation.BuildGraph(edges);
        }
    }
}
