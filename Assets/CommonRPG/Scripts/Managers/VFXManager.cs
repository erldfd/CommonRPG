using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public enum EVFXName
    {
        DragonUsurperFlame_Ground
    }

    public class VFXManager : MonoBehaviour
    {
        [SerializeField]
        private List<ParticleSystem> vfxPrefabList = new List<ParticleSystem>();

        //private Dictionary<EVFXName, Queue<ParticleSystem>> activatedVFXTable = new Dictionary<EVFXName, Queue<ParticleSystem>>();
        private Dictionary<EVFXName, Queue<ParticleSystem>> deactivatedVFXTable = new Dictionary<EVFXName, Queue<ParticleSystem>>();

        /// <summary>
        /// string : gameobject name
        /// </summary>
        private Dictionary<string, EVFXName> vfxNameTable = new Dictionary<string, EVFXName>();

        private void Awake()
        {
            int vfxPrefabListCount = vfxPrefabList.Count;

            for (int i = 0; i < vfxPrefabListCount; ++i)
            {
                if (deactivatedVFXTable.TryAdd((EVFXName)i, new Queue<ParticleSystem>()) == false)
                {
                    Debug.LogAssertion("DeactivatedVFXTable init failed");
                }

                if (vfxNameTable.TryAdd(vfxPrefabList[i].name, (EVFXName)i) == false)
                {
                    Debug.LogAssertion("Fail to add vfx");
                }
            }

            //InitVFXTable();
        }

        /// <summary>
        /// depawn vfx automatically in autoDespawnTime.
        /// </summary>
        public void SpawnVFX(Vector3 position, Quaternion rotation, EVFXName vfxName, float autoDespawnTime)
        {
            ParticleSystem vfx = SpawnVFX(position, rotation, vfxName);

            GameManager.TimerManager.SetTimer(autoDespawnTime, 0, 0, () =>
            {
                DespawnVFX(vfx);

            }, true);
        }

        public ParticleSystem SpawnVFX(Vector3 position, Quaternion rotation, EVFXName vfxName)
        {
            Debug.Assert(vfxPrefabList[(int)vfxName]);

            ParticleSystem particleSystem = null;

            if (deactivatedVFXTable[vfxName].Count > 0)
            {
                particleSystem = deactivatedVFXTable[vfxName].Dequeue();
            }
            else
            {
                particleSystem = Instantiate(vfxPrefabList[(int)vfxName]);
            }

            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            return particleSystem;
        }

        public ParticleSystem SpawnVFX(Transform parentTramsform, EVFXName vfxName, bool shouldAttach)
        {
            Debug.Assert(vfxPrefabList[(int)vfxName]);

            ParticleSystem particleSystem = SpawnVFX(parentTramsform.position, parentTramsform.rotation, vfxName);

            if (shouldAttach) 
            {
                particleSystem.transform.SetParent(parentTramsform);
            }

            return particleSystem;
        }

        public void DespawnVFX(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);

            EVFXName vFXName = vfxNameTable[particle.name[..^7]]; // same particle.name.Substring(0, particle.name.Length - 7)

            if (particle.transform.parent != null) 
            {
                particle.transform.SetParent(null);
            }

            deactivatedVFXTable[vFXName].Enqueue(particle);
        }

        public void InitVFXTable()
        {
            deactivatedVFXTable.Clear();

            int vfxPrefabListCount = vfxPrefabList.Count;

            for (int i = 0; i < vfxPrefabListCount; ++i)
            {
                if (deactivatedVFXTable.TryAdd((EVFXName)i, new Queue<ParticleSystem>()) == false)
                {
                    Debug.LogAssertion("DeactivatedVFXTable init failed");
                }

                //if (vfxNameTable.TryAdd(vfxPrefabList[i].name, (EVFXName)i) == false)
                //{
                //    Debug.LogAssertion("Fail to add vfx");
                //}
            }
        }
    }
}
