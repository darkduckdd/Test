using Game.UI.Pool;
using UnityEngine;

namespace Game.Level.Reception
{
    public sealed class ReceptionModuleView : MonoBehaviour
    {
        public ReceptionView Reception;

        public ComponentPoolFactory[] Customers;
        public ComponentPoolFactory Receptionist;
    }
}


