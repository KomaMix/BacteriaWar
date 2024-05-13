using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] private GameObject cellPositive;
    //[SerializeField] private GameObject cellNegative;
    [SerializeField] private int count = 5;
    float width;
    float height;
    float radius;


    void Start()
    {
        width = transform.localScale.x;
        height = transform.localScale.y;
        radius = cellPositive.gameObject.transform.localScale.x / 2;



        for (int i = 0; i < count; i++)
        {
			GameObject c = (GameObject)Object.Instantiate(cellPositive, 
                new Vector3(transform.position.x + UnityEngine.Random.Range(-width / 2 + radius, width / 2 + radius), transform.position.y + UnityEngine.Random.Range(-height / 2 + radius, height / 2 - radius), transform.position.z - 1), quaternion.identity);
		}
	}

    
    void Update()
    {
        
    }
}
