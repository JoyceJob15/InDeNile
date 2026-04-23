using System;
using UnityEngine.Events;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// Detects a collision with a tagged collider, replacing this object with a 'broken' version
    /// </summary>
    public class Breakable : MonoBehaviour
    {
        // switched to a static global counter so destroyed instances don't lose the accumulated count
        private static int s_CropCount = 0;
        private static int s_crockcount = 0;

        [SerializeField] private GameObject Arrow3;
        [SerializeField] private GameObject Arrow1;
        [SerializeField] private GameObject Medbag;
        [SerializeField] private GameObject Character;
        [SerializeField] private GameObject SceneTrigger;
        [SerializeField] private TMPro.TMP_Text Task;
        [Serializable] public class BreakEvent : UnityEvent<GameObject, GameObject> { }

        [SerializeField]
        [Tooltip("The 'broken' version of this object.")]
        GameObject m_BrokenVersion;

        [SerializeField]
        [Tooltip("The tag a collider must have to cause this object to break.")]
        string m_ColliderTag = "Destroyer";

        [SerializeField]
        [Tooltip("Events to fire when a matching object collides and break this object. " +
            "The first parameter is the colliding object, the second parameter is the 'broken' version.")]
        BreakEvent m_OnBreak = new BreakEvent();

        bool m_Destroyed = false;

        /// <summary>
        /// Events to fire when a matching object collides and break this object.
        /// The first parameter is the colliding object, the second parameter is the 'broken' version.
        /// </summary>
        public BreakEvent onBreak => m_OnBreak;

        void OnCollisionEnter(Collision collision)
        {
            if (m_Destroyed)
                return;

            if (collision.gameObject.tag.Equals(m_ColliderTag, System.StringComparison.InvariantCultureIgnoreCase))
            {
                m_Destroyed = true;
                var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);
                m_OnBreak.Invoke(collision.gameObject, brokenVersion);

                // increment global counter and log it
                s_CropCount++;
                s_crockcount++;
                UnityEngine.Debug.Log($"Breakable: global crop count = {s_CropCount}");
                if (s_crockcount >= 3)
                {
                    if (SceneTrigger != null) SceneTrigger.SetActive(true);
                }
                if (s_CropCount >= 15)
                {
                    if (Arrow3 != null) Arrow3.SetActive(false);
                    if (Arrow1 != null) Arrow1.SetActive(true);
                    if (Medbag != null) Medbag.SetActive(true);
                    if (SceneTrigger != null) SceneTrigger.SetActive(true);
                    Task.text = "Return to the farmer";

                    // rotate Character by 45 degrees around its local Y axis
                    if (Character != null)
                        Character.transform.Rotate(0f, 65f, 0f, UnityEngine.Space.Self);
                }

                Destroy(gameObject);
            }
        }
    }
}
