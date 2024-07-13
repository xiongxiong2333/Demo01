using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<Vector3> pathList;

    public int currentPathIndex;

    public Vector3 moveDir;

    //public float moveSpeed;

    //public float normalSpeed;

    public bool needTrace=true;
    
    //手动挂载
    public Animator animator;

    public Character character;
    public virtual void Hurt()
    {
        animator.SetTrigger("isHurt");
    }

    public virtual void Dead()
    {
        gameObject.layer = 12;
        animator.SetBool("isDead", true);
        StopAstarMove();
        character.moveSpeed = 0;
        //掉东西 逻辑写在MapMgr
        EventCenter.Instance.EventTrigger<Enemy>(E_EventType.E_Monster_Dead, this);
        //删对象由动画事件完成
    }
    #region 移动相关

    public virtual void Move()
    {
        //当自身坐标和玩家坐标很接近时停止移动，防止抽搐
        if (Vector3.Distance(this.transform.position, DataMgr.Instance.NewPosition) < 0.01f)
            return;
        //如果自身与玩家处在同一个格子内，不执行A星寻路，直接向玩家移动
        if (MapMgr.Instance.findPath.GetGrid().OnSameGrid(transform.position, DataMgr.Instance.NewPosition))
        {
            moveDir = DataMgr.Instance.NewPosition - transform.position;
            transform.position = transform.position + moveDir.normalized * character.moveSpeed * Time.deltaTime;
            StopAstarMove();
        }
        else
        {
            AstarMove();
        }
    }
    public void AstarMove()
    {
        //第一次执行A星或上一次的寻路已经完成时，获取新的目标点
        if (needTrace)
            GetTracePoint();
        //根据A星寻路获取到的路径移动
        if (pathList.Count > 0)
        {
            Vector3 targetPosition = pathList[currentPathIndex];
            //读取到的坐标点与现在的坐标直线距离大于0.5f移动过去，否则比较路径点列表中的下一个
            if (Vector3.Distance(this.transform.position, targetPosition) > 0.5f)
            {
                moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * character.moveSpeed * Time.deltaTime;
                //如果玩家的新移动方向和自己的移动方向反向了，重新计算A星寻路
                if (Vector3.Dot(DataMgr.Instance.PlayerMoveDir, moveDir) < 0)
                {
                    StopAstarMove();
                }
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathList.Count)
                {
                    StopAstarMove();
                    moveDir = (DataMgr.Instance.NewPosition - transform.position).normalized;
                    transform.position = transform.position + moveDir * character.moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    //调用A星寻路计算路径点
    public void GetTracePoint()
    {
        currentPathIndex = 0;
        needTrace = false;
        pathList = MapMgr.Instance.findPath.PathFinding(this.transform.position, DataMgr.Instance.NewPosition);

    }

    //停止A星移动
    public void StopAstarMove()
    {
        pathList = null;
        currentPathIndex = 0;
        needTrace = true;
    }
#endregion



}
