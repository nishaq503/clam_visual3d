using System.Collections.Generic;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.CameraController3D
{
    public static class CameraList
    {
        #region Variables

        static List<CameraRig> cameras = new List<CameraRig>();

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Getters / Setters

        public static List<CameraRig> GetCameraRigList()
        {
            return cameras;
        }

        public static void Register(CameraRig camera)
        {
            if (!cameras.Contains(camera))
            {
                cameras.Add(camera);
                Debug.Log(camera.name + " registered. " + cameras.Count + " total world cameras");
            }
        }

        public static void Unregister(CameraRig camera)
        {
            if(cameras.Contains(camera))
            {
                cameras.Remove(camera);
            }
        }

        #endregion

    } //class end
}
