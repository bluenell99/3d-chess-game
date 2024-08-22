    using Sirenix.OdinInspector;
    using UnityEngine;
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioService : Service
    {

        [SerializeField] private AudioClip[] _pieceMoveClips;

        [SerializeField, Required] private AudioSource _source;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (!_source)
            {
                _source = GetComponent<AudioSource>();
            }
        }


        public void OnPieceMove()
        {
            PlayRandomClip(_pieceMoveClips);
        }

        private void PlayRandomClip(AudioClip[] clips)
        {
            int random = Random.Range(0, _pieceMoveClips.Length);
            _source.PlayOneShot(clips[random]);
        }

    }
