using System;
using System.Collections.Generic;
using UnityEngine;

// The baby's data - also provides methods to save and retrieve data to database/json file
public class BabyModel : MonoBehaviour
{
    // The newborn's name - set during creation
    [SerializeField] private string name = null;
    public string Name { get { return name; } set { name = value; } }

    // The newborn's nickname - set during creation
    [SerializeField] private string nickName = null;
    public string NickName { get { return nickName; } set { nickName = value; } }

    // Colonist unique personnel ID
    [SerializeField] private static int uniqueColonistPersonnelID = 0;
    public static int UniqueColonistPersonnelID { get { return uniqueColonistPersonnelID; } set { uniqueColonistPersonnelID = value; } }
    [SerializeField] private int _uniqueColonistPersonnelID;

    // Colonist health - Can be reduced by conditions, damage taken during certain events?
    [SerializeField] private float health = 0.0f;
    public float Health { get { return health; } set { health = value; } }
    // Virus strains/diseases/conditions active on the colonist
    [SerializeField] private List<Condition> activeConditions = null;

    // Colonist level (for progression)
    [SerializeField] private int level = 0;
    public int Level { get { return level; } set { level = value; } }
    // Colonist skill points (acquired so far)
    [SerializeField] private int skillPoints = 1;
    public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    // Colonist genetic points (determined throughout progression)
    [SerializeField] private int geneticPoints = 10;
    public int GeneticPoints { get { return geneticPoints; } set { geneticPoints = value; } }

    // The baby's sex.
    [SerializeField] private string sex = null;
    // The property for the baby's sex.
    public string Sex { get{ return sex; } set { sex = value; } }
    // The newborn's age -> progresses overtime
    [SerializeField] private int age = 1;
    public int Age { get { return age; } set { age = value; } }
    // Life expectancy: starts at 100, changes throughout colonist's life depending on disease/viral strains/injury/moral/etc.
    [SerializeField] private int lifeExpectancy = 100;
    public int LifeExpectancy { get { return lifeExpectancy; } set { lifeExpectancy = value; } }
    // Quality of life years expected (how many years the colonist is expected to thrive healthily)
    [SerializeField] private int qalys;
    public int Qalys { get { return qalys; } set { qalys = value; } }

    // The baby's adult height.
    [SerializeField] private float adultHeight = 0.0f;
    // The property for the baby's adult height
    public float AdultHeight { get { return adultHeight; } set { adultHeight = value; } }
    // The R value of the material for the skin
    [SerializeField] private float skinColorR = 0.0f;
    public float SkinColorR { get { return skinColorR; } set { skinColorR = value; } }
    // The G value of the material for the skin
    [SerializeField] private float skinColorG = 0.0f;
    public float SkinColorG { get { return skinColorG; } set { skinColorG = value; } }
    // The B value of the material for the skin
    [SerializeField] private float skinColorB = 0.0f;
    public float SkinColorB { get { return skinColorB; } set { skinColorB = value; } }

    // The currently used mesh names for head and torso - to allow View to reload the proper mesh
    [SerializeField] private string activeHeadName;
    public string ActiveHeadName { get { return activeHeadName; } set{ activeHeadName = value; } }
    [SerializeField] private string activeTorsoName;
    public string ActiveTorsoName { get { return activeTorsoName; } set { activeTorsoName = value; } }
}
