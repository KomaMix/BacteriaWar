using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts
{
	internal class HelperClass
	{
		static public T Choose<T>(T[] items, float[] probabilities)
		{
			if (items.Length != probabilities.Length)
				throw new InvalidOperationException("Неправильные входные данные");

			// Генерация случайного числа от 0 до 1
			float randomPoint = (float)new Random().NextDouble();

			// Накопление вероятностей
			float cumulativeProbability = 0.0f;
			for (int i = 0; i < items.Length; i++)
			{
				cumulativeProbability += probabilities[i];

				// Если случайно сгенерированное число меньше текущей кумулятивной вероятности - возвращаем элемент
				if (randomPoint <= cumulativeProbability)
				{
					return items[i];
				}
			}

			// В случае, если что-то пошло не так, например, из-за проблем с округлением
			// – возвращаем последний элемент.
			return items[items.Length - 1];
		}

		static public void NormalizeVector(float[] array)
		{
			// Находим минимальное и максимальное значение в массиве
			float sum = array.Sum();

			// Нормализуем значения массива к диапазону от 0 до 1
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i] / sum;
			}
		}

		private static int lastID = 1;
		
		static public int GetID()
		{
			return lastID++;
		}

	
	}
}
