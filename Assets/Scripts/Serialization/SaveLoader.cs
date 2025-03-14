using System;
using ZeroPass;

namespace Serialization
{
    public class SaveLoader : RMonoBehaviour
    {
        [NonSerialized]
        public SaveManager saveManager;

        public static SaveLoader Instance
        {
            get;
            private set;
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

        protected override void OnPrefabInit()
        {
            Instance = this;
            saveManager = GetComponent<SaveManager>();
        }
    }
}