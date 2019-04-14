using System.Collections;
using System.Collections.Generic;
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
    private float beatInterval = 1.0f; // 노트 간의 시간 간격

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
    
    IEnumerator AwaitMakeNote(Note note) // 모든 노트를 정해진 시간에 출발도록 설정하기 위해
         // 코루틴 함수를 사용한다.(IEumerator)
    {
        int noteType = note.noteType;
        int order = note.order;
        yield return new WaitForSeconds(order * beatInterval); // 코루틴을 사용하여 유니티에게 얼마간(노트순서*인터벌) 기다렸다, 다시 실행하라는 명령
        MakeNote(note); //아래와 같이 Instantiate로 새로운 노트 인스턴스를 만드는게 아니라, 기존에 만들어놓은 오브젝트풀에서 해당 노트를 꺼내서 보여줄 수 있도록 한다.
                        // Instantiate(Notes[noteType - 1]); // 배열(인덱스)의 경우, 0부터 시작하므로, NOtes에 담겨있는 노트의 타입이 1이라면 
                        // 0번째 인덱스의 노트부터 출몰해야하므로, -1을 해준다.
    } 
    void Start()
    {
        noteObjectPooler = gameObject.GetComponent<ObjectPooler>(); // noteObjectPooler를 초기화해줄 수 있도록 해준다.
        notes.Add(new Note(1, 1)); // 위의 생성자로 선언해준대로 Note(noteType, order)
        notes.Add(new Note(2, 2));
        notes.Add(new Note(3, 3));
        notes.Add(new Note(4, 4));
        notes.Add(new Note(1, 5));
        notes.Add(new Note(2, 6));
        notes.Add(new Note(3, 7));
        notes.Add(new Note(4, 8));
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
