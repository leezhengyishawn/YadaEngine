using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class TreeSway : Component
    {
        private Transform transform;

        private Quaternion originalRotation;
        
        void Start()
        {
            transform = this.entity.GetComponent<Transform>();

            originalRotation = transform.localRotation;
        }

        void Update()
        {
            //transform.localRotation = Quaternion.CreateFromAxisAngle(new Vector3(Wind.getXDir(), 0, Wind.getZDir()), Wind.currWindStrength) * originalRotation;
            transform.localRotation = Wind.windAngle * originalRotation;
        }
    }
}
