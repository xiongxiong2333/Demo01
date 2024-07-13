using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<T>
{
    //列
    private int row;
    //行
    private int column;
    private int cellSize;
    private T[,] gridArray;
    private Vector3 originPosition;

    public GridSystem(int row, int column)
    {
        this.row = row;
        this.column = column;
        this.cellSize = 1;
        gridArray = new T[this.row, this.column];
        this.originPosition = Vector3.zero;

    }
    public GridSystem(int row, int column, int cellSize)
    {
        this.row = row;
        this.column = column;
        this.cellSize = cellSize;
        gridArray = new T[this.row, this.column];
        this.originPosition = Vector3.zero;
    }

    public GridSystem(int row, int column, int cellSize,Vector3 originPosition)
    {
        this.row = row;
        this.column = column;
        this.cellSize = cellSize;
        gridArray = new T[this.row, this.column];
        this.originPosition = originPosition;
    }

    public GridSystem(int row, int column, int cellSize, Vector3 originPosition, Func<GridSystem<T>,int,int,T> createGridObject)
    {
        this.row = row;
        this.column = column;
        this.cellSize = 1;
        gridArray = new T[this.row, this.column];
        this.originPosition = originPosition;
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = createGridObject(this,i,j);

            }
        }

    }

    public int GetRow()
    {
        return gridArray.GetLength(0);
    }

    public int GetColumn()
    {
        return gridArray.GetLength(1);
    }

    //传入数组的下标，返回该格子的世界坐标
    public Vector3 GetWorldPosition(int x,int y)
    {
        return new Vector3(x+0.5f, y+0.5f) * cellSize + originPosition;
    }

    //传入数组的下标，设置数组的具体值，Value类型可以是很多种，根据具体情况再对数组进行调整
    public void SetValue(int x,int y,T value)
    {
        if (x < 0 || y < 0 || x >= row || y >= column)
            gridArray[x, y] = value;
    }

    //传入世界坐标，返回这个坐标所在的格子数组的下标
    public void GetXY(Vector3 worldPosition,out int x,out int y)
    {
        x = Mathf.FloorToInt((worldPosition-originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

    }

    //传入世界坐标设置值的重载
    public void SetValue(Vector3 worldPosition, T value)
    {
        GetXY(worldPosition, out int x, out int y);
        gridArray[x, y] = value;
    }

    //传入grid数组下标返回数组中的对象
    public T GetGridObject(int x, int y)
    {
        if (x >= row || y >= column || x < 0 || y < 0)
            return default(T);
        else return gridArray[x, y];
    }

    public bool OnSameGrid(Vector3 a,Vector3 b)
    {
        GetXY(a, out int aX, out int aY);
        GetXY(b, out int bX, out int bY);
        return (aX == bX && aY == bY);
    }

    public void print()
    {
        //Debug.Log(gridArray.GetLength(0) + "列，" + gridArray.GetLength(1) + "行。");
        //Debug.Log(GetWorldPosition(1, 0));
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for(int j = 0; j < gridArray.GetLength(1); j++)
            {
                //Debug.Log(GetWorldPosition(1,0));
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);

            }
        }


    }


}
