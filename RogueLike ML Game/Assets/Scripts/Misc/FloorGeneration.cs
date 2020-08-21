using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGeneration : MonoBehaviour
{
    public GameObject[] floorPrefabs;
    public Rect floorPieceDimensions;
    public Rect floorDimensions;

    void Start()
    {
        GameObject randomFloorPiece;
        int randomRotation;

        for (int x = 0; x < floorDimensions.width; x++)
        {
            for (int y = 0; y < floorDimensions.height; y++)
            {
                randomFloorPiece = floorPrefabs[Random.Range(0, floorPrefabs.Length)];
                randomRotation = Random.Range(0, 4);

                GameObject newFloor = Instantiate(randomFloorPiece, transform);

                newFloor.transform.Translate(
                    new Vector3(
                        x * floorPieceDimensions.width,
                        transform.position.y,
                        y * floorPieceDimensions.height
                    )
                );

                //newFloor.transform.Rotate(0, randomRotation * 90, 0);
            }
        }
    }

}
