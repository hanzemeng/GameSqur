using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class CallDll : MonoBehaviour
{
    [DllImport("DLL.dll", EntryPoint = "Add")]
    static extern float Add(float a, float b);

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(Add(3, 4));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
