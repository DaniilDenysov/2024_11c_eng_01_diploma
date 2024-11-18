using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerLabelsContainer : LabelContainer<PlayerLabel, PlayerLabelsContainer>
    {
        public override void Add(PlayerLabel item)
        {
            base.Add(item);
            if (item.TryGetComponent(out RectTransform rect))
            {
                rect.localScale = new Vector3(1f,1f);
            }
        }

        public override PlayerLabelsContainer GetInstance()
        {
            return this;
        }
    }
}
