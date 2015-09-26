using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PuzzleManager : MonoBehaviour
{
    public GameObject ColorBox;

    public ArrayList[] Block = new ArrayList[6]; // arraylist for puzzle blocks

    public GameObject TouchedColorBox = null;
    public bool isMoving = false;

	// random angle for puzzle block
    Quaternion[] Angle = {  Quaternion.Euler(0f, 0f, 0f),
                            Quaternion.Euler(90f, 0f, 0f),
                            Quaternion.Euler(180f, 0f, 0f),
                            Quaternion.Euler(270f, 0f, 0f),
                            Quaternion.Euler(0f, 90f, 0f),
                            Quaternion.Euler(0f, 270f, 0f) };


	void Start()
	{
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

        for (int i = 0; i < 6; i++)
        {
            Block[i] = new ArrayList();
        }

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Block[x].Add(CreateRandomColorBox(x, new Vector3(-2f + (x * 1.01f), 7f + (y * 1.2f), 0f)));
            }
        }
	}

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (TouchedColorBox == null)
            {
                RaycastHit hit;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Physics.Raycast(pos, Vector3.forward, out hit, 100f))
                {
                    TouchedColorBox = hit.collider.gameObject; // save touched object
                }
            }
        }
        else if (TouchedColorBox != null)
        {
            if (!DOTween.IsTweening(TouchedColorBox)) // is sill moving
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float xx = Mathf.Abs(pos.x - TouchedColorBox.transform.position.x); // horizontal distance
                float yy = Mathf.Abs(pos.y - TouchedColorBox.transform.position.y); // vertical distance

                if (xx > yy && xx >= 0.1f)
                {
                    if (pos.x < TouchedColorBox.transform.position.x) // rotate left
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(0f, 90f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                    else if (pos.x > TouchedColorBox.transform.position.x) // rotate right
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(0f, -90f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                }
                else if (xx < yy && yy >= 0.1f)
                {
                    if (pos.y < TouchedColorBox.transform.position.y) // rotate down
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(-90f, 0f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                    else if (pos.y > TouchedColorBox.transform.position.y) // rotate up
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(90f, 0f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                }
            }
            TouchedColorBox = null;
        }
        DestroyMatchedBlock();
    }

    void DestroyMatchedBlock()
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                ColorBox target = ((Transform)Block[x][y]).GetComponent <ColorBox>();
                if (target.rigidbody.velocity.magnitude > 0.3f || target.isDead) return;
            }
        }

        // vertical matched block check
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ColorBox first = ((Transform)Block[x][y]).GetComponent<ColorBox>();
                ColorBox second = ((Transform)Block[x][y + 1]).GetComponent<ColorBox>();
                ColorBox third = ((Transform)Block[x][y + 2]).GetComponent<ColorBox>();

                if (first.FaceNumber() == second.FaceNumber() && second.FaceNumber() == third.FaceNumber())
                {
                    first.DestroyColorBox(0.2f, 0.4f);
                    second.DestroyColorBox(0.2f, 0.4f);
                    third.DestroyColorBox(0.2f, 0.4f);
                }
            }
        }

        // horizontal matched block check
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                ColorBox first = ((Transform)Block[x][y]).GetComponent<ColorBox>();
                ColorBox second = ((Transform)Block[x + 1][y]).GetComponent<ColorBox>();
                ColorBox third = ((Transform)Block[x + 2][y]).GetComponent<ColorBox>();

                if (first.FaceNumber() == second.FaceNumber() && second.FaceNumber() == third.FaceNumber())
                {
                    first.DestroyColorBox(0.2f, 0.4f);
                    second.DestroyColorBox(0.2f, 0.4f);
                    third.DestroyColorBox(0.2f, 0.4f);
                }
            }
        }
    }

    public Transform CreateRandomColorBox(int idx, Vector3 pos)
    {
        GameObject temp = Instantiate(ColorBox) as GameObject;
        temp.transform.parent = transform;

        temp.transform.localRotation = Angle[Random.Range(0, 6)];
        temp.GetComponent<ColorBox>().Index = idx;
        temp.transform.localPosition = pos;
        temp.name = "ColorBox";
        return temp.transform;
    }

    public void DeleteColorBox(GameObject ColorBox)
    {
        int x = ColorBox.GetComponent<ColorBox>().Index;
        Block[x].Remove(ColorBox.transform);
    }

    public void RebornColorBox(GameObject ColorBox)
    {
        int x = ColorBox.GetComponent<ColorBox>().Index;

        float y = Mathf.Max(5.0f, ((Transform)Block[x][Block[x].Count - 1]).transform.position.y + 2.0f);
        Block[x].Add(CreateRandomColorBox(x, new Vector3(-2f + (x * 1.01f), y, 0f)));
    }
}
