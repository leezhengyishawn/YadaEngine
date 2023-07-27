using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BossProjectile : Component
    {
        float explosionRadius = 2.0f;
        public BossProjectileLandingPredictor landing;
        Transform projectileTransform;
        void Start()
        {
            projectileTransform = this.entity.GetComponent<Transform>();
            Entity predictor = Entity.InstantiatePrefab("BossProjectileLandIndicator");
            landing = predictor.GetComponent<BossProjectileLandingPredictor>();
            landing.connectedProjectile = this.entity;
        }

        void OnCollisionEnter(Entity collider)
        {
            if (collider.GetComponent<PunchCheck>() != null)
                return;

            if (collider.tag == "Solid")
            {
                //Spawn explosion particles
                Entity fart = Entity.InstantiatePrefab("BossProjectileExplosion");
                AudioController.PlaySFX("SFX_Poof_Basic");
                fart.GetComponent<Transform>().globalPosition = projectileTransform.globalPosition;

                //Check point of impact distance from p1 and p2, if within range, deal enough dmg to stun the players
                Vector3 dispFromP1 = SceneController.mainPlayer1.GetComponent<Transform>().globalPosition - projectileTransform.globalPosition;
                Vector3 dispFromP2 = SceneController.mainPlayer2.GetComponent<Transform>().globalPosition - projectileTransform.globalPosition;
                if (dispFromP1.magnitudeSq <= explosionRadius * explosionRadius)
                    SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().ReceivePunch(1, dispFromP1, SceneController.punchForceNormal, SceneController.punchForceNormal, false);
                if (dispFromP2.magnitudeSq <= explosionRadius * explosionRadius)
                    SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().ReceivePunch(1, dispFromP2, SceneController.punchForceNormal, SceneController.punchForceNormal, false);

                //this.entity.active = false;
                Entity.DestroyEntity(landing.entity);
                Entity.DestroyEntity(this.entity);
            }
        }
    }
}
