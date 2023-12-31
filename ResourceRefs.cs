using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynamic_Lights
{
    internal static class ResourceRefs
    {
        public static Material paperOffMat;
        public static Material paperOffMatYellow;
        public static Material paperOffMatGreen;
        public static Material paperOffMatBlue;
        public static Material paperOffMatRed;

        public static void CreateMaterials()
        {

            if (paperOffMat == null) paperOffMat = new Material(Shader.Find("Standard"));
            if (paperOffMat != null) Debug.Log("found material: " + paperOffMat);
            else Debug.Log("failed to load paper");

            if (paperOffMatYellow == null)
            {
                paperOffMatYellow = new Material(ResourceRefs.paperOffMat)
                {
                    color = new Color(0.7075471f, 0.5587394f, 0.4305358f, 1),
                    name = "lamp yellow off"
                };
            }

            if (paperOffMatGreen == null)
            {
                paperOffMatGreen = new Material(ResourceRefs.paperOffMat)
                {
                    color = new Color(0.4060608f, 0.6886792f, 0.4331612f, 1),
                    name = "lamp green off"
                };
            }

            if (paperOffMatBlue == null)
            {
                paperOffMatBlue = new Material(ResourceRefs.paperOffMat)
                {
                    color = new Color(0.3676575f, 0.760615f, 0.9622641f, 1),
                    name = "lamp blue off"
                };
            }

            if (paperOffMatRed == null)
            {
                paperOffMatRed = new Material(ResourceRefs.paperOffMat)
                {
                    color = new Color(0.8018868f, 0.3758817f, 0.366901f, 1),
                    name = "lamp red off"
                };
            }
        }
    }
}
