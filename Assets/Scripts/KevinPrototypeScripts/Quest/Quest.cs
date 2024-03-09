[System.Serializable]
public class Quest
{
    public string description;
    public int requiredAmount;
    public int currentAmount;
    public bool isComplete => currentAmount >= requiredAmount;
}
