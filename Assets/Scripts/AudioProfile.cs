using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AudioProfileType
{
	Once,
	Repeat,
	Next
}

[Serializable]
public class AudioProfile
{
	[SerializeField] private string name;
	public string Name => name;

	[SerializeField] private List<AudioClip> clips;
	public IReadOnlyList<AudioClip> Clips => clips;

	[SerializeField] private AudioProfileType type;
	public AudioProfileType Type => type;

	private int currentClipIndex = -1;

	public AudioClip GetNextClip()
	{
		if (clips == null || clips.Count == 0) return null;

		currentClipIndex = (currentClipIndex + 1) % clips.Count;
		return clips[currentClipIndex];
	}

	public void ResetIndex()
	{
		currentClipIndex = -1;
	}

	public AudioClip GetRandomClip()
	{
		if (clips == null || clips.Count == 0) return null;
		return clips[Random.Range(0, clips.Count)];
	}

	public void SetRandomStartIndex()
	{
		if (clips == null || clips.Count == 0) return;
		currentClipIndex = Random.Range(0, clips.Count);
	}
}
