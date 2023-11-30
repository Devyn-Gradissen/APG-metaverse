namespace FiveMinuteChat.Enums
{
    public enum ConnectorType
    {
        Tcp,
        SignalRCore,
#if FiveMinuteChat_BestHttpEnabled
        SignalRBestHttp2,
#endif
    }
}
