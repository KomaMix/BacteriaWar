using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
	public class BreedingCellBehaviour : CellBehaviour
	{
		[SerializeField] private float pGenerateLifeCell = 0.25f;
		[SerializeField] private float pGenerateSimpleCell = 0.25f;
		[SerializeField] private float pGenerateBreedingCell = 0.25f;
		[SerializeField] private float pGenerateAggressiveCell = 0.25f;
		[SerializeField] private float timeToBreeding = 4f;

		[SerializeField] private float DistanceSpawn => radius * 3;
		[SerializeField] private int maxIterations = 10;

		protected override void Start()
		{
			base.Start();

			

			StartCoroutine(BreedingEveryTime());
		}

		protected override void Update()
		{
			base.Update();
		}


		protected override void OnTriggerEnter2D(Collider2D collision)
		{
			base.OnTriggerEnter2D(collision);
		}


		protected override void OnTriggerExit2D(Collider2D collision)
		{
			base.OnTriggerExit2D(collision);
		}


		IEnumerator BreedingEveryTime()
		{
			while (true)
			{
				yield return new WaitForSeconds(timeToBreeding);

				if (cur_power / max_power > 0.8)
					Breeding();

				
			}
		}


		private void Breeding()
		{
			bool positionFound = false;
			int iteration = 0;
			// ---------------Update (добавить другие клетки)
			float[] probabilities = new[] { pGenerateSimpleCell, pGenerateLifeCell, pGenerateBreedingCell };
			HelperClass.NormalizeVector(probabilities);
			CellType[] cellTypes = new[] { CellType.SimpleCell, CellType.LifeCell, CellType.BreedingCell };
			CellType newCellType = HelperClass.Choose(cellTypes, probabilities);

			GameObject newCell = Resources.Load<GameObject>($"Prefabs/{Enum.GetName(typeof(CellType), newCellType)}");

			while (!positionFound && iteration < maxIterations)
			{
				// Угол и позиция в полярных координатах
				float angle = UnityEngine.Random.Range(0f, 360f);
				Vector3 position = transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * DistanceSpawn);


				Collider2D[] colliders = Physics2D.OverlapCircleAll(position, newCell.GetComponentInChildren<Collider2D>().bounds.extents.x);
				// Проверяем, не пересекается ли новая позиция с другими коллайдерами
				if (!colliders.Any(collider => collider.gameObject.layer == 10 || collider.gameObject.layer == 12))
				{
					// Если не пересекается, создаем объект
					GameObject c = Instantiate(newCell, position, Quaternion.identity);
					c.GetComponent<CellBehaviour>().SetPower(cur_power / 2);
					cur_power = cur_power / 2;
					positionFound = true;
				}
				iteration++;
			}

			if (!positionFound)
			{
				Debug.Log("Не удается найти свободное место для создания объекта");
			}
		}
	}
}
