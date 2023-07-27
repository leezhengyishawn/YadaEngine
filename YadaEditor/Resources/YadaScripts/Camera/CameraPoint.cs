using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class CameraPoint : Component
	{
		private Transform myTransform;

		void Start()
        {
			this.entity.GetComponent<Renderer>().active = false;
			myTransform = this.entity.GetComponent<Transform>();
        }

		void Update()
        {
			myTransform.globalPosition = SceneController.middlePoint + (Vector3.up * 0.5f);
		}
	}
}