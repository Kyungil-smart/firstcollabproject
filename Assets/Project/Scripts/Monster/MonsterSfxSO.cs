using UnityEngine;
using UnityEngine.Audio;

namespace Monster
{
    [CreateAssetMenu(fileName = "MonsterSfx", menuName = "Scriptable Objects/Monster/SFX")]
    public class MonsterSfxSO : ScriptableObject
    {
        [Header("공통")]
        public AudioResource aggroSFX;
        public AudioResource attackSFX;
        public AudioResource hurtSFX;
        public AudioResource deadSFX;

        [Header("특수")]
        //public AudioResource bowSFX;        // 원거리 좀비 전용
        public AudioResource explosionSFX;  // 자살 폭탄 좀비 전용
    }
}
