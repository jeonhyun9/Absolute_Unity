using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//내비게이션 기능을 사용하기 위해 추가하는 네임스페이스
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    //순찰 지점들을 저장하기 위한 List 타입 변수
    public List<Transform> wayPoints;
    //다음 순찰 지점의 배열의 Index
    public int nextIdx;

    //변숫값을 변경할 필요가 없는 변수는 readonly 또는 cosnt 키워드를 사용해 가비지 컬렉터에 쌓이지 않도록 한다.
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    //회전할 때의 속도를 조절하는 계수
    private float damping = 1.0f;


    //NavMeshAgent 컴포넌트를 저장할 변수
    private NavMeshAgent agent;
    //적 캐릭터의 Transform 컴포넌트를 저장할 변수
    private Transform enemyTr;

    //순찰 여부를 판단하는 변수
    private bool _patrolling;
    //patrolling 프로퍼티 정의 (getter , setter)
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if(_patrolling)
            {
                agent.speed = patrolSpeed;
                //순찰 상태의 회전 계수
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    //추적 대상의 위치를 저장하는 변수
    private Vector3 _traceTarget;
    //traceTarget 프로퍼티 정의 (getter, setter)
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            //추적 상태의 회전 계수
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    //NavMeshAgent의 이동 속도에 대한 프로퍼티 정의 (getter)
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //적 캐릭터의 Transform 컴포넌트 추출 후 변수에 저장
        enemyTr = GetComponent<Transform>();
        //NavMeshAgent 컴포넌트를 추출한 후 변수에 저장
        agent = GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성화
        agent.autoBraking = false;
        //자동으로 회전하는 기능을 비활성화
        agent.updateRotation = false;
        agent.speed = patrolSpeed;

        //하이러키 뷰의 WayPointGroup 게임오브젝트를 추출
        var group = GameObject.Find("WayPointGroup");
        if(group != null)
        {
            //WayPointGroup 하위에 있는 모든 Transform 컴포넌트를 추출한 후
            //List 타입의 wayPoints 배열에 추가
            group.GetComponentsInChildren<Transform>(wayPoints);

            /*다음과 같이도 가능하다*/
            //Transform[] ways = group.GetComponentsInChildren<Transform>();
            //wayPoints.AddRange(ways);

            //배열의 첫 번째 항목 삭제
            //부모 컴포넌트 까지 배열에 담기 때문.
            wayPoints.RemoveAt(0);
        }

        MoveWayPoint();
    }

    //다음 목적지까지 이동 명령을 내리는 함수
    private void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        //속성값이 true인 경우는 경로가 더는 유효하지 않다는 것으로 Off Mesh Link 가 비활성화 되거나 Area Mask 값이 변경됐을 때 발생한다.
        if (agent.isPathStale) return;

        //다음 목적지를 wayPoints 배열에서 추출한 위치로 다음 목적지를 지정
        agent.destination = wayPoints[nextIdx].position;
        //내비게이션 기능을 활성화해서 이동을 시작함
        agent.isStopped = false;
    }

    //주인공을 추적할 때 이동시키는 함수
    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;

        agent.destination = pos;
        agent.isStopped = false;
    }

    //순찰 및 추적을 정지시키는 함수
    public void Stop()
    {
        agent.isStopped = true;
        //바로 정지하기 위해 속도를 0으로 설정
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        //적 캐릭터가 이동 중일 때만 회전
        if(agent.isStopped == false)
        {
            //NavMeshAgent가 가야 할 방향 벡터를 쿼터니언 타입의 각도로 변환
            //NavMeshAgent.desiredVelocity = 다음 목적지로 향하는 속도
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //보간 함수를 사용해 점진적으로 회전시킴
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        //NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부를 계산
        //remainingDistance : NavMeshAgent에 지정된 목적지까지 남은 거리를 반환. (처음엔 정지 상태이기 때문에 항상 0을 반환한다. 따라서 적 캐릭터가 이동 중이라는 조건도 함께 판단해야 한다.)
        //sqrMagnitude : Vector3.Distance 보다 성능이 좋음. 두 점 간의 거리를 구할 때 사용한다.
        //NavMeshAgent.velocity : 속도를 의미함. 이 속성의 크기로 이동 여부를 판단한다.
        //
        //nv.velocity.magnitude > 0.2f
        //nv.velocity.sqrMagnitude > 0.2f * 0.2f
        //위 수식은 둘다 속도의 크기가 0.2보다 크다 라는 의미이다.
        //sqrMagnitude : 정확성 상대적으로 낮음 , 성능 좋음
        //Magnitude : 정확성 높음 , 성능 낮음

        if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <=0.5f)
        {
            //다음 목적지의 배열 첨자를 계산
            //나머지 연산의 응용이다. 배열을 처음부터 끝까지 순회하는 로직에 사용!
            nextIdx = ++nextIdx % wayPoints.Count;
            //0 % 10 = 0
            //1 % 10 = 1
            //2 % 10 = 2 ...

            //다음 목적지로 이동 명령을 수행
            MoveWayPoint();
        }
    }
}
