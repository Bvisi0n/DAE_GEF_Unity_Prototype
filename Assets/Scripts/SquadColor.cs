using UnityEngine;

namespace Assets.Scripts
{
	public class SquadColor : MonoBehaviour
	{
		private static readonly int k_BaseColorId = Shader.PropertyToID("_BaseColor");

		[SerializeField] private Renderer _colorID;
		[SerializeField] private Color _color;

		public Color Color => _color;

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

		private void OnValidate()
		{
			SetSquadColor(_color);
		}

		public void SetSquadColor(Color newColor)
		{
			_color = newColor;

			if (_colorID == null)
			{
				return;
			}

			MaterialPropertyBlock newBlock = new();
			newBlock.SetColor(k_BaseColorId, _color);
			_colorID.SetPropertyBlock(newBlock);
		}
	}
}
