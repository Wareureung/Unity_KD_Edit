using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Grid : MonoBehaviour
{
    enum BLOCk_NAME
    { 
        BASE = 0
    }

    public struct Hash_Map_Pos
    {
        public float px;
        public float py;
    }

    public Dictionary<Hash_Map_Pos, int> hash_map_info = new Dictionary<Hash_Map_Pos, int>();
    public Hash_Map_Pos hmp;
    public GameObject for_ins_obj;

    public GameObject obj_tile;
    public GameObject obj_line;
    public GameObject[] obj_grid_left;
    public GameObject[] obj_grid_right;
    public int value_x;
    public int value_y;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //바닥 만들기
    public void Make_Ground()
    {
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
            Add_New_Block(hmp.px, hmp.py);

            //x축 생성 | (0,0)기준 오른쪽
            float qx = 0.5f;
            float qy = 0.25f;
            
            for (int j=0; j< value_x - 1; j++)
            {
                hmp.px = vx + qx;
                hmp.py = vy + qy;
                Add_New_Block(hmp.px, hmp.py);
                qx += 0.5f;
                qy += 0.25f;
            }
            vx -= 0.5f;
            vy += 0.25f;
        }
    }

    //그리드 그리기
    public void OnDraw_Grid()
    {
        obj_grid_left = new GameObject[value_x + 1];
        obj_grid_right = new GameObject[value_y + 1];

        //왼쪽사선
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

        //오른쪽사선
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

    public void Add_New_Block(float a, float b)
    {
        hmp.px = a;
        hmp.py = b;
        if (!Check_Value(hmp.px, hmp.py))
        {
            for_ins_obj = Instantiate(obj_tile, new Vector2(a, b), Quaternion.identity);
            for_ins_obj.transform.parent = GameObject.Find("Base_Ground").transform;
            
            hash_map_info.Add(hmp, 1);
        }
    }

    public void Remove_Block(float a, float b)
    {
        hmp.px = a;
        hmp.py = b;
        hash_map_info.Remove(hmp);
    }

    public bool Check_Value(float a, float b)
    {
        hmp.px = a;
        hmp.py = b;

        if (hash_map_info.ContainsKey(hmp))
            return true;
        else
            return false;
    }
}
