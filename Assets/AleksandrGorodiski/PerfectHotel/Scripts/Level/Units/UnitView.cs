using UnityEngine;
using UnityEngine.AI;

namespace Game.Level.Unit
{
    public enum AnimationType
    {
        Walk,
        Idle,
        Sleep,
        Clean,
        Toilet,
        Reception,
        Carry,
        Sit
    }

    public class UnitView : MonoBehaviour
    {
        [SerializeField] private Transform _localTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        public Transform LocalTransform => _localTransform;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        [HideInInspector] public int Index;

        AnimationType _currentType;

        internal void Walk()
        {
            PlayWalkAnimation();
        }

        internal void Walk(int inventories)
        {
            if (inventories > 0)
                _animator.SetLayerWeight(1, 1f);
            else _animator.SetLayerWeight(1, 0f);

            PlayWalkAnimation();
        }

        void PlayWalkAnimation()
        {
            PlayAnimation(AnimationType.Walk, GetRandomTime());
        }

        internal void Idle()
        {
            PlayIdleAnimation();
        }

        internal void Idle(int inventories)
        {
            if (inventories > 0)
                _animator.SetLayerWeight(1, 1f);
            else _animator.SetLayerWeight(1, 0f);

            PlayIdleAnimation();
        }

        void PlayIdleAnimation()
        {
            PlayAnimation(AnimationType.Idle, GetRandomTime());
        }

        internal void Sleep()
        {
            PlayAnimation(AnimationType.Sleep, GetRandomTime());
        }

        internal void Clean()
        {
            PlayAnimation(AnimationType.Idle, GetRandomTime());
        }

        internal void Toilet()
        {
            PlayAnimation(AnimationType.Sit, GetRandomTime());
        }

        internal void Reception()
        {
            PlayAnimation(AnimationType.Reception, GetRandomTime());
        }

        internal void Throw()
        {
            PlayAnimation(AnimationType.Idle, GetRandomTime());
        }

        float GetRandomTime()
        {
            return Random.Range(0f, 1f);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        internal void Unhide()
        {
            gameObject.SetActive(true);
        }

        internal void PlayAnimation(AnimationType animationType, float timeValue)
        {
            _currentType = animationType;
            var nameHash = Animator.StringToHash(_currentType.ToString());
            _animator.PlayInFixedTime(nameHash, 0, timeValue);

            _animator.Update(0);
        }
    }
}