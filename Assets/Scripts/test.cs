using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{/*LIST = [1,4,3,3,2,5,1,2,5,7,7, ...] with size N.

We know LIST contains only duplicated numbers (numbers that appear twice) except ONE integer which is called The Solitary Integer.

Write an efficient algorithm that will determine this integer ?

Can you solve the exercise in O(N) ?*/
    // Start is called before the first frame update
    void Start()
    {
        int[] testData = { 1,  3, 3, 2, 5, 1, 2, 5, 4, 7,4};
        Debug.Log( FindSoitaryInt(testData));
    }

    int FindSoitaryInt(int[] data)
    {
        Array.Sort<int>(data);//[1,1,2,2,3,3,4,5,5,7,7, ...]
        for (int i = 0; i < data.Length; i++)
        {
            if (i <data.Length-1)
            {
                if (data[i] == data[i + 1])
                {
                    i++;// dont need to check the next integer
                    continue;
                }
                else
                {
                    return data[i];
                }

            }
            
        }
        return 0;        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
