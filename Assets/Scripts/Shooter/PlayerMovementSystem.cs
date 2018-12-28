using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerMovementSystem : ComponentSystem
{
    private struct Filter
    {
        public Rigidbody Rigidbody;
        public InputComponent InputComponent;
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.deltaTime;

        foreach (var entity in GetEntities<Filter>())
        {
            var moveVector = new Vector3(entity.InputComponent.Horizontal, 0f, entity.InputComponent.Vertical);
            var movePosition = entity.Rigidbody.position + moveVector.normalized * 3f * deltaTime;

            entity.Rigidbody.MovePosition(movePosition);
        }
    }

}
