    using Sirenix.OdinInspector;
    using UnityEngine;

    namespace ChessGame
    {
        [RequireComponent(typeof(AudioSource))]

        public class AudioService : Service
        {

            [SerializeField] private AudioClip[] _pieceMoveClips;

            [SerializeField, Required] private AudioSource _source;

            protected override void Awake()
            {
                base.Awake();
                
                if (!_source)
                    _source = GetComponent<AudioSource>();
            }


            /// <summary>
            /// Plays a clip from the services PieceMovesClips array
            /// </summary>
            public void OnPieceMove()
            {
                PlayRandomClip(_pieceMoveClips);
            }

            /// <summary>
            /// Get a random clip from a collection of audio clips
            /// </summary>
            /// <param name="clips"></param>
            private void PlayRandomClip(AudioClip[] clips)
            {
                int random = Random.Range(0, _pieceMoveClips.Length);
                _source.PlayOneShot(clips[random]);
            }

        }

    }