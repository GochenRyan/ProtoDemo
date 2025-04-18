using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using ZeroPass;
using ZeroPass.Serialization;

namespace Serialization
{
    [SerializationConfig(ZeroPass.Serialization.MemberSerialization.OptIn)]
    public class SaveGame : RMonoBehaviour, ISaveLoadable
    {
        private static SaveGame _instance;

        public static SaveGame Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject(nameof(SaveGame));
                    DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<SaveGame>();
                    RPrefabID rPrefabID = gameObject.AddOrGet<RPrefabID>();
                    rPrefabID.PrefabTag = TagManager.Create(nameof(SaveGame), nameof(SaveGame));
                    gameObject.AddComponent<SaveLoadRoot>();
                }

                return _instance;
            }
        }

        public struct Header
        {
            public uint buildVersion;

            public int headerSize;

            public uint headerVersion;

            public int compression;

            public bool IsCompressed => 0 != compression;
        }

        public struct GameInfo
        {
            public string baseName;

            public bool isAutoSave;

            public string originalSaveName;

            public int saveMajorVersion;

            public int saveMinorVersion;

            public GameInfo(string baseName, bool isAutoSave)
            {

                this.baseName = baseName;
                this.isAutoSave = isAutoSave;
                this.originalSaveName = string.Empty;
                saveMajorVersion = 7;
                saveMinorVersion = 11;
            }
        }

        public byte[] GetSaveHeader(bool isAutoSave, bool isCompressed, out Header header)
        {
            string text = null;
            string baseName = "test";
            text = JsonConvert.SerializeObject(new GameInfo(baseName, isAutoSave));
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            header = default(Header);
            header.buildVersion = 365655u;
            header.headerSize = bytes.Length;
            header.headerVersion = 1u;
            header.compression = (isCompressed ? 1 : 0);
            return bytes;
        }

        public static Header GetHeader(BinaryReader br)
        {
            Header result = default(Header);
            result.buildVersion = br.ReadUInt32();
            result.headerSize = br.ReadInt32();
            result.headerVersion = br.ReadUInt32();
            if (1 <= result.headerVersion)
            {
                result.compression = br.ReadInt32();
            }
            return result;
        }

        public static GameInfo GetHeader(IReader br, out Header header)
        {
            header = default(Header);
            header.buildVersion = br.ReadUInt32();
            header.headerSize = br.ReadInt32();
            header.headerVersion = br.ReadUInt32();
            if (1 <= header.headerVersion)
            {
                header.compression = br.ReadInt32();
            }
            byte[] data = br.ReadBytes(header.headerSize);
            return GetGameInfo(data);
        }

        public static GameInfo GetGameInfo(byte[] data)
        {
            return JsonConvert.DeserializeObject<GameInfo>(Encoding.UTF8.GetString(data));
        }
    }
}