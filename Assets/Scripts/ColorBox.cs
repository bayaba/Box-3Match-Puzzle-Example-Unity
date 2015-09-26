using UnityEngine;
using System.Collections;

public class ColorBox : MonoBehaviour
{
    public bool isDead = false;
    public int Index = 0;

    public GameObject KillEffect; // effect prefab for when kill this object

    PuzzleManager manager; // PuzzleManager

    Vector3[] sides = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f) };

    void Awake()
    {
        rigidbody.velocity = new Vector3(0f, -5f, 0f);
    }

    void Start()
	{
        manager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
	}

    public int FaceNumber() // return face number of dice
    {
        int result = -1;
        float max = float.NegativeInfinity;

        for (int i = 0; i < sides.Length; i++)
        {
            Vector3 pos = transform.TransformDirection(sides[i]);

            if (pos.z > max)
            {
                result = i + 1;
                max = pos.z;
            }

            if (-pos.z > max)
            {
                result = 6 - i;
                max = -pos.z;
            }
        }
        return result;
    }

    public void DestroyColorBox(float hideDelay, float removeDelay) // destroy color block
    {
        if (!isDead)
        {
            Invoke("Hide", hideDelay); // hide hideDelay seconds later
            Invoke("Remove", removeDelay); // remove removeDelay seconds later`
            isDead = true;
        }
    }

    void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false; // renderer disable but collider is still enabled
        Instantiate(KillEffect, transform.position, Quaternion.identity); // create kill effect
    }

    void Remove()
    {
        manager.DeleteColorBox(gameObject); // remove this object on block list in PuzzleManager
        manager.RebornColorBox(gameObject); // create new block on the top (same column)
        Destroy(gameObject);
    }
}
