using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;  //중복제거
using System;       //배열 ReSize

public class Draw_Grid : MonoBehaviour
{
    enum BLOCk_NAME
    {
        BASE = 0
    }

    public struct Map_Pos
    {
        public float px;
        public float py;
    }
    public struct Map_Num
    {
        public int nx;
        public int ny;
    }

    //우선순위 큐 만들기 위한 구조체
    struct Priority_Queue
    {
        public int size_data;
        public int[] array_num;
    }

    //표준위치 확보용
    Priority_Queue pq_x = new Priority_Queue
    {
        size_data = 0,
        array_num = new int[101]
    };
    Priority_Queue pq_y = new Priority_Queue
    {
        size_data = 0,
        array_num = new int[101]
    };

    //그리드 새로그릴때 최신화용
    Priority_Queue pq_del_x = new Priority_Queue
    {
        size_data = 0,
        array_num = new int[101]
    };
    Priority_Queue pq_del_y = new Priority_Queue
    {
        size_data = 0,
        array_num = new int[101]
    };

    public Dictionary<Map_Num, Map_Pos> map_info = new Dictionary<Map_Num, Map_Pos>();
    public Map_Pos hmp;
    public Map_Num map_num;
    public GameObject for_ins_obj;

    public GameObject obj_tile;
    public GameObject obj_line;    
    public int value_x;
    public int value_y;
    bool check_make_state;

    void Start()
    {
        check_make_state = false;
    }

    void Update()
    {

    }

    int Compare_Pq_Value(int input, int array_value)
    {
        return input - array_value;
    }

    int FindChildValue(ref Priority_Queue pq, int index)
    {
        //왼쪽 차일드가 없을때
        if (index * 2 > pq.size_data)
            return 0;
        //왼쪽 차일드만 있을때
        else if (index * 2 == pq.size_data)
            return index * 2;
        //왼쪽, 오른쪽 둘 다 있을때 큰거 반환
        else
        {
            //왼쪽이 더 클때
            if (pq.array_num[index * 2] > pq.array_num[index * 2 + 1])
                return index * 2;
            //오른쪽이 더 클때
            else
                return index * 2 + 1;
        }
    }

    //우선순위큐(heap-배열 구현) 삭제
    void Pq_Insert(ref Priority_Queue pq, int data)
    {
        int index = pq.size_data + 1;

        while (index != 1)
        {
            if (Compare_Pq_Value(data, pq.array_num[index / 2]) > 0)
            {
                pq.array_num[index] = pq.array_num[index / 2];
                index /= 2;
            }
            else
            {
                break;
            }
        }

        pq.array_num[index] = data;
        pq.size_data += 1;
    }

    //우선순위큐(heap-배열 구현) 삭제
    void Pq_Delete(ref Priority_Queue pq)
    {
        int last_node_value = pq.array_num[pq.size_data];

        int pnode = 1;
        int cnode = FindChildValue(ref pq, pnode);

        while(true)
        {
            if(last_node_value >= pq.array_num[cnode])
                break;

            pq.array_num[pnode] = pq.array_num[cnode];            
            pnode = cnode;
            cnode = FindChildValue(ref pq, pnode);
        }

        pq.array_num[pnode] = last_node_value;
        pq.array_num[pq.size_data] = 0;
        pq.size_data--;
    }

