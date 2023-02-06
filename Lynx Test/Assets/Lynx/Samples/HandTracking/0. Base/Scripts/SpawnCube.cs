/**
 * @file SpawnCube.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Simple script to spawn a prefab at given position.
 */
using System;
using UnityEngine;

namespace lynx
{
    public class SpawnCube : MonoBehaviour
    {
        [SerializeField] private GameObject m_prefab = null;

        public void SpawnCubeAtPosition(Vector3 pos)
        {
            GameObject obj = Instantiate(m_prefab);
            Vector3 newPos = pos;
            newPos.y += 0.1f;

            obj.transform.position = pos;
        }
    }
}