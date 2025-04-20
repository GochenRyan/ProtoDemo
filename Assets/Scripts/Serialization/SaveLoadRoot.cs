using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using ZeroPass;
using ZeroPass.Serialization;
using Debug = ZeroPass.Debug;

namespace Serialization
{
    [SkipSaveFileSerialization]
    public class SaveLoadRoot : RMonoBehaviour
    {
        private bool hasOnSpawnRun;

        private bool registered = true;

        private static Dictionary<string, ISerializableComponentManager> serializableComponentManagers;

        public static void DestroyStatics()
        {
            serializableComponentManagers = null;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (registered)
            {
                SaveLoader.Instance.saveManager.Register(this);
            }
            hasOnSpawnRun = true;
        }

        protected override void OnPrefabInit()
        {
            if (serializableComponentManagers == null)
            {
                serializableComponentManagers = new Dictionary<string, ISerializableComponentManager>();
            }
        }

        public void SetRegistered(bool registered)
        {
            if (this.registered != registered)
            {
                this.registered = registered;
                if (hasOnSpawnRun)
                {
                    if (registered)
                    {
                        SaveLoader.Instance.saveManager.Register(this);
                    }
                    else
                    {
                        SaveLoader.Instance.saveManager.Unregister(this);
                    }
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            Transform transform = base.transform;
            writer.Write(transform.position);
            writer.Write(transform.rotation);
            writer.Write(transform.localScale);
            byte value = 0;
            writer.Write(value);
            SaveWithoutTransform(writer);
        }

        public void SaveWithoutTransform(BinaryWriter writer)
        {
            RMonoBehaviour[] components = GetComponents<RMonoBehaviour>();
            if (components != null)
            {
                int num = 0;
                RMonoBehaviour[] array = components;
                foreach (RMonoBehaviour rMonoBehaviour in array)
                {
                    if ((rMonoBehaviour is ISaveLoadableDetails || (object)rMonoBehaviour != null) && !rMonoBehaviour.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
                    {
                        num++;
                    }
                }
                foreach (KeyValuePair<string, ISerializableComponentManager> serializableComponentManager in serializableComponentManagers)
                {
                    ISerializableComponentManager value = serializableComponentManager.Value;
                    if (value.Has(base.gameObject))
                    {
                        num++;
                    }
                }
                writer.Write(num);
                RMonoBehaviour[] array2 = components;
                foreach (RMonoBehaviour rMonoBehaviour2 in array2)
                {
                    if ((rMonoBehaviour2 is ISaveLoadableDetails || (object)rMonoBehaviour2 != null) && !rMonoBehaviour2.GetType().IsDefined(typeof(SkipSaveFileSerialization), false))
                    {
                        writer.WriteRString(rMonoBehaviour2.GetType().ToString());
                        long position = writer.BaseStream.Position;
                        writer.Write(0);
                        long position2 = writer.BaseStream.Position;
                        if (rMonoBehaviour2 is ISaveLoadableDetails)
                        {
                            ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)rMonoBehaviour2;
                            Serializer.SerializeTypeless(rMonoBehaviour2, writer);
                            saveLoadableDetails.Serialize(writer);
                        }
                        else if ((object)rMonoBehaviour2 != null)
                        {
                            Serializer.SerializeTypeless(rMonoBehaviour2, writer);
                        }
                        long position3 = writer.BaseStream.Position;
                        long num2 = position3 - position2;
                        writer.BaseStream.Position = position;
                        writer.Write((int)num2);
                        writer.BaseStream.Position = position3;
                    }
                }
                foreach (KeyValuePair<string, ISerializableComponentManager> serializableComponentManager2 in serializableComponentManagers)
                {
                    ISerializableComponentManager value2 = serializableComponentManager2.Value;
                    if (value2.Has(base.gameObject))
                    {
                        string key = serializableComponentManager2.Key;
                        writer.WriteRString(key);
                        value2.Serialize(base.gameObject, writer);
                    }
                }
            }
        }

        public static SaveLoadRoot Load(Tag tag, IReader reader)
        {
            GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(tag);
            return Load(prefab, reader);
        }

        public static SaveLoadRoot Load(GameObject prefab, IReader reader)
        {
            Vector3 position = reader.ReadVector3();
            Quaternion rotation = reader.ReadQuaternion();
            Vector3 scale = reader.ReadVector3();
            reader.ReadByte();
            return Load(prefab, position, rotation, scale, reader);
        }

        public static SaveLoadRoot Load(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, IReader reader)
        {
            SaveLoadRoot saveLoadRoot = null;
            if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
            {
                GameObject gameObject = Util.RInstantiate(prefab, position, rotation, null, null, false, 0);
                gameObject.transform.localScale = scale;
                gameObject.SetActive(true);
                saveLoadRoot = gameObject.GetComponent<SaveLoadRoot>();
                if ((UnityEngine.Object)saveLoadRoot != (UnityEngine.Object)null)
                {
                    try
                    {
                        LoadInternal(gameObject, reader);
                        return saveLoadRoot;
                    }
                    catch (ArgumentException ex)
                    {
                        DebugUtil.LogErrorArgs(gameObject, "Failed to load SaveLoadRoot ", ex.Message, "\n", ex.StackTrace);
                        return saveLoadRoot;
                    }
                }
                Debug.Log("missing SaveLoadRoot", gameObject);
            }
            else
            {
                LoadInternal(null, reader);
            }
            return saveLoadRoot;
        }

        private static void LoadInternal(GameObject gameObject, IReader reader)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            RMonoBehaviour[] array = (!((UnityEngine.Object)gameObject != (UnityEngine.Object)null)) ? null : gameObject.GetComponents<RMonoBehaviour>();
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                string text = reader.ReadRString();
                int num2 = reader.ReadInt32();
                int position = reader.Position;
                if (serializableComponentManagers.TryGetValue(text, out ISerializableComponentManager value))
                {
                    value.Deserialize(gameObject, reader);
                }
                else
                {
                    int value2 = 0;
                    dictionary.TryGetValue(text, out value2);
                    RMonoBehaviour RMonoBehaviour = null;
                    int num3 = 0;
                    if (array != null)
                    {
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (array[j].GetType().ToString() == text)
                            {
                                if (num3 == value2)
                                {
                                    RMonoBehaviour = array[j];
                                    break;
                                }
                                num3++;
                            }
                        }
                    }
                    if ((UnityEngine.Object)RMonoBehaviour == (UnityEngine.Object)null)
                    {
                        reader.SkipBytes(num2);
                    }
                    else if ((object)RMonoBehaviour == null && !(RMonoBehaviour is ISaveLoadableDetails))
                    {
                        DebugUtil.LogErrorArgs("Component", text, "is not ISaveLoadable");
                        reader.SkipBytes(num2);
                    }
                    else
                    {
                        dictionary[text] = num3 + 1;
                        if (RMonoBehaviour is ISaveLoadableDetails)
                        {
                            ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)RMonoBehaviour;
                            Deserializer.DeserializeTypeless(RMonoBehaviour, reader);
                            saveLoadableDetails.Deserialize(reader);
                        }
                        else
                        {
                            Deserializer.DeserializeTypeless(RMonoBehaviour, reader);
                        }
                        if (reader.Position != position + num2)
                        {
                            DebugUtil.LogWarningArgs("Expected to be at offset", position + num2, "but was only at offset", reader.Position, ". Skipping to catch up.");
                            reader.SkipBytes(position + num2 - reader.Position);
                        }
                    }
                }
            }
        }
    }
}