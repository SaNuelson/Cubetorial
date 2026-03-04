using System;
using UnityEngine;

namespace Cube.Base
{
    public abstract class Cube<TAxis> : MonoBehaviour
        where TAxis : Enum
    {

        public abstract void Rotate(TAxis axis);

    }
}