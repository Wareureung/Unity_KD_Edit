using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Map_Editor : EditorWindow
{ 
    private string value_x = "1";
    private string value_y = "1";
    private bool make_state = false;
    private bool make_grid = false;
    private GameObject obj_tile = null;
    private GameObject obj_line = null;
    private GameObject dg_obj;

    private string edit_state_name = "Start Edit";
    private bool edit_state_bool = false;

    

    [MenuItem("Window/My_Map_Editor")]
    public static void ShowWin()
    {
        EditorWindow.GetWindow(typeof(Map_Editor));
    }

    //Update와 같은 함수
    //그려낼것들을 입력    
    private void OnGUI()
    {
        value_x = EditorGUILayout.TextField("x : ", value_x);
        value_y = EditorGUILayout.TextField("y : ", value_y);

        obj_tile = (GameObject)EditorGUILayout.ObjectField("스프라이트 타일 : ", obj_tile, typeof(GameObject), true);
        obj_line = (GameObject)EditorGUILayout.ObjectField("그리드 라인 : ", obj_line, typeof(GameObject), true);

        //땅 만들기
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Ground"))
        {
            if(!make_state)
                SetGround();
        }
        if(GUILayout.Button("Delete Ground"))
        {
            if(make_state)
                ResetGround();
        }
        GUILayout.EndHorizontal();

        //그리드 만들기
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Draw Grid"))
        {
            if (make_state && !make_grid)
                Draw_Grid();
        }
        if(GUILayout.Button("Delete Grid"))
        {
            if (make_grid && make_state)
                Delete_Grid();
        }
        GUILayout.EndHorizontal();

        //다 지우기
        if (GUILayout.Button("Delete All"))
        {
            ResetGround();
            Delete_Grid();
        }

        //바닥 수정
        if (!edit_state_bool)
            GUI.backgroundColor = Color.blue;
        else
            GUI.backgroundColor = Color.red;

        if (GUILayout.Button(edit_state_name))
        {
            GameObject mouse_click = GameObject.Find("Base_Ground");            

            if (!edit_state_bool)
            {
                edit_state_bool = true;
                mouse_click.GetComponent<Edit_Mouse>().edit_state = edit_state_bool;
                edit_state_name = "End Edit";
            }
            else
            {
                edit_state_bool = false;
                mouse_click.GetComponent<Edit_Mouse>().edit_state = edit_state_bool;
                edit_state_name = "Start Edit";
            }


        }

    }

    private void SetGround()
    {
        make_state = true;

        dg_obj = GameObject.Find("Draw_Grid");
        dg_obj.GetComponent<Draw_Grid>().value_x = int.Parse(value_x);
        dg_obj.GetComponent<Draw_Grid>().value_y = int.Parse(value_y);
        dg_obj.GetComponent<Draw_Grid>().obj_tile = obj_tile;
        dg_obj.GetComponent<Draw_Grid>().Make_Ground();
    }

    private void ResetGround()
    {
        make_state = false;

        //바닥 지우기
        GameObject obj = GameObject.Find("Base_Ground");
        int obj_count = obj.gameObject.transform.childCount;

        for (int i = 0; i < obj_count; i++)
            DestroyImmediate(obj.transform.GetChild(0).gameObject);

        //데이터 지우기
        GameObject obj_info = GameObject.Find("Draw_Grid");
        obj_info.GetComponent<Draw_Grid>().hash_map_info.Clear();
    }

    private void Draw_Grid()
    {
        make_grid = true;

        dg_obj = GameObject.Find("Draw_Grid");
        dg_obj.GetComponent<Draw_Grid>().obj_line = obj_line;
        dg_obj.GetComponent<Draw_Grid>().OnDraw_Grid();
    }

    private void Delete_Grid()
    {
        make_grid = false;
        //그리드 지우기
        GameObject line = GameObject.Find("Base_Grid");
        int line_count = line.gameObject.transform.childCount;

        for (int i = 0; i < line_count; i++)
            DestroyImmediate(line.transform.GetChild(0).gameObject);
    }
}
