using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathFind : Singleton<AIPathFind>
{
    // Astar Path Find
    AStarNode startNode = null;
    AStarNode targetNode = null;
    AStarNode currentNode = null;
    AStarNode checkOpenNode = null;
    List<AStarNode> openList = null;// new List<AStarNode>();
    List<AStarNode> closeList = null;// new List<AStarNode>();

    private void Start()
    {
        startNode = new AStarNode(Vector3Int.zero);
        targetNode = new AStarNode(Vector3Int.zero);
        openList= new List<AStarNode>();
        closeList = new List<AStarNode>();
    }

    bool CheckDistance(Vector2Int from, Vector2Int to) => (Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y)) > 1;

    // target은 플레이어로, target의 위치가 변함 >> startNode에서 targetNode로 가는 경로보다 target으로 가기 위한 방향방향만 알면 됨
    public CharacterDirection GetDirection(CharacterDirection currentDir, Vector3 startPosition, Vector3 targetPosition)
    {
        startNode.InitNode(MapTileManager.Instance.PositionToTilemapCoordinate(startPosition));
        targetNode.InitNode(MapTileManager.Instance.PositionToTilemapCoordinate(targetPosition));

        if (!CheckDistance(startNode.coordinate, targetNode.coordinate))
        {
            Debug.Log("AStar 중지");
            return currentDir;
        }
        //int dist = Mathf.Abs(startNode.)

        openList.Clear();
        openList.Add(startNode);
        closeList.Clear();

        while (openList.Count > 0)
        {
            currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
                if (openList[i].F <= currentNode.F && openList[i].H < currentNode.H)
                    currentNode = openList[i];

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if (currentNode.coordinate == targetNode.coordinate)
            {
                while (currentNode.parent != startNode) currentNode = currentNode.parent;
                return Utility.Vector2IntToCharacterDirection(currentNode.coordinate - startNode.coordinate);
            }

            for (int i = 0; i < 4; i++)
                OpenListAdd(currentNode, (CharacterDirection)(((int)currentDir + i) % 4));
        }
        return CharacterDirection.zero;
    }

    void OpenListAdd(AStarNode current, CharacterDirection diretion)
    {
        Vector2Int nodeCoordinate = current.coordinate + Utility.CharacterDirectionToVector2Int(diretion);
        if (CheckRange(nodeCoordinate))
        {
            if (!MapTileManager.Instance.IsCollider(current.coordinate))
            {
                if (closeList.Find(z => z.coordinate == nodeCoordinate) == null)
                {
                    checkOpenNode = openList.Find(z => z.coordinate == nodeCoordinate);
                    if (checkOpenNode != null)
                    {
                        if (currentNode.G + 1 < checkOpenNode.G)
                        {
                            checkOpenNode.parent = current;
                            checkOpenNode.G = current.G + 1;
                        }
                    }
                    else
                        openList.Add(new AStarNode(nodeCoordinate, current.G + 1, GetHCost(current.coordinate), current));
                }
            }
        }
    }

    int GetHCost(Vector2Int coordinate) => Mathf.Abs(targetNode.coordinate.x - coordinate.x) + Mathf.Abs(targetNode.coordinate.y - coordinate.y);


    bool CheckRange(Vector2Int coordinate)
    {
        if (coordinate.y > GameDatas.maxMap.y) return false;
        else if (coordinate.y < GameDatas.minMap.y) return false;
        else if (coordinate.x > GameDatas.maxMap.x) return false;
        else if (coordinate.x < GameDatas.minMap.x) return false;
        else return true;
    }
}
