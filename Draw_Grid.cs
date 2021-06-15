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

    public Dictionary<Map_Num, Map_Pos> map_info = new Dictionary<Map_Num, Map_Pos>();
    public Map_Pos hmp;
    public Map_Pos origin_mp;
    public Map_Num map_num;
    public GameObject for_ins_obj;

    public GameObject obj_tile;
    public GameObject obj_line;
    public GameObject[] obj_grid_left;
    public GameObject[] obj_grid_right;
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

    void Pq_Delete(ref Priority_Queue pq)
    {
        int root_node_value = pq.array_num[1];
        int last_node_value = pq.array_num[pq.size_data];

        int pnode = 1;
        int cnode;

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
            Pq_Insert(ref pq_x, i + 1);

            //x축 생성 | (0,0)기준 오른쪽
            float qx = 0.5f;
            float qy = 0.25f;

            for (int j = 1; j < value_x; j++)
            {
                hmp.px = vx + qx;
                hmp.py = vy + qy;
                Add_New_Block(hmp.px, hmp.py, j, i);
                Pq_Insert(ref pq_y, j);
                qx += 0.5f;
                qy += 0.25f;
            }
            vx -= 0.5f;
            vy += 0.25f;
        }
        //중복제거 && ReSize
        DelDuplication_ReSize(ref pq_x);
        DelDuplication_ReSize(ref pq_y);

        check_make_state = false;
    }

    //그리드 그리기
    public void OnDraw_Grid()
    {
        obj_grid_left = new GameObject[value_x + 1];
        obj_grid_right = new GameObject[value_y + 1];

        //왼쪽상단사선
        float startPos_x = 0;
        float startPos_y = 0;
        float endPos_x = -0.5f * (value_y - 1);
        float endPos_y = 0.25f * (value_y - 1);

        for (int i = 0; i < value_x + 1; i++)
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

        //오른쪽상단사선
        startPos_x = 0;
        startPos_y = 0;
        endPos_x = 0.5f * (value_x - 1);
        endPos_y = 0.25f * (value_x - 1);

        for (int i = 0; i < value_y + 1; i++)
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
    public void Add_New_Block(float hit_block_x, float hit_block_y, float hit_block_origin_x, float hit_block_origin_y, int hit_block_num_x, int hit_block_num_y)
    {
        hmp.px = hit_block_x;
        hmp.py = hit_block_y;

        origin_mp.px = hit_block_origin_x;
        origin_mp.py = hit_block_origin_y;

        ////좌측하단
        ////x - 1, y == y
        //if (origin_mp.px - hmp.px > 0 && origin_mp.py - hmp.py > 0)
        //{
        //    map_num.nx = hit_block_num_x - 1;
        //    map_num.ny = hit_block_num_y;
        //}
        ////우측상단
        ////x + 1, y == y
        //else if (origin_mp.px - hmp.px < 0 && origin_mp.py - hmp.py < 0)
        //{
        //    map_num.nx = hit_block_num_x + 1;
        //    map_num.ny = hit_block_num_y;
        //}
        ////우측하단
        ////x == x, y - 1
        //else if (origin_mp.px - hmp.px < 0 && origin_mp.py - hmp.py > 0)
        //{
        //    map_num.nx = hit_block_num_x;
        //    map_num.ny = hit_block_num_y - 1;
        //}
        ////좌측상단
        ////x == x, y + 1
        //else if (origin_mp.px - hmp.px > 0 && origin_mp.py - hmp.py < 0)
        //{
        //    map_num.nx = hit_block_num_x;
        //    map_num.ny = hit_block_num_y + 1;
        //}

        map_num.nx = hit_block_num_x;
        map_num.ny = hit_block_num_y;

        if (map_num.nx >= value_x)
        {
            value_x = map_num.nx + 1;
            //max_x.Push(value_x);
        }
        if (map_num.ny >= value_y)
        {
            value_y = map_num.ny + 1;
            //max_y.Push(value_y);
        }

        //중복키 확인후 생성
        if (!Check_Key(map_num.nx, map_num.ny))
        {
            for_ins_obj = Instantiate(obj_tile, new Vector2(hit_block_x, hit_block_y), Quaternion.identity);
            for_ins_obj.transform.parent = GameObject.Find("Base_Ground").transform;
            for_ins_obj.GetComponent<Ground_Data>().Set_Block_Number(map_num.nx, map_num.ny);

            map_info.Add(map_num, hmp);

            //그리드 최신화값 입력
            Pq_Insert(ref pq_x, map_num.nx);
            Pq_Insert(ref pq_y, map_num.ny);
            DelDuplication_ReSize(ref pq_x);
            DelDuplication_ReSize(ref pq_y);
        }
    }

    public void Remove_Block(int a, int b)
    {
        map_num.nx = a;
        map_num.ny = b;

        map_info.Remove(map_num);
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
