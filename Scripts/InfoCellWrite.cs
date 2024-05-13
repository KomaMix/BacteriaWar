using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InfoCellWrite : MonoBehaviour
{
	[SerializeField] private Text textComponent;
	[SerializeField] public GameObject objectWithVariables;
    

    
    void Update()
    {
        if (objectWithVariables != null)
        {
			CellBehaviour script = objectWithVariables.GetComponent<CellBehaviour>();

			textComponent.text = script.CellInfo();
            
		}


    }
}
