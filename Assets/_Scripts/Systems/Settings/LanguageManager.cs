using UnityEngine;

namespace rene_roid {    
    public class LanguageManager : MonoBehaviour
    {
        public TextAndString[] TextAndStrings;
        public string CurrentLanguage = "English";

        private void Awake() {
            DetectLanguage();
        }

        public void DetectLanguage() {
            if (PlayerPrefs.HasKey("Language")) {
                SetLanguage(PlayerPrefs.GetString("Language"));
                return;
            }

            for (int i = 0; i < TextAndStrings.Length; i++)
            {
                if (Application.systemLanguage == SystemLanguage.Spanish) {
                    TextAndStrings[i].TMP_text.text = TextAndStrings[i].esp;
                    CurrentLanguage = "Spanish";
                } else {
                    TextAndStrings[i].TMP_text.text = TextAndStrings[i].en;
                    CurrentLanguage = "English";
                }
            }
        }

        public void SetLanguage(string language) {
            for (int i = 0; i < TextAndStrings.Length; i++)
            {
                if (language == "Spanish")
                    TextAndStrings[i].TMP_text.text = TextAndStrings[i].esp;
                else
                    TextAndStrings[i].TMP_text.text = TextAndStrings[i].en;
            }

            CurrentLanguage = language;
        }
    }
}
