using UnityEngine;
using System.Collections.Generic;

namespace WaterProstecting
{
    public class FoliageFader : MonoBehaviour
    {
        public Transform target;
        public LayerMask foliageMask;
        public float checkRadius = 0.3f;

        private List<Renderer> hidden = new List<Renderer>();

        void LateUpdate()
        {
            if (target == null) return;

            RestoreAll();

            Vector3 lookPoint = target.position + Vector3.up * 1f;
            Vector3 direction = (transform.position - lookPoint).normalized;
            float distance = Vector3.Distance(lookPoint, transform.position);

            RaycastHit[] hits = Physics.SphereCastAll(
                lookPoint, checkRadius, direction, distance, foliageMask
            );

            foreach (var hit in hits)
            {
                //Не показ все рендеры на объ
                Renderer[] renderers = hit.collider.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers)
                {
                    if (rend.enabled)
                    {
                        rend.enabled = false;
                        hidden.Add(rend);
                    }
                }
            }
        }

        private void RestoreAll()
        {
            foreach (var rend in hidden)
            {
                if (rend != null) rend.enabled = true;
            }
            hidden.Clear();
        }
    }
}