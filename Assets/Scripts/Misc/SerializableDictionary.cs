using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> pairs = new List<SerializableKeyValuePair<TKey, TValue>>();
    private List<SerializableKeyValuePair<TKey, TValue>> keyValuePairs = new List<SerializableKeyValuePair<TKey, TValue>>();

    public void OnBeforeSerialize() {
        keyValuePairs.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this) {
            keyValuePairs.Add(new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
        }
    }

    public void OnAfterDeserialize() {
        this.Clear();

        for (int i = 0; i < keyValuePairs.Count; i++)
            this.Add(keyValuePairs[i].Key, keyValuePairs[i].Value);
    }
}

[Serializable]
public class SerializableKeyValuePair<TKey, TValue> {

    public TKey Key;
    public TValue Value;

    public SerializableKeyValuePair(TKey key, TValue value) {
        this.Key = key;
        this.Value = value;
    }
}