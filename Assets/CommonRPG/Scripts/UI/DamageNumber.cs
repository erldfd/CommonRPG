using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonRPG
{
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI damageText;

        [SerializeField]
        private float lifeTime;
        public float LifeTime { get { return lifeTime; } }

        [SerializeField]
        private float moveSpeed = 1;

        private void Update()
        {
            Quaternion billboardRotation = Camera.main.transform.rotation;
            billboardRotation = new Quaternion(0, billboardRotation.y, 0, billboardRotation.w);
            transform.rotation = billboardRotation;

            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }

        public void SetDamageText(float damageAmount)
        {
            damageText.text = $"{damageAmount:F1}";
        }
    }
}
