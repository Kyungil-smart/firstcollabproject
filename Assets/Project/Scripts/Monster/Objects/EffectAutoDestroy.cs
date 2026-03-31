using UnityEngine;

namespace Monster
{
    public class EffectAutoDestroy : MonoBehaviour
    {
        private readonly float _lifeTime = 1f;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }
    }
}