using System;
using System.Reflection;

public class IManager<T> where T : IManager<T> {
    private static T sInstance = null;
    private static readonly Type[] EmptyTypes = new Type[0];
    public static T Singleton {
        get {
            if (null == sInstance) {
                ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, EmptyTypes, null);
                if (null == ci) {
                    throw new InvalidOperationException("class must contain a private constructor");
                }
                sInstance = ci.Invoke(null) as T;
            }
            return sInstance;
        }
    }
}