    //바닥 만들기
    public void Make_Ground()
    {
        check_make_state = true;

        if (value_x > 100)
            value_x = 100;
        if (value_y > 100)
            value_y = 100;

        float vx = 0f;
        float vy = 0f;
         
        //y축 생성 | (0,0)기준 왼쪽
        for (int i = 0; i < value_y; i++)
        {
            hmp.px = vx;
            hmp.py = vy;
            Add_New_Block(hmp.px, hmp.py, 0, i);
            Pq_Insert(ref pq_x, 1);
            Pq_Insert(ref pq_y, i + 1);

            //x축 생성 | (0,0)기준 오른쪽
            float qx = 0.5f;
            float qy = 0.25f;

            //하나 생성된 위치에서 우측상단으로 -1갯수만큼 생성
            for (int j = 1; j < value_x; j++)
            {
                hmp.px = vx + qx;
                hmp.py = vy + qy;
                Add_New_Block(hmp.px, hmp.py, j, i);
                Pq_Insert(ref pq_x, j + 1);
                Pq_Insert(ref pq_y, i + 1);
                qx += 0.5f;
                qy += 0.25f;
            }            
            vx -= 0.5f;
            vy += 0.25f;
        }
        //중복제거 && ReSize
        //DelDuplication_ReSize(ref pq_x);
        //DelDuplication_ReSize(ref pq_y);

        check_make_state = false;
    }

    //그리드 그리기
    public void OnDraw_Grid()
    {
        int repeat_x = pq_del_x.size_data;
        for (int i = 0; i < repeat_x; i++)
        {
            if (pq_del_x.array_num[1] == pq_x.array_num[1])
            {
                Pq_Delete(ref pq_x);
                Pq_Delete(ref pq_del_x);
            }
            else
                break;
        }
        int repeat_y = pq_del_y.size_data;
        for (int i=0; i< repeat_y; i++)
        {
            if (pq_del_y.array_num[1] == pq_y.array_num[1])
            {
                Pq_Delete(ref pq_y);
                Pq_Delete(ref pq_del_y);
            }
            else
                break;
        }        

        GameObject[] obj_grid_left = new GameObject[pq_x.array_num[1] + 2];
        GameObject[] obj_grid_right = new GameObject[pq_y.array_num[1] + 2];

        //왼쪽 -> \ (칸수 길이 조절)
        float startPos_x = 0;
        float startPos_y = 0;
        float endPos_x = -0.5f * (pq_y.array_num[1] - 1);
        float endPos_y = 0.25f * (pq_y.array_num[1] - 1);

        //그리는 갯수 조절
        for (int i = 0; i < pq_x.array_num[1] + 1; i++)
        {
            obj_grid_left[i] = Instantiate(obj_line, new Vector2(0, 0), Quaternion.identity);
            obj_grid_left[i].transform.parent = GameObject.Find("Base_Grid").transform;
            obj_grid_left[i].GetComponent<LineRenderer>().SetPosition(0, new Vector2(startPos_x, startPos_y - 0.2f));
            obj_grid_left[i].GetComponent<LineRenderer>().SetPosition(1, new Vector2(endPos_x - 0.5f, endPos_y + 0.05f));
            startPos_x += 0.5f;
            startPos_y += 0.25f;
            endPos_x += 0.5f;
            endPos_y += 0.25f;
        }

        //오른쪽 -> / (칸수 길이 조절)
        startPos_x = 0;
        startPos_y = 0;
        endPos_x = 0.5f * (pq_x.array_num[1] - 1);
        endPos_y = 0.25f * (pq_x.array_num[1] - 1);

        //그리는 갯수 조절
        for (int i = 0; i < pq_y.array_num[1] + 1; i++)
        {
            obj_grid_right[i] = Instantiate(obj_line, new Vector2(0, 0), Quaternion.identity);
            obj_grid_right[i].transform.parent = GameObject.Find("Base_Grid").transform;
            obj_grid_right[i].GetComponent<LineRenderer>().SetPosition(0, new Vector2(startPos_x, startPos_y - 0.2f));
            obj_grid_right[i].GetComponent<LineRenderer>().SetPosition(1, new Vector2(endPos_x + 0.5f, endPos_y + 0.05f));
            startPos_x -= 0.5f;
            startPos_y += 0.25f;
            endPos_x -= 0.5f;
            endPos_y += 0.25f;
        }
    }

