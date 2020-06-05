//using Mono.Reflection;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
    private float initHp = 100.0f;
    public float currHp;
    //BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // Start is called before the first frame update
    void Start()
    {
        currHp = initHp;    
    }

    //충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태그가 BULLET이면 Player의 currHp를 차감
        if (coll.tag == bulletTag)
        {
            Destroy(coll.gameObject);

            //혈흔 효과를 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            Debug.Log("Player HP = " + currHp.ToString());

            //PLAYER의 생명이 0 이하이면 사망 처리
            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }

    //Player의 사망 처리 루틴
    private void PlayerDie()
    {
        //Debug.Log("PlayerDie !");
        ////"ENEMY" 태그로 지정된 모든 적 캐릭터를 추출해 배열에 저장
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //
        ////배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출
        ////SendMessage함수 : 특정 게임 오브젝트에 포함된 스크립트를 하나씩 검색해 호출하려는 함수가 있으면
        ////실행하라고 메시지를 전달.
        ////호출한 함수가 없으면 오류 메시지 반환.
        ////SendMessageOptions는 이런 오류 메시지를 리턴받을 것인지 여부를 결정하는 것이다. (빠른 속도를 위해 DontRequireReceiver로 설정)
        //for(int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        OnPlayerDie();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
