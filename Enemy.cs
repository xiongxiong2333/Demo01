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
    
    //�ֶ�����
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
        //������ �߼�д��MapMgr
        EventCenter.Instance.EventTrigger<Enemy>(E_EventType.E_Monster_Dead, this);
        //ɾ�����ɶ����¼����
    }
    #region �ƶ����

    public virtual void Move()
    {
        //������������������ܽӽ�ʱֹͣ�ƶ�����ֹ�鴤
        if (Vector3.Distance(this.transform.position, DataMgr.Instance.NewPosition) < 0.01f)
            return;
        //�����������Ҵ���ͬһ�������ڣ���ִ��A��Ѱ·��ֱ��������ƶ�
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
        //��һ��ִ��A�ǻ���һ�ε�Ѱ·�Ѿ����ʱ����ȡ�µ�Ŀ���
        if (needTrace)
            GetTracePoint();
        //����A��Ѱ·��ȡ����·���ƶ�
        if (pathList.Count > 0)
        {
            Vector3 targetPosition = pathList[currentPathIndex];
            //��ȡ��������������ڵ�����ֱ�߾������0.5f�ƶ���ȥ������Ƚ�·�����б��е���һ��
            if (Vector3.Distance(this.transform.position, targetPosition) > 0.5f)
            {
                moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * character.moveSpeed * Time.deltaTime;
                //�����ҵ����ƶ�������Լ����ƶ��������ˣ����¼���A��Ѱ·
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

    //����A��Ѱ·����·����
    public void GetTracePoint()
    {
        currentPathIndex = 0;
        needTrace = false;
        pathList = MapMgr.Instance.findPath.PathFinding(this.transform.position, DataMgr.Instance.NewPosition);

    }

    //ֹͣA���ƶ�
    public void StopAstarMove()
    {
        pathList = null;
        currentPathIndex = 0;
        needTrace = true;
    }
#endregion



}
