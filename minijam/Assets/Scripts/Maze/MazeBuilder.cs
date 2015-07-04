using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab = null, endpointPrefab = null, doorPrefab = null, keyPrefab = null, portalPrefab = null, player = null;

    [SerializeField]
    private MazePositioning MazePositioning;

    MazeGenerator mazeGenerator;

    List<Vector3> roomList = new List<Vector3>();
    //List<Vector3> removeRoomList = new List<Vector3>();
    bool ready;

    void Awake()
    {
        mazeGenerator = transform.GetComponent<MazeGenerator>();
    }

    void Update()
    {
        if (!ready)
            for (int i = 0; i < 100; i++)
                if (roomList.Count > 0)
                    BuildRoom();
                else if (mazeGenerator.Ready)
                {
                    BuildKeys();
                    BuildDoors();
                    BuildPortals();
                    SetEndPoint();
                    MazePositioning.SetMazeToCenter();
                    ready = true;
                    //Instantiate(player, new Vector3(0.5f, 2f, 0.5f), Quaternion.identity);
                    return;
                }
    }

    public void BuildMaze(int[, ,] maze)
    {
        for (int z = 0; z < maze.GetLength(2); z++)
            for (int y = 0; y < maze.GetLength(1); y++)
                for (int x = 0; x < maze.GetLength(0); x++)
                    SetRoom(new Vector3(x, y, z), maze[x, y, z]);
    }

    #region Room
    public void SetRoom(Vector3 currentRoom, int roomDescription)
    {
        roomList.Add(currentRoom);
    }

    void BuildRoom()
    {
        GameObject currentRoomObject = (GameObject)Instantiate(roomPrefab, roomList[0], Quaternion.identity);

        RemoveWalls(currentRoomObject.transform, mazeGenerator.Maze[(int)roomList[0].x, (int)roomList[0].y, (int)roomList[0].z]);

        currentRoomObject.transform.parent = this.transform;
        currentRoomObject.transform.name = "Room, " + currentRoomObject.transform.position.x + " " + currentRoomObject.transform.position.y + " " + currentRoomObject.transform.position.z;

        roomList.RemoveAt(0);
    }
    void SetEndPoint()
    {
        Instantiate(endpointPrefab, mazeGenerator.Endpoint, Quaternion.identity);
    }
    #endregion

    #region Key, Door
    void BuildKeys()
    {
        for (int i = 0; i < mazeGenerator.KeyList.Count; i++)
        {
            GameObject currentKeyObject = (GameObject)Instantiate(keyPrefab, mazeGenerator.KeyList[i].Room, Quaternion.identity);

            SetColor(currentKeyObject, mazeGenerator.KeyList[i].KeyCode);

            currentKeyObject.transform.parent = this.transform;
            currentKeyObject.transform.name = "Key " + mazeGenerator.KeyList[i].KeyCode + ", " + currentKeyObject.transform.position.x + " " + currentKeyObject.transform.position.y + " " + currentKeyObject.transform.position.z;
        }
    }

    void BuildDoors()
    {
        for (int i = 0; i < mazeGenerator.DoorList.Count; i++)
            BuildDoor(mazeGenerator.DoorList[i]);
    }

    void BuildDoor(Door door)
    {
        Vector3 position = door.Room;
        Quaternion rotation = Quaternion.identity;

        switch (door.Direction)
        {
            case Direction.Forward:
                position += Vector3.forward;
                break;
            case Direction.Right:
                position += Vector3.right;
                rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Direction.Back:
                break;
            case Direction.Left:
                rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Direction.Up:
                position += Vector3.up;
                rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.Down:
                rotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                return;
        }

        GameObject currentDoorObject = (GameObject)Instantiate(doorPrefab, position, rotation);

        SetColor(currentDoorObject, door.KeyCode);

        currentDoorObject.transform.parent = this.transform;
        currentDoorObject.transform.name = "Door, " + currentDoorObject.transform.position.x + " " + currentDoorObject.transform.position.y + " " + currentDoorObject.transform.position.z;
    } 
    #endregion

    #region Portal
    void BuildPortals()
    {
        for (int i = 0; i < mazeGenerator.PortalList.Count; i++)
        {
            for (int j = 0; j < mazeGenerator.PortalList[i].Portals.Length; j++)
            {
                GameObject currentPortalObject = (GameObject)Instantiate(portalPrefab, mazeGenerator.PortalList[i].Portals[j], Quaternion.identity);

                SetColor(currentPortalObject, i);

                currentPortalObject.transform.parent = this.transform;
                currentPortalObject.transform.name = "Portal " + i + ", " + currentPortalObject.transform.position.x + " " + currentPortalObject.transform.position.y + " " + currentPortalObject.transform.position.z;
            }
        }
    } 
    #endregion

    //public void RemoveRoom(Vector3 currentRoom)
    //{
    //    removeRoomList.Add(currentRoom);
    //}

    //void FindRoomsToDestroy(List<Vector3> destroyRoomList)
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        if(destroyRoomList.Exists(r => r == transform.GetChild(i).transform.position))
    //            DestroyRoom(transform.GetChild(i));
    //    }
    //}

    //void DestroyRoom(Transform currentRoom)
    //{
    //    Destroy(currentRoom);
    //}

    void SetColor(GameObject colorObject, int keyCode)
    {
        colorObject.transform.GetChild(0).GetComponent<Renderer>().material.color =
            keyCode == 0 ? Color.red :
            keyCode == 1 ? Color.blue :
            keyCode == 2 ? Color.yellow :
            keyCode == 3 ? Color.green :
            keyCode == 4 ? Color.cyan :
            Color.magenta;
    }

    void RemoveWalls(Transform currentRoomObject, int roomDescription)
    {
        foreach (Direction wall in mazeGenerator.GetOpenWalls(roomDescription))
            Destroy(currentRoomObject.FindChild(wall.ToString()).gameObject);
    }
}
