using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_Trigger_Check : MonoBehaviour
{
    public struct Map_Num
    {
        public int nx;
        public int ny;
    }
    
    public bool trigger_check = false;
    public Vector3 block_pos;
    public Map_Num block_num;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        trigger_check = true;
        block_pos = collision.transform.position;
        block_num.nx = collision.GetComponent<Ground_Data>().block_num.nx;
        block_num.ny = collision.GetComponent<Ground_Data>().block_num.ny;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        trigger_check = true;
        block_pos = collision.transform.position;
        block_num.nx = collision.GetComponent<Ground_Data>().block_num.nx;
        block_num.ny = collision.GetComponent<Ground_Data>().block_num.ny;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        trigger_check = false;
        block_pos = Vector3.zero;        
    }
}
