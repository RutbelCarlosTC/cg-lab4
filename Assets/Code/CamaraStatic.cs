using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraStatic : MonoBehaviour
{
    public List<Camera> cameras;
    public int index;
    public Camera Camera1;

    // Start is called before the first frame update
    void Start()
    {
        Camera1 = cameras[0];
        cameras[index].gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            index = index < cameras.Count ? index + 1 : 0;

            for(int i = 0;i < cameras.Count; i++)
            {
                cameras[i].gameObject.SetActive(i == index);
            }

            Camera1 = cameras[index];
        }
    }
}
