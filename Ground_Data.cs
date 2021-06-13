using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Data : MonoBehaviour
{
    public struct Map_Num
    {
        public int nx;
        public int ny;
    }
    public Map_Num block_num;

    void Start()
    {

    }


    void Update()
    {

    }

    public void Set_Block_Number(int x, int y)
    {
        block_num.nx = x;
        block_num.ny = y;
    }
}
