using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityGameFeel
{ 
    public class SwipeData
    {
        public Vector2 start;
        public Vector2 end;

        public Vector2 midPoint()
        {
            Vector2 dir = end - start;
            float mag = Mathf.Abs((end - start).magnitude);

            return dir * mag;
        }
    }
    //For spawning a trail and following it
    public static class TrailHandlar
    {
        //Input - trail gameObject, vectorTofollow
        //Output trail component

        private static GameObject trailRenderer;
        private static Vector3 vectorToFollow;

        private static bool isTrailing;
        public static void trailStart(GameObject trail, Vector3 posToSpawn)
        {
            trailRenderer = GameObject.Instantiate(trail, posToSpawn, Quaternion.identity);

            isTrailing = true;
        }

        public static void trailUpdate(Vector3 follow)
        {
            vectorToFollow = follow;

            if (!isTrailing) return;

            trailRenderer.transform.position = vectorToFollow;
        }

        public static void trailEnd()
        {
            Object.Destroy(trailRenderer);
            isTrailing = false;
        }
    }

    //For Camera Shake
    public static class CameraShake
    {
        private static Camera _mainCam;
        private static float _duration;
        private static float _magnitude;

        public static IEnumerator Shake(float magnitude, float duration)
        {
            _duration = duration;
            _magnitude = magnitude;

            _mainCam = Camera.main;

            float _elapsedTime = 0.0f;
            Vector3 originalPos = _mainCam.transform.position;

            while(_elapsedTime < _duration)
            {
                float xShake = Random.Range(-1f, 1f) * _magnitude;
                float yShake = Random.Range(-1f, 1f) * _magnitude;

                Vector3 shakeBy = new Vector3(xShake, originalPos.y, originalPos.z);
                _mainCam.transform.position = originalPos + shakeBy;

                _elapsedTime += Time.deltaTime;

                yield return null;
            }

            _mainCam.transform.position = originalPos;
        }
    }

    public static class Whitening
    {
        static List<MeshRenderer> objectsToWhiten;
        static List<Material> objectsOriginalMat;

        public static void Whiten(MeshRenderer first, MeshRenderer second, Material whiteMat)
        {
            objectsToWhiten = new List<MeshRenderer>();
            objectsOriginalMat = new List<Material>();

            objectsToWhiten.Add(first);
            objectsToWhiten.Add(second);

            objectsOriginalMat.Add(first.material);
            objectsOriginalMat.Add(second.material);

            first.material = whiteMat;
            second.material = whiteMat;
        }
        public static void deWhiten()
        {
            /*for (int i = 0; i < objectsToWhiten.Count; i++)
            {
                objectsToWhiten[i].material = objectsOriginalMat[i];
            }
*/
            objectsToWhiten = new List<MeshRenderer>();
            objectsOriginalMat = new List<Material>();

        }
    }
}