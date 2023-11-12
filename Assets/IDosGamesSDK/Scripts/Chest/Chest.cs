using System;
using System.Linq;
using UnityEngine;

namespace IDosGames
{
	public class Chest : MonoBehaviour
	{
		[SerializeField] private ChestAnimation _animation;

		public ChestAnimation Animation => _animation;

		[SerializeField] private SkinnedMeshRenderer _mesh;

		public ChestMaterial[] Materials;

		public void SetMaterialByRarity(ChestRarityType rarity)
		{
			var material = Materials.FirstOrDefault(x => x.rarity == rarity).material;
			_mesh.material = material;
		}
	}
}