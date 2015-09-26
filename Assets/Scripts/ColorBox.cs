using UnityEngine;
using System.Collections;

public class ColorBox : MonoBehaviour
{
    public bool isDead = false;
    public int Index = 0;

    public GameObject KillEffect; // 없어질때 이펙트 (유니티 무료 어셋)

    PuzzleManager manager; // PuzzleManager 객체

    Vector3[] sides = { new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f) };

    void Awake()
    {
        // 블럭의 생성 당시에는 벨로시티가 전부 0이기 때문에
        // 3매치가 된 블럭들이 착지 전에 삭제되는 문제가 생길 수 있다.
        // 그래서 y축의 벨로시티를 -5f로 설정해서 가속을 준다.
        rigidbody.velocity = new Vector3(0f, -5f, 0f);
    }

    void Start()
	{
        manager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
	}

    public int FaceNumber() // 화면쪽을 보고 있는 면의 번호를 리턴한다.
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

    public void DestroyColorBox(float hideDelay, float removeDelay) // 블럭을 삭제한다.
    {
        if (!isDead)
        {
            Invoke("Hide", hideDelay); // hideDelay초 이후에 블럭을 보이지 않게 하고 이펙트 생성
            Invoke("Remove", removeDelay); // removeDelay초 이후에 블럭을 완전히 삭제하고 새로 생성하도록 PuzzleManager 호출
            isDead = true;
        }
    }

    void Hide()
    {
        GetComponent<MeshRenderer>().enabled = false; // 렌더러를 off시켜 보이지 않게 한다. collider는 남아있다.
        Instantiate(KillEffect, transform.position, Quaternion.identity); // 없어질때 이펙트를 생성한다.
    }

    void Remove()
    {
        manager.DeleteColorBox(gameObject); // 리스트에서 현재 캐릭터를 삭제시킨다.
        manager.RebornColorBox(gameObject); // 새로운 캐릭터를 화면 위쪽에 생성한다.
        Destroy(gameObject); // 현재 캐릭터를 삭제한다.
    }
}
