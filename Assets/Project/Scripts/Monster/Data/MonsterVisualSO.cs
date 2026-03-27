using UnityEngine;

[CreateAssetMenu(fileName = "MonsterVisualSO", menuName = "Scriptable Objects/Monster/Visual")]
    public class MonsterVisualSO : SheetDataSOBase
    {
        // 타격 이펙트 시간(s)
        public float HitFlashTime;
        
        // 시체 물리 판정 시간(s)
        public float CorpseTime;
        
        // 넉백 저항력(0~1)
        public float KnkbResist;
        
        // 타격 경직 시간(s)
        public float HitStopDuration;
        
        public override void SetData(string[] data)
        {
            id = ParseInt(data[0]);
            HitFlashTime = ParseFloat(data[1]);
            CorpseTime = ParseFloat(data[2]);
            KnkbResist = ParseFloat(data[3]);
            HitStopDuration = ParseFloat(data[4]);
        }
    }
