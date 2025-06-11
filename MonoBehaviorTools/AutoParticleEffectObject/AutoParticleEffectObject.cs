#if CUSTOM_USING_ODIN && CUSTOM_USING_ZENJECT
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AutoParticleEffectObject : MonoBehaviour
    {
        [InjectOptional] private IViewManager viewManager;
        [SerializeField] private AutoSortingOrderMode autoSortingOrderMode;
        [SerializeField] private ParticleLifeMode particleLifeMode;

        [SerializeField] [ShowIf("@autoSortingOrderMode!=AutoSortingOrderMode.Inactive")]
        private ArchitectureView canvasSourceView;

        private List<ParticleSystem> particleObjectList;
        private ParticleSystem particleParent;

        private float Duration => particleParent == null ?
            0 :
            particleParent.main.duration;

        private void InitParticleDatas()
        {
            particleObjectList = new List<ParticleSystem>();

            particleParent = GetComponent<ParticleSystem>();
            particleObjectList.AddRange(GetComponentsInChildren<ParticleSystem>());
        }

        private void InitOrderInLayer()
        {
            if (viewManager == null)
                return;

            if (autoSortingOrderMode == AutoSortingOrderMode.Inactive || canvasSourceView == null)
                return;

            int newSortingOrder = viewManager.GetViewSortOrder(canvasSourceView.GetType());
            if (autoSortingOrderMode == AutoSortingOrderMode.AboveSourceView)
                newSortingOrder++;
            else if (autoSortingOrderMode == AutoSortingOrderMode.BelowSourceView)
            {
                newSortingOrder--;

                if (newSortingOrder < 0)
                    newSortingOrder = 0;
            }

            foreach (ParticleSystem par in particleObjectList)
            {
                par.GetComponent<Renderer>().sortingOrder = newSortingOrder;
            }
        }

        private void Awake()
        {
            InitParticleDatas();
            InitOrderInLayer();
        }

        private IEnumerator Cor_AutoHide()
        {
            yield return new WaitForSeconds(Duration);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (particleLifeMode == ParticleLifeMode.HideWhenParticleEnd)
                StartCoroutine(Cor_AutoHide());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
#endif