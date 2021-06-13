using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_Trigger_Check : MonoBehaviour
{
    public bool trigger_check = false;
    public Vector3 block_pos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        trigger_check = true;
        block_pos = collision.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        trigger_check = true;
        block_pos = collision.transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        trigger_check = false;
        block_pos = Vector3.zero;
    }
}
