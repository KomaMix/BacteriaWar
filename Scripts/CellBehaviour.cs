using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Progress;

public class CellBehaviour : MonoBehaviour
{

	protected Rigidbody2D rb;

	[SerializeField] public CellType cellType = CellType.SimpleCell;
    [SerializeField] public HashSet<Tuple<CellType, Rigidbody2D>> interactingCells = new HashSet<Tuple<CellType, Rigidbody2D>>();
	[SerializeField] public Dictionary<Tuple<CellType, Rigidbody2D>, LineRenderer> linkedCells = new Dictionary<Tuple<CellType, Rigidbody2D>, LineRenderer>();
	
	public int CountInteractingCells => interactingCells.Count;
	public int CountLinkedCells => linkedCells.Count;
	[SerializeField] private int maxLinkedCells = 3;
	[SerializeField] private float lineWidth = 0.05f;



	protected float radius;
	[SerializeField] protected float forceDirection = 1;
	[SerializeField] protected float G = 10f;
	[SerializeField] protected float K = 10f;


	[SerializeField] protected float max_power = 10f;
	[SerializeField] protected float cur_power = 10f;
	[SerializeField] protected float cost_power = 1f;
	

    protected virtual void Start()
    {
		radius = transform.localScale.x / 2;
		rb = gameObject.GetComponent<Rigidbody2D>();
		StartCoroutine(UpdateEverySecond());
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		// Слой обнаружения
		if (collision.gameObject.layer == 10)
		{
			if (collision.attachedRigidbody != null)
			{
				CellBehaviour script = collision.GetComponentInParent<CellBehaviour>();

				if (script != null)
				{
					if (!linkedCells.ContainsKey(Tuple.Create(script.cellType, collision.attachedRigidbody)) &&
					!script.linkedCells.ContainsKey(Tuple.Create(cellType, rb)))
					{
						if (maxLinkedCells > CountLinkedCells && script.maxLinkedCells > script.CountLinkedCells)
						{
							LineRenderer lineRenderer = CreateLine();

							linkedCells.Add(Tuple.Create(script.cellType, collision.attachedRigidbody), lineRenderer);
							script.linkedCells.Add(Tuple.Create(cellType, rb), lineRenderer);
						}
						else
						{
							interactingCells.Add(Tuple.Create(script.cellType, collision.attachedRigidbody));
						}
					}
				}
				

				
			}

			
		}
		
	}


	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		// Слой обнаружения
		if (collision.gameObject.layer == 10)
		{
			if (collision.attachedRigidbody != null)
			{
				CellBehaviour script = collision.GetComponentInParent<CellBehaviour>();

				if (script != null )
				{
					// Удалить, если элемент найден
					interactingCells.Remove(Tuple.Create(script.cellType, collision.attachedRigidbody));

					if (linkedCells.ContainsKey(Tuple.Create(script.cellType, collision.attachedRigidbody)))
					{
						var item = linkedCells[Tuple.Create(script.cellType, collision.attachedRigidbody)];
						Destroy(item.gameObject);
						linkedCells.Remove(Tuple.Create(script.cellType, collision.attachedRigidbody));
						script.linkedCells.Remove(Tuple.Create(cellType, rb));
					}
				}
				
			}
		}

	}


	protected virtual void Update()
    {
		// Если тело не связано, но находится в области видимости данного тела
        foreach (var alien in interactingCells)
		{
			Vector2 direction = (rb.position - alien.Item2.position).normalized;
			float distance = (rb.position - alien.Item2.position).magnitude;

			
			// Сила направленная от другой клетки
			var force = forceDirection * G * direction * alien.Item2.mass * rb.mass / distance / distance;

			rb.AddForce(force);
			
            
			
		}


		foreach (var item in linkedCells)
		{
			Vector2 direction = (rb.position - item.Key.Item2.position).normalized;
			float distance = (rb.position - item.Key.Item2.position).magnitude;

			// Сила направленная от другой клетки
			var force = K * direction * item.Key.Item2.mass * rb.mass * (3 * radius - distance);

			rb.AddForce(force);


			item.Value.SetPosition(0, transform.position);
			item.Value.SetPosition(1, item.Key.Item2.transform.position);
		}

		
	}


	public virtual string CellInfo()
	{
		string result = "";

		result += "Текущая энергия: " + cur_power + "\n";
		result += "Кол. окр. клеток: " + (CountInteractingCells).ToString() + "\n";
		result += "Кол. связ. клеток: " + CountLinkedCells.ToString() + "\n";

		return result;
	}


	protected virtual IEnumerator UpdateEverySecond()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);

			SpendPower();

			
		}
	}


	LineRenderer CreateLine()
	{
		GameObject lineObject = new GameObject($"Line{HelperClass.GetID()}");
		LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
		lineRenderer.transform.position += new Vector3(0, 0, 95);
		lineRenderer.material = Resources.Load<Material>($"Materials/BlackMaterial");
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		return lineRenderer;
	}


	private void SpendPower()
	{
		cur_power -= cost_power;
		if (cur_power < 0)
		{
			foreach (var item in linkedCells)
			{
				Destroy(item.Value.gameObject);
			}

			Destroy(gameObject);
		}
	}

	
	public void TransferPower(float power)
	{
		cur_power = Math.Min(cur_power + power, max_power);
	}


	public void SetPower(float power)
	{
		cur_power = power;
	}


}

public enum CellType
{
	LifeCell,
	SimpleCell,
	AggressiveCell,
	BreedingCell
}
