using Anima2D;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary
{
	public class FadeOutIfHostile : MonoBehaviour
	{
		private void Start()
		{
			if (!GetComponentInParent<UnitComponent>().IsHostile)
			{
				return;
			}

			foreach (var graphics in GetComponentsInChildren<Renderer>())
			{
				graphics.FadeOut(2.0f);
			}

			foreach (var graphics in GetComponentsInChildren<SpriteMeshInstance>())
			{
				graphics.FadeOut(2.0f);
			}
		}
	}
}