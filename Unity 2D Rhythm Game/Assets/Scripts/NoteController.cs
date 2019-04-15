using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    // 하나의 노트에 대한 정보를 담는 노트(Note) 클래스를 정의해주자
    class Note
    {
        public int noteType { get; set; } // 노트타입을 정할 수 있도록 선언해주고
        public int order { get; set; } // 특정한 노트의 떨어지는 순서를 결정할 수 있도록해주자
        public Note(int noteType, int order) // 노트타입과 순서를 결정하는 생성자를 만들어주고
        {
            this.noteType = noteType; // 해당 노트가 만들어질 때 그 노트의 타입이 무엇인지
            this.order = order; // 그 노트가 몇번째로 떨어지는지를 입력받아서 담을 수 있도록 한다.
        }
    }

    public GameObject[] Notes; //실제로 노트프리팹을 담아서 떨어뜨릴 수 있도록하기 위해서 
                               // 게임오브젝트로 Notes라는 변수를 만들어준다.그리고 리스트형태[](인덱스)로 담을 수 있도록 만들어준다.
    private ObjectPooler noteObjectPooler; // 오브젝트 풀링기법을 사용할 수 있도록 noteObjectPooler 변수를 만들어 준다.
    private List<Note> notes = new List<Note>();
    //실제로 떨어지게 될 노트들의 데이터를 담기위해 notes라는 리스트 변수를 만들어줌. 그리고 내부적으로만 사용되므로 private로 선언

    private float x, z, startY = 8.0f; // 비활성화 처리되어 있는 노트들은 아래쪽으로 많이 떨어져 있을테니, 해당 노트를 다시 위로 올려주면서 시작할 수 있도록
                                       // 즉, startY축을 무조건 8.0f로 고정시켜, 무조건 화면 위쪽에서 노트가 만들어져서 활성화처리되어 아래쪽으로 떨어질 수 있게 한다.
    

    void MakeNote(Note note) // 실제로 하나의 노트가 만들어졌을때를 처리하는 MakeNote함수를 만들어준다.
    {
        GameObject obj = noteObjectPooler.getObject(note.noteType); // noteObjectPooler에서 새롭게 하나의 오브젝트를 꺼낼수 있게
        
        // 설정된 시작 라인으로 노트를 이동시킵니다.
        x = obj.transform.position.x; // x축과
        z = obj.transform.position.z; // z축은 그대로..
        obj.transform.position = new Vector3(x, startY, z); // y만 startY로 변경될수 있도록 한다.
        obj.GetComponent<NoteBehavior>().Initialize(); // obj.GetComponent<NoteBehavior>에 접근해서 이전에 만들어 놨듯이 initialize로 초기화하여 판정을 다시 NONE으로 바꿔준다.
        obj.SetActive(true); // 다시 노트가 위에서 아래로 내려올 수 있도록 한다.
    }
    private string musicTitle;
    private string musicArtist;
    private int bpm;
    private int divider;
    private float startingPoint;
    private float beatCount;
    private float beatInterval; // 노트 간의 시간 간격

    IEnumerator AwaitMakeNote(Note note) // 모든 노트를 정해진 시간에 출발도록 설정하기 위해
         // 코루틴 함수를 사용한다.(IEumerator)
    {
        int noteType = note.noteType;
        int order = note.order;
        yield return new WaitForSeconds(startingPoint + order * beatInterval); // 코루틴을 사용하여 유니티에게 얼마간(노트순서*인터벌) 기다렸다, 다시 실행하라는 명령
        MakeNote(note); //아래와 같이 Instantiate로 새로운 노트 인스턴스를 만드는게 아니라, 기존에 만들어놓은 오브젝트풀에서 해당 노트를 꺼내서 보여줄 수 있도록 한다.
                        // Instantiate(Notes[noteType - 1]); // 배열(인덱스)의 경우, 0부터 시작하므로, NOtes에 담겨있는 노트의 타입이 1이라면 
                        // 0번째 인덱스의 노트부터 출몰해야하므로, -1을 해준다.
    } 
    void Start()
    {
        noteObjectPooler = gameObject.GetComponent<ObjectPooler>(); // noteObjectPooler를 초기화해줄 수 있도록 해준다.
       
        //리소스에서 비트(Beat)텍스트 파일을 불러오자
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + GameManager.instance.music);
        //textAsset라는 변수를 만들어주고, Resources폴더안에 텍스트파일이 있으므로, Load함수로 텍스트파일을 불러올수 있도록해주고
        // Beats폴더에서 게임매니저에 정의돼있는 그 음악 번호에 해당하는 곡의 정보를 텍스트파일에서 읽어올 수 있도록 한다.

        StringReader reader = new StringReader(textAsset.text); //StringReader :실제로 특정 텍스트파일을 읽어오게 하기위해 사용할 수 있는 라이브러리
        // 유니티에서 사용가능하도록 StringReader 문자위에 마우스위치하여 알트+엔터로 using System.IO;를 선택하여 상단에 추가해줘야한다.

       //첫번째 줄에 적힌 곡 이름을 읽을 수 있게 하자
        musicTitle = reader.ReadLine();
        //두번째 줄에 적힌 아티스트 이름을 읽도록
        musicArtist = reader.ReadLine();
        //세번째 줄에 적힌 비트 정보(BPM, Divider, 시작시간)을 읽어들이도록
        string beatInformation = reader.ReadLine(); //beatInformation: "160 30 3.5"
        bpm = Convert.ToInt32(beatInformation.Split(' ')[0]); //beatInformation에서 Split함수를 불러와서 공백을 기준으로 나누고, 0번째 인덱스, 즉 첫번째 문자열을 받아오겠다는 뜻
        // 그것은 바로 첫번째인 BPM이다.(즉 숫자 160이 담기는 것이다. bpm = 160) 그리고 Convert역시 외부라이브러리이기때문에 유니티에서 사용가능하도록 알트+엔트하여 using System;을 상단에 추가해줘야한다.
        divider = Convert.ToInt32(beatInformation.Split(' ')[1]);
        startingPoint = (float) Convert.ToDouble(beatInformation.Split(' ')[2]); //시작시간(startingPoint)는 정수형이 아닌 실수형이므로, ToDouble와 float를 사용
        //1초마다 떨어지는 비트의 개수를 정해주자 // 비트의 갯수 => bpm / divider => 160 / 60 : 1초마다 떨어지는 비트의 갯수
        beatCount = (float)bpm / divider; // bpm(160) / divider(30) // 노트가 좀 더 빠르게 떨어지게하기위해 원래의 60에서 30초로 줄였기때문에,
        // 결과적으로 1초에 비트가 160/30만큼 떨어진다.
        beatInterval = 1 / beatCount; // 비트가 떨어지는 시간 간격
        string line; //각 비트들이 떨어지는 위치 및 시간 정보를 읽을 수 있게하자. 음악정보가 담겨있는 1.txt파일에서 4번째줄부터..
        while((line = reader.ReadLine()) !=null)
        {
            Note note = new Note(
                Convert.ToInt32(line.Split(' ')[0]) + 1, //1.txt파일에서  0 2 // [0]:공백 기준으로 앞에것 즉 0, 그리고 +1하여, 즉 1번라인 위치에서 떨어지도록 
                Convert.ToInt32(line.Split(' ')[1]) // 1.txt파일에서 0 2 // [1]:공백 기준으로 뒤에것 즉 2, 2만큼의 시간으로 떨어지게 하겠다는 것
            );
            notes.Add(note); // 실제로 떨어지는 notes리스트에 해당 노트를 순서대로 추가
        }
        // 모든 노트를 정해진 시간에 출발할도록 설정해주자.
        for (int i = 0; i < notes.Count; i++)
        {
            StartCoroutine(AwaitMakeNote(notes[i]));
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
