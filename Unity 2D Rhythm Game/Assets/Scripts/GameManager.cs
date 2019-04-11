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

    public float noteSpeed;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
