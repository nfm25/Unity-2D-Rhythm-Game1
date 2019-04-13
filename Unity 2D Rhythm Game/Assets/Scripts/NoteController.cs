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
    private List<Note> notes = new List<Note>();
    //실제로 떨어지게 될 노트들의 데이터를 담기위해 notes라는 리스트 변수를 만들어줌. 그리고 내부적으로만 사용되므로 private로 선언
    private float beatInterval = 1.0f; // 노트 간의 시간 간격

    IEnumerator AwaitMakeNote(Note note) // 모든 노트를 정해진 시간에 출발도록 설정하기 위해
         // 코루틴 함수를 사용한다.(IEumerator)
    {
        int noteType = note.noteType;
        int order = note.order;
        yield return new WaitForSeconds(order * beatInterval); // 코루틴을 사용하여 유니티에게 얼마간 기다렸다, 다시 실행하라는 명령
        Instantiate(Notes[noteType - 1]); // 배열(인덱스)의 경우, 0부터 시작하므로, NOtes에 담겨있는 노트의 타입이 1이라면 
                                            // 0번째 인덱스의 노트부터 출몰했야하므로, -1을 해준다.
    } 
    void Start()
    {
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
