using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static RotateExtension;
using Random = UnityEngine.Random;

public class RotateModule : MonoBehaviour
{
    public ConstantRotate ConstantRotateModule;
    public RandomRotation RandomRotationModule;

    private void Start()
    {
        if (RandomRotationModule.active && RandomRotationModule.rotateOnSpawn) RandomRotationModule.Rotate();
    }

    private void FixedUpdate()
    {
        if (ConstantRotateModule.active) ConstantRotateModule.Rotate();
    }
}

public static class RotateExtension
{

    [Serializable]
    public struct ConstantRotate
    {
        [SerializeField] public bool active;
        [SerializeField] Vector3 rotation;
        [SerializeField] Transform transform;

        public void Rotate() => transform.Rotate(rotation); 
    }

    [Serializable]
    public struct RandomRotation
    {
        [SerializeField] public bool active;
        public bool x, y, z;
        public bool affectAllChildren;
        [SerializeField] Transform transform;
        [ShowIf("x")] public Deviatable xRot;
        [ShowIf("y")] public Deviatable yRot;
        [ShowIf("z")] public Deviatable zRot;
        

        public bool rotateOnSpawn;

        public void Rotate()
        {
            if (affectAllChildren) { RotateChildren(); return; }
            Vector3 r = transform.eulerAngles;

            if (x) r.x = xRot.value;
            if (y) r.y = yRot.value;
            if (z) r.z = zRot.value;

            transform.rotation = Quaternion.Euler(r);
        }

        void RotateChildren()
        {
            foreach(Transform child in transform)
            {
                Vector3 r = child.eulerAngles;

                if (x) r.x = xRot.value;
                if (y) r.y = yRot.value;
                if (z) r.z = zRot.value;

                child.rotation = Quaternion.Euler(r);
            }

        }

    }

}
