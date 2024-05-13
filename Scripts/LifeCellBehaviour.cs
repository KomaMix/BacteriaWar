using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	internal class LifeCellBehaviour : CellBehaviour
	{
		[SerializeField] public HashSet<SpriteRenderer> lightSources = new HashSet<SpriteRenderer>();
		[SerializeField] protected float costLightPower = 10f;
		[SerializeField] protected float outputPower = 0.5f;
		[SerializeField] protected float outputPowerDistance = 3f;
		public int CountLightSources => lightSources.Count;
		public int CountPowerConsumers { get; protected set; } = 0;
		protected override void Start()
		{
			StartCoroutine(UpdateLightPowerEverySecond());

			base.Start();

			cellType = CellType.LifeCell;
			
		}

		protected override void Update()
		{
			base.Update();

			
		}


		protected override void OnTriggerEnter2D(Collider2D collision)
		{
			base.OnTriggerEnter2D(collision);

			//Слой освещения
			if (collision.gameObject.layer == 15)
			{
				SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
				lightSources.Add(spriteRenderer);
				
			}
		}


		protected override void OnTriggerExit2D(Collider2D collision)
		{
			base.OnTriggerExit2D(collision);

			//Слой освещения
			if (collision.gameObject.layer == 15)
			{
				if (cellType == CellType.LifeCell)
				{
					SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
					lightSources.Remove(spriteRenderer);
				}
			}
		}


		public override string CellInfo()
		{
			string result = base.CellInfo();

			result += "Кол. ист. света: " + CountLightSources.ToString() + "\n";
			result += "Кол. пот. света: " + CountPowerConsumers.ToString() + "\n";

			return result;
		}


		protected IEnumerator UpdateLightPowerEverySecond()
		{
			while (true)
			{
				GetLightPower();

				yield return new WaitForSeconds(1f);

				

				ShareEnergy();
			}
		}


		private void GetLightPower()
		{
			foreach (var item in lightSources)
			{
				cur_power += item.color.a * costLightPower;
			}

			cur_power = Math.Min(cur_power, max_power);
		}


		private void ShareEnergy()
		{
			int cntPowerConsumers = 0;
			float curOutput = outputPower;
			if (CountPowerConsumers > 0)
			{
				curOutput = outputPower / CountPowerConsumers;
			}

			foreach (var item in linkedCells)
			{

				float distance = (rb.position - item.Key.Item2.position).magnitude;

				if (distance < outputPowerDistance)
				{
					cntPowerConsumers++;
					item.Key.Item2.gameObject.GetComponent<CellBehaviour>().TransferPower(curOutput);
					cur_power -= curOutput;
				}
			}

			CountPowerConsumers = cntPowerConsumers;
		}
	}
}
