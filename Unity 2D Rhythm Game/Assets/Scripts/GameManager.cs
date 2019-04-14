using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Resources를 사용하기 위해 UI라이브러리를 추가해준다.

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

    // 실제로 콤보, 점수, 판정 UI들을 화면에 보여줄 수 있도록 하자.
    public GameObject scoreUI; // 화면에 존재하는 scoreUI를 매개변수로 건네받아서 처리하도록 한다.
    private float score;  // 실제로 게임내에서 사용되는 score같은 경우는 내부적으로 처리돼야하므로, private
    private Text scoreText; // scoreText를 화면에 보여주기 위해 Text 객체를 하나 만들어준다.

    public GameObject comboUI;
    private int combo;
    private Text comboText;
    private Animator comboAnimator;

    public enum judges { NONE = 0, BAD, GOOD, PERFECT, MISS }; // enum자료형을 사용했으므로, 자동으로 none=0, bad= 1, good= 2, perfect =3, miss =4)
    // enum 자료형은 문자열(good, bad, perfect등..)과 같은 특정 변수에 숫자를 매칭하여, 그 숫자들로 관리할 수 있게한다.
    public GameObject judgeUI; // 판정관련 UI까지 받아오도록 한다.
    private Sprite[] judgeSprites; // judge같은 경우에는 스프라이트 이미지를 Resources폴더에서 바꿔가면서 받아오도록 한다.
    private Image judgementSpriteRenderer; // judgementSpriteRenderer 변수를 만들어서 Image랜더러를 활용해서 실제로 판정결과 이미지를 보여주도록 한다.
    private Animator judgementSpriteAnimator; // 판정UI 또한 애니메이션으로 보여주도록 한다.

    public GameObject[] trails;
    // 버튼을 누르고 있는지 알수 있도록 만들어준 trail(길, 가이드)이미지 오브젝트를 처리할 수 있도록 하기위해 trails라는 변수를 만들어줌
    private SpriteRenderer[] trailSpriteRenderers;
    // 각각의 trails 변수들의 SpriteRenderer에 접근해서 그 투명도의 값을 변경해주기 위해 trailSpriteRenderers를 통해 처리할 수 있도록 만들어 줌

    //음악 변수를 만들어주자
    private AudioSource audioSource;
    private string music = "Drops of H20"; // "음악파일이름"

    //음악을 실행하는 함수를 만들자
    void MusicStart()
    {
        // 리소스에서 비트(Beat) 음악 파일을 불러와 재생한다.
        AudioClip audioClip = Resources.Load<AudioClip>("Beats/" + music);
        audioSource = GetComponent<AudioSource>(); // GameManager에 있는 AudioSource 컴포넌트를 받아와서
        audioSource.clip = audioClip;
        audioSource.Play();
    }


    void Start()
    {
        Invoke("MusicStart", 2); // 게임실행 후 2초 뒤에 재생하도록..
        judgementSpriteRenderer = judgeUI.GetComponent<Image>();
        //judgementSpriteRenderer 경우에는 judgeUI에서 Image 컴포넌트를 받아와서 초기화
        judgementSpriteAnimator = judgeUI.GetComponent<Animator>();
        scoreText = scoreUI.GetComponent<Text>();
        comboText = comboUI.GetComponent<Text>();
        comboAnimator = comboUI.GetComponent<Animator>();

        // 판정 결과를 보여주는 스프라이트 이미지를 미리 초기화하자.
        judgeSprites = new Sprite[4]; // 판정 종류가 총 4가지이므로, 각각 4가지의 스프라이트가 들어갈 수 있도록 배열 공간을 만들어준다.
        judgeSprites[0] = Resources.Load<Sprite>("Sprites/Bad"); // Resource.Load 함수로 Resources폴더내의 Spirtes폴더의 Bad 이미지를 불러온다.
        judgeSprites[1] = Resources.Load<Sprite>("Sprites/Good");
        judgeSprites[2] = Resources.Load<Sprite>("Sprites/Miss");
        judgeSprites[3] = Resources.Load<Sprite>("Sprites/Perfect");


        trailSpriteRenderers = new SpriteRenderer[trails.Length]; //trailSpriteRenderers를 초기화 즉, 전체 노트가 떨어지는 길의 갯수, 즉 4개만큼 초기화한다.
        for (int i = 0; i < trails.Length; i++) // 각각의 trailSpriteRenderers에 접근해서, 실질적으로 각각의 trail에 포함되어 있는 SpriteRenderer를 넣어줄 수 있도록 한다.
        {
            trailSpriteRenderers[i] = trails[i].GetComponent<SpriteRenderer>();
            //[i] 인덱스의 trailSpriteRenderers는 해당 trail에서 가져온 SpriteRenderer 값이 될 수 있도록 한다.
        }
    }

    // Update is called once per frame
    void Update()
    {
        //사용자가 입력한 키에 해당하는 라인을 빛나게 처리하자
        if (Input.GetKey(KeyCode.D)) ShineTrail(0);
        if (Input.GetKey(KeyCode.F)) ShineTrail(1);
        if (Input.GetKey(KeyCode.J)) ShineTrail(2);
        if (Input.GetKey(KeyCode.K)) ShineTrail(3);

        //한번 빛나게 된 라인은 반복적으로 다시 어둡게 처리하도록 하자.
        for(int i = 0; i < trailSpriteRenderers.Length; i++) // 모든 trailSpriteRenderers에 접근해서
        {
            Color color = trailSpriteRenderers[i].color;
            color.a -= 0.01f; // 매프레임마다 -0.01만큼 알파값을 투명하게 처리하여 다시 어두워지게 만든다.
            trailSpriteRenderers[i].color = color; //  각각[i]번째 trailSpriteRenderers에 접근할 수 있도록 한다.
        }
    }

    //특정한 키를 눌러 해당 라인을 빛나게 처리하자
    public void ShineTrail(int index) // ShineTrail이 몇번째 라인이 빛나게 할지 받을 수 있도록한다.
    {
        Color color = trailSpriteRenderers[index].color;
        color.a = 0.32f; // 해당 인덱스의 SpriteRenderer에 포함돼있는 색상값의 알파값을 0.32로 설정해서 약한 하얀색을 띠도록 한다.
        trailSpriteRenderers[index].color = color; //trailSpriteRenderers를 그 변경된 컬러값을 그대로 적용할 수 있게 한다. 
    }

    //노트 판정 이후에 판정 결과를 화면에 보여주자.
    void showJudgement()
    {
        //점수 이미지를 보여주자
        string scoreFormat = "000000"; // 점수는 항상 6자리로 보여줄 수 있도록 만들어준다.
        scoreText.text = score.ToString(scoreFormat); // 현재 사용자의 점수를 ToString함수를 이용해 문자열형태로 
                                                      // 만들어준 뒤 scoreFormat, 즉 6자리로 만들어 보여줄 수 있도록 한다.
       
        //판정 이미지를 보여주도록 하자
        judgementSpriteAnimator.SetTrigger("Show"); // Show 트리거를 불러올수 있도록 한다. 
        //(판정애니매이션이 Show트리거에 의해서 화면에 보여줄 수 있게 만들어 놓았기 때문)

        // 콤보가 2이상일때만 콤보 이미지를 보여줄 수 있도록 하자.
        if(combo >= 2)
        {
            comboText.text = "COMBO" + combo.ToString();
            comboAnimator.SetTrigger("Show");
        }
    }
    // 노트 판정을 진행하자
    public void processJudge(judges judge, int noteType) // 실제로 몇번째 노트를 맞춰서 어떠한 판정을 받았다면 
                                                      // 해당 정보를 기준으로해서 판정을 수행할 수 있도록 한다.
    {
        if (judge == judges.NONE) return; // 판정 결과가 NONE라면 그냥 무시하자

        if (judge == judges.MISS) // 미스 판정을 받으면 콤보를 종료하고, 점수를 많이 깎도록 하자
        {
            judgementSpriteRenderer.sprite = judgeSprites[2]; // 위에서 judgeSprites[2]의 경우는 MISS이다.
            combo = 0; // 콤보를 초기화하고..
            if (score >= 15) score -= 15; // 점수가 마이너스가 되면 안되므로, 점수가 15이상일때만 15를 뺄수 있도록 한다.
        }
        // BAD 판정을 받은 경우 콤보를 종료하고 ,점수를 조금 깎도록하자
        else if(judge == judges.BAD)
        {
            judgementSpriteRenderer.sprite = judgeSprites[0];
            combo = 0;
            if (score >= 5) score -= 5;
        }
        // PERFECT 혹은 GOOD 판정을 받은 경우 콤보 및 점수를 올리도록 해주자
        else
        {
            if(judge == judges.PERFECT)
            {
                judgementSpriteRenderer.sprite = judgeSprites[3];
                score += 20; // score값을 +20
            }
            else if (judge == judges.GOOD)
            {
                judgementSpriteRenderer.sprite = judgeSprites[1];
                score += 15;
            }
            combo += 1;
            score += (float)combo * 0.1f; // 현재 콤보의 10% 가산점 또한 주도록 한다. ex)100콤보일 시 10점 가산점 부여
        }
        showJudgement();
    }

}
