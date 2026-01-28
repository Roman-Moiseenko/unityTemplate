using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class ExplosionBinder : MonoBehaviour
    {

        [SerializeField] private ParticleSystem sparks;
        [SerializeField] private ParticleSystem flash;
        [SerializeField] private ParticleSystem fire;
        [SerializeField] private ParticleSystem smoke;
        public void Bind()
        {
        }

        public void Play(Vector3 position)
        {
            transform.position = position;
            sparks.Play();
            flash.Play();
            fire.Play();
            smoke.Play();
        }

        public void Stop()
        {
            sparks.Stop();
            flash.Stop();
            fire.Stop();
            smoke.Stop();
        }
    }
}