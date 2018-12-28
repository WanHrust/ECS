using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Weapon : IComponentData
{

}

public class WeaponComponent : ComponentDataWrapper<Weapon>
{

}
