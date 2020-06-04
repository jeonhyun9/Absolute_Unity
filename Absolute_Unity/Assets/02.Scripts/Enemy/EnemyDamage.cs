using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    //생명 게이지
    private float hp = 100.0f;
    //피격시 사용할 혈흔 효과
    private GameObject bloodEffect;

    // Start is called before the first frame update
    void Start()
    {
        //혈흔 효과 프리팹을 로드
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
    }

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == bulletTag)
        {
            //혈흔 효과를 생성하는 함수 호출
            ShowBloodEffect(coll);
            //맞은 총알 삭제
            Destroy(coll.gameObject);
            //생명 게이지 차감
            //에러 메시지의 원인 -> 관리된 어셈블리를 사용하기 때문.
            //현재 스크립트는 EnemyAD.dll 에 컴파일 되고,
            //참조한 BulletCtrl은 PlayerAD.dll에 컴파일 된다.
            //따라서 EnemyAD에서 PlayerAD 어셈블리를 참조할 수 있또록 설정해야 한다.
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp<=0.0f)
            {
                //적 캐릭터의 상태를 DIE로 변경
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
            }
        }
    }

    private void ShowBloodEffect(Collision coll)
    {
        //총알이 충돌한 지점 산출
        //여러 지점이 충돌할 수 있기 때문에 [0]
        Vector3 pos = coll.contacts[0].point;
        //총알이 충돌했을 때의 법선 벡터
        Vector3 _normal = coll.contacts[0].normal;
        //총알의 충돌 시 방향 벡터의 회전 값 계산
        //총알 입사각의 반대로 설정해줘야 하기 때문에 z축 방향에 음수를 곱한다.
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        //1초 후 삭제
        Destroy(blood, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
