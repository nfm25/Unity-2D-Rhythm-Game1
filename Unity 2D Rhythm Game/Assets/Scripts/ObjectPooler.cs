using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    //이중리스트(2차원리스트: 리스트들의 리스트) 사용하기
    // Note 1 : 10개 만들어 => 리스트1 작성
    // Note 2 : 10개 만들어 => 리스트2 작성
    // Note 3 : 10개 만들어 => 리스트3 작성
    // Note 4 : 10개 만들어 => 리스트4 작성

    public List<GameObject> Notes; // 각 노트 프리팹에 대한 데이터를 담을 수 있는 게임오브젝트로 리스트를 만들어준다.
    private List<List<GameObject>> poolsOfNotes; //내부적으로 들어가는 자료형인 리스트 또한 리스트가 될 수 있도록 만들어준다.
    public int noteCount = 10;
    private bool more = true; // 노트가 10개 이상 필요할 경우를 대비해서 more라는 변수를 만들어준다..

    void Start()
    {
        poolsOfNotes = new List<List<GameObject>>(); //리스트들의 리스트가 들어갈 수 있는 비어있는 이중리스트(poolsOfNotes)를 만들어준다.
        for (int i = 0; i < Notes.Count; i++) //notes리스트안에 들어있는 원소만큼 반복 => 4번 반복 (0~3)
        {
            poolsOfNotes.Add(new List<GameObject>()); // <<NO.1>> 이중리스트인 poolsOfNotes 안에 리스트(들이)가 원소로서 추가되도록 한다.
            for (int n = 0; n < noteCount; n++) // 각 리스트들은 10개씩 데이터가 들어가야하므로 noteCount(=10개)만큼 반복 => 10번 반복
            {
                GameObject obj = Instantiate(Notes[i]); //<<NO.2>> 실제로 게임이 실행되면 10개만큼 노트를 만들 수 있게, 해당 노트(Notes[i])의 번호(종류)에 맞는 노트를 생성해서 obj(오브젝트)에 담아준다.
                obj.SetActive(false); //그리고 모든 오브젝트는 시작될때 비활성화돼 있도록 한다.
                poolsOfNotes[i].Add(obj); //<<NO.3> 해당 라인에 노트리스트에 해당 노트오브젝트를 넣을 수 있도록 
                // 각노트의 종류(4)에 따라서 10개씩 해당리스트에 노트오브젝트를 만들어서 넣어주겠다는 것(4 * 10 = 총 40개)
                // NO.1~NO.3까지의 부분은 총 4번, NO.2~NO.3까지의 영역은 총 10번 반복. 
            }
        }
    }            
    
    public GameObject getObject(int noteType) //어떠한 종류의 노트가 필요하니깐, 이제 그것을 오브젝트풀에서 꺼내오겠다고 명시
    {
        foreach(GameObject obj in poolsOfNotes[noteType -1]) //해당 노트의 종류에 맞는 그 리스트에 접근해서 그 리스트에 존재하는 모든 오브젝트 하나씩 확인하겠다는 뜻
        {
            if (!obj.activeInHierarchy) // 만약 현재화면에서 해당 오브젝트가 비활성화 처리돼 있다면
            {
                return obj; // 그 오브젝트를 사용하겠다고 알려주는 것
            }

        // 만일 현재 해당 노트 종류에 맞는 그 리스트에 노트가 전부다 사용중(활성화처리)이라면 위의 내용은 실행되지 않을 것이다.
        
        }
        if (more) // 즉 만약에 해당 종류의 노트가 10개 이상 화면에 등장해야되는 경우라면 
        {         // 오브젝트풀이 꽉차있는 경우라면..
            GameObject obj = Instantiate(Notes[noteType - 1]); // 해당 오브젝트를 그 순간에 만들어주어서
            poolsOfNotes[noteType - 1].Add(obj); // 오브젝트풀에 추가하는 방식으로 사용할 수 있다.
            return obj; // 그리고 그렇게 만들어진 오브젝트를 반환해준다.
        }
        return null; // 무언가 오류가 발생했을 때, null 값을 반환해줄 수 있도록 한다.
    }

    
    void Update()
    {
        
    }
}
