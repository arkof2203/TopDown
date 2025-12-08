using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Netologia.TowerDefence.Interface
{
	public class TowerButton : MonoBehaviour
	{
		[SerializeField]
		private Image _image;
		[SerializeField]
		private TextMeshProUGUI _nameText;
		[SerializeField]
		private TextMeshProUGUI _descriptionText; 
		[SerializeField]
		private TextMeshProUGUI _costText;
		
		[field: SerializeField]
		public Button Button { get; private set; }
		
		public Sprite Icon
		{
			get => _image.sprite;
			set => _image.sprite = value;
		}
		
		public string Name
		{
			get => _nameText.text;
			set => _nameText.text = value;
		}
		
		public string Description
		{
			get => _descriptionText.text;
			set => _descriptionText.text = value;
		}
		
		public int Cost
		{
			get => int.Parse(_costText.text);
			set => _costText.text = value.ToString();
		}
	}
}