using Unity.VisualScripting;
using UnityEngine;

public class CAnimationHandler : MonoBehaviour
{
    #region Fields
    #endregion Fields

    #region Members
    private Animator m_Animator;
    [SerializeField]
    private MonsterBaseController m_MonsterController;

    #endregion Members


    #region Methods
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_MonsterController = GetComponentInParent<MonsterBaseController>();
    }

    //public void EnterNextScene()
    //{
    //    // 애니메이션 재생
    //    m_Animator.Play("Animation_Name");
    //}

    public void AtAttackMelee()
    {
        //Debug.Log("범위 내 플레이어에 데미지");
        m_MonsterController.DamageToPlayer(haveRange: true);

    }
    public void AtAttackRange()
    {
        //Debug.Log("플레이어에 투사체 발사");
        m_MonsterController.ShotProjectile();

    }
    public void AtEndAttack()
    {
        //Debug.Log("공격 애니메이션 종료");
        m_MonsterController.EndAttackAnimation();
    }
    public void AtEndDie()
    {
        //Debug.Log("몬스터 파괴");
        m_MonsterController.DestroyObject();
    }

    #endregion Methods
}