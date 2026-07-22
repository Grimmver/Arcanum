using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	[Header("Audio Manager")]
	[SerializeField] private string playOnStartAudioProfile = "Default Background Musics";

	[Header("Sources")]
	[SerializeField] private AudioSource soundSource;
	[SerializeField] private AudioSource musicSource;

	[Header("Profiles")]
	[SerializeField] private List<AudioProfile> soundProfiles;
	[SerializeField] private List<AudioProfile> musicProfiles;

    [Header("Pitch range")]
    [SerializeField] private float minSoundPitch = 0.95f;
    [SerializeField] private float maxSoundPitch = 1.05f;

    private const string SOUND_PREF = "Sound";
	private const string MUSIC_PREF = "Music";

	private AudioProfile currentMusicProfile;
	private Coroutine currentMusicCoroutine;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		UpdateVolumes();
	}

	private void Start()
	{
		PlayMusic(playOnStartAudioProfile);
	}

	private void OnEnable() => PlayerPrefsEventBus.OnFloatChanged += OnVolumeChanged;
	private void OnDisable() => PlayerPrefsEventBus.OnFloatChanged -= OnVolumeChanged;

	private void OnVolumeChanged(string key, float value)
	{
		if (key == SOUND_PREF) soundSource.volume = value;
		else if (key == MUSIC_PREF) musicSource.volume = value;
	}

	private void UpdateVolumes()
	{
		soundSource.volume = PlayerPrefsEventBus.GetFloat(SOUND_PREF, 1f);
		musicSource.volume = PlayerPrefsEventBus.GetFloat(MUSIC_PREF, 1f);
	}

	public void PlayMusic(AudioClip clip)
	{
		if (clip == null) return;

		musicSource.clip = clip;
		musicSource.Play();
	}

	public void PlayMusic(string name)
	{
		currentMusicProfile = musicProfiles.Find(p => p.Name == name);
		if (currentMusicProfile == null) return;

		currentMusicProfile.SetRandomStartIndex();

		if (currentMusicCoroutine != null)
			StopCoroutine(currentMusicCoroutine);

		currentMusicCoroutine = StartCoroutine(HandleMusicProfilePlayback(currentMusicProfile));
	}

	public void StopMusic()
	{
		musicSource.Stop();
		if (currentMusicCoroutine != null) StopCoroutine(currentMusicCoroutine);
		currentMusicCoroutine = null;
	}

	public void NextMusic()
	{
		if (musicProfiles.Count == 0) return;

		AudioProfile randomProfile = musicProfiles[Random.Range(0, musicProfiles.Count)];
		PlayMusic(randomProfile.GetRandomClip());
	}

	public void PlaySound(AudioClip clip)
	{
		if (clip == null) return;

        float pitch = (Mathf.Approximately(minSoundPitch, maxSoundPitch))
                        ? minSoundPitch                          // fixed value
                        : Random.Range(minSoundPitch, maxSoundPitch);

        soundSource.pitch = pitch;          // one assignment per clip
        soundSource.PlayOneShot(clip);
    }

	public void PlaySound(string name)
	{
		AudioProfile profile = soundProfiles.Find(p => p.Name == name);
		if (profile != null)
			PlaySound(profile.GetRandomClip());
	}

	private IEnumerator HandleMusicProfilePlayback(AudioProfile profile)
	{
		switch (profile.Type)
		{
			case AudioProfileType.Next:
				while (true)
				{
					AudioClip nextClip = profile.GetNextClip();
					if (nextClip == null) yield break;

					musicSource.loop = false;
					musicSource.clip = nextClip;
					musicSource.Play();

					yield return new WaitWhile(() => musicSource.isPlaying);
				}

			case AudioProfileType.Repeat:
				{
					AudioClip firstClip = profile.GetRandomClip();
					if (firstClip == null) yield break;

					musicSource.loop = true;
					musicSource.clip = firstClip;
					musicSource.Play();
					yield break;
				}

			case AudioProfileType.Once:
				{
					AudioClip randomClip = profile.GetRandomClip();
					if (randomClip == null) yield break;

					musicSource.loop = false;
					musicSource.clip = randomClip;
					musicSource.Play();

					yield return new WaitWhile(() => musicSource.isPlaying);
					yield break;
				}
		}
	}
}
