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
        else if (noteType == 1) keyCode = KeyCode.J;
        else if (noteType == 1) keyCode = KeyCode.K;
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
            if (judge != GameManager.judges.NONE) Destroy(gameObject); // 노트가 판정선에 닿지 않은 상태가 아니라면, 즉..
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
            Destroy(gameObject); // miss 판정나면 바로 노트가 삭제되도록..
        }
        
    }
}
