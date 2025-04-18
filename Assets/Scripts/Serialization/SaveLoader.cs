using Ionic.Zlib;
using System;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using ZeroPass;
using ZeroPass.Serialization;
using Debug = ZeroPass.Debug;

namespace Serialization
{
    public class SaveLoader : RMonoBehaviour
    {
        [NonSerialized]
        public SaveManager saveManager;

        private bool compressSaveData = true;

        public SaveGame.GameInfo GameInfo
        {
            get;
            private set;
        }

        public SaveGame.Header LoadedHeader
        {
            get;
            private set;
        }

        private static SaveLoader _instance;

        public static SaveLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject(nameof(SaveLoader));
                    DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<SaveLoader>();
                }

                return _instance;
            }
        }

        public static void DestroyInstance()
        {
            _instance.DeleteObject();
            _instance = null;
        }

        protected override void OnPrefabInit()
        {
            saveManager = gameObject.AddOrGet<SaveManager>();
        }

        public string Save(string filename, bool isAutoSave)
        {
            byte[] buffer = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    Save(writer);
                    buffer = ((!compressSaveData) ? memoryStream.ToArray() : CompressContents(memoryStream.GetBuffer(), (int)memoryStream.Length));
                }
            }
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    SaveGame.Header header;
                    byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, compressSaveData, out header);
                    binaryWriter.Write(header.buildVersion);
                    binaryWriter.Write(header.headerSize);
                    binaryWriter.Write(header.headerVersion);
                    binaryWriter.Write(header.compression);
                    binaryWriter.Write(saveHeader);
                    Manager.SerializeDirectory(binaryWriter);
                    binaryWriter.Write(buffer);
                }
            }
            catch (Exception ex3)
            {
                throw ex3;
            }
            GC.Collect();

            return filename;
        }

        private void Save(BinaryWriter writer)
        {
            writer.WriteRString("world");
            saveManager.Save(writer);
        }

        private bool Load(IReader reader)
        {
            string a = reader.ReadRString();
            Debug.Assert(a == "world");
            Deserializer deserializer = new Deserializer(reader);
            if (GameInfo.saveMajorVersion != 7)
            {
                if (GameInfo.saveMinorVersion >= 8)
                {
                }
            }
            if (!saveManager.Load(reader))
            {
                DebugUtil.LogWarningArgs("\n--- Error loading save ---\n");
                return false;
            }
            return true;
        }

        public bool Load(string filename)
        {
            try
            {
                Manager.Clear();
                byte[] array = File.ReadAllBytes(filename);
                IReader reader = new FastReader(array);
                GameInfo = SaveGame.GetHeader(reader, out SaveGame.Header header);
                LoadedHeader = header;
                DebugUtil.LogArgs(string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename));
                object[] obj = new object[1];
                object[] obj2 = new object[7];
                obj2[0] = GameInfo.baseName;
                obj2[1] = GameInfo.isAutoSave;
                obj2[2] = GameInfo.originalSaveName;
                obj2[3] = GameInfo.saveMajorVersion;
                obj2[4] = GameInfo.saveMinorVersion;
                obj[0] = string.Format("GameInfo: baseName:{0}, isAutoSave:{1}, originalSaveName:{2}, saveVersion:{3}.{4}", obj2);
                DebugUtil.LogArgs(obj);
                if (GameInfo.saveMajorVersion == 7)
                {
                    if (GameInfo.saveMinorVersion < 4)
                    {
                        Helper.SetTypeInfoMask((SerializationTypeInfo)191);
                    }
                }
                Manager.DeserializeDirectory(reader);
                if (header.IsCompressed)
                {
                    int num = array.Length - reader.Position;
                    byte[] array2 = new byte[num];
                    Array.Copy(array, reader.Position, array2, 0, num);
                    byte[] bytes = DecompressContents(array2);
                    IReader reader2 = new FastReader(bytes);
                    Load(reader2);
                }
                else
                {
                    Load(reader);
                }
                if (GameInfo.isAutoSave)
                {
                    if (!string.IsNullOrEmpty(GameInfo.originalSaveName))
                    {
                        SaveGame.GameInfo gameInfo12 = GameInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogWarningArgs("\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace);
                return false;
            }
            DebugUtil.LogArgs("Loaded", "[" + filename + "]");
            GC.Collect();
            return true;
        }

        public static byte[] CompressContents(byte[] uncompressed, int length)
        {
            using (MemoryStream memoryStream = new MemoryStream(length))
            {
                using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed))
                {
                    zlibStream.Write(uncompressed, 0, length);
                    zlibStream.Flush();
                }
                memoryStream.Flush();
                return memoryStream.ToArray();
            }
        }

        public static byte[] DecompressContents(byte[] compressed)
        {
            return ZlibStream.UncompressBuffer(compressed);
        }

        public static string GetSavePrefixAndCreateFolder()
        {
            string savePrefix = GetSavePrefix();
            if (!Directory.Exists(savePrefix))
            {
                Directory.CreateDirectory(savePrefix);
            }
            return savePrefix;
        }

        public static string GetSavePrefix()
        {
            string path = Util.RootFolder();
            return Path.Combine(path, "save_files/");
        }
    }
}