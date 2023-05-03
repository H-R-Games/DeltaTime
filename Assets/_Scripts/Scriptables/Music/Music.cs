using UnityEngine;

namespace rene_roid {
    [CreateAssetMenu(fileName = "New Music", menuName = "ScriptableObjects/New Music", order = 2)]
    public class Music : ScriptableObject
    {
        public string Name;
        public string Artist;
        public AudioClip Clip;
    }
}
