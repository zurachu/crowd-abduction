public static class TitleConstData
{
    private static PlayFabTitleConstDataManagerSingleton Source => PlayFabTitleConstDataManagerSingleton.Instance;
    public static int AbductionCount => Source.GetInt("AbductionCount");
    public static float HumanVelocity => Source.GetFloat("HumanVelocity");
    public static float HumanVelocityInCircle => Source.GetFloat("HumanVelocityInCircle");
    public static int InitialHumanCount => Source.GetInt("InitialHumanCount");
    public static float Radius => Source.GetFloat("Radius");
    public static string TitleDescription => Source.GetString("TitleDescription");
}
