using UnityEngine;

namespace rene_roid {
    public class input : MonoBehaviour
    {
        void Start()
        {
            
        }

        void Update()
        {
            // print the input that is being pressed
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                        print("KeyCode down: " + kcode);
                }
            }
        }
    }
}
