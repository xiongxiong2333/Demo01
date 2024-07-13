using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridSystem<PathNode> grid;
    //��grid�����е��±�
    public int x;
    public int y;

    //����㵽��ǰλ�õ�Cost
    public int gCost;
    //�ӵ�ǰλ�õ��յ㲻�����ϰ���Cost
    public int hCost;
    public int fCost;
    public bool isWalkable=true;

    public int num;
    public PathNode cameFromPathNode;

    public PathNode(GridSystem<PathNode> grid,int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

}

public class FindPath 
{
    //public static FindPath Instance { get; private set; }
    private GridSystem<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closeList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    public FindPath(int row,int column)
    {
        //Instance = this;
        grid = new GridSystem<PathNode>(row, column,1,new Vector3(-11,-21),(grid,x,y)=>new PathNode(grid,x,y));
       
    }

    
    //���������б�ļ���·������
    public List<Vector3> PathFinding(Vector3 start,Vector3 end)
    {
        grid.GetXY(start, out int startX, out int startY);
        grid.GetXY(end, out int endX, out int endY);
        List<PathNode> nodeList = PathFinding(startX, startY, endX, endY);
        List<Vector3> positionList = new List<Vector3>();
        if (nodeList != null)
        {
            for(int i = 0; i < nodeList.Count; i++)
            {
                positionList.Add(grid.GetWorldPosition(nodeList[i].x, nodeList[i].y));
            }
        }
        return positionList;
    }

    //����·��
    public List<PathNode> PathFinding(int startX,int startY,int endX,int endY)
    {
        
        //��ʼ������յ㡢�����б�
        PathNode startNode = grid.GetGridObject(startX,startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        if (startNode == null || endNode == null)
            return null;
        openList = new List<PathNode> { startNode };
        closeList = new List<PathNode>();
        //��ʼ��ÿһ��pathNode����ֵ
        for(int x = 0; x < grid.GetRow(); x++)
        {
            for(int y = 0; y < grid.GetColumn(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromPathNode = null;
            }
        }

        //�������Cost
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        //��ʼ��������
        while (openList.Count > 0)
        {
            //openList����F��С��node��Ϊ��ǰnode
            PathNode currentNode = GetLowestFCostNode(openList);
            //�ִ��յ㷵��·��
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);


            //�������ھ�
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (!neighbourNode.isWalkable)
                {
                    closeList.Add(neighbourNode);
                    continue;
                }
                if (closeList.Contains(neighbourNode))
                {
                    continue;
                }
                //�����ھӵ�gCost
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                //����ھӵ�gCost���������õ�gCost˵���ھӽ�㻹û��ʼ���������ҵ�������·(�����ϲ�����ֵڶ������)
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.cameFromPathNode = currentNode;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }         
            }
        }
        //�޷��ִ�Ŀ���
        return null;

    }

    //�õ�ĳ��pathNode�İ˸�����ĺϷ��ھ�
    private List<PathNode> GetNeighbourList(PathNode pathNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        //Left
        if (pathNode.x - 1 >= 0)
        {
            neighbourList.Add(grid.GetGridObject(pathNode.x - 1, pathNode.y));
            //LeftDown
            if (pathNode.y - 1 >= 0)
            {
                neighbourList.Add(grid.GetGridObject(pathNode.x - 1, pathNode.y-1));
            }
            //LeftUP
            if (pathNode.y + 1 <= grid.GetColumn())
            {
                neighbourList.Add(grid.GetGridObject(pathNode.x - 1, pathNode.y + 1));
            }
        }
        //Right
        if (pathNode.x + 1 <= grid.GetRow())
        {
            neighbourList.Add(grid.GetGridObject(pathNode.x + 1, pathNode.y));
            //RightDown
            if (pathNode.y - 1 >= 0)
            {
                neighbourList.Add(grid.GetGridObject(pathNode.x + 1, pathNode.y - 1));
            }
            //RightUP
            if (pathNode.y + 1 <= grid.GetColumn())
            {
                neighbourList.Add(grid.GetGridObject(pathNode.x + 1, pathNode.y + 1));
            }
        }
        //Down 
        if (pathNode.y - 1 >= 0)
        {
            neighbourList.Add(grid.GetGridObject(pathNode.x, pathNode.y - 1));
        }
        //Up
        if (pathNode.y + 1 <= grid.GetColumn())
        {
            neighbourList.Add(grid.GetGridObject(pathNode.x, pathNode.y + 1));
        }

        return neighbourList;
    }

    //�������������̾��룬�������赲
    private int CalculateDistanceCost(PathNode a,PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int horizontalDistance = Mathf.Abs(xDistance - yDistance);

        return horizontalDistance * MOVE_STRAIGHT_COST + Mathf.Min(xDistance, yDistance) * MOVE_DIAGONAL_COST;
    }

    //��ȡopenList����Fֵ��͵ĵ� ������һ����չ����ʼ��
    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestNode = pathNodes[0];
        for(int i = 1; i < pathNodes.Count; i++)
        {
            if (lowestNode.fCost > pathNodes[i].fCost)
            {
                lowestNode = pathNodes[i];
            }
        }
        return lowestNode;
    }

    //�������·��
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes = new List<PathNode>();
        PathNode pathNode = endNode;
        while(pathNode.cameFromPathNode!=null)
        {
            pathNodes.Add(pathNode);
            pathNode = pathNode.cameFromPathNode;
        }

        pathNodes.Reverse();
        return pathNodes;
    }

    public GridSystem<PathNode> GetGrid()
    {
        return grid;
    }

    public void TestPrint()
    {
        grid.print();
    }

}
