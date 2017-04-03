using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T, U> {
    public T Key { get; set; }
    public U Value { get; set; }

    public Tuple(T key, U value) {
        Key = key;
        Value = value;
    }
}

public static class Tuple {
    public static Tuple<T, U> Create<T, U>(T key, U value) {
        return new Tuple<T, U>(key, value);
    }
}
