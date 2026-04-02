using UnityEngine;

namespace Monster
{
    /// <summary>
    /// 몬스터 사운드 재생 컴포넌트.
    /// 여러 개체가 동시에 사운드를 재생할 수 있도록 PlaySFXPool 을 사용합니다.
    /// </summary>
    public class MonsterSFX : MonoBehaviour
    {
        [SerializeField] MonsterSfxSO sfxData;

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
            AudioManager.Instance.PlayMonsterSfx(sfxData.aggroSFX);
            _aggroPlayed = true;
        }

        /// <summary>
        /// 공격 시
        /// </summary>
        public void PlayAttack()
        {
            if (sfxData.attackSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.attackSFX);
        }

        /// <summary>
        /// 피격 (죽지 않았을 때)
        /// </summary>
        public void PlayHurt()
        {
            if (sfxData.hurtSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.hurtSFX);
        }

        /// <summary>
        /// 사망 시
        /// </summary>
        public void PlayDead()
        {
            if (sfxData.deadSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.deadSFX);
        }

        // ─── 특수 ───

        /// <summary>
        /// 자살 폭탄 좀비 — 폭발 시
        /// </summary>
        public void PlayExplosion()
        {
            if (sfxData.explosionSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.explosionSFX);
        }

        // ─── 보스 전용 ───

        /// <summary>
        /// 영웅 좀비 보스 — 패턴 A 시전시 기합
        /// </summary>
        public void PlayPatternA()
        {
            if (sfxData.patternASFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.patternASFX);
        }
        public void PlayPatternB()
        {
            if (sfxData.patternBSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.patternBSFX);
        }
        public void PlayPatternC()
        {
            if (sfxData.patternCSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.patternCSFX);
        }
        public void PlayPatternD()
        {
            if (sfxData.patternDSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.patternDSFX);
        }

        /// <summary>
        /// 영웅 좀비 보스 — 투사체 시전시 효과음
        /// </summary>
        public void PlayProjectile()
        {
            if (sfxData.projectileSFX == null) return;
            AudioManager.Instance.PlayMonsterSfx(sfxData.projectileSFX);
        }
    }
}
