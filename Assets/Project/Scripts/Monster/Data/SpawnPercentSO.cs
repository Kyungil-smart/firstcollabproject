using UnityEngine;

namespace Monster
{
    [CreateAssetMenu(fileName = "SpawnPercentSO", menuName = "Scriptable Objects/Spawn/Percent")]
    public class SpawnPercentSO : SheetDataSOBase
    {
        public float Normal;
        public float Ranged;
        public float Bomb;
        public float Brute;
        
        public override void SetData(string[] data)
        {
            id = ParseInt(data[0]);
            Normal = ParseFloat(data[1]);
            Ranged = ParseFloat(data[2]);
            Bomb =ParseFloat(data[3]);
            Brute = ParseFloat(data[4]);
        }
    }
}