using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    #region Variables
    [Header("General player level")]
    public int playerLevel = 1;
    //Spells levels
    public int fireBall = 1, waterBall = 1, magneticBasic = 1, electricBasic = 1;
    //Ability points to upgrade ALL spells
    public int abilityPoints = 0;
    #endregion

}
