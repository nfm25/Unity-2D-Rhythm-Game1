using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{
    public int noteType;
    private GameManager.judges judge;
    private KeyCode keyCode;

    void Start()
    {
        if (noteType == 1) keyCode = KeyCode.D; //노트타입이 1(즉, 1번라인)이라면..
        else if (noteType == 2) keyCode = KeyCode.F;
        else if (noteType == 3) keyCode = KeyCode.J;
        else if (noteType == 4) keyCode = KeyCode.K;
    }

    public void Initialize()  //오브젝트 풀링 기법을 이용시 추가적인 초기화가 필요할 수 있으므로
    {
        judge = GameManager.judges.NONE; // 맨처음에 노트오브젝트가 초기화 되었을때, 판정값으로 NONE값을 가지고 있도록 해줘야한다.
        // 왜냐하면 새로 생성된 노트가 기존의 판정값을 가지고 있으면 노트가 아무행동없이도 여러가지 판정값(굿, 퍼펙 등의..)을 가지게 되므로..
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * GameManager.instance.noteSpeed);
        // 사용자가 노트 키를 입력한 경우
        if(Input.GetKey(keyCode))
        {
            //해당 노트에 대한 판정을 진행합니다.
            Debug.Log(judge);

            //노트가 판정 선에 닿기 시작한 이후로는 노트를 제거해준다.
            if (judge != GameManager.judges.NONE) gameObject.SetActive(false); // 노트가 판정선에 닿지 않은 상태가 아니라면, 즉..
                                                                       // 노트가 판정선에 닿았다면 노트를 제거함.
        }
    }
    //  각 노트의 현재 위치에 대하여 판정을 수행해주자!
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bad Line")
        {
            judge = GameManager.judges.BAD;
        }
        else if (other.gameObject.tag == "Good Line")
        {
            judge = GameManager.judges.GOOD;
        }
        else if (other.gameObject.tag == "Perfect Line")
        {
            judge = GameManager.judges.PERFECT;
        }
        else if (other.gameObject.tag == "Miss Line")
        {
            judge = GameManager.judges.MISS;
            gameObject.SetActive(false); // miss 판정나면 바로 노트가 삭제되도록..
        }
        
    }
}
