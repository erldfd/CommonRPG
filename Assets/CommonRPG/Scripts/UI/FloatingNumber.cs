using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CommonRPG
{
    public class FloatingNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI floatingText;

        [SerializeField]
        private float lifeTime;
        public float LifeTime { get { return lifeTime; } }

        [SerializeField]
        private float floatingSpeed = 1;

        private void Update()
        {
            Quaternion billboardRotation = Camera.main.transform.rotation;
            billboardRotation = new Quaternion(0, billboardRotation.y, 0, billboardRotation.w);
            transform.rotation = billboardRotation;

            transform.Translate(Vector2.up * floatingSpeed * Time.deltaTime);
        }

        public void SetFloatingText(float number)
        {
            SetFloatingText($"{number:F1}");
        }

        public void SetFloatingText(string text)
        {
            floatingText.text = text;
        }
    }
}
