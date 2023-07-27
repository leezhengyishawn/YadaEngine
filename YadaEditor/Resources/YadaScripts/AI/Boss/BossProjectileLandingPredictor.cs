using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BossProjectileLandingPredictor : Component
    {
        public Entity connectedProjectile;
        RigidBody projectileRB;
        Transform projectileTransform;
        Collider projectileCollider;
        private float time = 0;
        void Start()
        {
            projectileRB = connectedProjectile.GetComponent<RigidBody>();
            projectileTransform = connectedProjectile.GetComponent<Transform>();
            projectileCollider = connectedProjectile.GetComponent<Collider>();
        }

        public Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            return start + startVelocity * time + new Vector3(0, -9.8f, 0) * time * time * 0.5f;
        }

        void Update()
        {
            Vector3 newVelocity = new Vector3(projectileRB.linearVelocity.x, -(Math.Abs(projectileRB.linearVelocity.y)), projectileRB.linearVelocity.z);

            Vector3 position = new Vector3(projectileTransform.globalPosition.x, projectileTransform.globalPosition.y, projectileTransform.globalPosition.z);
            Vector3 newPos = position;

            //playerCollider.active = false;

            ColliderRaycastInfoMulti ray;
            ray.m_isHit = false;

            int countLoop = 100;
            while (!ray.m_isHit)
            {
                time += Time.deltaTime;
                newPos = new Vector3(PlotTrajectoryAtTime(position, newVelocity, time));
                ray = projectileCollider.RaycastColliderMulti(position, newPos);

                --countLoop;

                if (countLoop == 0) //temporary failsafe
                    break;
            }

            time = 0;

            this.entity.GetComponent<Transform>().globalPosition = new Vector3(newPos.x, newPos.y + 0.1f, newPos.z);

            //playerCollider.active = true;
            //if (connectedProjectile.active == false)
            //    this.entity.active = false;
        }
    }
}