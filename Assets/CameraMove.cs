using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        Application.targetFrameRate = 24;
        QualitySettings.vSyncCount = 0;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Input.GetAxisRaw("Mouse X") != 0)
            {
                transform.Translate(-Vector3.right * Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed);
            }
            if (Input.GetAxisRaw("Mouse Y") != 0)
            {
                transform.Translate(-Vector3.up * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed);
            }
        }
    }
    public void Plus()
    {
        GetComponent<Camera>().orthographicSize++;
        if (GetComponent<Camera>().orthographicSize > 10)
        {
            GetComponent<Camera>().orthographicSize = 10;
        }
    }
    public void Min()
    {
        GetComponent<Camera>().orthographicSize--;
        if (GetComponent<Camera>().orthographicSize < 1)
        {
            GetComponent<Camera>().orthographicSize = 1;
        }
    }
}
