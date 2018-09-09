using UnityEngine;
using UnityEngine.AI;
using ModelOutline = cakeslice.Outline; // 3D https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/outline-effect-78608

public class Creature : MonoBehaviour
{
    [Header("Creature Stats")]
    public string creatureName;

    public int health;

    public int maxHealth;

    public int mana;

    public int maxMana;

    public bool isAlive = true;

    public int damage;

    public float attackRange;

    public NavMeshAgent navAgent;

    [Header("Creature Animations")]
    public GameObject modelParent; // The parent GameObject containing the bones and the base model
    public GameObject model; // The base model itself
    Animation animationController;
    public AnimationClip moveAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip deathAnimation;
    ModelOutline modelOutline;

    protected virtual void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (modelParent != null)
        {
            if (moveAnimation == null || idleAnimation == null || attackAnimation == null || deathAnimation == null)
            {
                Debug.LogWarning("Warning: " + creatureName + " has insufficient animations set to assign an animation controller!");
            }
            else
            {
                moveAnimation.legacy = true;
                idleAnimation.legacy = true;
                attackAnimation.legacy = true;
                deathAnimation.legacy = true;
                animationController = modelParent.AddComponent<Animation>();
                animationController.AddClip(moveAnimation, moveAnimation.name);
                animationController.AddClip(idleAnimation, idleAnimation.name);
                animationController.AddClip(attackAnimation, attackAnimation.name);
                animationController.AddClip(deathAnimation, deathAnimation.name);
            }

            if (GetComponent<Animator>() != null)
            {
                // Whilst I know that Animator is technically newer, the Animation component allows more generic code
                Debug.LogError("This class doesn't support the Animator - Please drag the animation clips into the appropriate slots and delete the Animator");
            }
        }

        if (model == null)
        {
            Debug.LogWarning("Please assign the model to this creature script!");
        }
        else
        {
            modelOutline = model.AddComponent<ModelOutline>();
            modelOutline.color = 0;
            modelOutline.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (!isAlive || animationController == null)
        {
            return;
        }

        if (!this.IsMoving() && !animationController.IsPlaying(idleAnimation.name) && !animationController.IsPlaying(attackAnimation.name))
        {
            PlayAnimation(idleAnimation);
        }
        else if (this.IsMoving() && !animationController.IsPlaying(moveAnimation.name))
        {
            PlayAnimation(moveAnimation);
        }
    }

    public void PlayAnimation(string animationName, float animationSpeed = 1f)
    {
        if (animationController[animationName] == null)
        {
            Debug.LogWarning("The animation " + animationName + " does not exist as part of the animation controller!");
            return;
        }
        animationController[animationName].speed = animationSpeed;
        animationController.CrossFade(animationName);
    }

    public void PlayAnimation(AnimationClip animationClip, float animationSpeed = 1f)
    {
        if (animationController[animationClip.name] == null)
        {
            Debug.LogWarning("The animation " + animationClip.name + " does not exist as part of the animation controller!");
            return;
        }
        animationController[animationClip.name].speed = animationSpeed;
        animationController.CrossFade(animationClip.name);
    }

    public void AlterLife(int lifeValue, Vector3 hitDirection = new Vector3())
    {
        health += lifeValue;
        if (health <= 0)
        {
            Die(hitDirection);
        }
    }

    protected virtual void Die(Vector3 hitDirection = new Vector3())
    {
        Debug.Log("I have died!");
        isAlive = false;
        animationController.CrossFade(deathAnimation.name);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void LookAt(Vector3 toLook)
    {
        transform.LookAt(new Vector3(toLook.x, 0, toLook.z)); // 0 -> Don't want to tilt up and down :p
    }

    public void StopMoving()
    {
        if (navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
        }
        else
        {
            Debug.LogError(name + " is not on a navMesh and will be deleted!");
            DestroyGameObject();
        }
    }

    // Slow things apparently aren't really moving (This is since things slow down before they stop...)
    public bool IsMoving()
    {
        return (navAgent.velocity.magnitude > 1f);
    }

    public void ToggleOutline()
    {
        if (modelOutline != null)
        {
            modelOutline.enabled = !modelOutline.enabled;
        }
        else
        {
            Debug.LogError("You tried to outline a creature that has no outline!");
        }
    }
}