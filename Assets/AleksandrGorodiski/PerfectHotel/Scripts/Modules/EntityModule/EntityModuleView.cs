using UnityEngine;

namespace Game.Level.Entity
{
    public sealed class EntityModuleView : MonoBehaviour
    {
        [HideInInspector] public AreaView[] AreaViews;
        [HideInInspector] public RoomView[] RoomViews;
        [HideInInspector] public CleanerView[] CleanerViews;
        [HideInInspector] public ToiletView[] ToiletViews;
        [HideInInspector] public UtilityView[] UtilityViews;
        [HideInInspector] public ElevatorView[] ElevatorViews;
        [HideInInspector] public CorpsmanView[] CorpsmanViews;

        private void Awake()
        {
            AreaViews = GetComponentsInChildren<AreaView>();
            RoomViews = GetComponentsInChildren<RoomView>();
            CleanerViews = GetComponentsInChildren<CleanerView>();
            ToiletViews = GetComponentsInChildren<ToiletView>();
            UtilityViews = GetComponentsInChildren<UtilityView>();
            ElevatorViews = GetComponentsInChildren<ElevatorView>();
            CorpsmanViews = GetComponentsInChildren<CorpsmanView>();
        }
    }
}
