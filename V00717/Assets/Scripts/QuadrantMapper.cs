using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrantMapper : MonoBehaviour
{
    private AdjacencyMatrix adjacencyMatrix;
    private AdjacencyMap adjacencyMap;

    public EdgeObject[] edgeObjects;
    public GameWaypoint[] gameWayPoints;
    public GameObject[] gameWaypointsCameraUILayouts;

    // Map for the buttons parented to their horizontal layout UI associated with each gameWayPoint
    public GameObject[] gameWaypointToCameraUIMap;

    public bool isDirected = false;

    public void Start()
    {
        adjacencyMatrix = new AdjacencyMatrix(edgeObjects);
        adjacencyMap = new AdjacencyMap(isDirected);
        gameWaypointToCameraUIMap = new GameObject[gameWayPoints.Length];
        for(int i = 0; i < gameWayPoints.Length; i++)
        {
            gameWaypointToCameraUIMap[i] = gameWaypointsCameraUILayouts[i];
        }
    }

    /// <summary>
    /// Adds an event to waypoint v that will seek and collide with
    /// character participants.
    ///
    /// Can be instant or after a delay.
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="instant"></param>
    public IEnumerator AddEventToWayPoint(GameWaypoint v, WaypointEvent e, float delay)
    {
        yield return new WaitForSeconds(delay);
        v.waypointEvent = e;
    }

    /// <summary>
    /// Removes any waypoint even currently at waypoint v,
    /// after an optional delay.
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator RemoveEventsAtWaypoint(GameWaypoint v, float delay)
    {
        yield return new WaitForSeconds(delay);
        v.waypointEvent = null;
    }

    /// <summary>
    /// Disable a quadrant in the game's map either
    /// permanently or not, and optionally for a delay.
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="delay"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    public IEnumerator DisableQuadrant(GameWaypoint v, float delay, bool permanent)
    {
        yield return new WaitForSeconds(delay);
        foreach(GameWaypoint e in v.Outgoing.Keys)
        {
            adjacencyMap.RemoveEdge(v.Outgoing[e]);
        }
        if (!permanent)
        {
            StartCoroutine(EnableQuadrant(v, delay));
        }
    }

    /// <summary>
    /// Restores the non-traversable edges of a GameWaypoint v in the quadrant
    /// after an optional delay.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator EnableQuadrant(GameWaypoint v, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (GameWaypoint e in v.Outgoing.Keys)
        {
            adjacencyMap.EnableEdge(v.Outgoing[e]);
        }
    }


    public delegate bool NavigationAttempt(CharacterModel characterModel, GameWaypoint newWaypoint);
    /// <summary>
    /// Seek the quadrant at the int key.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="quadrantIntKey"></param>
    public bool GoToQuadrant(CharacterModel character, GameWaypoint newWaypoint)
    {
        // Update the current quadrant location for the calculation of the next paths
        character.InQuadrant = newWaypoint.intKey;
        Bot brain = character.GetComponent<Bot>();
        if (!brain)
        {
            brain = character.GetComponentInChildren<Bot>();
            brain.quadrantTarget = newWaypoint;
        }

<<<<<<< Updated upstream
        bool successful = character.GetComponent<Bot>().Seek(newWaypoint.transform.position);
=======
        NavMeshAgent nav = character.gameObject.GetComponent<NavMeshAgent>();
        // Update the current quadrant location for the calculation of the next paths
        character.gameObject.GetComponent<Rigidbody>().useGravity = false;
        character.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        if (nav == null)
        {
            nav = character.gameObject.AddComponent<NavMeshAgent>();
        }
        nav.agentTypeID = 0;
        nav.radius = 0.3f;
        nav.baseOffset = 2.1f;

        bool successful = brain.Seek(newWaypoint.transform.position);
>>>>>>> Stashed changes
        if(successful)
        {
            character.GetComponent<Animator>().SetBool("isWalking", true);
        }

        // Do something if unsuccessful
        int attempt = 0;
        int MAX_ATTEMPTS = 50;

        if(!successful)
        {
            while (attempt < MAX_ATTEMPTS)
            {
                successful = brain.Seek(newWaypoint.transform.position);
                ++attempt;
            }
        }
        return successful;
    }

    /// <summary>
    /// Evade the quadrant at the int key.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="quadrantIntKey"></param>
    public void EvadeQuadrant(CharacterModel character, int quadrantIntKey)
    {
        //character.GetComponent<Bot>().Flee(gameWayPoints[quadrantIntKey].transform.position);
    }
}
