using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Start End Point
// keine Türen mehr nach endpoint gesetzt

public enum Direction
{
    None = 0,
    Forward = 1,
    Right = 2,
    Up = 4,
    Back = 8,
    Left = 16,
    Down = 32,
    All = 63
}

public struct Waypoint
{
    public Vector3 Room { get; private set; }
    public int Distance { get; private set; }

    public Waypoint(Vector3 currentRoom, int distance)
    {
        Room = currentRoom;
        Distance = distance;
    }
}

public struct Key
{
    public Vector3 Room { get; private set; }
    public int KeyCode { get; private set; }
    public int Distance { get; private set; }

    public Key(Vector3 currentRoom, int keyCode, int distance)
    {
        Room = currentRoom;
        KeyCode = keyCode;
        Distance = distance;
    }
}

public struct Door
{
    public Vector3 Room { get; private set; }
    public Direction Direction { get; private set; }
    public int KeyCode { get; private set; }

    public Door(Vector3 currentRoom, Direction direction, int keyCode)
    {
        Room = currentRoom;
        Direction = direction;
        KeyCode = keyCode;
    }
}

public struct Portal
{
    public Vector3[] Portals { get; private set; }

    public Portal(params Vector3[] portals)
    {
        Portals = portals;
    }
}

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    int mazeWidth = 10, mazeDepth = 10, mazeHight = 1;

    [SerializeField]
    bool drawInTime = false, deadEnds = true, doors = false, portals = false;

    [SerializeField]
    [Range(0, 100)]
    float doorChance = 1f, portalChance = 20f;

    Vector3 startpoint = Vector3.zero, currentPoint;
    public Vector3 Endpoint { get; private set; }
    MazeBuilder mazeBuilder;

    List<Waypoint> waypointList = new List<Waypoint>();
    //bool recurringWaypoints;
    int currentDistance, maxDistance;

    public int[, ,] Maze { get; private set; }
    public float StartTime { get; private set; }
    public bool Ready { get; private set; }

    // Keys and doors
    List<int> placedKeysList = new List<int>();
    List<int> posibleKeysList = new List<int>() { 0, 1, 2, 3, 4, 5 };
    public List<Key> KeyList { get; private set; }
    public List<Door> DoorList { get; private set; }

    // Portals
    public List<Portal> PortalList { get; private set; }

    void Awake()
    {
        mazeBuilder = transform.GetComponent<MazeBuilder>();

        KeyList = new List<Key>();
        DoorList = new List<Door>();
        PortalList = new List<Portal>();
    }

    void Start()
    {
        Maze = new int[mazeWidth, mazeHight, mazeDepth];

        currentPoint = startpoint;

        StartTime = Time.time;

        if (portals)
            deadEnds = true;
        if (!deadEnds)
            drawInTime = false;
    }

    void Update()
    {
        if (!Ready)
        {
            Debug.Log(Time.deltaTime);

            for (int i = 0; i < 300; i++)
            {
                GetNextRandomRoom(ref currentPoint);

                if (Ready)
                {
                    RemoveKeys(placedKeysList.ToArray());

                    Debug.Log("Worktime: " + (Time.time - StartTime));
                    if (!drawInTime)
                        mazeBuilder.BuildMaze(Maze);

                    return;
                }
            }
        }
    }

    void DescripeRoom(Vector3 currentRoom, int roomDescription)
    {
        Maze[(int)currentRoom.x, (int)currentRoom.y, (int)currentRoom.z] = roomDescription;
    }
    int GetRoomDescription(Vector3 currentRoom)
    {
        return Maze[(int)currentRoom.x, (int)currentRoom.y, (int)currentRoom.z];
    }

    void PrintRooms(int[, ,] maze)
    {
        for (int z = 0; z < maze.GetLength(2); z++)
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                string xLine = "";

                for (int x = 0; x < maze.GetLength(0); x++)
                    xLine += maze[x, y, z] + " ";

                Debug.Log(xLine);
            }
    }

    void GetNextRandomRoom(ref Vector3 currentRoom)
    {
        List<Direction> posibleDirections = GetPosibleDirectionList(currentRoom, false);

        Direction randomDirection;

        if (posibleDirections.Count > 0)
        {
            if (posibleDirections.Count > 1)
            {
                waypointList.Add(new Waypoint(currentRoom, currentDistance));
                randomDirection = posibleDirections[Random.Range(0, posibleDirections.Count)];
            }
            else
            {
                randomDirection = posibleDirections[0];
                //recurringWaypoints = false;
            }

            ConnectRooms(ref currentRoom, ref randomDirection, posibleDirections.Count);

            // Increase distance to startpoint
            currentDistance++;
        }
        else
        {
            if (!deadEnds)
            {
                List<Direction> openWalls = GetOpenWalls(GetRoomDescription(currentRoom));

                if (openWalls.Count == 1)
                {
                    posibleDirections.Clear();
                    posibleDirections = GetPosibleDirectionList(currentRoom, true);
                    posibleDirections.Remove(openWalls[0]);

                    randomDirection = posibleDirections[Random.Range(0, posibleDirections.Count)];

                    ConnectRooms(ref currentRoom, ref randomDirection, 1);
                }
            }

            if (drawInTime)
                mazeBuilder.SetRoom(currentRoom, GetRoomDescription(currentRoom));

            if (deadEnds && currentRoom == startpoint || waypointList.Count == 0)
                Ready = true;
            else
            {
                if (currentDistance > maxDistance)
                {
                    Endpoint = currentRoom;
                    maxDistance = currentDistance;
                }
                if (!portals && waypointList.Count > 3)
                {
                    currentRoom = waypointList[waypointList.Count - 3].Room;
                    currentDistance = waypointList[waypointList.Count - 3].Distance;
                    waypointList.RemoveAt(waypointList.Count - 3);
                }
                else
                {
                    currentRoom = waypointList[waypointList.Count - 1].Room;
                    currentDistance = waypointList[waypointList.Count - 1].Distance;
                    waypointList.RemoveAt(waypointList.Count - 1);
                }
            }

            AddPortal(ref currentRoom);

            //if (!recurringWaypoints)
            //{
            //    RemoveKeys(placedKeysList.ToArray());
            //    recurringWaypoints = true;
            //}
        }
    }

    void ConnectRooms(ref Vector3 currentRoom, ref Direction direction, int posibleDirections)
    {
        // leaved Room
        int roomDescription = GetRoomDescription(currentRoom);
        RemoveWall(ref roomDescription, direction);
        DescripeRoom(currentRoom, roomDescription);

        if (drawInTime && posibleDirections == 1)
            mazeBuilder.SetRoom(currentRoom, GetRoomDescription(currentRoom));

        AddKey(currentRoom);

        AddDoor(currentRoom, direction);

        currentRoom = MoveToNextRoom(currentRoom, direction);

        // arrival-room
        roomDescription = GetRoomDescription(currentRoom);
        GetOpositeDirection(ref direction);
        RemoveWall(ref roomDescription, direction);
        DescripeRoom(currentRoom, roomDescription);
    }

    void AddKey(Vector3 currentRoom)
    {
        if (doors && posibleKeysList.Count > 0 && Random.Range(0, 100) < doorChance / 1.5f && !KeyList.Exists(k => k.Room == currentRoom))
        {
            int randomKey = Random.Range(0, posibleKeysList.Count - 1);
            int keyCode = posibleKeysList[randomKey];
            posibleKeysList.RemoveAt(randomKey);
            KeyList.Add(new Key(currentRoom, keyCode, currentDistance));
            placedKeysList.Add(keyCode);
        }
    }
    void RemoveKey(int keyCode)
    {
        KeyList.Remove(KeyList.FindLast(k => k.KeyCode == keyCode));
        placedKeysList.Remove(keyCode);
        posibleKeysList.Add(keyCode);
    }
    void RemoveKeys(params int[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
            RemoveKey(keys[i]);
    }

    void AddDoor(Vector3 currentRoom, Direction direction)
    {
        if (doors && placedKeysList.Count > 0 && Random.Range(0, 100) < doorChance)
        {
            int randomKey = Random.Range(0, placedKeysList.Count - 1);
            int keyCode = placedKeysList[randomKey];
            if (KeyList.FindLast(k => k.KeyCode == keyCode).Distance < currentDistance)
            {
                posibleKeysList.Add(keyCode);
                DoorList.Add(new Door(currentRoom, direction, keyCode));
                placedKeysList.RemoveAt(randomKey);
            }
        }
    }

    void AddPortal(ref Vector3 currentRoom)
    {
        if (portals && PortalList.Count < 6 && Random.Range(0, 100) < portalChance / PortalList.Count)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 randomRoom = new Vector3(Random.Range(0, mazeWidth), Random.Range(0, mazeHight), Random.Range(0, mazeDepth));

                    if (GetRoomDescription(randomRoom) == 0 && !PortalList.Exists(p => p.Portals[0] == randomRoom && p.Portals[1] == randomRoom))// && !KeyList.Exists(k => k.Room == randomRoom))
                    {
                        PortalList.Add(new Portal(currentRoom, randomRoom));

                        currentRoom = randomRoom;
                        return;
                    }
                }
            }
    }

    void GetOpositeDirection(ref Direction direction)
    {
        if ((int)direction < 8)
            direction = (Direction)((int)direction * 8);
        else
            direction = (Direction)((int)direction / 8);
    }

    List<Direction> GetPosibleDirectionList(Vector3 currentRoom, bool deadEnd)
    {
        List<Direction> posibleDirections = new List<Direction>();

        if (currentRoom.x > 0 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Left)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Left);
        if (currentRoom.x < mazeWidth - 1 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Right)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Right);

        if (currentRoom.y > 0 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Down)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Down);
        if (currentRoom.y < mazeHight - 1 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Up)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Up);

        if (currentRoom.z > 0 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Back)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Back);
        if (currentRoom.z < mazeDepth - 1 && (GetRoomDescription(MoveToNextRoom(currentRoom, Direction.Forward)) == 0 || deadEnd))
            posibleDirections.Add(Direction.Forward);

        return posibleDirections;
    }

    public List<Direction> GetOpenWalls(int roomDescription)
    {
        List<Direction> posibleDirections = new List<Direction>();

        for (int i = -32; i < 0; i /= 2)
            if (i >= roomDescription)
            {
                roomDescription -= i;
                posibleDirections.Add((Direction)(i * -1));
            }

        return posibleDirections;
    }

    Vector3 MoveToNextRoom(Vector3 currentRoom, Direction direction)
    {
        switch (direction)
        {
            case Direction.Forward:
                return currentRoom + Vector3.forward;
            case Direction.Right:
                return currentRoom + Vector3.right;
            case Direction.Back:
                return currentRoom + Vector3.back;
            case Direction.Left:
                return currentRoom + Vector3.left;
            case Direction.Up:
                return currentRoom + Vector3.up;
            case Direction.Down:
                return currentRoom + Vector3.down;
            default:
                return currentRoom;
        }
    }

    void RemoveWall(ref int roomDescription, Direction removeDirection)
    {
        roomDescription -= (int)removeDirection;
    }
}
