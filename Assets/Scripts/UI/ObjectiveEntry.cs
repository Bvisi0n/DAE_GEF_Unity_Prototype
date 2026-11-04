using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Feature - Change _tailText into a percentage.
// Or even better a visual that fills up! (better for cognitive load)

namespace Assets.Scripts.UI
{
	public sealed class ObjectiveEntry : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _instructionText;
		[SerializeField] private TMP_Text _statusText;
		[SerializeField] private TMP_Text _tailText;

		private void Awake()
		{
			if (_icon == null)
			{
				Debug.LogError("Icon not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_instructionText == null)
			{
				Debug.LogError("Instruction Text not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_statusText == null)
			{
				Debug.LogError("Status Text not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_tailText == null)
			{
				Debug.LogError("Tail Text not assigned, script disabled.", this);
				enabled = false;
				return;
			}
		}

		public void Initialize(string instruction, int remainingObjectives)
		{
			_instructionText.text = instruction;
			_statusText.text = $"{remainingObjectives} remaining";
			_tailText.text = $"{remainingObjectives}";
		}

		public void Initialize(Sprite icon, string instruction, int remainingObjectives)
		{
			_icon.sprite = icon;
			Initialize(instruction, remainingObjectives);
		}

		public void UpdateContent(int remainingObjectives)
		{
			// TODO: Feature - Make it fancy when remainingObjectives == 0
			// Icon: A completed checkmark
			// Instruction: Strikethrough
			// Status: Completed and green
			// Tail: Empty?

			_statusText.text = $"{remainingObjectives} remaining";
			_tailText.text = $"{remainingObjectives}";
		}
	}
}
