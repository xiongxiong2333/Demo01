using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<T>
{
    //��
    private int row;
    //��
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

    //����������±꣬���ظø��ӵ���������
    public Vector3 GetWorldPosition(int x,int y)
    {
        return new Vector3(x+0.5f, y+0.5f) * cellSize + originPosition;
    }

    //����������±꣬��������ľ���ֵ��Value���Ϳ����Ǻܶ��֣����ݾ�������ٶ�������е���
    public void SetValue(int x,int y,T value)
    {
        if (x < 0 || y < 0 || x >= row || y >= column)
            gridArray[x, y] = value;
    }

    //�����������꣬��������������ڵĸ���������±�
    public void GetXY(Vector3 worldPosition,out int x,out int y)
    {
        x = Mathf.FloorToInt((worldPosition-originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

    }

    //����������������ֵ������
    public void SetValue(Vector3 worldPosition, T value)
    {
        GetXY(worldPosition, out int x, out int y);
        gridArray[x, y] = value;
    }

    //����grid�����±귵�������еĶ���
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
        //Debug.Log(gridArray.GetLength(0) + "�У�" + gridArray.GetLength(1) + "�С�");
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
