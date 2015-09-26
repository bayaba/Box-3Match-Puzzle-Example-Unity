using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour
{
	void Update()
	{
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -7f;
            transform.position = pos;
        }
	}
}
