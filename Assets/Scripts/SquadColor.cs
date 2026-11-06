using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts
{
	public sealed class SquadColor : MonoBehaviour
	{
		private static readonly int k_BaseColorId = Shader.PropertyToID("_BaseColor");

		[SerializeField] private Renderer _colorID;

		private Color _color;

		private void Awake()
		{
			if (_colorID == null)
			{
				Debug.LogError("No Color ID assigned, script disabled.", this);
				enabled = false;
				return;
			}
		}
		private void Start()
		{
			SetSquadColor(_color);
		}
		/// <summary>
		/// <br>Sets the ColorID of the object to a specific color.</br>
		/// <br>This color is used to indicate to the player which objects belong together and how dangerous they are.</br>
		/// <br>The colors are inspired by the belts in martial arts.</br>
		/// </summary>
		public void SetSquadColor(Enemy.Belt newColor)
		{
			switch (newColor)
			{
				case Belt.White:
					SetSquadColor(Color.white);
					break;
				case Belt.Yellow:
					SetSquadColor(Color.yellow);
					break;
				case Belt.Orange:
					SetSquadColor(Color.orange);
					break;
				case Belt.Purple:
					SetSquadColor(Color.purple);
					break;
				case Belt.Brown:
					SetSquadColor(Color.brown);
					break;
				case Belt.Black:
					SetSquadColor(Color.black);
					break;
			}
		}

		/// <summary>
		/// <br>Sets the ColorID of the object to a specific color.</br>
		/// <br>This color is used to indicate to the player which objects belong together and how dangerous they are.</br>
		/// <br>The colors are inspired by the belts in martial arts.</br>
		/// </summary>
		/// <remarks>
		/// Use the overload with Enemy.Belt for default behaviour.
		/// </remarks>
		public void SetSquadColor(Color newColor)
		{
			if (_colorID == null)
			{
				return;
			}

			_color = newColor;
			MaterialPropertyBlock newBlock = new();
			newBlock.SetColor(k_BaseColorId, _color);
			_colorID.SetPropertyBlock(newBlock);
		}
	}
}
