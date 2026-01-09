public interface IMiniGameSaveable
{
    MiniGameData GetSaveData();
    void LoadFromData(MiniGameData data);
}
