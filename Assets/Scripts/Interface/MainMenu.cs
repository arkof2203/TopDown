using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Netologia.TowerDefence.Interface
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _source;
		
		[SerializeField, Space]
		private Button _startGameButton;
		[SerializeField]
		private Slider _audioScroll;
		[SerializeField]
		private Button _exitButton;

		private void NewGame()
			=> SceneManager.LoadScene(1);

		private void OnVolumeChanged(float value)
			=> _source.volume = value;

		private void ExitGame() =>
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif

		private void Awake()
		{
			_startGameButton.onClick.AddListener(NewGame);
			_audioScroll.onValueChanged.AddListener(OnVolumeChanged);
			_exitButton.onClick.AddListener(ExitGame);
			
			_audioScroll.value = PlayerPrefs.GetFloat(AudioManager.c_volumePresetKey, 1f);
			_source.Play();
		}

		private void OnDestroy()
		{
			_startGameButton.onClick.RemoveListener(NewGame);
			_audioScroll.onValueChanged.RemoveListener(OnVolumeChanged);
			_exitButton.onClick.RemoveListener(ExitGame);
			
			PlayerPrefs.SetFloat(AudioManager.c_volumePresetKey, _audioScroll.value);
		}
	}
}