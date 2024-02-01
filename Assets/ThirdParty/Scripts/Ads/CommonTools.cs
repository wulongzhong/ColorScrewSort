using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.Scripts.Common
{

    public class CommonTools
    {
        public static GameObject FindSceneRoot(string name)
        {
            foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go.name.Equals(name)) return go;
            }
            return null;
        }

        public static void ClearContainerGo(Transform container)
        {
            StableClearContainerGo(container);
        }


        public static List<Transform> GetAllChildren(Transform container)
        {
            List<Transform> res = new List<Transform>();
            for (int i = 0; i < container.childCount; i++)
            {
                res.Add(container.GetChild(i));
            }

            return res;
        }


        public static void StableClearContainerGo(Transform container)
        {
            GameObject destory = new GameObject("_Destory");
            //Debug.Log("Destory count: " + container.childCount);
            GameObject[] preGos = new GameObject[container.childCount];
            for (int i = 0; i < container.childCount; i++)
            {
                preGos[i] = container.GetChild(i).gameObject;
            }
            foreach (var pre in preGos)
            {
                pre.transform.SetParent(destory.transform);
            }

            destory.SetActive(false);
            GameObject.Destroy(destory);
        }


        public static int Clamp(int val, int min, int max)
        {
            if (val > max) return max;
            if (val < min) return min;
            return val;
        }

        public static float Remap(float inval, float inMin, float inMax, float outMin, float outMax)
        {
            if (inval <= inMin) return outMin;
            if (inval >= inMax) return outMax;
            return ((inval - inMin) * (outMax - outMin)) / (inMax - inMin) + outMin;
        }


        public static float LinearRemap(float inval, float inLeft, float inRight, float outLeft, float outRight)
        {
            if(inLeft <= inRight && outLeft <= outRight)
            {
                return Remap(inval, inLeft, inRight, outLeft, outRight);
            }

            else if(inLeft <= inRight && outRight < outLeft)
            {
                float inter = Remap(inval, inLeft, inRight, outRight, outLeft);
                return outLeft - (inter - outRight);
            }

           else  if(inRight < inLeft && outLeft <= outRight)
            {
                float inter = Remap(inval, inRight, inLeft, outLeft, outRight);
                return outLeft + (outRight - inter);
            }
            else
            {
                return Remap(inval, inRight, inLeft, outRight, outLeft);
            }
        }


        public static void MoveUp(Transform tran, float delta)
        {
            var pos = tran.position;
            pos.y += delta;
            tran.position = pos;
        }


        public static void QuickDestroy(Transform trans)
        {
            GameObject destory = new GameObject("_Destory");
            trans.SetParent(destory.transform);
            trans.gameObject.SetActive(false);
            GameObject.Destroy(destory);
        }


        public static void QuickDestroy(Transform pos, string name)
        {
            var tar = pos.Find(name);
            if(tar != null)
            {
                QuickDestroy(tar);
            }
        }

        public static void ChildrenUnactive(Transform trans)
        {
            List<Transform> ch = new List<Transform>();
            for (int i = 0; i < trans.childCount; i++)
            {
                ch.Add(trans.GetChild(i));
            }

            foreach (var t in ch)
            {
                t.gameObject.SetActive(false);
            }
        }

        public static Color AlphaColor(Color tar, float alpha)
        {
            var c = new Color(tar.r, tar.b, tar.g, alpha);
            return c;
        }


        public static long ConvertTimeStamp(DateTime time)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - start).TotalSeconds;
        }

        public static DateTime ConvertDateTime(long stamp)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return start.AddSeconds(stamp);
        }

        public static DateTime DefaultDateTime()
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        }

        public static long DefaultTimeStamp()
        {
            return ConvertTimeStamp(TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)));
        }
    }

}


