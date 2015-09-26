using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PuzzleManager : MonoBehaviour
{
    public GameObject ColorBox;

    // 6X10 짜리 퍼즐 게임이므로 일단 가로 길이인 6개 만큼의 리스트 배열을 만든다.
    // 이 배열에 다시 어레이 리스트를 생성시켜 2차원 리스트 배열을 만들 것이다.
    public ArrayList[] Block = new ArrayList[6];

    public GameObject TouchedColorBox = null;
    public bool isMoving = false;

    // 블럭 생성시 랜덤하게 방향을 정하게 될 앵글 값 (정육면체이므로 6개의 앵글 값을 미리 만들어둔다.)
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
            Block[i] = new ArrayList(); // 이러면 Block[6][10] 사이즈의 2차원 리스트가 된다.
        }

        for (int x = 0; x < 6; x++) // 가로 개수
        {
            for (int y = 0; y < 10; y++) // Y 세로 개수
            {
                // 블럭를 생성시켜 각 컬럼에 10개씩 추가시킨다.
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
                    TouchedColorBox = hit.collider.gameObject; // 터치한 오브젝트를 저장해둔다.
                }
            }
        }
        else if (TouchedColorBox != null) // 마우스를 뗐을때
        {
            if (!DOTween.IsTweening(TouchedColorBox)) // 아직 회전중이면 처리하지 않음
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float xx = Mathf.Abs(pos.x - TouchedColorBox.transform.position.x); // 가로 거리
                float yy = Mathf.Abs(pos.y - TouchedColorBox.transform.position.y); // 세로 거리

                if (xx > yy && xx >= 0.1f) // 가로로 더 많이 이동했으면 가로로 회전
                {
                    if (pos.x < TouchedColorBox.transform.position.x) // 왼쪽으로 회전
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(0f, 90f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                    else if (pos.x > TouchedColorBox.transform.position.x) // 오른쪽으로 회전
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(0f, -90f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                }
                else if (xx < yy && yy >= 0.1f) // 세로로 더 많이 이동했으면 세로로 회전
                {
                    if (pos.y < TouchedColorBox.transform.position.y) // 아래쪽으로 회전
                    {
                        TouchedColorBox.transform.DORotate(new Vector3(-90f, 0f, 0f), 0.3f, RotateMode.WorldAxisAdd);
                    }
                    else if (pos.y > TouchedColorBox.transform.position.y) // 위쪽으로 회전
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
        // 모든 블럭를 체크해서, 움직이고 있거나, 터져서 사라지는 중이면 리턴시킨다.
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                ColorBox target = ((Transform)Block[x][y]).GetComponent <ColorBox>();
                if (target.rigidbody.velocity.magnitude > 0.3f || target.isDead) return;
            }
        }

        // 세로 라인의 3매치를 맨 아래쪽에서 위쪽으로 체크한다.
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++) // 동시에 3개씩을 체크하므로 세로 라인은 10이 아니라 8까지만 체크한다.
            {
                // 위쪽으로 3개의 블럭를 가져온다.
                ColorBox first = ((Transform)Block[x][y]).GetComponent<ColorBox>();
                ColorBox second = ((Transform)Block[x][y + 1]).GetComponent<ColorBox>();
                ColorBox third = ((Transform)Block[x][y + 2]).GetComponent<ColorBox>();

                // 3개의 localRotate가 서로 같으면 3개를 모두 터지도록 처리한다.
                if (first.FaceNumber() == second.FaceNumber() && second.FaceNumber() == third.FaceNumber())
                {
                    first.DestroyColorBox(0.2f, 0.4f);
                    second.DestroyColorBox(0.2f, 0.4f);
                    third.DestroyColorBox(0.2f, 0.4f);
                }
            }
        }

        // 가로 라인의 3매치를 좌측부터 우측으로 체크한다.
        for (int x = 0; x < 4; x++) // 동시에 3개씩 체크하므로 가로 가인은 6이 아니라 4개까지만 체크한다.
        {
            for (int y = 0; y < 10; y++)
            {
                // 오른쪽으로 3개의 블럭를 가져온다.
                ColorBox first = ((Transform)Block[x][y]).GetComponent<ColorBox>();
                ColorBox second = ((Transform)Block[x + 1][y]).GetComponent<ColorBox>();
                ColorBox third = ((Transform)Block[x + 2][y]).GetComponent<ColorBox>();

                // 3개의 ClipName이 서로 같으면 3개를 모두 터지도록 처리한다.
                if (first.FaceNumber() == second.FaceNumber() && second.FaceNumber() == third.FaceNumber())
                {
                    first.DestroyColorBox(0.2f, 0.4f);
                    second.DestroyColorBox(0.2f, 0.4f);
                    third.DestroyColorBox(0.2f, 0.4f);
                }
            }
        }
    }

    public Transform CreateRandomColorBox(int idx, Vector3 pos) // idx는 컬럼의 인덱스 번호이고, pos는 실제 생성 위치
    {
        GameObject temp = Instantiate(ColorBox) as GameObject;
        temp.transform.parent = transform; // 생성된 블럭를 PuzzleManager의 자식으로 넣는다.

        // 애니메이션 클립 이름을 char01 ~ 05까지 5종류로 생성한다. (클립은 6까지 있으므로 Range(1, 7)로도 가능하다)

        temp.transform.localRotation = Angle[Random.Range(0, 6)];
        temp.GetComponent<ColorBox>().Index = idx; // 컬럼 번호를 Index에 넣는다.
        temp.transform.localPosition = pos;
        temp.name = "ColorBox";
        return temp.transform;
    }

    public void DeleteColorBox(GameObject ColorBox) // 블럭를 Block에서 삭제한다.
    {
        int x = ColorBox.GetComponent<ColorBox>().Index; // 블럭가 속한 컬럼 번호를 가져온다.
        Block[x].Remove(ColorBox.transform); // 해당 컬럼에서 전달된 블럭를 찾아 삭제한다.
    }

    public void RebornColorBox(GameObject ColorBox) // 블럭를 Block에 다시 생성한다.
    {
        int x = ColorBox.GetComponent<ColorBox>().Index; // 블럭가 삭제된 위치의 컬럼 번호를 가져온다.

        // 화면 위쪽 5.0f부터 블럭를 생성하고, 이미 생성된 블럭가 있으면 마지막 블럭 좌표에서 2.0f 위쪽에 생성한다.
        float y = Mathf.Max(5.0f, ((Transform)Block[x][Block[x].Count - 1]).transform.position.y + 2.0f);

        // 해당 컬럼의 리스트에 새로운 블럭를 생성해서 넣는다.
        Block[x].Add(CreateRandomColorBox(x, new Vector3(-2f + (x * 1.01f), y, 0f)));
    }
}
