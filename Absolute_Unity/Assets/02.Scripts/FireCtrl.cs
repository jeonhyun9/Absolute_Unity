using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사 좌표
    public Transform firepos;
    //탄피 추출 파티클
    public ParticleSystem cartridge;
    //총구 화염 파티클
    private ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        //FirePs 하위에 있는 컴포넌트 추출
        //하위 컴포넌트가 여러개라면 , 맨 처음 컴포넌트가 반환된다.
        muzzleFlash = firepos.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        //Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firepos.position, firepos.rotation);
        //탄피 파티클 실행
        cartridge.Play();
        //총구 화염 파티클 실행
        muzzleFlash.Play();
    }
}
