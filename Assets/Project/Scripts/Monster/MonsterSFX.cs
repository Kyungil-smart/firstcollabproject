using UnityEngine;

namespace Monster
{
    /// <summary>
    /// 몬스터 사운드 재생 컴포넌트.
    /// 여러 개체가 동시에 사운드를 재생할 수 있도록 PlaySFXPool 을 사용합니다.
    /// </summary>
    public class MonsterSFX : MonoBehaviour
    {
        [SerializeField] MonsterSfxS sfxData;

        private bool _aggroPlayed;

        public void Init()
        {
            _aggroPlayed = false;
        }

        // ─── 공통 ───

        /// <summary>
        /// 어그로 (최초 1회만 재생, ResetState 로 초기화)
        /// </summary>
        public void PlayAggro()
        {
            if (_aggroPlayed) return;
            if (sfxData.aggroSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.aggroSFX);
            _aggroPlayed = true;
        }

        /// <summary>
        /// 공격 시
        /// </summary>
        public void PlayAttack()
        {
            if (sfxData.attackSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.attackSFX);
        }

        /// <summary>
        /// 피격 (죽지 않았을 때)
        /// </summary>
        public void PlayHurt()
        {
            if (sfxData.hurtSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.hurtSFX);
        }

        /// <summary>
        /// 사망 시
        /// </summary>
        public void PlayDead()
        {
            if (sfxData.deadSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.deadSFX);
        }

        // ─── 특수 ───

        /// <summary>
        /// 원거리 좀비 — 활을 쏠 때
        /// </summary>
        public void PlayBow()
        {
            if (sfxData.bowSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.bowSFX);
        }

        /// <summary>
        /// 자살 폭탄 좀비 — 폭발 시
        /// </summary>
        public void PlayExplosion()
        {
            if (sfxData.explosionSFX == null) return;
            AudioManager.Instance.PlaySFXPool(sfxData.explosionSFX);
        }
    }
}
