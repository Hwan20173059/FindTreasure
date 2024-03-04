using Unity.VisualScripting;
using UnityEngine;

public class CMonsterAnimationHandler : MonoBehaviour
{
    #region 멤버변수

    private Animator m_Animator;
    private MonsterMelee m_MonsterMelee;
    private MonsterRange m_MonsterRange;
    private MonsterBaseController m_MonsterController; // 몬스터 종류에 따른 공통 구문을 한 라인으로 처리하기 위해 사용

    #endregion


    #region 메서드

    void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_MonsterMelee = GetComponentInParent<MonsterMelee>();
        m_MonsterRange = GetComponentInParent<MonsterRange>();
        m_MonsterController = GetComponentInParent<MonsterBaseController>();
    }

    // Melee 몬스터 사용 메서드
    public void AtAttackMelee()
    {
        //Debug.Log("범위 내 플레이어에 데미지");
        m_MonsterMelee.DamageToPlayer(haveRange: true);

    }

    // Range 몬스터 사용 메서드
    public void AtAttackRange()
    {
        //Debug.Log("플레이어에 투사체 발사");
        m_MonsterRange.ShotProjectile();

    }

    // 몬스터 공통 사용 메서드
    public void AtEndAttack()
    {
        //Debug.Log("공격 애니메이션 종료, Idle로 상태 전환 등");
        m_MonsterController.EndAttackAnimation();

    }
    public void AtEndDie()
    {
        //Debug.Log("몬스터 파괴");
        m_MonsterController.DestroyObject();
    }

    #endregion
}