using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider != null)
				{
					// CollisionLayer
					if (hit.collider.gameObject.layer == 10)
					{
						GameObject textObject = GameObject.Find("TextInfo");

						InfoCellWrite script = textObject.GetComponent<InfoCellWrite>();
						script.objectWithVariables = hit.collider.transform.parent.gameObject;
					}
				}
			}
		}
    }
}
