using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    [SerializeField] 
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField] 
    private int iterations;

    public int walkLength;
    public bool startRandomlyEachIteration = true;


    public void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();

        foreach (var position in floorPositions)
        {
            Debug.Log(position);
        }
    }

    private HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPosition = new HashSet<Vector2Int>();

        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGeneration.RandomWalk(currentPosition, walkLength);
            floorPosition.UnionWith(path);

            if (startRandomlyEachIteration)
                currentPosition = floorPosition.ElementAt(Random.Range(0, floorPosition.Count));

        }
        
        return floorPosition;
    }
}
