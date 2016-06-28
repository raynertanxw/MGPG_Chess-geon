using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageParticles : MonoBehaviour
{
	#region Object Pooling
	private static List<DamageParticles> particlePool = null;

	private void Awake()
	{
		if (particlePool == null)
		{
			particlePool = new List<DamageParticles>();
		}

		Setup();
		particlePool.Add(this);
	}

	private void OnDestroy()
	{
		particlePool.Remove(this);
		if (particlePool.Count == 0)
		{
			particlePool = null;
		}
	}

	public static DamageParticles Spawn(Vector3 _pos)
	{
		DamageParticles curParticles = null;

		for (int i = 0; i < particlePool.Count; i++)
		{
			if (particlePool[i].ParticleSys.IsAlive() == false)
			{
				curParticles = particlePool[i];

				curParticles.transform.position = _pos;
				curParticles.ParticleSys.Play();
				break;
			}
		}

		#if UNITY_EDITOR
		if (curParticles == null)
			Debug.LogWarning("EnemyPiece pool out of objects. Consider increasing pool size");
		#endif

		return curParticles;
	}
	#endregion

	private ParticleSystem mParticleSys;
	public ParticleSystem ParticleSys { get { return mParticleSys; } }

	private void Setup()
	{
		mParticleSys = gameObject.GetComponent<ParticleSystem>();
		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}
}
