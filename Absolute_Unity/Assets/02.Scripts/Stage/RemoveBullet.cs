using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //스파크 프리팹을 저장할 변수
    public GameObject sparkEffect;

    //충돌이 시작할 때 발생하는 이벤트
    private void OnCollisionEnter(Collision collision)
    {
        //스파크 효과 함수 호출
        ShowEffect(collision);
        if(collision.collider.tag == "BULLET")
        {
            Destroy(collision.gameObject);
        }
    }

    private void ShowEffect(Collision collision)
    {
        //충돌 지점의 정보를 추출
        ContactPoint contact = collision.contacts[0];
        //법선 벡터가 이루는 회전 각도를 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward/*벽 입장에서의 법선 벡터를 반환하기 위해 마이너스를 곱한다*/, contact.normal/*법선*/);

        //스파크 효과를 생성
        GameObject spark = Instantiate(sparkEffect, contact.point/*충돌지점의위치*/, rot);
        //스파크 효과의 부모를 드럼통 또는 벽으로 설정
        spark.transform.SetParent(this.transform);
    }
}