    //입력값으로 바닥 자동생성
    public void Add_New_Block(float hit_block_x, float hit_block_y, int map_num_x, int map_num_y)
    {
        hmp.px = hit_block_x;
        hmp.py = hit_block_y;

        map_num.nx = map_num_x;
        map_num.ny = map_num_y;

        if (!Check_Key(map_num.nx, map_num.ny))
        {
            for_ins_obj = Instantiate(obj_tile, new Vector2(hit_block_x, hit_block_y), Quaternion.identity);
            for_ins_obj.transform.parent = GameObject.Find("Base_Ground").transform;
            for_ins_obj.GetComponent<Ground_Data>().Set_Block_Number(map_num_x, map_num_y);

            map_info.Add(map_num, hmp);
        }
    }

    //마우스 클릭으로 바닥 추가생성
    //매개변수(오리지널 블록의 월드좌표 x,y / 오리지널 블록의 표시좌표 x,y / 새로 생성될 블록의 표시좌표 x,y)
    public void Add_New_Block(float hit_block_x, float hit_block_y, float hit_block_origin_x_num, float hit_block_origin_y_num, int hit_block_num_x, int hit_block_num_y)
    {
        hmp.px = hit_block_x;
        hmp.py = hit_block_y;

        map_num.nx = hit_block_num_x;
        map_num.ny = hit_block_num_y;

        //중복키 확인후 생성
        if (!Check_Key(map_num.nx, map_num.ny))
        {
            for_ins_obj = Instantiate(obj_tile, new Vector2(hit_block_x, hit_block_y), Quaternion.identity);
            for_ins_obj.transform.parent = GameObject.Find("Base_Ground").transform;
            for_ins_obj.GetComponent<Ground_Data>().Set_Block_Number(map_num.nx, map_num.ny);

            map_info.Add(map_num, hmp);

            //x좌표쪽으로 새로 생성됬을때
            if (hit_block_num_x  > 0)
                Pq_Insert(ref pq_x, hit_block_num_x + 1);
            //y좌표쪽으로 새로 생성됬을때
            if (hit_block_num_y  > 0)
                Pq_Insert(ref pq_y, hit_block_num_y + 1);

            //DelDuplication_ReSize(ref pq_x);
            //DelDuplication_ReSize(ref pq_y);
        }
    }

    //블럭 제거
    public void Remove_Block(int a, int b)
    {
        map_num.nx = a;
        map_num.ny = b;

        //왼쪽 상단
        if (map_num.nx >= 0)
        {
            if (Array.IndexOf(pq_y.array_num, b + 1) >= 0)
            {
                Pq_Insert(ref pq_del_y, b + 1);            
            }
                //BFSFindValue(ref pq_y, b + 1);
        }
        //오른쪽 상단
        if (map_num.ny >= 0)
        {
            if (Array.IndexOf(pq_x.array_num, a + 1) >= 0)
            {
                Pq_Insert(ref pq_del_x, a + 1);
            }
                //BFSFindValue(ref pq_x, a + 1);            
        }
        map_info.Remove(map_num);
        Debug.Log("HELLO WORLD");
    }

    public void ReSet_Value()
    {
        Array.Clear(pq_x.array_num, 0, 101);
        pq_x.size_data = 0;
        Array.Clear(pq_del_x.array_num, 0, 101);
        pq_del_x.size_data = 0;

        Array.Clear(pq_y.array_num, 0, 101);
        pq_y.size_data = 0;
        Array.Clear(pq_del_y.array_num, 0, 101);
        pq_del_y.size_data = 0;
    }

    public bool Check_Key(int a, int b)
    {
        map_num.nx = a;
        map_num.ny = b;

        if (map_info.ContainsKey(map_num))
            return true;
        else
            return false;
    }

    public void Set_Limit_Grid(int a, int b)
    {
        value_x = a;
        value_y = b;
    }

    void DelDuplication_ReSize(ref Priority_Queue pq)
    {
        pq.array_num = pq.array_num.Distinct().ToArray();
        Array.Resize(ref pq.array_num, pq.array_num.Length + (101 - pq.array_num.Length));
    }
}
