using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode",menuName = "Create new game mode")]
public class GameModeSO : ScriptableObject
{
    public string Title;
    public string Description;
    [Range(0,10000),Tooltip("Time in seconds")] public int RoundTime,RoundDelay;
    [Range(0,100)] public int Rounds;


#if UNITY_EDITOR
    private void OnEnable()
    {
        foreach (var team in teams)
        {
            team.Setup();
        }
    }
#endif
    public List<Team> teams;

    public int GetTotalPlayerCount ()
    {
        return teams.Sum((t)=>t.maxPlayerAmount);
    }

    public int GetPlayerCountForTeam (string teamGuid)
    {
        var team = teams.FirstOrDefault((t)=>t.Guid.Equals(teamGuid));
        return team == null ? 0 : team.maxPlayerAmount;
    }
}
