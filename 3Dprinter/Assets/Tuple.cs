using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T, U> {
    public readonly T Key;
    public readonly U Value;

    public Tuple(T key, U value) {
        Key = key;
        Value = value;
    }
}

public class WriteTuple<T, U> {
    public readonly T Key;
    public U Value { get; set; }

    public WriteTuple(T key, U value) {
        Key = key;
        Value = value;
    }
}
