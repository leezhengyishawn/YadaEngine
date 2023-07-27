using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class Particle : Component
    {
        public float currLifeTime = 0.0f;
        public float endLifeTime = 0.0f;

        public bool particleActive = false;

        private float initialLifeTime = 0.0f;
        private Transform m_transform;
        private Renderer  m_renderer;
        private RigidBody m_rigidBody;
        private Collider m_collider;

        private Vector3 m_startScale;
        private Vector3 m_endScale;

        private Vector3 forceDirection;
        private Vector3 forceToApply;
        private Vector3 m_direction;
        private Vector3 m_velocity;

        void Start()
        {
            m_transform = this.entity.GetComponent<Transform>();
            m_renderer  = this.entity.GetComponent<Renderer>();
            m_rigidBody = this.entity.GetComponent<RigidBody>();
            m_collider = this.entity.GetComponent<Collider>();

            initialLifeTime = currLifeTime;

            m_rigidBody.AddForce(m_rigidBody.mass * m_velocity);
        }

        public void FixedUpdate()
        {
            currLifeTime += Time.deltaTime;

            if (currLifeTime >= endLifeTime)
            {
                particleActive = false;
                this.entity.active = false;
                m_rigidBody.active = false;
                m_collider.active = false;
                return;
            }

            float ratio = (currLifeTime / endLifeTime);

            //m_rigidBody.linearVelocity = m_rigidBody.mass * m_velocity * 30.0f;

            m_transform = this.entity.GetComponent<Transform>();
            m_transform.globalScale = Lerp(ratio, m_startScale, m_endScale);

            if (m_transform.globalScale.x <= 0.0f &&
                m_transform.globalScale.y <= 0.0f &&
                m_transform.globalScale.z <= 0.0f)
            {
                particleActive = false;
                this.entity.active = false;
                m_rigidBody.active = false;
                m_collider.active = false;
            }
        }

        public void SetForceDirection(Vector3 dir)
        {
            forceDirection = dir;
        }

        public void SetForce(Vector3 force)
        {
            forceToApply = force;
        }

        public void SetDirection(Vector3 dir)
        {
            m_direction = dir;
        }

        public void SetVelocity(Vector3 vel)
        {
            m_velocity = vel;
        }

        public void SetScale(Vector3 start, Vector3 end)
        {
            m_startScale = start;
            m_endScale = end;
        }
        Vector3 Lerp(float ratio, Vector3 start, Vector3 end)
        {
            //Console.WriteLine(ratio);
            return (end - start) * ratio + start;
        }
    }
}
