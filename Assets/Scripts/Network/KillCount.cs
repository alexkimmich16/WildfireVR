using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCount : MonoBehaviour
{

	#region Singleton

	public static KillCount instance;

	void Awake()
	{
		instance = this;
	}

	#endregion

	public int CiviliansKilledCount;
	public int ZombiesKilledCount;
	public Animation animation;
	public AudioSource soundClip;

	public bool playAnim = false;
	public bool playAudio = false;

	public bool PlayAnimationOneFrame = false;

	public void ZombieKilled()
	{
		ZombiesKilledCount += 1;
	}

	public void CivilianKilled()
	{
		CiviliansKilledCount += 1;
	}

	void Update()
	{
		//example
		//if more than 5 zombies are killed and under 2 civilians, execute a function
		if (ZombiesKilledCount > 5 && CiviliansKilledCount < 2)
		{
			//spawn people or load level ect

			//if play anim is true, and i haven't played animation yet
			if (PlayAnimationOneFrame == false)
            {
                if (playAnim == true)
                {
					animation.Play();
				}
				if (playAudio)
                {
					soundClip.Play();
				}
					
				//set this to true so its only triggered ONE TIME
				PlayAnimationOneFrame = true;
			}
				
			
		}

	}
}
