using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    Transform player = null;

    [SerializeField] GameObject minimapRoom;
    [SerializeField] GameObject minimapDoorway;
    [SerializeField] GameObject playerObj;
    [HideInInspector] public Dictionary<Vector2Int, GameObject> minimapDictionary;

    public float dragSpeed = 1.0f;
    public float pinchSpeed = 1.0f;
    float pinch = 4.0f;
    public Vector2 pinchRange = new Vector2(1.0f, 8.0f);
    Camera cam = null;

    private void Awake()
    {
        if (cam == null)
        {
            cam = GetComponentInChildren<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = pinch;
        }
        if (player == null)
            player = Instantiate(playerObj, Vector3.zero, Quaternion.identity, transform).transform;
        InactiveMinimap();
    }

    void ClearMinimap()
    {

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).gameObject == player.gameObject)
                continue;
            if (transform.GetChild(i).gameObject == cam.gameObject)
                continue;

            Destroy(transform.GetChild(i).gameObject);
        }

        if (minimapDictionary != null)
            minimapDictionary.Clear();
        else
            minimapDictionary = new Dictionary<Vector2Int, GameObject>();
    }


    public void ActiveMinimap()
    {
        cam.transform.localPosition = new Vector3(DungeonBuilder.Inst.currentRoom.x, DungeonBuilder.Inst.currentRoom.y, -1f);
        player.localPosition = new Vector3(DungeonBuilder.Inst.currentRoom.x, DungeonBuilder.Inst.currentRoom.y, 0f);
        gameObject.SetActive(true);
    }
    public void InactiveMinimap() => gameObject.SetActive(false);

    public void PinchEvent(float delta)
    {
        Debug.Log(delta);
        pinch += delta * pinchSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(pinch, pinchRange.x, pinchRange.y);
    }

    public void DragEvent(Vector2 delta)
    {
        Vector3 _delta = Vector3.zero;
        _delta.x = delta.x;
        _delta.y = delta.y;
        cam.transform.localPosition -= _delta * (dragSpeed * (pinch / pinchRange.y)) * Time.deltaTime;
    }

    public void AddMinimapDictionary(Vector2Int coordinate, byte doorway)
    {
        if (coordinate == Vector2.zero)
            ClearMinimap();

        GameObject mapRoom = Instantiate(minimapRoom, new Vector3(coordinate.x, coordinate.y, 0f), Quaternion.identity, transform);

        GameObject mapDoorway = null;

        if ((doorway & 1 << (int)Orientation.north) != 0)
        {
            mapDoorway = Instantiate(minimapDoorway, mapRoom.transform.position, Quaternion.identity, mapRoom.transform);
            mapDoorway.transform.Rotate(Vector3.back * 0f);
            mapDoorway = null;
        }

        if ((doorway & 1 << (int)Orientation.east) != 0)
        {
            mapDoorway = Instantiate(minimapDoorway, mapRoom.transform.position, Quaternion.identity, mapRoom.transform);
            mapDoorway.transform.Rotate(Vector3.back * 90f);
            mapDoorway = null;
        }

        if ((doorway & 1 << (int)Orientation.south) != 0)
        {
            mapDoorway = Instantiate(minimapDoorway, mapRoom.transform.position, Quaternion.identity, mapRoom.transform);
            mapDoorway.transform.Rotate(Vector3.back * 180f);
            mapDoorway = null;
        }

        if ((doorway & 1 << (int)Orientation.west) != 0)
        {
            mapDoorway = Instantiate(minimapDoorway, mapRoom.transform.position, Quaternion.identity, mapRoom.transform);
            mapDoorway.transform.Rotate(Vector3.back * 270f);
            mapDoorway = null;
        }

        minimapDictionary.Add(coordinate, mapRoom);
        mapRoom.SetActive(coordinate == Vector2Int.zero);
    }
}
