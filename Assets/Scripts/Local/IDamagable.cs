using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingSystem.Local
{
    public interface IDamagable
    {
        void DoDamage(float damage, NetworkConnectionToClient conn = null);
    }
}
