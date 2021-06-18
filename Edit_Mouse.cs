using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_Mouse : MonoBehaviour
{
    public struct Map_Pos
    {
        public float px;
        public float py;
    }

    public bool edit_state = false;

    Vector2 MousePosition;
    Camera Cam;
    GameObject edit_pivot;
    GameObject[] edit_connect = new GameObject[4];
    int edit_connect_num = 50;

    void Start()
    {
        Cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        edit_pivot = GameObject.Find("Edit_Add");

        edit_connect[0] = GameObject.Find("edit_right_top");
        edit_connect[1] = GameObject.Find("edit_right_bottom");
        edit_connect[2] = GameObject.Find("edit_left_top");
        edit_connect[3] = GameObject.Find("edit_left_bottom");
    }

    void Update()
    {
        if (edit_state)
        {
            GameObject get_block_info = GameObject.Find("Draw_Grid");
            int mx, my = 0;

            MousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
            edit_pivot.transform.position = new Vector3(MousePosition.x, MousePosition.y, 0);

            //우클릭 블럭 제거
            if (Input.GetMouseButton(1))
            {
                Ray2D ray = new Ray2D(MousePosition, Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 0.3f);
                if (hit.collider != null && hit.collider.tag != "edit_block")
                {
                    mx = hit.collider.GetComponent<Ground_Data>().block_num.nx;
                    my = hit.collider.GetComponent<Ground_Data>().block_num.ny;
                    get_block_info.GetComponent<Draw_Grid>().Remove_Block(mx, my);
                    Destroy(hit.transform.gameObject);
                }
            }

            float limit_x = (get_block_info.GetComponent<Draw_Grid>().value_x + 1) * 0.5f;
            float limit_y = (get_block_info.GetComponent<Draw_Grid>().value_y + 1) * 0.25f;
            //좌클릭 블럭 생성
            if (Input.GetMouseButton(0))
            {
                //어느 블록이랑 충돌했는지 확인
                if (edit_connect[0].GetComponent<Edit_Trigger_Check>().trigger_check)
                    edit_connect_num = 0;
                else if (edit_connect[1].GetComponent<Edit_Trigger_Check>().trigger_check)
                    edit_connect_num = 1;
                else if (edit_connect[2].GetComponent<Edit_Trigger_Check>().trigger_check)
                    edit_connect_num = 2;
                else if (edit_connect[3].GetComponent<Edit_Trigger_Check>().trigger_check)
                    edit_connect_num = 3;
                else
                    edit_connect_num = 50;

                //충돌 블럭 정보 받아오기
                Vector3 edit_new_block;
                float px, py = 0;
                int numx, numy = 0;

                //구분해서 생성
                switch (edit_connect_num)
                {
                    //right top
                    case 0:
                        edit_new_block = edit_connect[0].GetComponent<Edit_Trigger_Check>().block_pos;
                        numx = edit_connect[0].GetComponent<Edit_Trigger_Check>().block_num.nx - 1;
                        numy = edit_connect[0].GetComponent<Edit_Trigger_Check>().block_num.ny;
                        px = edit_new_block.x - 0.5f;
                        py = edit_new_block.y - 0.25f;

                        if (!get_block_info.GetComponent<Draw_Grid>().Check_Key(numx, numy) && numx >= 0 && numy >= 0)
                            get_block_info.GetComponent<Draw_Grid>().Add_New_Block(px, py, numx + 1, numy, numx, numy);
                        break;
                    //right bottom
                    case 1:
                        edit_new_block = edit_connect[1].GetComponent<Edit_Trigger_Check>().block_pos;
                        numx = edit_connect[1].GetComponent<Edit_Trigger_Check>().block_num.nx;
                        numy = edit_connect[1].GetComponent<Edit_Trigger_Check>().block_num.ny + 1;
                        px = edit_new_block.x - 0.5f;
                        py = edit_new_block.y + 0.25f;

                        if (!get_block_info.GetComponent<Draw_Grid>().Check_Key(numx, numy) && numx >= 0 && numy >= 0)
                            get_block_info.GetComponent<Draw_Grid>().Add_New_Block(px, py, numx, numy - 1, numx, numy);
                        break;
                    //left top
                    case 2:
                        edit_new_block = edit_connect[2].GetComponent<Edit_Trigger_Check>().block_pos;
                        numx = edit_connect[2].GetComponent<Edit_Trigger_Check>().block_num.nx;
                        numy = edit_connect[2].GetComponent<Edit_Trigger_Check>().block_num.ny - 1;
                        px = edit_new_block.x + 0.5f;
                        py = edit_new_block.y - 0.25f;

                        if (!get_block_info.GetComponent<Draw_Grid>().Check_Key(numx, numy) && numx >= 0 && numy >= 0)
                            get_block_info.GetComponent<Draw_Grid>().Add_New_Block(px, py, numx, numy+1, numx, numy);
                        break;
                    //left bottom
                    case 3:
                        edit_new_block = edit_connect[3].GetComponent<Edit_Trigger_Check>().block_pos;
                        numx = edit_connect[3].GetComponent<Edit_Trigger_Check>().block_num.nx + 1;
                        numy = edit_connect[3].GetComponent<Edit_Trigger_Check>().block_num.ny;
                        px = edit_new_block.x + 0.5f;
                        py = edit_new_block.y + 0.25f;

                        if (!get_block_info.GetComponent<Draw_Grid>().Check_Key(numx, numy) && numx >= 0 && numy >= 0)
                            get_block_info.GetComponent<Draw_Grid>().Add_New_Block(px, py, numx - 1, numy, numx, numy);
                        break;
                }
            }
        }
        else
        {
            edit_pivot.transform.position = new Vector3(0, 0, -10);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
