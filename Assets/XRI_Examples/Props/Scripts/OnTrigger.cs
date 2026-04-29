using System;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// Calls functionality when a physics trigger occurs
    /// </summary>
    public class OnTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject Arrow2;
        [SerializeField] private GameObject Arrow3;
        [SerializeField] private GameObject Sickle;
        [SerializeField] private TMPro.TMP_Text Task;
        public AudioSource audioSource;
        public AudioClip soundEffect;
        [Serializable] public class TriggerEvent : UnityEvent<GameObject> { }

        [SerializeField]
        [Tooltip("If set, this trigger will only fire if the other gameobject has this tag.")]
        string m_RequiredTag = string.Empty;

        [SerializeField]
        [Tooltip("Events to fire when a matcing object collides with this trigger.")]
        TriggerEvent m_OnEnter = new TriggerEvent();

        [SerializeField]
        [Tooltip("Events to fire when a matching object stops colliding with this trigger.")]
        TriggerEvent m_OnExit = new TriggerEvent();

        /// <summary>
        /// If set, this trigger will only fire if the other gameobject has this tag.
        /// </summary>
        public string requiredTag => m_RequiredTag;

        /// <summary>
        /// Events to fire when a matching object collides with this trigger.
        /// </summary>
        public TriggerEvent onEnter => m_OnEnter;

        /// <summary>
        /// Events to fire when a matching object stops colliding with this trigger.
        /// </summary>
        public TriggerEvent onExit => m_OnExit;
        void Start()
        {
            if (audioSource != null && soundEffect != null)
            {
                audioSource.clip = soundEffect;

            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (CanTrigger(other.gameObject))
                m_OnEnter?.Invoke(other.gameObject);
            if (Arrow2 != null)
                Arrow2.SetActive(false);
            if (Arrow3 != null)
                Arrow3.SetActive(false);
            Arrow3.SetActive(true);
            Sickle.SetActive(true);
            Task.text = "Harvest the crops";
            audioSource.Play();
        }

        void OnTriggerExit(Collider other)
        {
            if (CanTrigger(other.gameObject))
                m_OnExit?.Invoke(other.gameObject);
            // If the incoming object is tagged "bait", disable Arrow2
            if (other.gameObject.CompareTag("bait"))
            {
                
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if (CanTrigger(other.gameObject))
                m_OnEnter?.Invoke(other);
        }

        bool CanTrigger(GameObject otherGameObject)
        {
            if (m_RequiredTag != string.Empty)
                return otherGameObject.CompareTag(m_RequiredTag);
            else
                return true;
        }
    }
}
