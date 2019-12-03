namespace DarkBestiary.Messaging
{
    public delegate void Payload<in T1>(T1 arg);

    public delegate void Payload<in T1, T2>(T1 arg1, T2 arg2);

    public delegate void Payload<in T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    public delegate void Payload();
}