using System;
using Behaviours;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Netologia.TowerDefence.Behaviors
{
	public class InterfaceController : MonoBehaviour, Director.IManualUpdate
	{
		private AudioManager _audio;		//injected
		private WaveController _controller;	//injected
		
		private bool _inWave;
		
		
		[SerializeField, Header("---In Game UI---")]
		private TextMeshProUGUI _hp;
		[SerializeField]
		private TextMeshProUGUI _gold;
		[SerializeField]
		private TextMeshProUGUI _wave;
		[SerializeField]
		private TextMeshProUGUI _timer;

		[SerializeField, Header("---Menu Pause---")]
		private GameObject _pauseGamePanel;
		[SerializeField]
		private Button _resumeButton;
		[SerializeField]
		private Button _mainMenuButton;
		[SerializeField]
		private Slider _audioScroll;
		[SerializeField]
		private Button _openPauseButton;
		
		[SerializeField, Header("---End Game Panel---")]
		private GameObject _endGamePanel;
		[SerializeField]
		private GameObject _winText;
		[SerializeField]
		private GameObject _loseText;
		[SerializeField]
		private Button _endGameButton;

		public void SetHP(int value)
			=> _hp.text = value.ToString();
		
		public void SetGold(float value)
			=> _gold.text = Mathf.RoundToInt(value).ToString();

		public void ManualUpdate()
		{
			if (_inWave != _controller.InWave)
			{
				_inWave = _controller.InWave;
				_wave.text = $"{_controller.Data.Wave + 1} / {_controller.WaveCount}";
			}
			else _timer.text = string.Empty;
			
			if (!_inWave)
				_timer.text = Mathf.RoundToInt(_controller.Delay).ToString();
		}
		
		public void GameLose()
		{
			TimeManager.IsGame = false;
			_hp.text = 0.ToString();
			_endGamePanel.SetActive(true);
			_loseText.SetActive(true);
			_winText.SetActive(false);
		}
		
		public void GameWin()
		{
			TimeManager.IsGame = false;
			_endGamePanel.SetActive(true);
			_loseText.SetActive(false);
			_winText.SetActive(true);
		}

		private void LoadMainMenu()
			=> SceneManager.LoadScene(0);

		private void OpenMenuPause()
		{
			_pauseGamePanel.SetActive(true);
			TimeManager.IsGame = false;
			_openPauseButton.interactable = false;
		}

		private void CloseMenuPause()
		{
			_pauseGamePanel.SetActive(false);
			TimeManager.IsGame = true;
			_openPauseButton.interactable = true;
		}
		
		private void OnSoundChanged(float value)
			=> _audio.Volume = value;

		private void Awake()
		{
			_resumeButton.onClick.AddListener(CloseMenuPause);
			_mainMenuButton.onClick.AddListener(LoadMainMenu);
			_audioScroll.onValueChanged.AddListener(OnSoundChanged);
			_openPauseButton.onClick.AddListener(OpenMenuPause);
			_endGameButton.onClick.AddListener(LoadMainMenu);

			_audioScroll.value = PlayerPrefs.GetFloat(AudioManager.c_volumePresetKey, 1f);
			_endGamePanel.SetActive(false);
			_pauseGamePanel.SetActive(false);
		}
		
		private void OnDestroy()
		{
			_resumeButton.onClick.RemoveListener(CloseMenuPause);
			_mainMenuButton.onClick.RemoveListener(LoadMainMenu);
			_audioScroll.onValueChanged.RemoveListener(OnSoundChanged);
			_openPauseButton.onClick.RemoveListener(OpenMenuPause);
			_endGameButton.onClick.RemoveListener(LoadMainMenu);
		}

		[Inject]
		private void Construct(AudioManager audio, WaveController wave)
			=> (_audio, _controller) = (audio, wave);
	}
}