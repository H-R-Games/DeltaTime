using UnityEngine;
using System;
using System.IO;

namespace hr_saves
{
    public class SaveSystem : MonoBehaviour
    {
        private void Awake()
        {
            CreateFile();
        }
        
        private void CreateFile()
        {
            string path = Application.persistentDataPath + "/notasavefile.json";
            if (!File.Exists(path)) File.Create(path);
        }
    }
}
