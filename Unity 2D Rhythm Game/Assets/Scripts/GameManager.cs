using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //전체 게임내에서 공통적으로 사용되는 데이터를 담고 있는 스크립트는 GameManager를 싱글톤처리가 효과적이다.
    public static GameManager instance { get; set; }
    private void Awake()
    {
        if (instance == null) instance = this; // instance가 초기화되지 않았다면,즉 null값을 가진다면 instance를 자기자신으로 설정하여 
                                               //자기자신의 오브젝트를 instance 변수에 넣어줌으로써 intance 변수를 초기화해줄 수 있다. 
        else if (instance != this) Destroy(gameObject); //instance가 현재의 GameManager를 의미하는 값이 아니라면 게임오브젝트를 파괴해줌으로써
                                                        //만일의 오류 발생을 방지해준다.
    }

    public float noteSpeed; // 노트스피드는 유니티 인스펙터 창에서 0.05로 값을 입력해주었다.

    public enum judges {  NONE = 0, BAD, GOOD, PERFECT, MISS}; // enum자료형을 사용했으므로, 자동으로 none=0, bad= 1, good= 2, perfect =3, miss =4)
    // enum 자료형은 문자열(good, bad, perfect등..)과 같은 특정 변수에 숫자를 매칭하여, 그 숫자들로 관리할 수 있게한다.


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
