public class TrainNPCData
{
    public int id;
    public string characterName;
    public int strength;
    public int agility;
    public int intelligence;
    public ParametersExperience experience;
    public TrainNPCAgent sceneInstance;

    public TrainNPCData(Character data, TrainNPCAgent sceneInstance)
    {
        id = data.id;
        characterName = data.name;
        strength = data.strength;
        agility = data.agility;
        intelligence = data.intelligence;
        experience = new ParametersExperience(data.paramsExperience);
        this.sceneInstance = sceneInstance;
    }
}

public class ParametersExperience
{
    public int strength;
    public int agility;
    public int intelligence;

    public ParametersExperience(CharacterParametersExperience experience)
    {
        strength = experience.strength;
        agility = experience.agility;
        intelligence = experience.intelligence;
    }   
}
