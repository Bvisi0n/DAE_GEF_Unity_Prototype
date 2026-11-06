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

		public void SetSquadColor(Enemy.Belt newColor)
		{
			// TODO: Refactor - To many unrelated objects involve the Belt enum.
			// This is messy but it provides a single source of truth for color and I'm out of budget (time) to do it properly.
			// I don't like the naming of some of the involved classes etc and how they are all tangled up.
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
