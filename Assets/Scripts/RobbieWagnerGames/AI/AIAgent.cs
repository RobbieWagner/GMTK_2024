using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace RobbieWagnerGames.AI
{
    public enum AIState
    {
        NONE = -1,
        IDLE = 0,
        MOVING = 1,
        CHASING = 2,
        SEARCHING = 3
    }

    // DEFINES THE BASE AI AGENT AND HELPFUL METHODS
    // USEFUL FOR EASY, SIMPLE USES OF AI PATHFINDING, POTENTIALLY IF YOU HAVE A LOT OF AGENTS TO CONTROL
    // YOU CAN ALSO CREATE A CHILD CLASS OF THIS IF YOU'D LIKE TO CREATE ALTERNATE BEHAVIORS
    public class AIAgent : MonoBehaviour
    {
        public NavMeshAgent agent;

        public float idleWaitTime = 3f;
        protected float currentWaitTime;

        [SerializeField] protected float movementRange = 100f;

        protected List<AITarget> currentTargets = new List<AITarget>(); // Use [HideInInspector] if needed
        public AITarget chasingTarget { get; protected set; }

        [SerializeField] protected Animator animator;
        [SerializeField] protected AudioSource footstepSounds;
        [SerializeField] protected LayerMask raycastLayers;

        protected AIState currentState = AIState.NONE;
        public AIState CurrentState
        {
            get 
            {
                return currentState;
            }
            set 
            {
                if(value == currentState)
                    return;
                currentState = value;
                OnStateChange?.Invoke(currentState);
            }
        }
        public delegate void AIStateDelegate(AIState state);
        public event AIStateDelegate OnStateChange;
        //TODO: Add a delegate and event? Does Observer pattern need to be implemented?

        protected virtual void Awake()
        {
            OnStateChange += UpdateAnimator;
        }

        protected virtual void UpdateAnimator(AIState state)
        {
            if (state == AIState.MOVING || state == AIState.CHASING || state == AIState.SEARCHING)
            {
                if(footstepSounds != null)
                {
                    if (footstepSounds.time != 0)
                        footstepSounds.UnPause();
                    else
                        footstepSounds.Play();
                }
                
                animator.SetBool("Walking", true);
            }
            else if (state == AIState.IDLE)
            {
                if (footstepSounds != null)
                {
                    if (footstepSounds.isPlaying)
                        footstepSounds.Pause();
                }
                
                animator.SetBool("Walking", false);
            }
        }

        #region State Changing
        public virtual void GoIdle()
        {
            if (agent != null)
            {
                agent.isStopped = true;
                CurrentState = AIState.IDLE;
            }
        }

        public virtual bool MoveAgent(Vector3 destination)
        {
            if (agent != null)
            {
                CurrentState = AIState.MOVING;
                bool success = SetDestination(destination);

                if (!success)
                {
                    GoIdle();
                    Debug.LogWarning("failed to move agent");
                }

                return success;
            }
            return false;
        }

        public virtual bool ChaseNearestTarget()
        {
            //Debug.Log("Chase Nearest Target.");
            AITarget closestTarget = null;
            float closestDistance = float.MaxValue;

            if (currentTargets == null || !currentTargets.Any())
            {
                Debug.LogWarning("Could not chase target: current targets list found empty.");
                GoIdle();
                return false;
            }

            foreach (AITarget target in currentTargets)
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(target.transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = AIManager.GetPathLength(path);
                    if (pathLength < closestDistance)
                    {
                        closestDistance = pathLength;
                        closestTarget = target;
                    }
                }
            }

            if (closestTarget != null)
            {
                CurrentState = AIState.CHASING;
                chasingTarget = closestTarget;
                return true;
            }

            return false;
        }
        #endregion

        #region States and Updates

        protected void Update()
        {
            switch (currentState) 
            {
                case AIState.IDLE:
                    UpdateIdleState();
                    break;
                case AIState.MOVING:
                    UpdateMovingState();
                    break;
                case AIState.CHASING:
                    UpdateChaseState();
                    break;
                case AIState.SEARCHING:
                    UpdateSearchState();
                    break;
                default:
                    break;
            }
        }

        protected virtual void UpdateIdleState()
        {
            currentWaitTime += Time.deltaTime;
            if(currentWaitTime >= idleWaitTime)
            {
                currentWaitTime = 0;
                MoveToRandomSpot(movementRange);
            }
        }

        protected virtual void UpdateMovingState()
        {
            if (agent != null)
            {
                if (agent.destination == null || AIManager.GetPathLength(agent.path) < .05f)
                    GoIdle();
            }
        }

        protected virtual void UpdateChaseState()
        {
            if (agent != null && chasingTarget != null)
            {
                SetDestination(chasingTarget.transform.position);

                if (agent.destination == null || chasingTarget == null || Vector3.Distance(transform.position, chasingTarget.transform.position) < 3.5f) //AIManager.GetPathLength(agent.path) < .05f)
                {
                    OnReachTarget(chasingTarget);
                }
            }
            if (chasingTarget == null)
                chasingTarget = StealthPlayer.Instance.GetComponent<AITarget>();
        }
        
        // update search check if path you're following is incomplete, if you reach end then go idle but if you see player again go back to chase
        protected virtual void UpdateSearchState()
        {
            
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if(CurrentState == AIState.CHASING)
            {
                AITarget target = collision.gameObject.GetComponent<AITarget>();

                if(target != null && chasingTarget == target)
                    OnReachTarget(chasingTarget);
            }
        }

        protected virtual void OnReachTarget(AITarget target)
        {
            target.OnCaught(this);
            currentTargets.Remove(chasingTarget);
            ChaseNearestTarget();
        }
        #endregion

        #region Worldspace Movement

        public virtual bool SetDestination(Vector3 destination)
        {
            if (agent != null)
            {
                agent.isStopped = false;
                bool success = agent.SetDestination(destination);
                return success;
            }
            return false;
        }

        public virtual void MoveToRandomSpot(float range = 100f)
        {
            StartCoroutine(MoveToRandomSpotCo(transform.position, range, 10000));
        }

        public virtual IEnumerator MoveToRandomSpotCo(Vector3 offset, float range = 100f, int tryLimit = 10000, int triesBeforeYield = 25)
        { 
            int tries = 0;
            bool success = false;
            while (tries < tryLimit)
            {
                tries++;
                if(tries % triesBeforeYield == 0)
                    yield return null;
                
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range; // transform.position?
                randomDirection += offset;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
                {
                    MoveAgent(hit.position);
                    success = true;
                    yield break;
                }
            }
            
            if(!success)
                Debug.LogWarning($"Could not find a path after trying {tryLimit} times!");
        }
        #endregion

        #region AITarget Chasing
        public virtual void SetTargets(List<AITarget> targets, bool removeOldTargets = false, bool chaseNearestTarget = false)
        {
            if (targets != null && targets.Any())
            {
                if (removeOldTargets)
                    currentTargets.Clear();

                currentTargets.AddRange(targets);

                if (chaseNearestTarget)
                    ChaseNearestTarget();
            }
        }
        #endregion
    }
}